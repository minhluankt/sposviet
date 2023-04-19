using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DeliveryCompany : AuditableEntity//đươn vị giao hàng
    {
        [Required]
        [StringLength(700)]
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sort { get; set; }
        public virtual ICollection<Order> Order { get; set; }
        //tạm thời để đây chưa biết làm gì thêm
    }
}
