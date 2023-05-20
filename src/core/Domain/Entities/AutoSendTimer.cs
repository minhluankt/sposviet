using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AutoSendTimer : AuditableEntity //cấu hình tự động gửi
    {
        public AutoSendTimer()
        {
            this.JobId= Guid.NewGuid();
        }
        public Guid JobId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(400)]
        public string Content { get; set; }
        [StringLength(700)]
        public string PatternSerial { get; set; }
        public int ComId { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public bool Active { get; set; }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
        public DateTime? CreateDate { get; set; }//dành cho thời gian cố định chạy 1 lần
        public List<HistoryAutoSendTimer> HistoryAutoSendTimers { get; set; }
        [NotMapped]
        public string secret { get; set; }
        [NotMapped]
        [JsonIgnore]
        public List<ManagerPatternEInvoice> ManagerPatternEInvoices { get; set; }
    }
    public class HistoryAutoSendTimer
    {
        public HistoryAutoSendTimer() { }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [Key]
        public int Id { get; set; }//tên 
        public int IdAutoSendTimer { get; set; }//tên 
        public string Name { get; set; }//tên 
        public string Error { get; set; }//chi tiết lỗi nếu có
        [ForeignKey(nameof(IdAutoSendTimer))]
        public AutoSendTimer AutoSendTimer { get; set; }

    }
}
