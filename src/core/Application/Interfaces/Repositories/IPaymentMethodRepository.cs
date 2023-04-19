using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IPaymentMethodRepository
    {
        Task AddAsync(PaymentMethod model);
        Task<bool> DeleteAsync(int ComId, int Id);
        IQueryable<PaymentMethod> GetAll(int comid, bool? Active=null);
        Task<bool> UpdateAsync(PaymentMethod model);
    }
}
