using Application.Enums;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IOrderTableRepository
    {
        Task<Result<string>> ConvertInvoice(int comid, Guid idOrder, EnumTypeProduct enumTypeProduct = EnumTypeProduct.BAN_LE);
        Task<Result<string>> GenHtmlPrintBep(List<NotifyOrderNewModel> model, int ComId);
        Task<Result<string>> AddNote(int comid, Guid idOrder, string note);
        Task<Result<OrderTable>> UpdateTableOrRoomOfOrder(int comid,bool isBringBack, Guid idOrder, Guid? idOldTableOrder, Guid? idRoomOrtable, EnumTypeProduct enumTypeProduct = EnumTypeProduct.AMTHUC);
        Task<Result<OrderTableModel>> RemoveOrder(int comid, Guid idOrder, string CasherName, string IdCashername, EnumTypeProduct enumTypeProduct = EnumTypeProduct.AMTHUC);
        Task<Result<bool>> RemoveCustomerOrder(int comid, Guid idOrder, EnumTypeProduct enumTypeProduct);
        Task<Result<bool>> SplitOrderAsync(int comid, Guid idOrderOld, List<Guid> lstIdoder, EnumTypeSpitOrder Type, string CasherName, string IdCashername);
        Task<Result<bool>> SplitOrderSeparateAsync(int comid, Guid idOrderOld, List<DetailtSpitModel> lstitem, bool IsCreate, bool IsBringBack, Guid? IdOrderNew, Guid? IdTable, string CasherName, string IdCasher);
        Task<Result<PublishInvoiceResponse>> CheckOutOrderAsync(int comid, int Idpayment, Guid idOrder, decimal discountPayment,  decimal discount, decimal? AmountCusPayment,decimal Amount,decimal VATAmount, string Cashername, string IdCasher, bool vat, int? Vatrate, int? ManagerPatternEInvoices, EnumTypeProduct enumType = EnumTypeProduct.AMTHUC);
        Task<Result<PublishInvoiceResponse>> CheckOutOrderInvoiceAsync(OrderInvoicePaymentSaleRetailModel model, EnumTypeProduct enumType = EnumTypeProduct.BAN_LE);
        Task UpdateCustomerOrder(int comid, Guid idOrder, Customer customer, EnumTypeProduct enumTypeProduct = EnumTypeProduct.AMTHUC);
        Task<Result<OrderTable>> UpdateItemOrderAsync(int? IdCustomer, string CusCode, int comid, Guid idOrder, Guid idItem, Guid? idTable, bool IsBringBack, decimal Quantity, string Cashername, string IdCasher, string note = "", bool IsRemoverow = false, bool IsCancelItem = false);
        Task<Result<OrderTable>> UpdateAllQuantityOrderTable(int comid, Guid idOrder, Guid idOrderItem, decimal quantity);
        Task<Result<OrderTable>> AddOrUpdateOrderTable(bool IsNewOrder, OrderTable model, OrderTableItem item);
        Task<OrderTable> GetByIdAsync(int id);
        IQueryable<OrderTable> GetOrderByBringback(int ComId, EnumStatusOrderTable enumStatusOrderTable, EnumTypeProduct enumTypeProduct);
        IQueryable<OrderTable> GetOrderInvoiceRetail(int ComId, EnumStatusOrderTable enumStatusOrderTable, EnumTypeProduct enumTypeProduct = EnumTypeProduct.BAN_LE);
    }
}
