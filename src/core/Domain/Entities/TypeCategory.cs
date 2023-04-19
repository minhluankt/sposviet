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
    public class TypeCategory : AuditableEntity
    {
        [StringLength(100)]
        public string Code { get; set; }
        public string View { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        
        public virtual ICollection<CategoryPost> Categorys { get; set; }
    }
}
