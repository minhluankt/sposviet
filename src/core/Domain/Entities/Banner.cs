using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Banner : AuditableEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public double Size { get; set; }
        public int Sort { get; set; }
        [Required]
        public string Slug { get; set; }
        public bool Active { get; set; }
    }
}
