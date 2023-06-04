using Application.CacheKeys;
using Application.Interfaces.CacheRepositories;
using Application.Extensions.Caching;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Identity;

namespace Infrastructure.Infrastructure.CacheRepositories
{
    public class UserManagerCacheRepository : IUserManagerCacheRepository<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDistributedCache _distributedCache;
        public UserManagerCacheRepository(IDistributedCache distributedCache, UserManager<ApplicationUser> userManager)
        {
            _distributedCache = distributedCache;
            _userManager = userManager;
        }

        public List<ApplicationUser> GetListDataUserCache()
        {
            var productList = _distributedCache.Get<List<ApplicationUser>>(UsersCacheKeys.ListKey);
            if (productList == null)
            {
                productList = _userManager.Users.ToList();
                _distributedCache.Set(UsersCacheKeys.ListKey, productList);
            }
            return productList;
        }

    }

}
