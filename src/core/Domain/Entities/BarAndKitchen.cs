using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BarAndKitchen : AuditableEntity//nhà bếp
    {
        [StringLength(100)]
        public string Name { get; set; }  
        [StringLength(300)]
        public string Note { get; set; }
        [StringLength(200)]
        public string Slug { get; set; }
        public int ComId { get; set; }
        public bool Active { get; set; }
        [NotMapped]
        public string secret { get; set; }
        public List<ProductInBarAndKitchen> ProductInBarAndKitchens { get; set; }
    }
    public class ProductInBarAndKitchen : AuditableEntity
    {
        public int IdBarAndKitchen { get; set; }
        public int IdProduct { get; set; }
        public BarAndKitchen BarAndKitchen { get; set; }
    }
}
