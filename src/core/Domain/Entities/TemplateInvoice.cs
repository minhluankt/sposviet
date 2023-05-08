using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class TemplateInvoice : AuditableEntity //mauaxx giấy in
    {
        public TemplateInvoice() { }
        public EnumTypeTemplatePrint TypeTemplatePrint { get; set; }//loại mẫu in
        public int ComId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Slug { get; set; }
        public string Template { get; set; }
        public bool Active { get; set; }
        public string Note { get; set; }
        [NotMapped]
        public List<SelectListItem> Selectlist { get; set; }
    }
}
