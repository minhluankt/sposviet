using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public int? IdProduct { get; set; }
        public int? IdCustomer { get; set; }
        public int IdPattern { get; set; }
        public string IdUser { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public string ListImg { get; set; }
        public int DeviceType { get; set; } //là máy tính hay mobile...
        public string? DeviceName { get; set; } //ten là máy tính hay mobile...
        public string? Browser { get; set; } //trình dueyjet...
        public string? OS { get; set; } //hệ điều hành...
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime? EditDate { get; set; }
        [ForeignKey("IdProduct")]
        public Product Product { get; set; }
        [ForeignKey("IdCustomer")]
        public Customer Customer { get; set; }
    }
}
