using Domain.Entities;
using X.PagedList;

namespace Web.ManagerApplication.Areas.Admin.Models
{
    public class ProductSellModel
    {
        public string IdCode { get; set; }
        public int Id { get; set; }
        public int? IdParent { get; set; }
        public int Sort { get; set; }
        public string Slug { get; set; }
        public string SlugCateParent { get; set; }
        public string Img { get; set; }
        public string sortby { get; set; }
        public string Name { get; set; }
        public string ImgNotData { get; set; }
        public IPagedList<Product> products { get; set; }
        public List<Product> productslist { get; set; }

    }
}
