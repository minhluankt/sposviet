using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using HelperLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class EInvoice : AuditableEntity
    {
        public EInvoice() {
            this.Fkey = Guid.NewGuid();
            this.FkeyEInvoice = KeyGenerator.GetUniqueKey(10);
            this.EInvoiceItems = new List<EInvoiceItem>();
        }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }// vi dụ VNPT
        //[DefaultValue(EnumTypeVATEinvoice.VAT_MTT)]
        //public EnumTypeVATEinvoice TypeVATEinvoice { get; set; }// là hóa đơn thường hay hóa owdn MTT
        public int ComId { get; set; }//
        public int IdManagerPatternEInvoice { get; set; }//mẫu số ký hiệu nào
        public int IdInvoice { get; set; }//cua đơn nào ở hóa đơn bán hàng
        [StringLength(50)]
        public Guid Fkey { get; set; }//tên người tạo
        [StringLength(20)]
        public string FkeyEInvoice { get; set; }//tên người tạo
        public int? IdCustomer { get; set; }// khách hàng
        public int? IdHoaDonHKD { get; set; }// id hóa đơn của HKD
        public bool IsDelete { get; set; }//Hóa đơn xóa bỏ khỏi hệ thốn
        public int InvoiceNo { get; set; }//số hóa đơn
        [StringLength(50)]
        public string MCQT { get; set; }//mã cơ quan thuế
        [StringLength(20)]
        public string EInvoiceCode { get; set; }//mã hóa đơn điện tử
        [StringLength(20)]
        public string InvoiceCode { get; set; }//mã đơn hàng
        [StringLength(15)]
        public string Pattern { get; set; }//tên người tạo
        [StringLength(15)]
        public string Serial { get; set; }//tên người tạo
        [StringLength(50)]
        public string CasherName { get; set; }//tên người tạo
        public DateTime? ArisingDate { get; set; }//
        public DateTime PublishDate { get; set; }//
        public decimal? ExchangeRate { get; set; }// tyr gias
        [StringLength(50)]
        public string CurrencyUnit { get; set; }// dwon vij VND USD
        [StringLength(500)]
        public string AmountInWords { get; set; }//tên khách
        public string CusPhone { get; set; }//tên khách
        [StringLength(200)]
        public string Buyer { get; set; }//tên khách
        [StringLength(50)]
        public string CusCode { get; set; }//ma khách
        [StringLength(50)]
        public string CCCD { get; set; }//cccd khách
        [StringLength(50)]
        public string EmailDeliver { get; set; }//email khách
        [StringLength(500)]
        public string CusName { get; set; }//tên
        public string CusTaxCode { get; set; }//mst
        public string Address { get; set; }//
        [StringLength(200)]
        public string PaymentMethod { get; set; }//
        [StringLength(50)]
        public string CusBankNo { get; set; }
        [StringLength(400)]
        public string CusBankName { get; set; }
        public decimal Total { get; set; }//
        public float? Discount { get; set; }//
        public decimal? DiscountAmount { get; set; }//chiết khấu
        public decimal TGTKCThue { get; set; }//tiền giảm trừ k chịu thuế
        public decimal TGTKCKhac { get; set; }//tiền giảm trừ khác
        public decimal Amount { get; set; }//tổng tiền
        public decimal VATAmount { get; set; }//tổng tiền
        public float VATRate { get; set; }//thuế s
        [NotMapped]
        public string Secret { get; set; }//thuế s
        public StatusEinvoice StatusEinvoice { get; set; }//thuế s
        [ForeignKey("IdCustomer")]
        public Customer Customer { get; set; }//khách hàng
        public virtual ICollection<EInvoiceItem> EInvoiceItems { get; set; }
        public virtual ICollection<HistoryEInvoice> HistoryEInvoices { get; set; }
    }
    public class EInvoiceItem
    {
        public int Id { get; set; }
        public int IdInvoice { get; set; }
        public TCHHDVuLoai IsSum { get; set; }//Tchat
        public string ProCode { get; set; }
        public string ProName { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public float VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal Total { get; set; }
        public decimal Amount { get; set; }
        public float? Discount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public virtual EInvoice EInvoice { get; set; }
    }
    public class HistoryEInvoice
    {
        public HistoryEInvoice()
        {
            this.CreateDate = DateTime.Now;
        }
        public StatusStaffEventEInvoice StatusEvent { get; set; }//
        public int IdEInvoice { get; set; }// bàn/phòng
        public int Id { get; set; }
        public string Name { get; set; }//
        [StringLength(30)]
        public string EInvoiceCode { get; set; }//  mã đơn của bàn mới cho lúc chuyển món
        [StringLength(50)]
        public string Carsher { get; set; }// người thêm
        [StringLength(100)]
        public string IdCarsher { get; set; }// người thêm
        public DateTime CreateDate { get; set; }
        public virtual EInvoice EInvoice { get; set; } // trạng thái
    }
}
