using Application.Enums;
using Domain.Entities;
using HelperLibrary;
using Library;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using X.PagedList;

namespace Model
{
    public class CommentModel
    {
        public int? IdCustomer { get; set; }
        public int IdProduct { get; set; }
        public int IdPattern { get; set; }
        public bool Like { get; set; }
        public bool DisLike { get; set; }
        public int DeviceType { get; set; } //là máy tính hay mobile...
        public string DeviceName { get; set; } //ten là máy tính hay mobile...
        public string Browser { get; set; } //ten là máy tính hay mobile...
        public string OS { get; set; } // hệ điều hành
        public string Comment { get; set; }
        public string CusName { get; set; }
        public string CusEmail { get; set; }
        public string CusPhone { get; set; }
    }
    public class ProductResponseModel
    {
        public int Id { get; set; }
        public string Img { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool isPromotion { get; set; }
        public bool isPromotionRun { get; set; }
        public decimal Price { get; set; }
        public decimal PriceSell { get; set; }
        public decimal PriceSellRun { get; set; }
    }
    public class ProductApiModelSearch : ParametersPageModel
    {
        public bool loadmore { get; set; }
        public int idCategory { get; set; }
        public string? keyword { get; set; }
        public string? Code { get; set; }
        public bool isPromotion { get; set; }
        public bool isPromotionRun { get; set; }
    }
    public class ProductSearch
    {
        public string keyword { get; set; }
        public int ComId { get; set; }
        public EnumTypeProduct TypeProduct { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string IdUser { get; set; } // người tạo
        public bool isCustomer { get; set; } // người tạo
        public bool isPromotion { get; set; } // là sản phẩm khuyến mãi sell
        public int? idCustomer { get; set; } // người tạo
        public string Slug { get; set; }
        public string search { get; set; }
        public int Status { get; set; }
        public int pageNumber { get; set; }
        public int pagesite { get; set; }
        public string Img { get; set; }
        public string albumImg { get; set; }
        //tỉnh
        public int[] lstidProduct { get; set; }
        public int[] lstidCategory { get; set; }

        [Required]
        public double Price { get; set; } // còn lai

        public string sortby { get; set; }//sắp xếp ở danh mục sãn phẩm
        public int CategorySerach { get; set; }// dg tìm sap hay bài viết
        public int TypeSerach { get; set; }// tìm theo chuyên mục hay từ khóa 
        public string orderName { get; set; }
        public string orderType { get; set; }
        public int idPrice { get; set; } // khoản giá
        public int idCategory { get; set; } // loại đất
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
        public ProductModelView()
        {
            this.ContentPromotionProducts = new List<ContentPromotionProduct>();
            this.UploadImgProducts = new List<UploadImgProduct>();
            this.CartDetailts = new List<CartDetailt>();
            this.OrderDetailts = new List<OrderDetailts>();
            this.StyleProducts = new List<StyleProduct>();
            this.JsonListComboProducts = new List<ComboProductModel>();
            this.JsonListExtraToppingProductModel = new List<ComboProductModel>();
            this.OptionsDetailtProducts = new List<OptionsDetailtProduct>();
        }
        public int Id
        {
            get;
            set;
        }
        public int? IdPromotion { get; set; }
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
        public bool StopBusiness { get; set; }//nguunwfg giao dịch
        public string Name { get; set; }
        public string Code { get; set; }
        public string IdUser { get; set; } // người tạo

        public bool isCustomer { get; set; } // người tạo
        public int Comid { get; set; } // người tạo
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
        public EnumTypeProduct TypeProduct { get; set; }
        public bool IsInventory { get; set; }//sản phẩm k quản lý tồn kho
        public bool IsServiceDate { get; set; } // sản phẩm tính tiền giờ
        public bool DirectSales { get; set; } // là sản phẩm bán trực tiếp tại quầy
        public bool ExtraTopping { get; set; } // là món thêm
        public decimal Weight { get; set; }
        public EnumTypeProductCategory TypeProductCategory { get; set; } // 
        public string Packing { get; set; } //Quy cách 
        public string seokeyword { get; set; } // tạo độ
        public string seotitle { get; set; } // tạo độ
        public string seoDescription { get; set; } // tạo độ
        //khuyến mãi sell theo ngày hết hạn
        public bool isPromotion { get; set; } // là sản phẩm khuyến mãi sell
        public float Discount { get; set; } // % chiết khấu sản phẩm khuyến mãi
        public double PriceDiscount { get; set; } // giá khuyến mãi sau chiết khấu
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? ExpirationDateDiscount { get; set; } //// ngày hết hạn khuyến mãi
        //end
        //chương trình chạy khuyến mãi sell theo giờ
        public int IdPromotionRun { get; set; } // là id của chuongw trinhf run
        public bool isRunPromotion { get; set; } // là sản phẩm có chạy khuyến mãi sell
        public float DiscountRun { get; set; } // sản phẩm khuyến mãi
        public double PriceDiscountRun { get; set; } // giá khuyến mãi

        public string Promotion { get; set; } // nội dung khuyến mãi
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PromotionFromDate { get; set; } // ngày bắt đầu nội dung khuyến mãi 
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PromotionToDate { get; set; } // ngày kết thúc nội dung khuyến mãi

        public string TechnicialParameter { get; set; } // nội dung thông số kỹ thuật
        public bool isHotNew { get; set; } // sản phẩm nổi bật
        public bool isBestseller { get; set; } // sản phẩm bán chạy
        public bool IsOutstock { get; set; } // sản phẩm hết hàng
        public bool IsAddingOptions { get; set; } // Sản phẩm này có các tùy chọn, như kích thước hoặc màu sắc
        public string JsonTableByStylePro { get; set; } // chi tiết màu sắc, kích thước
        public string JsonListStylePro { get; set; } // màu sắc, kích thước
        public string JsonListComboProduct { get; set; } // thành phần combo

        public int? idPrice { get; set; } // khoản giá
        public int? IdBrand { get; set; } // khoản giá
        public int idCategory { get; set; } // danh mục
        public int? IdUnit { get; set; }
        public string NameCategory { get; set; } // danh mục
        public string UrlParameters { get; set; } // tạo độ
        public string[] idattachment { get; set; } // tạo độ
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<PromotionRun> PromotionRuns { get; set; }
        [JsonIgnore]
        public Product Product { get; set; } // danh sách các phụ kiến đính kèm
        [JsonIgnore]
        public CategoryProduct CategoryProduct { get; set; } // danh sách các phụ kiến đính kèm

        [JsonIgnore]
        public CompanyAdminInfo CompanyInfoAdmin { get; set; } // danh sách các phụ kiến đính kèm
        public Customer Customer { get; set; } // danh sách các phụ kiến đính kèm
        [JsonIgnore]
        public Specifications PriceIC { get; set; } //  kung giá
        [JsonIgnore]
        public List<CategoryProduct> CategoryProducts { get; set; }
        [JsonIgnore]
        public IEnumerable<Specifications> PriceICs { get; set; }
        [JsonIgnore]
        public ICollection<Document> Documents { get; set; }
        [JsonIgnore]
        public ICollection<Brand> Brands { get; set; }
        [JsonIgnore]
        public ICollection<UploadImgProduct> UploadImgProducts { get; set; }
        [JsonIgnore]
        public ICollection<CartDetailt> CartDetailts { get; set; }
        [JsonIgnore]
        public ICollection<OrderDetailts> OrderDetailts { get; set; }
        [JsonIgnore]
        public ICollection<Comment> Comments { get; set; } // danh sách các phụ kiến đính kèm
        [JsonIgnore]
        public List<ComboProductModel> JsonListComboProductModel { get; set; } //dành cho thành phần
        [JsonIgnore]
        public List<ComboProductModel> JsonListExtraToppingProductModel { get; set; }//dành cho món thêm, mục đích món này là khi đặt món thì có chọn món thêm thì list ra các món này
        [JsonIgnore]
        public List<ComboProductModel> JsonListComboProducts
        {
            get
            {
                return !string.IsNullOrEmpty(JsonListComboProduct) ? ConvertSupport.ConverJsonToModel<List<ComboProductModel>>(JsonListComboProduct) : new List<ComboProductModel>();
            }
            set { }
        } // danh sách các style kích thước, màu sắc
        [JsonIgnore]
        public ICollection<StyleProduct> StyleProducts
        {
            get
            {
                if (this.Id > 0)
                {
                    return new List<StyleProduct>();
                }
                return !string.IsNullOrEmpty(JsonListStylePro) ? ConvertSupport.ConverJsonToModel<List<StyleProduct>>(JsonListStylePro) : new List<StyleProduct>();
            }
            set { }
        } // danh sách các style kích thước, màu sắc
        [JsonIgnore]
        public ICollection<OptionsDetailtProduct> OptionsDetailtProducts
        {
            get
            {
                if (this.Id > 0)
                {
                    return new List<OptionsDetailtProduct>();
                }
                return !string.IsNullOrEmpty(JsonTableByStylePro) ? ConvertSupport.ConverJsonToModel<List<OptionsDetailtProduct>>(JsonTableByStylePro) : new List<OptionsDetailtProduct>();
            }
            set { }
        } // danh sách các style kích thước, màu sắc
        [JsonIgnore]
        public ICollection<StyleProductModel> StyleProductModels
        {
            get
            {
                if (this.Id == 0)
                {
                    return new List<StyleProductModel>();
                }
                return !string.IsNullOrEmpty(JsonListStylePro) ? ConvertSupport.ConverJsonToModel<List<StyleProductModel>>(JsonListStylePro) : new List<StyleProductModel>();
            }
            set { }
        } // danh sách các style kích thước, màu sắc
        [JsonIgnore]
        public ICollection<OptionsDetailtProductModel> OptionsDetailtProductModels
        {
            get
            {
                if (this.Id == 0)
                {
                    return new List<OptionsDetailtProductModel>();
                }
                return !string.IsNullOrEmpty(JsonTableByStylePro) ? ConvertSupport.ConverJsonToModel<List<OptionsDetailtProductModel>>(JsonTableByStylePro) : new List<OptionsDetailtProductModel>();
            }
            set { }
        } // danh sách các style kích thước, màu sắc
        [JsonIgnore]
        public ICollection<ContentPromotionProduct> ContentPromotionProducts { get; set; } // danh sách lịch sử nội dung khuyến mãi
        public decimal Quantity
        {
            get
            {
                return !string.IsNullOrEmpty(_Quantity) ? decimal.Parse(_Quantity.Replace(",", "."), LibraryCommon.GetIFormatProvider()) : 0;
            }
            set
            {
                if (!string.IsNullOrEmpty(_Quantity))
                {
                    value = decimal.Parse(_Quantity.Replace(",", "."), LibraryCommon.GetIFormatProvider());
                }
                //this.Price = value;
            }
        }

        [Display(Name = "Đơn giá nhập vào")]
        public string _Quantity { get; set; }
        
        public string _Price { get; set; }
        [Display(Name = "Đơn giá bán lẻ")]
        public string _RetailPrice { get; set; }
        [Display(Name = "Đơn giá")]
        public string _PriceNoVAT { get; set; }
        public decimal Price
        {
            get
           {
                return !string.IsNullOrEmpty(_Price) ? decimal.Parse(_Price.Replace(",", "."), LibraryCommon.GetIFormatProvider()) : 0;
            }
            set
            {
                if (!string.IsNullOrEmpty(_Price))
                {
                    value = decimal.Parse(_Price.Replace(",", "."), LibraryCommon.GetIFormatProvider());
                }
                //this.Price = value;
            }
        }
        public bool IsVAT { get; set; } // sản phẩm đơn giá có  thuế
        public decimal VATRate { get; set; } // thuế sản phẩm
        public decimal PriceNoVAT {
            get
            {
                return !string.IsNullOrEmpty(_PriceNoVAT) ? decimal.Parse(_PriceNoVAT.Replace(",", "."), LibraryCommon.GetIFormatProvider()) : 0;
            }
            set
            {
                if (!string.IsNullOrEmpty(_PriceNoVAT))
                {
                    value = decimal.Parse(_PriceNoVAT.Replace(",", "."), LibraryCommon.GetIFormatProvider());
                }
                //this.Price = value;
            }
        } // giá trước thuế

        public decimal RetailPrice
        {
            get
            {
                return !string.IsNullOrEmpty(_RetailPrice) ? decimal.Parse(_RetailPrice.Replace(",", "."), LibraryCommon.GetIFormatProvider()) : 0;
            }
            set
            {
                if (!string.IsNullOrEmpty(_RetailPrice))
                {
                    value = decimal.Parse(_RetailPrice.Replace(",", "."), LibraryCommon.GetIFormatProvider());
                }
                //this.Price = value;
            }
        }
        public virtual ICollection<ComponentProduct> ComponentProducts { get; set; } // danh sách các thanhhf phần thuộc combo và món chế biến


    }
    public class StyleAndOptionNameModelproduct
    {
        public List<StyleProductModel> styleProductModels { get; set; }
        public List<OptionsDetailtProductModel> OptionsDetailtProductModels { get; set; }
    }
    public class ComboProductModel
    {
        public int Id { get; set; }
        public int IdPro { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }//giá bán
        public decimal RetailPrice { get; set; }//giá nhập vào , giá vốn
    }
   
