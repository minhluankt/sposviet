using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Unit : AuditableEntity
    {
        public int ComId { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        [NotMapped]
        public string IdString { get; set; }
        [NotMapped]
        public int useCount { get; set; }//số sp dg dùng
        public List<Product> Products { get; set; }
    }
}
