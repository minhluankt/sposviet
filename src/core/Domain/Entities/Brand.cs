using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Brand : AuditableEntity
    {
        [Required]
        public string Name { get; set; }
        public string Code { get; set; }
        public int Sort { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
