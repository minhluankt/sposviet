using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BankAccount : AuditableEntity
    {
        public int IdPaymentMethod { get; set; }
        public string AccountName { get; set; }//tên tài khonar
        public string BankName { get; set; }//tên ngân hàng
        public string BankNumber { get; set; } // số tài khoản
        public string BankAddress { get; set; } // địa chỉ ngân hàng
        public string Slug { get; set; }
        public bool Active { get; set; }
        public virtual ICollection<Order> Order { get; set; }
    }
}
