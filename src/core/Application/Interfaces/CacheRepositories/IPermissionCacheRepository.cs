using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.CacheRepositories
{
    public interface IPermissionCacheRepository
    {
        Task<List<Permission>> GetCachedListAsync();
        Task<Permission> GetByIdAsync(int id);
    }
}
