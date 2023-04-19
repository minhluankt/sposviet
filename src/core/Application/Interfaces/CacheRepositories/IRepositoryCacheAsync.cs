using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.CacheRepositories
{
    public interface IRepositoryCacheAsync<T> where T : class
    {
        Task<List<T>> GetCachedListAsync(string cacheKey = "", Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        Task<IQueryable<T>> GetCachedIQueryableAsync(string cacheKey = "");
        Task<T> GetByIdAsync(int id, string cacheKey = "");
        Task<T> GetFirstAsync(string cacheKey = "");
    }
}
