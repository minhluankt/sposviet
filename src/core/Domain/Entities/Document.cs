using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Document
    {
        [Key]
        public int Id { get; set; }
        public int IdProduct { get; set; }
        public string Name { get; set; }
        public double Size { get; set; }
        [ForeignKey("IdProduct")]
        public Product Product { get; set; }
    }
}
