using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ParameterMailTemplate
    {
        public string Title { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public string Fullname { get; set; }
        public string CusPhone { get; set; }
        public string DeliveryAddress { get; set; }
        public string Address { get; set; }
        public string ComTaxCode { get; set; }
        public string IdUserSend { get; set; }
        public int IdTypeUserSend { get; set; }
        public DateTime beforeDay { get; set; }
        public int manday { get; set; }
        public string UserUpdate { get; set; }
        public string yeucaurequest { get; set; }
        public string ThoiGian { get; set; }
        public string TableDonHang { get; set; }
        public double orderbyamount { get; set; } // tieenf don hang
        public double amount { get; set; }
        public double total { get; set; }
        public string Url { get; set; }
        public string website { get; set; }
        public string Hotline { get; set; }
    }
}
