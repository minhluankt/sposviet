using Application.Enums;
using Application.Hepers;
using AspNetCoreHero.Results;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IAutoSendTimerRepository<T> where T : class
    {
        IQueryable<T> Entities { get; }
        Task<T> GetById(int id,int ComId);
        Task<T> AddAsync(T entity);
        Task<IResult> DeleteAsync(int Id, int Comid, ENumSupplierEInvoice TypeSupplierEInvoice);
        Task<Result<AutoSendTimer>> UpdateAsync(AutoSendTimer entity);
        Task<PaginatedList<T>> GetPageList(int ComId, string sortColumn, string sortColumnDirection, int Currentpage, int pageSite, ENumSupplierEInvoice eNumSupplierEInvoice);
        //--------------job
        Task StartJobEInvoiceAsync(T entity);
        Task<IResult> DeleteJobAsync(int Id, int Comid, ENumSupplierEInvoice TypeSupplierEInvoice);
        Task<IResult> StartJobAsync(int Id, int Comid, ENumSupplierEInvoice TypeSupplierEInvoice);
    }
}
