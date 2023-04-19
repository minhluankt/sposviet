using Application.CacheKeys;
using Application.Extensions.Caching;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.CacheRepositories
{
    public class CustomerCacheRepository : ICustomerCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<Customer> _Repository;
        public CustomerCacheRepository(IDistributedCache distributedCache,
             IRepositoryAsync<Customer> Repository)
        {
            _Repository = Repository;
            _distributedCache = distributedCache;
        }
        public async Task<Customer> GetByIdAsync(int productId)
        {
            //string cacheKey = PermissionCacheKeys.GetKey(productId);
            //var product = await _distributedCache.GetAsync<Customer>(cacheKey);
            //if (product == null)
            //{
            //    product = await _Repository.GetByIdAsync(productId);
            //    Throw.Exception.IfNull(product, " Customer", "No  Customer Found");
            //    //await _distributedCache.SetAsync(cacheKey, product);
            //}
            var product = await _Repository.GetByIdAsync(productId);
            return product;
        }

        public async Task<IQueryable<Customer>> GetCachedListAsync(int idtype)
        {
            string cacheKey = CustomerCacheKeys.ListKey;
            var productList = await _distributedCache.GetAsync<List<Customer>>(cacheKey);
            if (productList == null || productList.Count() == 0)
            {
                productList = await _Repository.GetAllAsync();
                await _distributedCache.SetAsync(cacheKey, productList);
            }
            return productList.AsQueryable();
        }
    }
}
