using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class PurchaseOrder : AuditableEntity// nhập hàng
    {
        public int Comid { get; set; } // mã hóa đơn trả hàng là từ hóa đơn nào đối với trả hàng trên đơn khách đặt
        public int? IdInvoice { get; set; } // mã hóa đơn trả hàng là từ hóa đơn nào đối với trả hàng trên đơn khách đặt
        public int? IdSuppliers { get; set; } // nhà cung cấp
        public string PurchaseOrderCode { get; set; } // là mã phiếu nhập hàng,  tức là thèn trả hàng nhập là trả cho đơn nhập hàng nào
        public string SuppliersName { get; set; }// 
        public string SuppliersCode { get; set; }// 
        public string PurchaseNo { get; set; }// số phiếu
        public DateTime CreateDate { get; set; }// ngầy nhận của khách nhập
        public EnumTypePurchaseOrder Type { get; set; }// trả hàng, trả hàng nhập, nhập hàng
        public string Code { get; set; }// mã
        public decimal Total { get; set; }// tổng tiền 
        public decimal Amount { get; set; }//tổng tiền hàng trước giảm
        public decimal AmountSuppliers { get; set; }//  tổng tiền  tar nhà cung cấp
        public decimal DebtAmount { get; set; }// ghi vào công nợ tức là nợ nhà cung cấp
        public decimal DisCount { get; set; }
        public decimal DisCountAmount { get; set; }
        public decimal Quantity { get; set; }
        public string Note { get; set; }
        public string Carsher { get; set; }// người nhập
        public int? IdPayment { get; set; }//
        public EnumStatusPurchaseOrder Status { get; set; }// trạng thái
        public virtual ICollection<ItemPurchaseOrder> ItemPurchaseOrders { get; set; }
        [ForeignKey("IdInvoice")]
        public virtual Invoice Invoice { get; set; }
        [ForeignKey("IdSuppliers")]
        public virtual Suppliers Suppliers { get; set; }
    }
    public class ItemPurchaseOrder
    {
        public int Id { get; set; }
        public int IdPurchaseOrder { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public float Discount { get; set; } // % ck
        public decimal DiscountAmount { get; set; } // ck
        [ForeignKey("IdPurchaseOrder")]
        public virtual PurchaseOrder PurchaseOrder { get; set; } // trạng thái
    }

}
