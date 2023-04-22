using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SposVietPluginKySo
{
    public class Enum
    {
        public enum TypeEventWebSocket
        {
            SendTestConnect = -1,//ký hóa đơn
            SignEInvoice = 0,//ký hóa đơn
            PrintEInvoice = 1,//in hóa đon điện tử
            PrintInvoice = 2, //in bill
            PrintBep = 3, //in bill
        }
    }
}
