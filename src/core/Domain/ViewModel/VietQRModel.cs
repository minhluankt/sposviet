using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class VietQRModel
    {
        public int Id { get; set; }
        public string AccountName { get; set; }//tên tài khonar
        [StringLength(500)]
        public string BankName { get; set; }//tên ngân hàng
        [StringLength(50)]
        public string BankNumber { get; set; } // số tài khoản
        [StringLength(300)]
        public string BankAddress { get; set; } // địa chỉ ngân hàng
        [StringLength(300)]
        public string Note { get; set; } // ghi chú
        public string Code { get; set; }
        public string ShortName { get; set; } // ví dụ seabank, vietinbank
        public string template { get; set; } 
        public int? BinVietQR { get; set; } // mã bin ngân hàng VietQR
      
    }
}
