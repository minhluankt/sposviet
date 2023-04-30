using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class CompanyAdminInfo : AuditableEntity
    {
        public CompanyAdminInfo()
        {
            this.VFkeyPhone = $"PK{IdDichVu}{PhoneNumber}";
            if (!string.IsNullOrEmpty(CusTaxCode))
            {
                this.VFkeyCusTaxCode = $"PK{IdDichVu}{CusTaxCode}";
            }
        }

        public string CusTaxCode { get; set; }
        public string AccountName { get; set; }//tài khoản quản trị
        [Required]
        public string Name { get; set; }
        [StringLength(20)]
        public string VFkeyCusTaxCode
        {
            get
            {
                if (!string.IsNullOrEmpty(CusTaxCode))
                {
                    return $"PK{IdDichVu}{CusTaxCode}";
                }
                return null;
            }
            set
            {

            }
        }
        [StringLength(20)]
        public string VFkeyPhone
        {
            get
            {
                return $"PK{IdDichVu}{PhoneNumber}";
            }
            set
            {

            }
        }

        [StringLength(50)]
        public string Website { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }//tên heiern thị
        public string Keyword { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public string Image { get; set; }
        [Required]
        public string Address { get; set; }
        public ENumTypeCustomer IdType { get; set; } // loại khách hàng cá nhân hay doanh nghiệp
        public int? IdCity { get; set; }
        public int? IdDistrict { get; set; }
        public int? IdWard { get; set; }
        [DefaultValue(EnumTypeProduct.THOITRANG)]
        public EnumTypeProduct IdDichVu { get; set; }//dịch vụ gì
        public string Email { get; set; }
        public string Hotline { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Domain { get; set; }
        public DateTime? StartDate { get; set; }//ngày hoạt động
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateExpiration { get; set; }//ngày hết hạn
        public int NumberDateExpiration { get; set; }//số tháng khách mua 1 năm 2 năm
        [DefaultValue(true)]
        public bool Active { get; set; }//khóa/mở
        public EnumStatusCompany Status { get; set; }//trạng thái
        public EnumTypeCompany TypeCompany { get; set; }//chính  thức hay thử nghiệm
    }
}
