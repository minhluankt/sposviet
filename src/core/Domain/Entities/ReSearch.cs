using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ReSearch
    {
        public int Id { get; set; }
        public string Name { get; set; } // từ khóa
        public int ProductType { get; set; }//0 là sản phẩm thêm các loại hình bán khác
        public string Slug { get; set; }
        public string Url { get; set; }
        public int? IdCustomer { get; set; }
        public int NumberSearches { get; set; } // số lần tìm kiếm
        public DateTime Date { get; set; } // 

    }
}
