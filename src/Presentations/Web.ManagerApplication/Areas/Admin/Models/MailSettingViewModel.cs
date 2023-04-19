using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ManagerApplication.Areas.Admin.Models
{
    public class MailSettingViewModel
    {
        public bool ChangePass { get; set; }
        public int Id { get; set; }
        [Display(Name = "Tài khoản gửi mail")]
        public string From { get; set; }
        [Display(Name = "Host email")]
        public string Host { get; set; }
        [Display(Name = "Port mail")]
        public int Port { get; set; }
        public bool SSL { get; set; }
        [Display(Name = "Tên tài khoản email")]
        public string UserName { get; set; }
        [Display(Name = "Mật khẩu email")]
        public string Password { get; set; }
        [Display(Name = "Tên email")]
        public string DisplayName { get; set; }
    }
}
