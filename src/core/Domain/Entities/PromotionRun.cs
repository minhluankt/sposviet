using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Entities
{
    public class PromotionRun : AuditableEntity
    {
        [Required]
        public string Name { get; set; }    // tên chương trình
        public string Description { get; set; } // mô tả nếu có
        public string Slug { get; set; }
        public int Status { get; set; } // trạng thái
        public bool IsActive { get; set; } // kích hoạt/ 
        public bool IsCancelEvent { get; set; } // hủy bỏ sự kiện
        public DateTime StartDate { get; set; }// thời gian bắt đầu
        public DateTime EndDate { get; set; }// thời gian kết thúc
        public double TimeRemain { get; set; }// thời gian còn lại

        public string JobStart { get; set; }// id jod
        public string JobEnd { get; set; }// id jod end
    }
}
