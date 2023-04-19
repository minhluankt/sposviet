using Domain.ViewModel;

namespace Web.ManagerApplication.Areas.Admin.Models
{
    public class ConfigSystemViewModel : ConfigSystemModel
    {
        public int?[] lstIdAndNameCategoryShowInHomeModel { get; set; }
        public List<CategoryConfig> listcategory { get; set; }
    }
    public class CategoryConfig
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}
