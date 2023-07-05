using Application.Enums;
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
        public string giovao { get; set; }
        public string kyhieuhoadon { get; set; }
        public string sohoadon { get; set; }//số bên diện tử
        public string invoiceNo { get; set; }//số bên bán hàng nôi bộ
        public string comname { get; set; }
        public string comphone { get; set; }
        public string comemail { get; set; }
        public string comaddress { get; set; }

        public string buyer { get; set; }
        public string cusPhone { get; set; }
        public string cuscode { get; set; }
        public string cusAddress { get; set; }
        public string casherName { get; set; }
        public string staffName { get; set; }
        public string tableProduct { get; set; }
        public string ngaythangnamxuat { get; set; }
        /// <summary>
        /// //
        /// </summary>
        public bool isVAT { get; set; }
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
      
        public string tongsoluong { get; set; }
        public string tenbanphong { get; set; }

        ////
        public string lienhehotline { get; set; }//là đơn vị cung cấp giả pháp sposviet
        //
        public string linktracuu { get; set; }//thong tin tra cứu hóa đơn
        public string matracuu { get; set; }//mã tra cứu
        public string macoquanthue { get; set; }//mã cơ quan thuế
        public string chu_tai_khoan { get; set; }
        public string so_tai_khoan { get; set; }
        public string ten_ngan_hang { get; set; }
        public string infoqrcodethanhtoan { get; set; }//thông tin qrocde thanh toán
        public EnumTypeTemplatePrint TypeTemplatePrint { get; set; }

    }
}
