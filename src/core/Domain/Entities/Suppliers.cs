using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Suppliers : AuditableEntity // nhà cung cấp .đối tác
    {
        public string CodeSupplier { get; set; }//mã
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Phonenumber { get; set; }
        public int ComId { get; set; }//comid
        public int? IdCity { get; set; }//thành phố
        public int? IdDistrict { get; set; }//quạn hueyenh
        public string Address { get; set; }
        public string TaxCode { get; set; }
        public string Note { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
        [NotMapped]
        public string secret { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual ICollection<AdJusPaymentSupplier> AdJusPaymentSuppliers { get; set; }

    }
    public class AdJusPaymentSupplier : AuditableEntity // điều chỉnh công nợ, tạo lịch sử trả nợ
    {
        public int IdSuppliers { get; set; }
        public int ComId { get; set; }//phải có com vì nó sinh code
        public string Code { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateRecord { get; set; }//ngày than htoasn ghi nhận
        public virtual Suppliers Suppliers { get; set; }
    }
}
