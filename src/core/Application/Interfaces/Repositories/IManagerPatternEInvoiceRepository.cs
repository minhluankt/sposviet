using Application.Enums;
using AspNetCoreHero.Results;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IManagerPatternEInvoiceRepository<T> where T : class
    {
        IQueryable<T> Entities { get; }
        Task<IResult> DeleteAsync(int Id, int Comid, ENumSupplierEInvoice TypeSupplierEInvoice);
        Task AddAsync(T Entity);
        Task<T> GetbykeyAsync(string key);
        Task<T> GetbyIdAsync(int id, bool AsNoTracking = false);
        Task<IResult> UpdateAsync(T Entity);
        Task UpdateRangeAsync(List<T> Entity);
        IQueryable<T> GetAllAsync();
    }
}
