using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IHistoryOrderRepository<T> where T : class
    {
        Task AddHistoryOrder(List<T> model);
    }
}
