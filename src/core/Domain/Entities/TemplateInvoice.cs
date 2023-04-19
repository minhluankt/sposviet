using AspNetCoreHero.Abstractions.Domain;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class TemplateInvoice : AuditableEntity //mauaxx giấy in
    {
        public TemplateInvoice() { }
        public int ComId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Slug { get; set; }
        public string Template { get; set; }
        public bool Active { get; set; }
        public string Note { get; set; }
    }
}
