using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DefaultFoodOrder : AuditableEntity// đăng ký tư vấn
    {
        public Guid IdItem { get; set; }
        public int ComId { get; set; }
        public int IdProduct { get; set; }
        public decimal Quantity { get; set; }
        public Product Product { get; set; }
    }
}

