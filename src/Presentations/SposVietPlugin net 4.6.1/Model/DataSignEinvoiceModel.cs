using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SposVietPlugin_net_4._6._1.Enum;

namespace SposVietPlugin_net_4._6._1.Model
{
    public class DataSignEinvoiceModel
    {
        public int? type { get; set; }//type string
        public TypeEventWebSocket TypeEventWebSocket {

            get
            {
                if (type!=null)
                {
                    return (TypeEventWebSocket)type;
                }
                return TypeEventWebSocket.SendTestConnect;
            }
        }
        public string hash { get; set; }
        public string xmlbyhash { get; set; }
        public string serialCert { get; set; }
        public string html { get; set; }
    }
    public class ResponseModel<T>
    {
        public int type { get; set; }//type string
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
