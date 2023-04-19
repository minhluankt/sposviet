using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Cart : AuditableEntity
    {
        public Cart()
        {
            this.CartDetailts = new HashSet<CartDetailt>();
        }
        [Required]
        [ForeignKey("IdCustomer")]
        public int IdCustomer { get; set; }
        public decimal Total { get; set; }
        public decimal Amount { get; set; }
        public float VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public int Quantity { get; set; }
        public int IdPharmaceutical { get; set; }// nhân viên sell
        public string AmountInWord { get; set; }
        public string Note { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<CartDetailt> CartDetailts { get; set; }
    }
}
