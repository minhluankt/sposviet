using AspNetCoreHero.Abstractions.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class ContentPromotionProduct : AuditableEntity // table quản lý nội dung khuyến mãi sản phẩm
    {
        //public bool Active { get; set; } // kích hoạt hiển thị nội dung khuyến mãi
        // public bool Close { get; set; } // kích hoạt hiển thị nội dung khuyến mãi
        // public DateTime? DateClose { get; set; } // kích hoạt hiển thị nội dung khuyến mãi
        //public DateTime? DateActive { get; set; } // kích hoạt hiển thị nội dung khuyến mãi
        public int IdProduct { get; set; } // nội dung khuyến mãi
        public string Promotion { get; set; } // nội dung khuyến mãi
        public DateTime PromotionFromDate { get; set; } // ngày bắt đầu nội dung khuyến mãi 
        public DateTime PromotionToDate { get; set; } // ngày kết thúc nội dung khuyến mãi
        [ForeignKey("IdProduct")]
        public Product Product { get; set; }
    }
}
