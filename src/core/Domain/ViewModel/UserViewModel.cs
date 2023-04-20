using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class UserViewModel
    {
        [Display(Name = "Số diện thoại")]
        public string PhoneNumber { get; set; }
        public bool LockoutForever { get; set; }
        [Display(Name = "Tên đệm")]
        public string FirstName { get; set; }
        [Display(Name = "Họ và tên")]

        public string FullName { get; set; }
        [Display(Name = "Họ tên")]
        public string LastName { get; set; }
        [Display(Name = "Email")]

        public string Email { get; set; }
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }
        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = true;
        [Required(ErrorMessage = "Vui lòng nhật mật khẩu")]
        [Display(Name = "Mật khẩu")]
        [StringLength(20, ErrorMessage = "Mật khẩu ít nhất {2} ký tự và tối đa {0} ký tự", MinimumLength = 6)]

       // [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string ConfirmPassword { get; set; }

        public byte[] ProfilePicture { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Id { get; set; }
        [Display(Name = "Người tạo")]
        public string CreatedBy { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
