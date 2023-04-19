using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Model
{
    public class ProductSearch
    {

        public string keyword { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string IdUser { get; set; } // người tạo
        public bool isCustomer { get; set; } // người tạo
        public int? idCustomer { get; set; } // người tạo
        public string Slug { get; set; }
        public int Status { get; set; }
     
        public string Img { get; set; }
        public string albumImg { get; set; }
         //tỉnh
   
       
        [Required]
        public double Price { get; set; } // còn lai

        public string orderName { get; set; }
        public string orderType { get; set; }
        public int? idPrice { get; set; } // khoản giá
        public int idCategory { get; set; } // loại tin đăng// loại đất
        public int?[] hang { get; set; }
        public int?[] danhmuc { get; set; }//thương heieuj
        public int?[] gia { get; set; }
        public CompanyAdminInfo CompanyInfoAdmin { get; set; } // danh sách các phụ kiến đính kèm
        public bool SearchCustom { get; set; } // tạo độ
        public IPagedList<Product> ProductPagedList { get; set; }
        public IPagedList<Post> PostPagedList { get; set; }
        public Specifications PriceIC { get; set; } //  kung giá
        public CategoryProduct CategoryProduct { get; set; } // danh sách các phụ kiến đính kèm
        public List<CategoryProduct> CategoryProducts { get; set; } // danh sách các phụ kiến đính kèm

        public City City { get; set; } // danh sách các phụ kiến đính kèm

        public IEnumerable<Specifications> PriceICs { get; set; }
        public IEnumerable<City> Citys { get; set; } // danh sách các phụ kiến đính kèm
        public Customer Customer { get; set; } // danh sách các phụ kiến đính kèm
        public ICollection<UploadImgProduct> UploadImgProducts { get; set; }
        public ICollection<Comment> Comments { get; set; } // danh sách các phụ kiến đính kèm
    }
    public class ProductModelView
    {
        public int Id
        {
            get;
            set;
        }

        public string CreatedBy
        {
            get;
            set;
        }
        public string PostedbyAdmin
        {
            get;
            set;
        }

        public DateTime CreatedOn
        {
            get;
            set;
        }

        public string LastModifiedBy
        {
            get;
            set;
        }

        public DateTime? LastModifiedOn
        {
            get;
            set;
        }
        public string Name { get; set; }
        public string Code { get; set; }
        public string IdUser { get; set; } // người tạo

        public bool isCustomer { get; set; } // người tạo
        public int? idCustomer { get; set; } // người tạo
        public string CusName { get; set; } // người tạo
        public string Slug { get; set; }
        public int ViewNumber { get; set; }
        public int Status { get; set; }
        public bool Active { get; set; }
        public int Statuspurchase { get; set; } // đã bán hay chưa bán
        public string Img { get; set; }
        public string albumImg { get; set; }
        public IFormFile ImgUpload { get; set; }
        public IList<IFormFile> albumImgUpload { get; set; }
        public IList<IFormFile> Document { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Promotion { get; set; } // nội dung khuyến mãi
        public string Specification { get; set; } // quy cách
        public string seokeyword { get; set; } // tạo độ
        public string seotitle { get; set; } // tạo độ
        public string seoDescription { get; set; } // tạo độ
                                                   //khuyến mãi sell
        public bool isPromotion { get; set; } // là sản phẩm khuyến mãi sell
        public float Discount { get; set; } // % chiết khấu sản phẩm khuyến mãi
        public double PriceDiscount { get; set; } // giá khuyến mãi sau chiết khấu
        //end
        //chương trình chạy khuyến mãi sell theo giờ
        public int IdPromotionRun { get; set; } // là id của chuongw trinhf run
        public bool isRunPromotion { get; set; } // là sản phẩm có chạy khuyến mãi sell
        public float DiscountRun { get; set; } // sản phẩm khuyến mãi
        public double PriceDiscountRun { get; set; } // giá khuyến mãi
        //end
        public bool isHotNew { get; set; } // sản phẩm nổi bật
        public bool isBestseller { get; set; } // sản phẩm bán chạy
        public bool IsOutstock { get; set; } // sản phẩm hết hàng

        public int? idPrice { get; set; } // khoản giá
        public int? IdBrand { get; set; } // khoản giá
        public int idCategory { get; set; } // danh mục
        public string NameCategory { get; set; } // danh mục
        public string UrlParameters { get; set; } // tạo độ
        public string[] idattachment { get; set; } // tạo độ
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<PromotionRun> PromotionRuns { get; set; }
        public Product Product { get; set; } // danh sách các phụ kiến đính kèm
        public CategoryProduct CategoryProduct { get; set; } // danh sách các phụ kiến đính kèm


        public CompanyAdminInfo CompanyInfoAdmin { get; set; } // danh sách các phụ kiến đính kèm
        public Customer Customer { get; set; } // danh sách các phụ kiến đính kèm
        public Brand Brand { get; set; } // danh sách các phụ kiến đính kèm
        public Specifications PriceIC { get; set; } //  kung giá
        public ICollection<UploadImgProduct> UploadImgProducts { get; set; }
        public ICollection<Comment> Comments { get; set; } // danh sách các phụ kiến đính kèm
        public List<CategoryProduct> CategoryProducts { get; set; }
      
        public IEnumerable<Specifications> PriceICs { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Brand> Brands { get; set; }
        public float Quantity { get; set; }
        public string _Price { get; set; }
        [Display(Name = "Đơn giá")]

        public double Price
        {
            get
            {
                return _Price != null ? decimal.Parse(_Price.Replace(",", "")) > 0 ? double.Parse(_Price.Replace(",", "")) : Price : 0;
            }
            set
            {
                //Price = value;
            }
        }
        // nhập vào

    }
    public class AutocompleteViewModel
    {
        public bool HistoryLoca { get; set; }
        public string Slug { get; set; }
        public string Price { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
    }
}
