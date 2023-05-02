using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CategoryInvoiceTemplate : AuditableEntity //danh mục mẫu hóa đơn
    {
        [StringLength(200)]
        public string Slug { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [StringLength(250)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public bool Active { get; set; }
        public virtual ICollection<TemplateInvoice> TemplateInvoices { get;set; }
    }
}
