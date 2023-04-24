using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SposVietPluginKySo.Enum;

namespace SposVietPluginKySo.Model
{
    public class PrintModel
    {
        public string data { get; set; }
        public int ComId { get; set; }
        public int Type { get; set; }
        public EnumTypePrint type
        {
            get
            {
                return (EnumTypePrint)Type;
            }
        }
        
    }
}
