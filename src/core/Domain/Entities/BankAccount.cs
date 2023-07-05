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
    public class BankAccount : AuditableEntity
    {
        public int ComId { get; set; }
        [StringLength(500)]
        public string AccountName { get; set; }//tên tài khonar
        [StringLength(500)]
        public string BankName { get; set; }//tên ngân hàng
        [StringLength(50)]
        public string BankNumber { get; set; } // số tài khoản
        [StringLength(300)]
        public string BankAddress { get; set; } // địa chỉ ngân hàng
        public string Note { get; set; } // ghi chú
        [StringLength(50)]
        public string Code { get; set; } 
        [StringLength(100)]
        public string ShortName { get; set; } // ví dụ seabank, vietinbank
        public int? BinVietQR { get; set; } // mã bin ngân hàng VietQR
        public bool IsSetDefault { get; set; } // mặc định 
        public bool Active { get; set; }
        [NotMapped]
        public string secret { get; set; }
        public VietQR? VietQR { get; set; }
    }
}
