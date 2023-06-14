using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SposVietPluginKySo
{
    public class Enum
    {
        public enum EnumTypePrint // loại in
        {
            TEST = 0,//TEST
            PrintBaoBep = 1,//báo hủy và chế biến
        }
        public enum TypeEventWebSocket
        {
            SendTestConnect = -1,//ký hóa đơn
            SignEInvoice = 0,//ký hóa đơn
            PrintEInvoice = 1,//in hóa đon điện tử
            PrintInvoice = 2,
            PrintBep = 3, 
            SignListEInvoiceToken = 4, //ký hóa đơn token
        }
    }
}
