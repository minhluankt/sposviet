using Application.Enums;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    public class CustomerModel
    {
        public CustomerModel()
        {
            this.IdCodeGuid = Guid.NewGuid();
        }
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
        public DateTime? LoginLast
        {
            get;
            set;
        }
        public DateTime? BirthDate
        {
            get;
            set;
        }
        public Guid IdCodeGuid { get; set; }
        public int? Comid { get; set; }
        public string Salt { get; set; }
        public string UserName { get; set; }
        public string LoginProvider { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Name { get; set; }
        public string Buyer { get; set; }
        public string Logo { get; set; }
        public string Image { get; set; } // ảnh đại diện
        public IFormFile LogoUpload { get; set; } // ảnh đại diện
        public IFormFile ImageUpload { get; set; } // ảnh đại diện

        [StringLength(50)]
        public string CusBankNo { get; set; }
        [StringLength(400)]
        public string CusBankName { get; set; }
        public string Address { get; set; }
        public int? IdDistrict { get; set; } // quận
        public int? IdCity { get; set; }
        public int? IdWard { get; set; }
        public string Email { get; set; }
        public string createdate { get; set; }
        public string Taxcode { get; set; }
        public string CCCD { get; set; }
        public string Nationality { get; set; }//quốc tịch
     
        public string Passport { get; set; }//số hộ chiếu khác CCCD nha
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
        public ENumTypeCustomerSEX Sex { get; set; }
        public ENumTypeCustomer TypeCustomer { get; set; } = ENumTypeCustomer.Personal;
        public string UrlParameters { get; set; }
        public string TextPhoneOrEmail { get; set; }
        public int Status { get; set; } // tọa độ
        public decimal? Total { get; set; } // tọa độ
        [NotMapped]
        public bool IsPos { get; set; } // tọa độ
    }

    public class CustomerModelView : LoginCustomerViewModel
    {
        public string TextPhoneOrEmail { get; set; }
        public string EmailConfirm { get; set; }
        public bool isEmailConfirm { get; set; }
        public int payment { get; set; }
        public string ConfirmPassword { get; set; }
        public string PasswordOld { get; set; }
        public Guid IdCodeGuid { get; set; }
        public string Salt { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Image { get; set; }
        public IFormFile ImageUpload { get; set; }
        public IFormFile LogoUpload { get; set; }

        public string Address { get; set; }
        public int Id { get; set; } // quận
        public int? IdDistrict { get; set; } // quận
        public int? IdCity { get; set; }
        public int? IdWard { get; set; }
        public DateTime? LoginLast { get; set; } // đăng nhập lần cuối
        public string LoginProvider { get; set; } // login bằng gg hay face

        public string ProviderKey { get; set; } // key login gg hay face
        public string PhoneNumber { get; set; }
        public string secretId { get; set; }
        public bool Active { get; set; } // tọa độ
        public bool RegisterPopup { get; set; } // đăng ký bật popup
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<OrderViewModel> Orders { get; set; }
        public virtual ICollection<CartDetailtModel> CartDetailts { get; set; }
        public int TotalRow { get; set; }
        public int IdCart { get; set; }
        public IEnumerable<City> Citys { get; set; }
        public City City { get; set; }
        public District District { get; set; }
        public Ward Ward { get; set; }
        public Customer Customer { get; set; }
        public CompanyAdminInfo CompanyAdminInfo { get; set; }
        public CartModel CartModel { get; set; }
    }

    public class CartModel
    {
        public decimal Price { get; set; } // giá gốc
        public string Name { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal Total { get; set; }
        public int Id { get; set; }
        public string secretId { get; set; }
    }
    public class CartDetailtModel
    {
        public DateTime? ExpirationDateDiscount { get; set; } //
        public float Discount { get; set; } //
        public float DiscountRun { get; set; } //
        public decimal PriceDiscount { get; set; } //
        public decimal PriceDiscountRun { get; set; } //
        public decimal Price { get; set; } // giá gốc
        public string Name { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public bool isSelected { get; set; }// tích chọn để mua
        public bool isDisable { get; set; }// nếu sản phẩm bị xóa hoặc hết hàng thì disable k cho đăt
        public bool isPromotion { get; set; }// nếu sản phẩm bị xóa hoặc hết hàng thì disable k cho đăt
        public bool isPromotionRun { get; set; }// nếu sản phẩm bị xóa hoặc hết hàng thì disable k cho đăt
        public int Id { get; set; }
        public int IdProduct { get; set; }
        public int IdCart { get; set; }
        public string Img { get; set; }
        public string CreatedOn { get; set; }
        public string secretId { get; set; }
    }
    public class AutocompleteCustomerModel
    {
        public int Id { get; set; }
        public string Cccd { get; set; }
        public string CusBankNo { get; set; }
        public string CusBankName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Taxcode { get; set; }
        public string Code { get; set; }
        public string Buyer { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Img { get; set; }
    }
}
