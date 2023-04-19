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


namespace Infrastructure.Infrastructure.Repositories
{
    public class EmailRepository : IEmailRepository<MailSettings>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<MailSettings> _Repository;
        public EmailRepository(IDistributedCache distributedCache,
             IRepositoryAsync<MailSettings> Repository)
        {
            _Repository = Repository;
            _distributedCache = distributedCache;
        }
        public async Task<MailSettings> GetMailSettingCacheAsync()
        {
            string cacheKey = MailSetting.Getkey;
            var product = await _distributedCache.GetAsync<MailSettings>(cacheKey);
            if (product == null)
            {
                product = await _Repository.GetFirstAsNoTrackingAsync();
                await _distributedCache.SetAsync(cacheKey, product);
            }
            return product;
        } 
        public MailSettings GetMailSetting()
        {
            return _Repository.GetFirstAsNoTracking(); 
        }
    }
}
