using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class LoginCustomerViewModel
    {
        [DataType(DataType.EmailAddress, ErrorMessage = "Email không đúng định dạng")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]

        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }
        public string ReturnUrl { get; set; }
        public List<AuthenticationScheme> ExternalLogins { get; set; }

        [Required]
        [Display(Name = "Mật khẩu")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{6,}$", ErrorMessage = "Mật khẩu ít nhất 6 ký tự, có chứa chữ hoa, chữ thường và ký tự đặt biệt")]
        [StringLength(20, ErrorMessage = "Mật khẩu ít nhất {2} ký tự và tối đa {0} ký tự", MinimumLength = 6)]
        //[StringLength(maximumLength:50, ErrorMessage="Mật khẩu tối đa 50 ký tự")]
        public string Password { get; set; }
    }
}
