using Application.EInvoices.Interfaces.VNPT;
using Infrastructure.Webservice.Webservice.VNPT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Webservice.Repository.VNPT
{
    public class VNPTPublishServiceRepository : IVNPTPublishServiceRepository
    {
        public async Task<string> GetMCCQThueByFkeys(string Account, string ACpass, string username, string password, string pattern, string fkeys, string url)
        {
            var rs = await WebServiceHelper.PublishService(url).GetMCCQThueByFkeysAsync(Account, ACpass, username, password,pattern, fkeys);
            return rs.Body.GetMCCQThueByFkeysResult;
        }

        public async Task<string> ImportAndPublishInvMTTAsync(string url, string xml, string userservice, string passservice, string useradmin, string passadmin, string pattern, string serial)
        {
            var rs = await WebServiceHelper.PublishService(url).ImportAndPublishInvMTTAsync(useradmin, passadmin,xml,userservice, passservice, pattern,serial,0);
            return rs.Body.ImportAndPublishInvMTTResult;
        } 
    }
}
