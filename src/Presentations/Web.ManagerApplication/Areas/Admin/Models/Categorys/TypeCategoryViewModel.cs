using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ManagerApplication.Areas.Admin.Models.Categorys
{
    public class TypeCategoryViewModel
    {
        [Display(Name = "Tên danh mục")]
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Tên view")]
        [Required]
        public string View { get; set; }
    }
}
