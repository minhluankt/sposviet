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
    public interface ISupplierEInvoiceRepository<T> where T : class
    {
        IQueryable<T> Entities { get; }
        Task AddAsync(T Entity);
        Task<IResult> UpdateAsync(T Entity, int Comid);
        Task<IResult> DeleteAsync(int Id,int Comid);
        Task<SupplierEInvoice> GetByIdAsync(int Comid, ENumSupplierEInvoice TypeSupplierEInvoice);
    }
}
