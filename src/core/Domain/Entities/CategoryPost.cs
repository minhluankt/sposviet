using AspNetCoreHero.Abstractions.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class CategoryPost : AuditableEntity
    {
        [StringLength(100)]
        public string Code { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; }
        public string Slug { get; set; }
        public string Url { get; set; }
        public string Class { get; set; }
        public string Icon { get; set; }
        public int IdLevel { get; set; }
        public int? IdPattern { get; set; }
        public int IdTypeCategory { get; set; } // chuyên mục cho loại dịch vụ nào ví dụ sản phẩm hay tin tức
        public bool Active { get; set; }
        [ForeignKey("IdTypeCategory")]
        public TypeCategory TypeCategory { get; set; }
        [ForeignKey("IdPattern")]
        public CategoryPost CategoryChild { get; set; }
        public ICollection<CategoryPost> CategoryChilds { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
