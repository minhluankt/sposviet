using Application.Enums;
using Application.Hepers;
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
        Task<PaginatedList<T>> GetPageList(int ComId, string sortColumn, string sortColumnDirection, int Currentpage, int pageSite, ENumSupplierEInvoice eNumSupplierEInvoice);
    }
}
