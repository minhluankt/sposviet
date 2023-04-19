using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PagePost : AuditableEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Decription { get; set; }
        public int ViewNumber { get; set; }
        public int Sort { get; set; }
        public string Img { get; set; }
        public bool Active { get; set; }
        [StringLength(200)]
        public string seokeyword { get; set; } // 
        [StringLength(200)]
        public string seotitle { get; set; } //
        [StringLength(200)]
        public string seoDescription { get; set; } //
    }
}
