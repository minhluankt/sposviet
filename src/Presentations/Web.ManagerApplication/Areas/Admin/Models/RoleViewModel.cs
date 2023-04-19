using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ManagerApplication.Areas.Admin.Models
{
    public class RoleViewModel
    {
        [Display(Name = "Tên nhóm quyền")]
        public string Name { get; set; }
        [Display(Name = "Mã nhóm")]
        public string Code { get; set; }
        [Display(Name = "Id nhóm quyền")]
        public string Id { get; set; }
    }
}
