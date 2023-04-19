using Application.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel
{
    public class CompanyAdminInfoViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Mã số thuế")]
        public string AccountName { get; set; }//tài khoản quản trị
        public string CusTaxCode { get; set; }
        [Display(Name = "Tên công ty")]
        public string Name { get; set; }
        [Display(Name = "Website")]
        public string Website { get; set; }
        [Display(Name = "Title")]
        public string Title { get; set; }//tên nickname
        // public string Name_en { get; set; }
        [Display(Name = "Url web site")]
        public string Url { get; set; }
        [Display(Name = "Keyword dùng để seo")]
        public string Keyword { get; set; }
        [Display(Name = "Mô tả dùng để seo")]
        public string Description { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string Image { get; set; }
        [Display(Name = "Ảnh đại diện")]
        public IFormFile ImageUpload { get; set; }
        [Display(Name = "Logo")]
        public string Logo { get; set; }
        public IFormFile LogoUpload { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        // public string Address_en { get; set; }
        [DefaultValue(EnumTypeProduct.THOITRANG)]
        public EnumTypeProduct IdDichVu { get; set; }//dịch vụ gì
        public ENumTypeCustomer IdType { get; set; } // loại khách hàng
        public string Email { get; set; }
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Số fax")]
        public string FaxNumber { get; set; }
        public DateTime? StartDate { get; set; }//ngày hoạt động
        public DateTime? DateExpiration { get; set; }//ngày hết hạn
        public DateTime? CreatedOn { get; set; }
        public int NumberDateExpiration { get; set; }//số tháng khách mua 1 năm 2 năm

        public bool Active { get; set; }//khóa/mở
        public EnumStatusCompany Status { get; set; }//trạng thái
        public EnumTypeCompany TypeCompany { get; set; }//chính  thức hay thử nghiệm
        public string secret { get; set; }
    }
}
