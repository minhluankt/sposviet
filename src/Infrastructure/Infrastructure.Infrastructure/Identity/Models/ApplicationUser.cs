using Application.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Infrastructure.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int ComId { get; set; }
        public EnumTypeProduct IdDichVu { get; set; } = EnumTypeProduct.AMTHUC;//loại dịch vụ ẩm thực hay thời trang
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] ProfilePicture { get; set; }
        public int Level { get; set; }//là admin cấp cao supper, dc tạo từ company
        public bool IsActive { get; set; } = false;
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool LockoutForever { get; set; }
        public bool IsStoreOwner { get; set; }//chủ cửa hàng
    }
    public class ApplicationRole :  IdentityRole<string>
    {
        public ApplicationRole() { }
        public ApplicationRole(string roleName) : this()
        {
            Id = Guid.NewGuid().ToString();
            Name = roleName;
        }
       
        public int ComId { get; set; }
        [StringLength(30)]
        public string Code { get; set; } 
        [StringLength(40)]
        public string VKey { get {
                if (ComId>0 && !string.IsNullOrEmpty(Code))
                {
                    return $"{ComId}{Code.ToLower()}";
                }
                return "";
            }
            set { }
        }
    }
}