    public class OptionsDetailtProductModel
    {
        public string ClassName { get; set; }
        public string? IdOptionsName { get; set; }//group các id
        public int? Id { get; set; }
        public int IdProduct { get; set; }
        public string Name { get; set; }
        public string? Img { get; set; }
        public double Price { get; set; }
        public decimal Quantity { get; set; }
        public string? SKU { get; set; }
    }
    public class StyleProductModel
    {
        public int IdProduct { get; set; }
        public int IdStyleOptionsProduct { get; set; }
        public int Sort { get; set; }
        public List<OptionsNameProductModel> OptionsNames { get; set; }
    }
    public class OptionsNameProductModel
    {
        public int IdStyleProduct { get; set; }
        public int Sort { get; set; }
        public int? Id { get; set; }
        public string Name { get; set; }
    }
    public class AutocompleteViewModel
    {
        public bool HistoryLoca { get; set; }
        public string Slug { get; set; }
        public int Length { get; set; }
        public string Price { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
    }
    public class AutocompleteProductPosModel
    {
        public int typeProductCategory { get; set; }
        public int Length { get; set; }
        public string Vatrate { get; set; }
        public string PriceNoVAT { get; set; }
        public string Price { get; set; }
        public string RetailPrice { get; set; }
        public string Quantity { get; set; }//số lượng tồn
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public bool IsVAT { get; set; }
        public bool IsInventory { get; set; }
    }
}
