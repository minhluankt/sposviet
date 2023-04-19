using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
namespace Domain.Entities
{
    public class Menus : AuditableEntity // thực đơn
    {
        public string Name { get; set; }
        public EnumImgMenus Type { get; set; } = EnumImgMenus.BIEUTUONG;
        public short STT { get; set; }
        public string Icon { get; set; }
    }
    public class Menus_Product
    {
        public int IdMenu { get; set; }
        public int IdProduct { get; set; }
        public virtual Menus Menus { get; set; }
        public virtual Product Product { get; set; }
    }
}
