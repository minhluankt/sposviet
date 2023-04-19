using Application.CacheKeys;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Extensions.Caching;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class CategoryProductRepository : ICategoryProductRepository<CategoryProduct>
    {
        private readonly ILogger<CategoryProductRepository> _log;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<CategoryProduct> _repository;
        private readonly IProductPepository<Product> _repositoryProduct;
        private readonly ITypeCategoryRepository<TypeCategory> _typeCategory;
        public IQueryable<CategoryProduct> Entities => _repository.Entities;
        public CategoryProductRepository(IRepositoryAsync<CategoryProduct> repository,
             IProductPepository<Product> repositoryProduct,
            ILogger<CategoryProductRepository> log,
            ITypeCategoryRepository<TypeCategory> typeCategory,
            IDistributedCache distributedCache)
        {
            _log = log;
            _repositoryProduct = repositoryProduct;
            _typeCategory = typeCategory;
            _repository = repository; _distributedCache = distributedCache;
        }
        public async Task<Task> DeleteByIdPattern(int idPattern)
        {
            var categories = Entities.Where(m => m.IdPattern == idPattern);
            if (categories != null && categories.Count() > 0)
            {
                await _repository.DeleteRangeAsync(categories);
            }

            return Task.CompletedTask;
        }


        public async Task<List<CategoryProduct>> GetListByIdPatternAsync(int id)
        {
            var data = await _repository.Entities.Where(m => m.IdPattern == id).ToListAsync();
            return data;
        }

        public async Task<List<CategoryMenuModel>> GetListByCodeCacheAsync(string code)
        {
            try
            {
                string cachekey = CategoryCacheKeys.GetKey(code);
                var productList = await _distributedCache.GetAsync<List<CategoryMenuModel>>(cachekey);
                if (productList == null)
                {
                    var get = _typeCategory.GetByCode(code);
                    var data = _repository.Entities.Where(m => m.IdPattern == get.Id).Select(x => new CategoryMenuModel
                    {
                        Id = x.Id,
                        IdLevel = x.IdLevel,
                        IdPattern = x.IdPattern,
                        Name = x.Name,
                        Code = x.Code,
                        Slug = x.Code
                    }).ToList();
                    //  productList = _repository.Entities.AsNoTracking().Where(m => m.IdPattern == get.Id).Include(m=>m.TypeCategory).ToList();
                    productList = data;
                    //await _distributedCache.SetAsync(cachekey, productList);
                }
                return productList;
            }
            catch (Exception e)
            {
                _log.LogError(e, e.Message);
                throw;
            }

        }

        public IEnumerable<CategoryProduct> GetListIncludeProduct(int?[] listid, bool checkActiveProduct = false, int task = 15)
        {
            try
            {
                //lấy các các danh mục cha và các sản phẩm thuộc danh mục cha nhưng chỉ chứa các danh mục con của danh mục cha dc chỉ định khi cấu hình, tức là danh mục cha có
                // 4 danh mục con nhưng cấ hình chỉ cần lấy sản phẩm của 2 danh mục con
                if (listid == null)
                {
                    return Entities.Where(m => m.IdPattern == null).OrderBy(m => m.Sort).Include(m => m.CategoryChilds).ThenInclude(x => x.Products).Include(m => m.Products.OrderByDescending(m => m.Id).Take(task));
                }
                //var databycode = Entities.Where(m=>m.IdPattern==null&& listid.Contains(m.Id))
                //    .OrderBy(m => m.Sort)
                //    .Include(m=>m.CategoryChilds.Where(m => listid.Contains(m.Id))).ThenInclude(x=>x.Products)
                //    .Include(m=>m.Products.Where(m=> listid.Contains(m.IdCategory))
                //    .OrderByDescending(m=>m.Id).Take(task));
                //List<int> list = new List<int>();

                var databycode = Entities.Where(m => m.IdPattern == null && listid.Contains(m.Id)).OrderBy(m => m.Sort);
                foreach (var item in databycode)
                {
                    List<int> list = new List<int>();
                    list.Add(item.Id);
                    List<CategoryProduct> listCategoryProduct = new List<CategoryProduct>();
                    var getAllChild = Entities.Where(m => m.IdPattern == item.Id && listid.Contains(m.Id)).Include(m => m.CategoryChilds);
                    list.AddRange(getAllChild.Select(x => x.Id).ToList());
                    foreach (var itemch in getAllChild)
                    {
                        if (itemch.CategoryChilds.Count() > 0)
                        {
                            list.AddRange(itemch.CategoryChilds.Select(x => x.Id).ToList());
                        }
                    }

                    if (list.Count() > 0)
                    {
                        if (checkActiveProduct)
                        {
                            item.Products = _repositoryProduct.GetProductbyListCategoryId(list.ToArray()).Where(m => m.Active && !m.isRunPromotion).OrderByDescending(m => m.Id).Take(task).ToList();
                        }
                        else
                        {
                            item.Products = _repositoryProduct.GetProductbyListCategoryId(list.ToArray()).Where(m => !m.isRunPromotion).OrderByDescending(m => m.Id).Take(task).ToList();
                        }

                    }
                }
                return databycode;
            }
            catch (Exception e)
            {
                _log.LogError(e, e.Message);
                return null;
            }

        }

        public async Task<List<CategoryProduct>> GetAllAsync()
        {
            return await _repository.GetAllQueryable().AsNoTracking().ToListAsync();
        }

        public async Task<List<CategoryProduct>> GetListChillAllByIdAsync(int id)
        {
            var getArr = await this.GetListArrayChillAllByIdAsync(id);
            return await Entities.Where(m => getArr.Contains(m.Id)).ToListAsync();
        }
        public async Task<int[]> GetListArrayChillAllByIdAsync(int id)
        {
            List<int> list = new List<int>();
            List<CategoryProduct> listfull = new List<CategoryProduct>();

            list.Add(id);

            var databycode = await Entities.Where(m => m.IdPattern == id).OrderBy(x => x.Id).ToListAsync();
            listfull.AddRange(databycode);
            while (listfull.Count() > 0)
            {
                var getlast = listfull.LastOrDefault();
                listfull.Remove(getlast);
                if (getlast != null)
                {
                    list.Add(getlast.Id);
                    databycode = await Entities.Where(m => m.IdPattern == getlast.Id).OrderBy(x => x.Id).ToListAsync();
                    if (databycode.Count() > 0)
                    {
                        listfull.AddRange(databycode);
                    }
                }
            }
            return list.ToArray();
        }

        public async Task<CategoryProduct> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
    }
    public class CategoryProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class CategoryPostRepository : ICategoryPostRepository<CategoryPost>
    {
        private readonly ILogger<CategoryPostRepository> _log;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<CategoryPost> _repository;
        private readonly ITypeCategoryRepository<TypeCategory> _typeCategory;
        public IQueryable<CategoryPost> Entities => _repository.Entities;
        public CategoryPostRepository(IRepositoryAsync<CategoryPost> repository,
            ILogger<CategoryPostRepository> log,
            ITypeCategoryRepository<TypeCategory> typeCategory,
            IDistributedCache distributedCache)
        {
            _log = log;
            _typeCategory = typeCategory;
            _repository = repository; _distributedCache = distributedCache;
        }
        public async Task<Task> DeleteByIdPattern(int idPattern)
        {
            var categories = Entities.Where(m => m.IdPattern == idPattern);
            if (categories != null && categories.Count() > 0)
            {
                await _repository.DeleteRangeAsync(categories);
            }

            return Task.CompletedTask;
        }
        public async Task<CategoryPost> GetBySlugAndTypeAsync(string slug, int? idType)
        {
            string code = "bat-dong-san";
            if (idType == 1) // là phụ tùng 0 là Post
            {
                code = "tin-tuc";
            }
            var data = _typeCategory.GetByCode(code);
            return await _repository.Entities.Where(m => m.Code == slug && m.IdTypeCategory == data.Id).Include(m => m.TypeCategory).SingleOrDefaultAsync();
        }
        public CategoryPost GetByIdTypeCategory(int id)
        {
            var categories = _repository.GetAll(expression: m => m.IdTypeCategory == id).FirstOrDefault();
            return categories;
        }
        public async Task<List<CategoryPost>> GetByIdTypeAsync(int idType)
        {
            string code = "bat-dong-san";
            if (idType == 1) // là phụ tùng 0 là Post
            {
                code = "tin-tuc";
            }
            var data = _typeCategory.GetByCode(code);
            return await this.GetListByIdTypeCategoryAsync(data.Id);
        }
        public async Task<List<CategoryPost>> GetByCodeAsync(string code)
        {
            var data = _typeCategory.GetByCode(code);
            return await this.GetListByIdTypeCategoryAsync(data.Id);
        }
        public List<CategoryPost> GetByCode(string code)
        {
            var data = _typeCategory.GetByCode(code);
            return this.GetListByIdTypeCategory(data.Id);
        }
        private List<CategoryPost> GetListByIdTypeCategory(int idtype)
        {
            var data = _repository.Entities.Where(m => m.IdTypeCategory == idtype).ToList();
            return data;
        }
        public int CountbyIdTypeCategory(int id)
        {
            var categories = _repository.GetAll(expression: m => m.IdTypeCategory == id).Count();
            return categories;
        }

        public IQueryable<CategoryPost> GetListByCode(string code)
        {
            var data = _repository.Entities.Where(m => m.Code == code.ToLower()).SingleOrDefault();
            return _repository.Entities.Where(m => m.IdPattern == data.Id);
        }
        public async Task<List<CategoryPost>> GetListByIdPatternAsync(int id)
        {
            var data = await _repository.Entities.Where(m => m.IdPattern == id).ToListAsync();
            return data;
        }
        public async Task<List<CategoryPost>> GetListByIdTypeCategoryAsync(int idtype)
        {
            var data = await _repository.Entities.Where(m => m.IdTypeCategory == idtype).ToListAsync();
            return data;
        }
        public async Task<List<CategoryMenuModel>> GetListByCodeCacheAsync(string code)
        {
            try
            {
                string cachekey = CategoryCacheKeys.GetKey(code);
                var productList = await _distributedCache.GetAsync<List<CategoryMenuModel>>(cachekey);
                if (productList == null)
                {
                    var get = _typeCategory.GetByCode(code);
                    var data = _repository.Entities.Where(m => m.IdPattern == get.Id).Include(m => m.TypeCategory).Select(x => new CategoryMenuModel
                    {
                        Id = x.Id,
                        IdLevel = x.IdLevel,
                        IdPattern = x.IdPattern,
                        IdTypeCategory = x.IdTypeCategory,
                        Name = x.Name,
                        Code = x.Code,
                        Slug = x.Code,
                        slugPattern = x.TypeCategory != null ? x.TypeCategory.Code : string.Empty
                    }).ToList();
                    //  productList = _repository.Entities.AsNoTracking().Where(m => m.IdPattern == get.Id).Include(m=>m.TypeCategory).ToList();
                    productList = data;
                    await _distributedCache.SetAsync(cachekey, productList);
                }
                return productList;
            }
            catch (Exception e)
            {
                _log.LogError(e, e.Message);
                throw;
            }

        }

        public IEnumerable<CategoryPost> GetListIncludePost(int[] listid, int task = 10)
        {
            try
            {
                var databycode = Entities.Where(m => listid.Contains(m.Id)).Include(m => m.Posts.OrderByDescending(m => m.Id).Take(task));
                return databycode;
            }
            catch (Exception e)
            {
                _log.LogError(e, e.Message);
                return null;
            }

        }

        public async Task<CategoryPost> GetByIdAsync(int Id)
        {
            return await _repository.GetByIdAsync(Id);
        }

        public async Task<int[]> GetListArrayChillAllByIdAsync(int id)
        {

            List<int> list = new List<int>();
            List<CategoryPost> listfull = new List<CategoryPost>();

            list.Add(id);

            var databycode = await Entities.Where(m => m.IdPattern == id).OrderBy(x => x.Id).ToListAsync();
            listfull.AddRange(databycode);
            while (listfull.Count() > 0)
            {
                var getlast = listfull.LastOrDefault();
                listfull.Remove(getlast);
                if (getlast != null)
                {
                    list.Add(getlast.Id);
                    databycode = await Entities.Where(m => m.IdPattern == getlast.Id).OrderBy(x => x.Id).ToListAsync();
                    if (databycode.Count() > 0)
                    {
                        listfull.AddRange(databycode);
                    }
                }
            }
            return list.ToArray();

        }
    }
}
