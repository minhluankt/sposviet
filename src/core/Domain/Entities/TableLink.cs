using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("TableLink")]
    public partial class TableLink
    {
        [Key]
        public int ID { get; set; }
        public string slug { get; set; }
        public int? tableId { get; set; }
        [StringLength(50)]
        public string type { get; set; }
        public int? parentId { get; set; }
        public int Comid { get; set; }
    }
}
