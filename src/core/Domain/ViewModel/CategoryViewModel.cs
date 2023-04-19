using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.ViewModel
{
    public class CategoryViewModel
    {
        public int id { get; set; }
        public int IdLevel { get; set; }
        public int? IdPattern { get; set; }
        public int IdTypeCategory { get; set; } // chuyên mục cho loại dịch vụ nào ví dụ sản phẩm hay tin tức
        public int Sort { get; set; } // chuyên mục cho loại dịch vụ nào ví dụ sản phẩm hay tin tức
        public string prefix { get; set; }
        [Display(Name = "Tên danh mục")]
        public string fullName { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string position { get; set; }
        public bool active { get; set; }
        public bool expanded { get; set; }
        public bool createItem { get; set; }
        public bool selected { get; set; }
        public IEnumerable<CategoryViewModel> items { get; set; }
        public IEnumerable<TypeCategory> Itemategory { get; set; }
        public IEnumerable<SelectListItem> SelectListItemCate { set; get; }
    }
}
