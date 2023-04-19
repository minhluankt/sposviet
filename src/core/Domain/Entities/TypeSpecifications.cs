using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{   // loai thông số kỹ thuật
    public class TypeSpecifications : AuditableEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<Specifications> Specifications { get; set; }
    }
}
