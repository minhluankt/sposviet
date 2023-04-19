using Application.Enums;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.ManagerApplication.Areas.Selling.Models
{
    public class RoomTableModel
    {
        public string Name { get; set; }
        public int IdArea { get; set; }
        public bool Active { get; set; }
        public List<SelectListItem> SelectList { get; set; }
        public List<Area> Areas { get; set; }
    }
    public class AreasModel
    {
        public string Name { get; set; }
    }
    public class CategoryCevenueModel
    {
        public string RangesDate { get; set; }
        public EnumTypeRevenueExpenditure Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

}
