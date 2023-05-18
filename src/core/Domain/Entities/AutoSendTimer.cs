using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AutoSendTimer : AuditableEntity //cấu hình tự động gửi
    {
        public AutoSendTimer() { }
        public Guid JobId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int ComId { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public bool Active { get; set; }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
        public DateTime? CreateDate { get; set; }//dành cho thời gian cố định chạy 1 lần
        [NotMapped]
        public string secret { get; set; }
    }
}
