using Application.Enums;
using Domain.Entities;
using Domain.ViewModel;
using X.PagedList;

namespace Web.ManagerApplication.Models
{
    public class HomeViewModel
    {
        public List<CategoryProduct> CategoryProducts { get; set; }
        public List<CategoryPost> CategoryPosts { get; set; }
        public List<Banner> Banners { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Product> ProductSell { get; set; }
        public PromotionRun PromotionRun { get; set; }
        public SellModelSetting SellModelSetting { get; set; }
        public int layoutHeader { get; set; }
    }
    public class CategoryProductSellIndexModel
    {
        public IPagedList<Product> Products { get; set; }
        public CategoryProduct CategoryProduct { get; set; }
        public int idPrice { get; set; } // khoản giá
        public string sortby { get; set; }//sắp xếp ở danh mục sãn phẩm
    }

    public class SearchViewModel
    {
        public ProductEnumcs TypeProduct { get; set; }
        public string keyword { get; set; }
        public int TypeSerach { get; set; }
        public int idcategory { get; set; }
        public int idPrice { get; set; } // khoản giá
        public string sortby { get; set; }//sắp xếp ở danh mục sãn phẩm
        public string name { get; set; }
        public bool isPromotion { get; set; }
        public bool history { get; set; }
        public int ProductType { get; set; }
        public int Task { get; set; }
        public int pagenumber { get; set; } = 1;
        public int pagesite { get; set; } = 15;
    }
}
