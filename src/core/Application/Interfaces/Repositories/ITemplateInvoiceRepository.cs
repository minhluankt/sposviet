using Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface ITemplateInvoiceRepository<T> where T : class
    {
        IQueryable<T> GetAllAsync(int ComId);
        IQueryable<T> Entities { get; }
        Task AddAsync(TemplateInvoice model);
        Task Delete(int ComId, int id);
        Task<TemplateInvoice> GetByIdAsync(int ComId, int id);
        Task<TemplateInvoice> GetTemPlate(int ComId);
        Task UpdateAsync(TemplateInvoice model);
    }

}
