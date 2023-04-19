using System;

namespace Domain.ViewModel
{
    public class TemplateInvoiceModel
    {
        public TemplateInvoiceModel() { }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string screct { get; set; }
        public bool Active { get; set; }
    }
    public class TemplateInvoiceParameter
    {
        public TemplateInvoiceParameter()
        {
            this.ngaythangnamxuat = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
        public string kyhieuhoadon { get; set; }
        public string sohoadon { get; set; }//số bên diện tử
        public string invoiceNo { get; set; }//số bên bán hàng nôi bộ
        public string comname { get; set; }
        public string comaddress { get; set; }
        public string buyer { get; set; }
        public string cusPhone { get; set; }
        public string cusAddress { get; set; }
        public string casherName { get; set; }
        public string tableProduct { get; set; }
        public string ngaythangnamxuat { get; set; }
        /// <summary>
        /// //
        /// </summary>
        public string tientruocthue { get; set; }
        public string tienthue { get; set; }
        public string thuesuat { get; set; }
        public string tongtien { get; set; }
        public string khachcantra { get; set; }
        public string giamgia { get; set; }
        public string khachthanhtoan { get; set; }
        public string tienthuatrakhach { get; set; }
        public string thongtinthue { get; set; }
        public string thongtintracuuhoadon { get; set; }

        ////
        public string lienhehotline { get; set; }

    }
}
