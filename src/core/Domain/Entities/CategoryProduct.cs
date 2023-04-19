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
    public class CategoryProduct : AuditableEntity
    {
        [StringLength(100)]
        public string Code { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; }
        public int ComId { get; set; }
        public bool IsPos { get; set; }//là bán hàng máy pos
        public string Slug { get; set; }
        public string Url { get; set; }
        public string Class { get; set; }
        public string Icon { get; set; }
        public int IdLevel { get; set; }
        public int? IdPattern { get; set; }
        public bool Active { get; set; }
   
        [ForeignKey("IdPattern")]
        public CategoryProduct CategoryChild { get; set; }
        public ICollection<CategoryProduct> CategoryChilds { get; set; }
        public ICollection<Product> Products { get; set; }
    }

}
