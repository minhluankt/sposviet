using Application.Enums;
using Application.Hepers;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using System;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IInvoicePepository<T> where T : class
    {
        Task<Result<PublishInvoiceModelView>> CancelInvoice(Guid IdInvoice,int ComId,string CasherName,string Note, EnumTypeEventInvoice type);
        Task<Result<PublishInvoiceModelView>> UpdateCustomerInvoice(Guid IdInvoice,int ComId,int IdCustomer,string CasherName);
        Task<Result<PublishInvoiceModelView>> DeleteIsMergeInvoice(Guid IdInvoice,int ComId,string CasherName);
        Task<Result<PublishInvoiceModelView>> CancelInvoice(int[] lstid,int ComId,string CasherName,string Note, EnumTypeEventInvoice type,bool IsDelete,bool IsDeletePT = false);
        Task<Result<PublishInvoiceModelView>> PublishEInvoiceMerge(PublishInvoiceMergeModel model, int ComId);
        Task<Result<PublishInvoiceModelView>> PublishInvoice(PublishInvoiceModel model);
        Task<Result<PublishInvoiceResponse>> PublishInvoice(T Invoice, PublishInvoiceModel model, int ComId, string IdCasher, string CasherName);
        Task AddAsync(T entity);
        Task JobDeleteInvoiceAsync();
        Task<PaginatedList<Invoice>> GetAllDatatableAsync(int? Comid, InvoiceModel textSearch, string sortColumn, string sortColumnDirection, int pageSize, int skip, EnumTypeProduct enumTypeProduct = EnumTypeProduct.THOITRANG);
    }
}
