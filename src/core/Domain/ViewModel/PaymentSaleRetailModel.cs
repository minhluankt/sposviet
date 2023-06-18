using Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class OrderInvoicePaymentSaleRetailModel
    {
        public EnumTypeProduct EnumTypeProduct { get; set; } = EnumTypeProduct.AMTHUC;
        public string Cashername { get; set; }
        public string IdCasher { get; set; }
        public int ComId  { get; set; }
        public int IdPaymentMethod  { get; set; }
        public int? IdPattern  { get; set; }
        public bool VATMTT { get; set; }
        public decimal Amount { get; set; }
        public decimal VATAmount { get; set; }
        public decimal Total { get; set; }
        public decimal? VATRate { get; set; }
        public string ArisingDate { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal CusSendAmount { get; set; }//tiền khách đưa
        public decimal Amoutchange { get; set; }//tiền thừa
        public CustomerOrderInvoiceModel Customer { get; set; }//khách hàng
        public List<ItemOrderInvoicePayment> Items { get; set; }//tiền thừa
    }
    public class ItemOrderInvoicePayment
    {
        public int Id { get; set; }
        public string Unit { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public float Discount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Price { get; set; }
        public decimal PriceNew { get; set; }
        public decimal PriceNoVAT { get; set; }
        public decimal VATRate { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Total { get; set; }
        public decimal VATAmount { get; set; }
        public decimal Amount { get; set; }
        public bool IsVAT { get; set; }
    }
    public class CustomerOrderInvoiceModel
    {
        public int Id { get; set;}
        public string Code { get; set;}
    }
}
