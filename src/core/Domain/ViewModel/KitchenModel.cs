using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class KitchenModel
    {
        public int IdItemOrderNew { get; set; }
        public int IdItemOrder { get; set; }
        public int? IdProduct { get; set; }
        public decimal Quantity { get; set; }
    }
}
