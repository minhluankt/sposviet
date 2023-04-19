using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IManagerIdCustomerRepository
    {
        Task<ManagerIdCustomer> GetByComIdAsync(int comid);
        Task<int> UpdateIdAsync(int comid);
    }
}
