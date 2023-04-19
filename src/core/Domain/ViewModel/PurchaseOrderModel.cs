using Application.Enums;
using Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class PurchaseOrderItemModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public float Discount { get; set; } // % ck
        public decimal DiscountAmount { get; set; } // ck
    }
    public class PurchaseReturnsModel//dành cho trả về view trả hàng nhập
    {
        public PurchaseReturnsModel()
        {
            this.PurchaseOrderItems = new List<PurchaseOrderItemModel>();
            //this.PurchaseOrder = new PurchaseOrder();
        }
        public PurchaseOrder PurchaseOrder { get; set; }
        public List<PurchaseOrderItemModel> PurchaseOrderItems { get; set; }
    }
    public class PurchaseOrderModel
    {
        
        public EnumTypePurchaseOrder Type { get; set; }
        public EnumStatusPurchaseOrder Status { get; set; }// trạng thái
        public int Id { get; set; }
        public int? IdSuppliers { get; set; }
        public int? IdPurchaseOrder { get; set; }
        public int? IdPayment { get; set; }
        public string PurchaseNo { get; set; }
        public string Date { get; set; }
        public decimal Total { get; set; }//tổng tiền hàng trước giảm
        public decimal DiscountAmount { get; set; }//ck
        public decimal Amount { get; set; }//tổng tiền hàng
        public decimal Quantity { get; set; }//tổng sl
        public decimal AmountSuppliers { get; set; }//tiền trả nhà cung cấp
        public decimal DebtAmount { get; set; }//tiền nợ nhà cung cấp, công nợ
        public string SuppliersCode { get; set; }
        public string SuppliersName { get; set; }
        public string UrlParameters { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? CreateDate { get; set; }
        public string Note { get; set; }
        [JsonIgnore]
        public string JsonItem { get; set; }

    }

}
