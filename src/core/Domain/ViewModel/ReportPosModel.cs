using Application.Enums;
using Domain.Entities;
using HelperLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class SearchReportPosModel
    {
        public int Comid { get; set; }
        public int idCategory { get; set; }
        public string productname { get; set; }
        public string productcode { get; set; }
        public string FromDate { get; set; }
        public string rangesDate { get; set; }
        public DateTime? srartDate {
            get {
                if (!string.IsNullOrEmpty(rangesDate))
                {
                    var splt = rangesDate.Split('-');
                    return LibraryCommon.ConvertStringToDateTime(splt[0]);
                }
                return null;
            }
        }
        public DateTime? endDate {
            get {
                if (!string.IsNullOrEmpty(rangesDate))
                {
                    var splt = rangesDate.Split('-');
                    return LibraryCommon.ConvertStringToDateTime(splt[1]);
                }
                return null;
            }
            set { }
        }
        public EnumTypeReportPos typeReportPos { get; set; }
        public EnumTypeReportProduct typeReportProduct { get; set; }
        public EnumTypeReportDashboard TypeReportDashboard { get; set; }
        public EnumTypeReportEInvoice TypeReportEInvoice { get; set; }
    }
    public class InvoiceDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public int IdItemInvoice { get; set; }
        public int IdProduct { get; set; }
        public int IdInvoice { get; set; }
        public string CategoryProductName { get; set; }
        public string Code { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal EntryPrice { get; set; }// giá vóno
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
    public class ReportXuatNhapTonKho
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
        public string CategoryProductName { get; set; }
        public decimal SoLuongTonDauKy { get; set; }
        public decimal ThanhTienTonDauKy { get; set; }
        public decimal SoLuongTonCuoiKy { get; set; }
        public decimal ThanhTienTonCuoiKy { get; set; }
        public decimal SoLuongNhapTrongKy { get; set; }
        public decimal ThanhTienNhapTrongKy { get; set; }
        public decimal SoLuongXuatTrongKy { get; set; }
        public decimal ThanhTienXuatTrongKy { get; set; }
    }
    public class ResponseReport
    {
        public List<Product> Products { get; set; }
        public EnumTypeReportPos TypeReportPos { get; set; }
        public ReportPosModel ReportPosModel { get; set; }
        public ReportHuyDon ReportHuyDon { get; set; }
        public List<ReportXuatNhapTonKho> ReportXuatNhapTonKhos { get; set; }
        public ReportHinhThucPhucVu ReportHinhThucPhucVu { get; set; }
        public bool isShowChart { get; set; }
        public byte[] dataExcel { get; set; }
    }
    public class ReportPosModel {
        public int InvoiceAll { get; set; }//tổng hóa đơn
        public int InvoiceCancel { get; set; }//tổng hóa đơn hủy
        public decimal Product { get; set; }//số lượng mặt hàng
        public decimal Amount { get; set; }//Tổng doanh thu
        public decimal DiscountAmount { get; set; }//Tổng doanh thu
        public List<ListItemReport> ListItemReports { get; set; }// list item theo ngày
        public List<ListItemReport> ListItemReportsByInvoice { get; set; }// list item theo ngày
        public List<Chart> Charts { get; set; }// list item chart

    }
    public class ReportHuyDon
    {
        public List<ListItemReportHuyDon> ListItemReports { get; set; }// theo hình thức
        public List<Chart> Charts { get; set; }// list item chart
    }
    public class ReportProduct
    {
        public List<ItemReportProduct> ListItemReports { get; set; }// theo hình thức
        public List<ItemReportProductDetailt> ListItemReportDetails { get; set; }// theo hình thức
        public List<Chart> Charts { get; set; }// list item chart
    }
    public class ReportHinhThucPhucVu
    {
        public List<ListItemReportHinhThucPhucVu> ListItemReports { get; set; }// theo hình thức
        public List<Chart> Charts { get; set; }// list item chart
    }
    public class Chart
    {
        public string Key { get; set; }
        public decimal Value { get; set; }
        public decimal Value2 { get; set; }
    }
    public class ListItemReportHinhThucPhucVu
    {
        public string Name { get; set; }
        public int InvoicePaymentCount { get; set; }//hóa đơn thanh toán
        public int InvoicePartCount { get; set; }//hoàn tiền 1 phần
        public int InvoiceCancelCount { get; set; }//hủy hoặc hoàn tiền all
        public decimal Amount { get; set; }
        public decimal VATAmount { get; set; }
    }
    public class ItemReportProduct
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int IdCategoryProduct { get; set; }
        public string CategoryName { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
        public decimal Price { get; set; }
        public decimal EntryPrice { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
    }
    public class ItemReportProductDetailt : ItemReportProduct
    {
        public string CreateDate { get; set; }
        public string Buyer { get; set; }
        public string InvoiceNo { get; set; }
        public string CusCode { get; set; }
        [JsonIgnore]
        public int IdInvoice { get; set; }

    }

    public class ListItemReportHuyDon
    {
        public string CasherName { get; set; }
        public string CreatDate { get; set; }
        public string InvoiceNo { get; set; }
        public int InvoiceId { get; set; }
        public string Note { get; set; }//lý do hủy
        public decimal VATAmount { get; set; }
        public decimal Amount { get; set; }
        [JsonIgnore]
        public DateTime Date { get; set; }
    }
    public class ListItemReport
    {
        public string RoomName { get; set; }//bàn
        public int InvoiceId { get; set; }
        public string InvoiceCode { get; set; }
        public string CasherName { get; set; }
        public string RankName { get; set; }//thứ 2, thứ 3
        public string Buyer { get; set; }//thứ 2, thứ 3
        public decimal VATAmount { get; set; }//tổng tiền hàng
        public decimal Total { get; set; }//tổng tiền hàng
        public decimal TotalCancel { get; set; }//tổng tiền hàng của các hóa đơn hủy
        public decimal DiscountAmount { get; set; }//giảm giá
        public string StatusName { get; set; }//giảm giá
        public EnumStatusInvoice Status { get; set; }//giảm giá
        public decimal Amount { get; set; }//tổng tiền các hóa đơn đã thanh toán
        public decimal ServiceChargeAmount { get; set; }//phí dịch vụ
        public string CreateDate { get; set; }
        [JsonIgnore]
        public DateTime Date { get; set; }
        [JsonIgnore]
        public int Hour { get; set; }


    }

    public class ReportMonthProductEInvoice{
        public string Patern { get; set; }
        public string Serial { get; set; }
        public int InvoiceNo { get; set; }
        public DateTime SignDate { get; set; }
        public string CusName { get; set; }
        public string Buyer { get; set; }
        public string Payment { get; set; }
        public string TaxCode { get; set; }
        public string Address { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ProductUnit { get; set; }
        public decimal ProductQuantity { get; set; }
        public decimal ProductPrice { get; set; }
        public float ProductVATRate { get; set; }
        public decimal ProductTotal { get; set; }
        public decimal ProductVATAmount { get; set; }
        public string Note { get; set; }
        public StatusEinvoice Status { get; set; }
    }
}
