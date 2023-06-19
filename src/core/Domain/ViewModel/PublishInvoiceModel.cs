using Application.Enums;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class PublishInvoiceModelView
    {
        public PublishInvoiceModelView()
        {
            this.DetailInvoices = new List<DetailInvoice>();
        }
        public bool IsError { get; set; }
        public string Note { get; set; }// ghi chú bất kỳ
        public string Hash { get; set; }// chuỗi hash VNPT trả về
        public string XmlByHashValue { get; set; }// chuỗi xml có hash VNPT trả về
        public string Pattern { get; set; }// 
        public string Serial { get; set; }//
        public string SerialCert { get; set; }//
        public ENumTypeSeri TypeSeri { get; set; }// dạng ký số gì
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }// phast hafnh hay tap moi
        public EnumTypeEventInvoice TypeEventInvoice { get; set; }// phast hafnh hay tap moi
        public List<DetailInvoice> DetailInvoices { get; set; }
        public PublishInvoiceResponse PublishInvoiceResponse { get; set; }
    }
    public class PublishInvoiceResponse
    {
        public Invoice Invoice { get; set; }
        public string Pattern { get; set; }
        public string Serial{ get; set; }
        public int InvoiceNo{ get; set; }
        public string UrlDomain{ get; set; }
        public string Fkey{ get; set; }
        public string MCQT{ get; set; }
        public string Message { get; set; }
        public bool IsSuccess{ get; set; }
        public bool IsProductVAT { get; set; }//sản phẩm có thuế
    }
    public class PublishInvoiceModel
    {
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }// vi dụ VNPT
        public int[] lstid { get; set; }//
        public int ComId { get; set; }//mẫu số ký hiệu nào
        public EnumTypeEventInvoice TypeEventInvoice { get; set; }// phast hafnh hay tap moi
        public int IdManagerPatternEInvoice { get; set; }//mẫu số ký hiệu nào
       
        public float VATRate { get; set; }//thuế s
        public decimal VATAmount { get; set; }//
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ArisingDate { get; set; }//
        public decimal Total { get; set; }//
        public decimal Amount { get; set; }//
        public string CasherName { get; set; }//thuế s
        public string IdCarsher { get; set; }//thuế s
        public bool isVAT { get; set; }//có xuất hóa đơn 
        public ENumTypeEInvoice TypeEInvoice { get; set; }//hóa đơn GTGt
    }
    public class DetailInvoice
    {
        public ENumTypePublishEinvoice TypePublishEinvoice { get; set; }
        public string code { get; set; }//số hóa đơn bên đơn bán
        public string note { get; set; }
        public string token { get; set; }
    }
}
