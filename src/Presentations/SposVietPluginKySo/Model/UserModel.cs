using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SposVietPluginKySo.Model
{
    public class UserModel
    {
        public string Id { get; set; }
        public string email { get; set; }
        public string userName { get; set; }
        public string jwToken { get; set; }
        public int comId { get; set; }
    }
}
