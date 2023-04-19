using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.EInvoices.Interfaces.VNPT
{
    public interface IVNPTPublishServiceRepository
    {

        public Task<string> GetMCCQThueByFkeys(string Account, string ACpass, string username, string password, string pattern, string fkeys, string url);
        public Task<string> ImportAndPublishInvMTTAsync(string url,string xml, string userservice, string passservice,string useradmin, string passadmin,string pattern,string serial);
    }
}
