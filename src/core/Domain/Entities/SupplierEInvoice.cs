using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SupplierEInvoice : AuditableEntity// cấu hình các nhà einvoice
    {
        public int ComId { get; set; }//
        public SupplierEInvoice() { }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }// vi dụ VNPT
        public ENumTypeSeri TypeSeri { get; set; }// chữ ký số
        public string DomainName { get; set; }//
        public string UserNameAdmin { get; set; }//
        public string PassWordAdmin { get; set; }//
        public string UserNameService { get; set; }//
        public string PassWordService { get; set; }//
        public string SerialCert { get; set; }// seri chứng thư số
        public bool Selected { get; set; }//mặc định hiển thị nhà cung cấp này
        public bool Active { get; set; }
        [NotMapped]
        public List<SelectListItem> Selectlist { get; set; }
        public virtual ICollection<ManagerPatternEInvoice> ManagerPatternEInvoices { get; set; }
    }
}
