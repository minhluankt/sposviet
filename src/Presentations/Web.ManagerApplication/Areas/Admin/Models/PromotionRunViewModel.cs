using Domain.Entities;

namespace Web.ManagerApplication.Areas.Admin.Models
{
    public class PromotionRunViewModel: PromotionRun
    {
        public string JsonProduct { get; set; }
        public List<Product> Products { get; set; }
    }
}
