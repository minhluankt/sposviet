using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class VietQR : AuditableEntity
    {
        public int ComId { get; set; }
        public int IdBankAccount { get; set; }//id của số tài khoản ngân hàng đã lưu bảng BankAccount
        public string Template { get; set; }
       
        public string qrCode { get; set; }//thẻ chứ data qrcode
        [NotMapped]
        public string qrDataURL { get; set; } //này là base64 qrcode
        [NotMapped]
        public string secret { get; set; }
        public BankAccount? BankAccount { get; set; }
    }
}
