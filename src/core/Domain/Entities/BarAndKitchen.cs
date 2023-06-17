using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BarAndKitchen : AuditableEntity//nhà bếp
    {
        [StringLength(100)]
        public string Name { get; set; }
        public string Slug { get; set; }
        public int ComId { get; set; }
    }
}
