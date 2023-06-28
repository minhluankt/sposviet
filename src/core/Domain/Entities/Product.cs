using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Domain.Entities
{
    public class Product : AuditableEntity
    {
        [Required]
        public string Name { get; set; }
        [StringLength(20)]
        public string Code { get; set; }
        public bool isCustomer { get; set; } //  nhà bán hàng thứ 2 là khách hàng
        public int? IdCustomer { get; set; } // nhà bán hàng thứ 2 là khách hàng
        public int? IdUnit { get; set; } // ddwon vij tinsh
        //public string Name { get; set; } // ddwon vij tinsh
        public int ComId { get; set; } // số lượt xem
        public int ViewNumber { get; set; } // số lượt xem
        public string Slug { get; set; }
        public int Status { get; set; } // nổi bật/ hot/
        public bool StopBusiness { get; set; } // ngừng kinh daonh
        public bool Active { get; set; } // hiển thị khóa bài
        public bool IsPrintItem { get; set; } // in tem mặt hàng
        public bool IsBarcode { get; set; } // có thêm Barcode
      
        public string? Barcode { get; set; } // tên mã vạch/Barcode

        public EnumSalesChannel SalesChannel { get; set; } = EnumSalesChannel.BAN_TAI_NHA_HANG; // Kênh bán hàng
        public string Img { get; set; }
        public string AlbumImg { get; set; }
        public string Description { get; set; }
        public string Title { get; set; } // mô tả ngắn
        public string NoteProdName { get; set; } // ghi chú cho sp
        public string TechnicialParameter { get; set; } // thông số sản phẩm
        [Required]
        public decimal Price { get; set; } // giá bán lẻ  
        public decimal RetailPrice { get; set; } // giá nhập vào giá vốn nhé
        public string Unit { get; set; } // đơn vị tính
        public string Packing { get; set; } // quy cách
        public int idPrice { get; set; } // mức giá
        public decimal PricePromotion { get; set; } // giá khuyến mãi

        public bool IsVAT { get; set; } // sản phẩm đơn giá có  thuế
        public decimal VATRate { get; set; } // thuế sản phẩm
        public decimal PriceNoVAT { get; set; } // giá trước thuế

        public decimal Quantity { get; set; } // số lượng nhập vào tồn kho
        public int IdCategory { get; set; } // danh  mục sp
        public int? IdBrand { get; set; } // hãng sản xuất

        //---------nghành vlxd---------//
        public decimal Weight { get; set; } // trọng lượng
        public int IdUnitWeight { get; set; } // đơn vị tính của trọng lượng
      
        //-------------------//

        //khuyến mãi sell
        public bool isPromotion { get; set; } // là sản phẩm khuyến mãi sell
        public float Discount { get; set; } // % chiết khấu sản phẩm khuyến mãi
        public decimal PriceDiscount { get; set; } // giá khuyến mãi sau chiết khấu
        public DateTime? ExpirationDateDiscount { get; set; } // ngày hết hạn khuyến mãi
        /// <summary>
        /// ///
        /// </summary>
        public bool IsEnterInOrder { get; set; } // nhập giá khi bán
        public bool IsKitchen { get; set; } // Hàng hóa không cần báo chế biến
        public bool IsInventory { get; set; } // cho phép không quản lý tồn kho
        public bool IsServiceDate { get; set; } // sản phẩm tính tiền giờ
        public bool DirectSales { get; set; } // là sản phẩm bán trực tiếp tại quầy
        public bool ExtraTopping { get; set; } // là món thêm
        public EnumTypeProductCategory TypeProductCategory { get; set; } // món chế biến, sản phẩm, hay chế biến.....
        /// <summary>
        /// ///
        /// </summary>
        //end
        //chương trình chạy khuyến mãi sell theo giờ
        public int IdPromotionRun { get; set; } // là id của chuongw trinhf run
        public bool isRunPromotion { get; set; } // là sản phẩm có chạy khuyến mãi sell
        public float DiscountRun { get; set; } // sản phẩm khuyến mãi
        public decimal PriceDiscountRun { get; set; } // giá khuyến mãi

        //end

        public string Promotion { get; set; } // nội dung khuyến mãi
        public DateTime? PromotionFromDate { get; set; } // ngày bắt đầu nội dung khuyến mãi 
        public DateTime? PromotionToDate { get; set; } // ngày kết thúc nội dung khuyến mãi
        public bool isHotNew { get; set; } // sản phẩm nổi bật
        public bool isBestseller { get; set; } // sản phẩm bán chạy
        public bool IsOutstock { get; set; } // sản phẩm hết hàng
        public bool IsAddingOptions { get; set; } // Sản phẩm này có các tùy chọn, như kích thước hoặc màu sắc
        public EnumTypeProduct TypeProduct { get; set; } = EnumTypeProduct.THOITRANG; // là sản phẩm quần áo, mỹ phẩm hay là bán hàng cafe, phân loại dịch vụ nhé


        [StringLength(200)]
        public string seokeyword { get; set; } // 
        [StringLength(200)]
        public string seotitle { get; set; } //
        [StringLength(200)]
        public string seoDescription { get; set; } //
        [NotMapped]
        public string base64 { get; set; } //
                                                   //public string longlat { get; set; } // tạo độ


        [ForeignKey("IdUnit")]
        public Unit UnitType { get; set; } // dodwn vij tinsh
        [ForeignKey("IdCategory")]
        public CategoryProduct CategoryProduct { get; set; } // danh mục
        [ForeignKey("IdBrand")]
        public Brand Brand { get; set; } // danh mục

        [ForeignKey("IdCustomer")]
        public Customer Customer { get; set; } // danh sách các phụ kiến đính kèm
        public DefaultFoodOrder DefaultFoodOrder { get; set; } // danh sách các phụ kiến đính kèm
        //[ForeignKey("IdKitchen")]
        //public Kitchen Kitchen { get; set; } // thuộc bếp/bar nào
        public virtual ICollection<UploadImgProduct> UploadImgProducts { get; set; }
        public virtual ICollection<CartDetailt> CartDetailts { get; set; }
        public virtual ICollection<OrderDetailts> OrderDetailts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } // danh sách các phụ kiến đính kèm
        public virtual ICollection<StyleProduct> StyleProducts { get; set; } // danh sách các style kích thước, màu sắc
        public virtual ICollection<OptionsDetailtProduct> OptionsDetailtProducts { get; set; } // danh sách các style kích thước, màu sắc
        public virtual ICollection<ContentPromotionProduct> ContentPromotionProducts { get; set; } // danh sách lịch sử nội dung khuyến mãi
        public virtual ICollection<ComponentProduct> ComponentProducts { get; set; } //danh sách các combo, thành phần sản phẩm kèm theo
    
    }
    public class StyleProduct // ví dụ  như kích thước, màu sắc
    {
        public int Id { get; set; }
        public int IdProduct { get; set; }
        public int IdStyleOptionsProduct { get; set; }
        public short Sort { get; set; }
        public virtual ICollection<OptionsName> OptionsNames { get; set; }// trong mỗi kích thước có nhiều loại
        [ForeignKey("IdStyleOptionsProduct")]
        // public virtual StyleOptionsProduct StyleOptionsProduct { get; set; }
        public virtual Specifications StyleOptionsProduct { get; set; }
        public virtual Product Product { get; set; }
    }
    public class OptionsName
    {
        public int IdStyleProduct { get; set; }
        public int Sort { get; set; }
        public int Id { get; set; }
        [ForeignKey("IdStyleProduct")]
        public virtual StyleProduct StyleProduct { get; set; }
        public string Name { get; set; }
    }
    public class OptionsDetailtProduct // chi tiết của mỗi loại ví dụ kích thước size M giá..
    {
        public string? IdOptionsName { get; set; }//group các id
        public int Id { get; set; }
        public int IdProduct { get; set; }
        public string Name { get; set; }
        public string? Img { get; set; }
        public double Price { get; set; }
        public decimal Quantity { get; set; }
        public string? SKU { get; set; }
        [ForeignKey("IdProduct")]
        public virtual Product Product { get; set; }
    }
    public class ComponentProduct // chi tiết của sản phẩm là combo,đóng gói, hay thành phần sản phẩm của món chế biến
    {
        public int Id { get; set; }
        public int IdProduct { get; set; }//khóa ngoại
        public int IdPro { get; set; }// là phẩm nào
        public EnumtypeComponentProduct TypeComponentProduct { get; set; }// là món thêm, HAY LÀ THÀNH PHẦN
        public decimal Quantity { get; set; }
        [ForeignKey("IdProduct")]
        public virtual Product Product { get; set; }
    }
}
