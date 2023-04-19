using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CartDetailt
    {
        public int Id { get; set; }
        [ForeignKey("IdCart")]
        [Required]
        public int IdCart { get; set; }
   
        [ForeignKey("IdProduct")]
        public int? IdProduct { get; set; }
        public int Quantity { get; set; }
        public bool isSelected { get; set; }// tích chọn để mua
        public bool isDisable { get; set; }// tích chọn để mua
        public string Discount { get; set; }
        public string Name { get; set; }//nếu sp bị xóa mới dùng name này
        public virtual Cart Cart { get; set; }
        public virtual Product Product { get; set; }
    }
}
