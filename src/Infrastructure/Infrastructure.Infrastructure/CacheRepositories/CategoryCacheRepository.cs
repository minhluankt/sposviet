using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using Domain.Entities;

using System;
using AspNetCoreHero.Extensions.Caching;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreHero.ThrowR;
using Application.CacheKeys;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Infrastructure.CacheRepositories
{
    public class CategoryCacheRepository : ICategoryCacheRepository
    {
        private readonly ILogger<CategoryCacheRepository> _log;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<CategoryProduct> _Repository;
        public CategoryCacheRepository(IDistributedCache distributedCache, ILogger<CategoryCacheRepository> log,
             IRepositoryAsync<CategoryProduct> Repository)
        {
            _log = log;
            _Repository = Repository;
            _distributedCache = distributedCache;
        }
        public async Task<List<CategoryProduct>> GetListIncludeProduct(string code,string cache, int[] listid=null)
        {
            try
            {
                var productList = await _distributedCache.GetAsync<List<CategoryProduct>>(cache);
                var databycode = _Repository.Entities.Where(m => m.Code == code.ToLower()).SingleOrDefault();
                if (productList == null)
                {
                    if (listid!=null && listid.Length >0)
                    {
                        productList =  _Repository.Entities.Where(m=> listid.Contains(m.Id) && m.IdPattern== databycode.Id).Include(m=>m.Products).ToList();
                   
                    }
                    else
                    {
                        productList =  _Repository.Entities.Where(m=>m.IdPattern==databycode.Id).Include(m => m.Products).ToList();
                        
                    }
                    await _distributedCache.SetAsync(cache, productList);
                }
                return productList;
            }
            catch (Exception e)
            {
                _log.LogError(e,e.Message);
                return new List<CategoryProduct>();
            }
        }
    }
}
