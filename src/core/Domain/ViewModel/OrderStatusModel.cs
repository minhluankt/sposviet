using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class OrderStatusModel
    {
        public List<StatusOrder> StatusOrders { get; set; }
        public Order Order { get; set; }
    }
}
