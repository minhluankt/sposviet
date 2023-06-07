using Application.Enums;
using Domain.Entities;
using Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ViewModel
{
    public class InvoiceModel
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Date { get; set; }
        public string RangesDate { get; set; }
        public string Code { get; set; }
        public StatusEinvoice Status { get; set; }
        public int? InvoiceNo { get; set; }
        public int Currentpage { get; set; }//page trong datatable
    }
    public class PublishEInvoiceMergeModel
    {
        public bool IsDelete { get; set; }//có xóa các đơn cũ
        public string[] LstInvoiceCode { get; set; }
        public Invoice Invoice { get; set; }
        public SupplierEInvoiceModel SupplierEInvoice { get; set; }
    }
    public class PublishInvoiceMergeModel
    {
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; } = ENumSupplierEInvoice.VNPT;// vi dụ VNPT 
        public EnumTypeProduct TypeProduct { get; set; }// cafe, bán lẻ, vlxd
        public bool IsDelete { get; set; } // là cho xóa các hóa đơn cũ
        public bool IsRetailCustomer { get; set; } // là khách lẻ
        public bool IsCreateCustomer { get; set; } // có cho phép tạo mới khách hàng luôn
        public string JsonInvoiceOld { get; set; } // các hóa đơn cũ
        public string JsonProduct { get; set; } // json sản phẩm
        public string CasherName { get; set; } //
        public string IdCasher { get; set; } // 
        public string Note { get; set; } // ghi chú
        public string CusCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Buyer { get; set; }
        public string CusName { get; set; }
        public string CCCD { get; set; }
        public string Taxcode { get; set; }
        public string Address { get; set; }
        public string CusBankNo { get; set; }
        public string CusBankName { get; set; }
        public string Email { get; set; }
        public int? IdCustomer { get; set; }
        public int IdPaymentMethod { get; set; }
        public int ManagerPatternEInvoices { get; set; }
        public decimal DiscountAmount { get; set; } // ck
        public decimal Total { get; set; } // tổng tiền chưa giảm
        public decimal Amount { get; set; } // tổng tiền khách trả dã giảm
        public decimal? VATRate { get; set; } // tổng tiền khách trả dã giảm
        public decimal VATAmount { get; set; } // tổng tiền khách trả dã giảm
        public int Quantity { get; set; } // tổng số lượng
        public DateTime? ArrivalDate { get; set; } // giờ đến
        public DateTime? PurchaseDate { get; set; } // giờ xuất đơn tính tiền
        public List<PublishInvoiceItemModel> PublishInvoiceItemModel { get
            {
                return !string.IsNullOrEmpty(JsonProduct) ? ConvertSupport.ConverJsonToModel<List<PublishInvoiceItemModel>>(JsonProduct): null;
            }
        }
    }
    public class PublishInvoiceItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public float VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal Amount { get; set; }
        public EnumTypeProductCategory TypeProductCategory { get; set; }
        /// <summary>
        ///  dành cho tính thuế khi xuất hóa đơn có thuế
        /// </summary>


    }
}
