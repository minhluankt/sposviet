using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class ProductInBarAndKitchenModel
    {
        public int Id { get; set; }
        public int IdBarAndKitchen { get; set; }
        public int IdProduct { get; set; }
        public int IdCategoryProduct { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string ProName { get; set; }
        public string ProCode { get; set; }
        public string Img { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string secret { get; set; }
    }
}
