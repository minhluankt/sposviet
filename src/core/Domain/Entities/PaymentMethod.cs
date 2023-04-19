using AspNetCoreHero.Abstractions.Domain;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class PaymentMethod : AuditableEntity
    {
        public string Name { get; set; }
        public int ComId { get; set; }
        public string Content { get; set; }//nội dung ví dụ nếu momo thì nhập SĐT 
        public string Code { get; set; }
        [StringLength(20)]
        public string Vkey { 
            get {
                if (!string.IsNullOrEmpty(Code))
                {
                    return $"{ComId}HTTT{Code}";
                }
                return "";
            }
            set 
            { } }
        [StringLength(100)]
        public string Slug { get; set; }
        public string Avatar { get; set; }
        
        public bool Active { get; set; }
        [JsonIgnore]
        public virtual ICollection<Order> Order { get; set; }
        [JsonIgnore]
        public virtual ICollection<Invoice> Invoices { get; set; }
        /// <summary>
        /// trường k thuộc trong db
        /// </summary>
        [NotMapped]
        public string secret { get; set; }
        [NotMapped]
        public IFormFile ImageUpload { get; set; }
    }
}
