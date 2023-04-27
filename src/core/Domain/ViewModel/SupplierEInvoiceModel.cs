using Application.Enums;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class SupplierEInvoiceModel
    {
        public SupplierEInvoiceModel() { 
        this.ManagerPatternEInvoices = new List<ManagerPatternEInvoice> ();
        }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }// vi dụ VNPT
        public int Id { get; set; }//
        public ENumTypeSeri TypeSeri { get; set; }// chữ ký số
        public string CompanyName { get; set; }//
        public string screcttype { get; set; }//
        public string screct { get; set; }//
        public string DomainName { get; set; }//
        public string UserNameAdmin { get; set; }//
        public string PassWordAdmin { get; set; }//
        public string UserNameService { get; set; }//
        public string PassWordService { get; set; }//
        public bool IsHKD { get; set; }
        public bool Selected { get; set; }
        public bool Active { get; set; }
        public int SaleRetail { get; set; }
        public  List<ManagerPatternEInvoice> ManagerPatternEInvoices { get; set; }
    }
}
