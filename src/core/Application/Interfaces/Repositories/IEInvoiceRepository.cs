using Application.Enums;
using Application.Hepers;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IEInvoiceRepository<T> where T : class
    {
        IQueryable<T> Entities { get; }
        Task<IResult<string>> DownloadInvFkeyNoPayAsync(int IdEInvoice, int Comid, string Carsher, string IdCarsher);
        Task<IResult<string>> ConvertForStoreFkeyMutiInvoiceAsync(int[] lstid, int Comid, ENumTypePrint typePrint, string Carsher, string IdCarsher);
        Task<IResult<string>> ConvertForStoreFkeyAsync(int IdEInvoice, int Comid, string Carsher, string IdCarsher);
        Task<IResult<string>> GetHashTokenVNPTAsync(int[] lstid, int Comid);
        Task<IResult<string>> CheckConnectWebserviceAsync(string doamin, string userservice, string passservice, string useradmin, string passadmin);
        Task<IResult<PublishInvoiceModelView>> PublishInvoiceAsync(int[] lst, int ComId, string Carsher, string IdCarsher);
        Task<IResult<PublishInvoiceModelView>> SycnInvoiceAsync(int[] lst, int ComId, string Carsher, string IdCarsher, EnumTypeSyncEInvoice TypeSyncEInvoice);
        Task CreateAsync(T Entity, string Carsher, string IdCarsher);
        Task<IResult<PublishInvoiceModelView>> SendCQTAsync(int[] lstid, int Comid, string Carsher, string IdCarsher);
        Task<IResult<PublishInvoiceModelView>> SendCQTTokenAsync(int[] lstid, string dataXml, int Comid, string Carsher, string IdCarsher);
        Task UpdateAsync(T Entity);
        Task<T> FindByIdAsync(int id, bool asNotracking = false);
        Task CreateRangeAsync(List<T> Entity);
        Task<IResult<string>> GetInvViewFkeyAsync(int IdEInvoice, int Comid, string Carsher, string IdCarsher);
        Task<IResult<string>> ImportAndPublishInvMTTAsync(T einvoice, SupplierEInvoice company, string Carsher, string IdCarsher);
        //Task<IResult<string>> ImportAndPublishInvMTTAsync(int ComId, int Id, string pattern, string serial, string Carsher, string IdCarsher);
        IQueryable<T> GetAllAsync();
        Task<DashboardEInvoiceModel> GetDashboardAsync(int Comid, DateTime date);
        Task<IResult<PublishInvoiceModelView>> CancelEInvoiceAsync(int[] lst, int ComId, string Carsher, string IdCarsher);
        Task<IResult<PublishInvoiceModelView>> RemoveEInvoiceAsync(int[] lst, int ComId, string Carsher, string IdCarsher);
        Task<PaginatedList<T>> GetAllDatatableAsync(int? Comid, InvoiceModel textSearch, string sortColumn, string sortColumnDirection, int pageSize, int skip,EnumTypeProduct enumTypeProduct = EnumTypeProduct.THOITRANG);
        //report
        Task<List<EInvoice>> GetReportMonth(DateTime todate, DateTime enddate, int ComId);
      
    }
}
