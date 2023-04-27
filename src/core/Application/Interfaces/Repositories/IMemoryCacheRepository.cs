using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IMemoryCacheRepository
    {
        void CacheRemoce(string key);
        T CacheTryGetValue<T>(string key);
        List<T> CacheTrySetValue<T>(string key, List<T> value, double FromSeconds = 30);
        T CacheTrySetValue<T>(string key, T value, double FromSeconds = 30);
        List<T> CacheTryGetValueSet<T>(string key, List<T> value, double FromSeconds = 30);
    }
}
