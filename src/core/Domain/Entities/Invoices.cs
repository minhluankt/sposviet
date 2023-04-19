using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Invoice : AuditableEntity // đặt bàn/HÓA ĐƠN
    {
        public Invoice()
        {
            this.InvoiceItems = new List<InvoiceItem>();
            this.IdGuid = Guid.NewGuid();
        }
        public Guid IdGuid { get; set; }
        public bool IsBringBack { get; set; }// là khách mag về
        public int? IdEInvoice { get; set; }// id của hóa đơn điện tử
        [StringLength(50)]
        public string InvoiceCode { get; set; }// mã hóa đươn
        [StringLength(300)]
        public string Buyer { get; set; }// tên khách
        public string CusName { get; set; }// tên khách
        [StringLength(300)]
        public string Email { get; set; }
        [StringLength(20)]
        public string Taxcode { get; set; }
        [StringLength(50)]
        public string CusCode { get; set; }// mã khách
        [StringLength(50)]
        public string PhoneNumber { get; set; }// sđt khách
        [StringLength(100)]
        public string TableNameArea { get; set; }// tên bàn,khu vực
        public string Address { get; set; }
        public string CCCD { get; set; }
        [StringLength(70)]
        public string CusBankNo { get; set; }
        [StringLength(300)]
        public string CusBankName { get; set; }
        public int ComId { get; set; }// là của đơn vị nào
        public int? IdOrderTable { get; set; }// mã đặt hàng/mã đặt bàn
        public int? IdCustomer { get; set; }// khách hàng
        public bool IsDelete { get; set; }// là xóa bỏ khỏi hệ thống

        public int? IdRoomAndTable { get; set; } // bàn/phòng
        public bool IsRetailCustomer { get; set; } // là khách lẻ
        public EnumStatusPublishInvoiceOrder StatusPublishInvoiceOrder { get; set; } // trạng thái phát hành hóa đơn hay tạo mới đơn điện tử
        public string Note { get; set; } // ghi chú
        [StringLength(50)]
        public string? IdCasher { get; set; } // id thu ngân
        [StringLength(50)]
        public string CasherName { get; set; } // tên thu ngân
        [StringLength(50)]
        public string StaffName { get; set; } //  nhân viên phục vụ
        [StringLength(50)]
        public string IdStaff { get; set; } // id nhân viên phục vụ

        public bool IsMerge { get; set; } // là hóa đơn này đã được merge thành 1 đơn
        public bool IsDeleteMerge { get; set; } // là đã xóa các đơn đã gộp
        public string InvoiceCodePatern { get; set; } // id hóa đơn cha sau khi đã được merge thành

        public int IdPaymentMethod { get; set; } // hình thưc thanh toán
        public float Discount { get; set; } // % ck
        public decimal ServiceChargeAmount { get; set; } // phí phục vụ
        public float ServiceRate { get; set; } //% phí phục vụ
        public decimal DiscountAmount { get; set; } // ck
        public decimal DeliveryCharges { get; set; } // phí gioa hàng
        public decimal Total { get; set; } // tổng tiền chưa giảm
        public decimal Amonut { get; set; } // tổng tiền khách trả dã giảm
 
        public float? VATRate { get; set; } // tổng tiền khách trả dã giảm
        public decimal VATAmount { get; set; } // tổng tiền khách trả dã giảm
        public decimal Quantity { get; set; } // tổng số lượng
        public decimal? AmountCusPayment { get; set; } // tiền khách đưa
        public decimal? AmountChangeCus { get; set; } // tiền thừa
        public DateTime? ArrivalDate { get; set; } // giờ đến
        public DateTime? PurchaseDate { get; set; } // giờ xuất đơn tính tiền
        [DefaultValue(EnumTypeInvoice.INVOICE)]
        public EnumTypeInvoice TypeInvoice { get; set; }
        public virtual EnumStatusInvoice Status { get; set; } // trạng thái
        public virtual EnumTypeProduct TypeProduct { get; set; } // loại hóa đơn của dịch vụ nào
        // public virtual OrderTable OrderTable { get; set; } // trạng thái
        [ForeignKey("IdCustomer")]
        public virtual Customer Customer { get; set; } // trạng thái
        [ForeignKey("IdRoomAndTable")]
        public virtual RoomAndTable RoomAndTable { get; set; } // trạng thái
        [ForeignKey("IdPaymentMethod")]
        public virtual PaymentMethod PaymentMethod { get; set; } // trạng thái
        [ForeignKey("IdOrderTable")]
        public virtual OrderTable OrderTable { get; set; } // trạng thái
 

        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } // trạng thái

        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } // gồm nhiều đơn trả hàng nếu có
        public virtual List<HistoryInvoice> HistoryInvoices { get; set; } // trạng thái
        [NotMapped]
        public string Secret { get; set; } // mõa hóa url 
        [NotMapped]
        public string secretEinvoice { get; set; } // mõa hóa id hóa đơn điện tử
    }

    public class InvoiceItem
    {
        public int Id { get; set; }
        public int? IdProduct { get; set; }
        public int IdInvoice { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal EntryPrice { get; set; }// giá vóno
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public EnumTypeProductCategory TypeProductCategory { get; set; }
        public float Discount { get; set; } // % ck
        public decimal DiscountAmount { get; set; } // ck
        /// <summary>
        ///  dành cho tính thuế khi xuất hóa đơn có thuế
        /// </summary>
        public decimal Amonut { get; set; } // 
        public float? VATRate { get; set; } // 
        public decimal VATAmount { get; set; } // 
        [ForeignKey("IdInvoice")]
        public virtual Invoice Invoice { get; set; } // trạng thái


    }
    public class HistoryInvoice
    {
        public HistoryInvoice()
        {
            this.CreateDate = DateTime.Now;
        }

        public int IdInvoice { get; set; }// bàn/phòng
        public int Id { get; set; }
        public string Name { get; set; }//
        public string InvoiceCode { get; set; }//  mã đơn của bàn mới cho lúc chuyển món
        public string Carsher { get; set; }// người thêm
        public DateTime CreateDate { get; set; }
        [NotMapped]
        public int? IdProduct { get; set; }
        [ForeignKey("IdInvoice")]
        public virtual Invoice Invoice { get; set; } // trạng thái
    }
}
