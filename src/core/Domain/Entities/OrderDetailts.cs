using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrderDetailts
    {
        public int Id { get; set; }
        [ForeignKey("IdOrder")]
        public int IdOrder { get; set; } 
        [ForeignKey("IdProduct")]
        public int? IdProduct { get; set; } // nếu sản phẩm bị xóa thì null
        [Required]
        public string ProdName { get; set; }
        [Required]
        public string ProdCode { get; set; }
        public string Img { get; set; }
        public string NoteProdName { get; set; }//ghi chú của sản phẩm
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal Total { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public string Packing { get; set; } // quy cách
        public bool IsPromotion { get; set; }//sản phầm này là khuyến mãi
        public bool IsRunPromotion { get; set; }//sản phầm này là khuyến mãi chạy theo chương trình
        public string PromotionName { get; set; }//nd khuyến mãi
        public decimal DiscountAmonnt { get; set; }
        public float DiscountRate { get; set; }

        public virtual Product Product { get; set; }
        public virtual Order Order { get; set; }
    }
}
