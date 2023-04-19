using Application.EInvoices.Interfaces.VNPT;
using Infrastructure.Webservice.Webservice.VNPT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Webservice.Repository.VNPT
{
    public class VNPTBusinessServiceRepository : IVNPTBusinessServiceRepository
    {
        public async Task<string> cancelInvNoPayAsync(string useradmin, string passadmin,string fkey, string userservice, string passservice, string url)
        {
            var rs = await WebServiceHelper.BusinessService(url).cancelInvNoPayAsync(useradmin,passadmin,fkey, userservice, passservice);
            return rs.Body.cancelInvNoPayResult;
        }
       
        public async Task<string> SendInvMTTFkeyAsync(string useradmin, string passadmin, string lstfkey, string userservice, string passservice, string pattern, string serial, string serialCert, string url)
        {
            var rs = await WebServiceHelper.BusinessService(url).SendInvMTTFkeyAsync(useradmin, passadmin, lstfkey, userservice, passservice, pattern, serial, serialCert);
            return rs.Body.SendInvMTTFkeyResult;
        }
        //token
        public async Task<string> GetHashInvMTTFkeyByTokenAsync(string useradmin, string passadmin, string lstfkey, string userservice, string passservice, string pattern, string serial, string serialCert, string url)
        {
            var rs = await WebServiceHelper.BusinessService(url).GetHashInvMTTFkeyByTokenAsync(useradmin, passadmin, lstfkey, userservice, passservice, pattern, serial, serialCert);
            return rs.Body.GetHashInvMTTFkeyByTokenResult;
        }
         public async Task<string> SendInvMTTFkeyByTokenAsync(string useradmin, string passadmin, string xml, string userservice, string passservice, string pattern, string serial, string url)
        {
            var rs = await WebServiceHelper.BusinessService(url).SendInvMTTFkeyByTokenAsync(useradmin, passadmin, xml, userservice, passservice, pattern, serial);
            return rs.Body.SendInvMTTFkeyByTokenResult;
        }

    }
}
