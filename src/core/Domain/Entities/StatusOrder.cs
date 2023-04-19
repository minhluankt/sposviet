using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class StatusOrder
    {
        [Key]
        public int Id { get; set; }
        public int IdOrder { get; set; }
        public bool IsCustomer { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
        public string FullNameUpdate { get; set; }
        public int CountUpdate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
