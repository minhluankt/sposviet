using Application.CacheKeys;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Extensions.Caching;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.CacheRepositories
{
    public class PermissionCacheRepository : IPermissionCacheRepository
    {

        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<Permission> _Repository;
        public PermissionCacheRepository(IDistributedCache distributedCache,
             IRepositoryAsync<Permission> Repository)
        {
            _Repository = Repository;
            _distributedCache = distributedCache;
        }
        public async Task<Permission> GetByIdAsync(int productId)
        {
            //string cacheKey = PermissionCacheKeys.GetKey(productId);
            //var product = await _distributedCache.GetAsync<Permission>(cacheKey);
            //if (product == null)
            //{
            //    product = await _Repository.GetByIdAsync(productId);
            //    Throw.Exception.IfNull(product, " Permission", "No  Permission Found");
            //    //await _distributedCache.SetAsync(cacheKey, product);
            //}
            var product = await _Repository.GetByIdAsync(productId);
            return product;
        }

        public async Task<List<Permission>> GetCachedListAsync()
        {
            string cacheKey = PermissionCacheKeys.ListKey;
            var productList = await _distributedCache.GetAsync<List<Permission>>(cacheKey);
            if (productList == null)
            {
                var data = _Repository.GetAllQueryable().AsNoTracking();
                productList = data.ToList();

                await _distributedCache.SetAsync(cacheKey, productList);
            }
            return productList;
        }

    }
}
