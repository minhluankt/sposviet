using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class DefaultFoodOrderModel
    {
        public int Id { get; set; }
        public Guid IdItem { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public string ProName { get; set; }
        public string CategoryName { get; set; }
        public int? IdCategory { get; set; }//tên danh mục
        public string Unit { get; set; }
    }
}
