using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IDetailtKitchenRepository<T> where T : class
    {
        void Add(T entity);
        Task AddRangeAsync(List<T> entity);
        Task UpdateRangeAsync(List<T> kitchens);
        Task UpdateRangeAsync(List<Kitchen> kitchens);
        Task Remove(int idKitchen);
        Task RemoveRangeAsync(List<int> kitchens);
    }
}
