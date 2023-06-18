using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Customer : AuditableEntity
    {
        public Customer()
        {
            this.IdCodeGuid = Guid.NewGuid();
        }
        public int? Comid { get; set; }
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IdCodeGuid { get; set; }
        public string Salt { get; set; }
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime? BirthDate { get; set; }
        
        [StringLength(500)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Buyer { get; set; }//họ tên người mua hàng
        public string Code { get; set; }//mã khách hàng
        [StringLength(14)]
        public string Taxcode { get; set; }
        [StringLength(30)]
        public string CCCD { get; set; }
        [StringLength(50)]
        public string Nationality { get; set; }//quốc tịch
        [StringLength(30)]
        public string Passport { get; set; }//số hộ chiếu khác CCCD nha
        [StringLength(100)]
        public string PhoneNumber { get; set; }
        public string Logo { get; set; }
        public string Image { get; set; } // ảnh đại diện
        public string Address { get; set; }
        public string CusBankNo { get; set; }
        public string CusBankName { get; set; }
        public int? IdDistrict { get; set; } // quận
        public int? IdCity { get; set; }
        public int? IdWard { get; set; }
        // [Required]
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(100)]
        public string EmailConfirm { get; set; }
        public bool isEmailConfirm { get; set; }
        public ENumTypeCustomer TypeCustomer { get; set; } = ENumTypeCustomer.Personal;
        public ENumTypeCustomerSEX Sex { get; set; }

        //public bool Active { get; set; } // tọa độ
        public int Status { get; set; } // trạng thái
        public DateTime? LoginLast { get; set; } // đăng nhập lần cuối
        [StringLength(50)]
        public string LoginProvider { get; set; } // login bằng gg hay face
        [StringLength(50)]
        public string ProviderKey { get; set; } // key login gg hay face
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<OrderTable> OrderTables { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }

        [ForeignKey("IdCity")]
        public City City { get; set; }
        [ForeignKey("IdDistrict")]
        public District District { get; set; }
        [ForeignKey("IdWard")]
        public Ward Ward { get; set; }


    }
    public class ManagerIdCustomer
    {
        public int Id { get; set; }
        public int ComId { get; set; }
        public int CusNo { get; set; }
    }
}
