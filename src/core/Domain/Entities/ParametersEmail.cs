using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ParametersEmail : AuditableEntity// cấu hình các tham số email
    {
        [Required]
        [StringLength(200)]
        public string Key { get; set; }
        public int ComId { get; set; }
        public string Title { get; set; }//tiêu đè mail
        public string Content { get; set; }//mô tả
        public string Value { get; set; }//nội dung
    }
}
