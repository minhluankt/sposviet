using AspNetCoreHero.Abstractions.Domain;

namespace Domain.Entities
{
    public class StyleOptionsProduct : AuditableEntity//styleoption như màu sắc kích thước
    {
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}
