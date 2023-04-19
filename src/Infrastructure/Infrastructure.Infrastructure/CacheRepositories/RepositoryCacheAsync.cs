using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreHero.ThrowR;
using Application.CacheKeys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Application.Extensions.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Infrastructure.CacheRepositories
{
    public class RepositoryCacheAsync<T> : IRepositoryCacheAsync<T> where T : class
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<T> _Repository;
        public RepositoryCacheAsync(IDistributedCache distributedCache,
             IRepositoryAsync<T> Repository)
        {
            _Repository = Repository;
            _distributedCache = distributedCache;
        }
        public async Task<T> GetByIdAsync(int productId, string cacheKey = "")
        {
            // cacheKey = PermissionCacheKeys.GetKey(productId);
            var product = await _distributedCache.GetAsync<T>(cacheKey);
            if (product == null)
            {
                product = await _Repository.GetByIdAsync(productId);
                if (product == null)
                {
                    return null;
                }
                await _distributedCache.SetAsync(cacheKey, product);
            }
            // var product = await _Repository.GetByIdAsync(productId);
            return product;
        }
        public async Task<T> GetFirstAsync(string cacheKey)
        {
            var product = await _distributedCache.GetAsync<T>(cacheKey);
            if (product == null)
            {
                product = await _Repository.GetFirstAsNoTrackingAsync();
                if (product==null)
                {
                    return null;
                }
                await _distributedCache.SetAsync(cacheKey, product);
            }
            return product;
        }

        public async Task<List<T>> GetCachedListAsync(string cacheKey = "", Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            try
            {
                var productList = await _distributedCache.GetAsync<List<T>>(cacheKey);
                if (productList == null)
                {
                    var data = _Repository.GetAllQueryable().AsNoTracking();
                    productList = data.ToList();

                    await _distributedCache.SetAsync(cacheKey, productList);
                }
                return productList;
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public async Task<IQueryable<T>> GetCachedIQueryableAsync(string cacheKey = "")
        {
            try
            {
                var productList = await _distributedCache.GetAsync<List<T>>(cacheKey);
                if (productList == null)
                {
                    var data = _Repository.GetAllQueryable();
                    productList = data.ToList();

                    await _distributedCache.SetAsync(cacheKey, productList);
                }
                return productList.AsQueryable();
            }
            catch (Exception e)
            {

                throw;
            }

        }

    }
}
