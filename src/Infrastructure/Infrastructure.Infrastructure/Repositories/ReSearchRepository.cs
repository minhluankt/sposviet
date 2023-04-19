using Application.CacheKeys;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Infrastructure.DbContexts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class ReSearchRepository : IReSearchRepository
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IDistributedCache _distributedCache;
        private IMapper _mapper;
        private readonly ILogger<ReSearchRepository> _log;
        private IUnitOfWork _unitOfWork;
        private readonly IRepositoryCacheAsync<ReSearch> _reSearchCacheRepository;
        private readonly IRepositoryCacheAsync<HistoryReSearch> _historyReSearchCacheRepository;
        private readonly IProductPepository<Product> _productRepository;
        public ReSearchRepository(
            IUnitOfWork unitOfWork, ILogger<ReSearchRepository> log, IServiceScopeFactory serviceScopeFactory,
            IMapper mapperInstance, IDistributedCache distributedCache,
            IProductPepository<Product> productRepository,
             IRepositoryCacheAsync<ReSearch> reSearchCacheRepository,
             IRepositoryCacheAsync<HistoryReSearch> historyReSearchCacheRepository
            )
        {
         
            _serviceScopeFactory = serviceScopeFactory;
            _distributedCache = distributedCache;
            _productRepository = productRepository;
            _mapper = mapperInstance;
            _historyReSearchCacheRepository = historyReSearchCacheRepository;
            _reSearchCacheRepository = reSearchCacheRepository;
            _log = log;
            _unitOfWork = unitOfWork;
        }
        public void Add(ReSearch reSearch)
        {
            // bảng ReSearch là lưu từ khóa k check theo ngày
            // HistoryReSearch lưu từ khóa theo ngày để theo dõi
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    ApplicationDbContext db = scope.ServiceProvider.GetService<ApplicationDbContext>();

                    reSearch.Date = DateTime.Now;
                    string slug = Common.ConvertToSlug(reSearch.Name);
                    reSearch.Slug = slug;
                    var reSearchRepository = db.ReSearch.Where(m => m.Slug == slug).SingleOrDefault();
                    if (reSearchRepository != null)
                    {
                        reSearchRepository.NumberSearches = ++reSearchRepository.NumberSearches;
                        reSearchRepository.Date = DateTime.Now;
                        db.Update(reSearchRepository);
                    }
                    else
                    {
                        reSearch.NumberSearches = 1;
                        db.Add(reSearch);
                    }

                    var historyReSearchRepository = db.HistoryReSearch.Where(m => m.Slug == slug && (m.Date.Date == DateTime.Now.Date)).SingleOrDefault();
                    if (historyReSearchRepository != null)
                    {
                        historyReSearchRepository.NumberSearches = ++historyReSearchRepository.NumberSearches;
                        db.Update(historyReSearchRepository);
                    }
                    else
                    {
                        var model = new HistoryReSearch()
                        {
                            ProductType = reSearch.ProductType,
                            IdCustomer = reSearch.IdCustomer,
                            Url = reSearch.Url,
                            Name = reSearch.Name,
                            Slug = reSearch.Slug,
                            Date = DateTime.Now,
                            NumberSearches = 1
                        };
                        this.AddHistoryReSearch(model, db);
                    }
                    db.SaveChanges();
                    db.Dispose();
                    _distributedCache.Remove(HistoryReSearchCacheKeys.ListKey);
                    _distributedCache.Remove(ReSearchCacheKeys.ListKey);
                }
            }
            catch (Exception e)
            {
                _log.LogError(e, e.Message);
            }

        }
        private void AddHistoryReSearch(HistoryReSearch historyReSearch, ApplicationDbContext db)
        {
            historyReSearch.Date = DateTime.Now;
            db.Add(historyReSearch);
        }

        public async Task<List<ReSearch>> SearchAsync(string text, ProductEnumcs ProductType = ProductEnumcs.Procuct)
        {
            List<ReSearch> reSearchList = new List<ReSearch>();
            int maxitem = 10;
            string slug = !string.IsNullOrEmpty(text) ? Common.ConvertToSlug(text) : text;


            // lấy tiếp từ bảng research

            var productList = await _reSearchCacheRepository.GetCachedListAsync(ReSearchCacheKeys.ListKey);
            if (productList.Count() > 0)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    var listdata = productList.Where(m => m.ProductType == (int)ProductType && (m.Slug.Contains(slug) || m.Name.ToLower().Contains(text.ToLower()))).OrderByDescending(m => m.NumberSearches).ThenByDescending(m => m.Name).Take(maxitem - reSearchList.Count()).ToList();
                    if (listdata.Count() > 0)
                    {
                        reSearchList.AddRange(listdata);
                    }
                }
                else
                {
                    var listdata = productList.Where(m => m.ProductType == (int)ProductType).OrderByDescending(m => m.NumberSearches).ThenByDescending(m => m.Name).Take(maxitem - reSearchList.Count()).ToList();
                    if (listdata.Count() > 0)
                    {
                        var listnew = listdata.Where(m => !reSearchList.Select(m => m.Name.ToLower()).ToArray().Contains(m.Name.ToLower())).ToList();
                        if (listnew.Count() > 0)
                        {
                            reSearchList.AddRange(listnew);

                        }

                    }
                }

            }

            // nếu lấy xong cả 2 mà chưa đủ 10 item thì tiếp tục lấy từ database product hoặc phụ tùng
            if (reSearchList.Count() < maxitem)
            {
               
                    var getproduct = await _productRepository.GetListProductCacheAsync(slug, true);
                    if (getproduct.Count() > 0)
                    {
                        var listnew = getproduct.Where(m => !reSearchList.Select(m => m.Name.ToLower()).ToArray().Contains(m.Name.ToLower())).ToList();

                        if (listnew.Count() > 0)
                        {
                            listnew = listnew.Take(maxitem - reSearchList.Count()).ToList();

                            List<ReSearch> newlst = new List<ReSearch>();
                            foreach (var item in listnew)
                            {
                                newlst.Add(new ReSearch()
                                {
                                    Name = item.Name,
                                    Slug = item.Slug
                                });
                            }
                            reSearchList.AddRange(newlst);
                        }
                    }
            

            }
            return reSearchList;
        }

        public async Task<List<ReSearch>> GetHistoriAsync(ProductEnumcs ProductType = ProductEnumcs.Procuct, int? take = null, bool showhistorylayout = false)
        {
            var productList = await _reSearchCacheRepository.GetCachedListAsync(ReSearchCacheKeys.ListKey);
            if (productList.Count() > 0)
            {
                productList = productList.Where(m => m.ProductType == (int)ProductType).OrderByDescending(m => m.NumberSearches).Take(10).ToList();
            }
            if (showhistorylayout && take != null)
            {
                if (productList.Count() > 0)
                {
                    return productList.Take(take.Value).ToList();
                }
                return new List<ReSearch>();
            }
           
                var getproduct = await _productRepository.GetListProductCacheAsync();
                if (getproduct.Count() > 0)
                {
                    var list = new List<ReSearch>();
                    foreach (var item in getproduct.Where(m => !productList.Select(x => x.Name.ToLower()).ToArray().Contains(m.Name.ToLower())).OrderByDescending(m => m.Id).Take(10 - productList.Count()).ToList())
                    {
                        list.Add(new ReSearch()
                        {
                            Name = item.Name,
                            Slug = item.Slug
                        });
                    }
                    if (productList.Count() == 0)
                    {
                        return list;
                    }
                    productList.AddRange(list);
                    return productList;
                }
            

            return productList;
        }
    }
}
