using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class CartModelView
    {
        public int Id { get; set; }
        public int IdProduct { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
        public decimal Amount { get; set; }
        public float VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public int IdPharmaceutical { get; set; }// nhân viên sell
        public string AmountInWord { get; set; }
        public string Note { get; set; }
        public bool AddCart { get; set; }
        public Product Product { get; set; }

    } 
    public class OrderModelView
    {
        public Customer Customer { get; set; }
        public Order Order { get; set; }

    }
   
}
