using Application.EInvoices.Interfaces.VNPT;
using Application.Enums;
using Infrastructure.Webservice.Webservice.VNPT;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
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
            var rs = await WebServiceHelper.PublishService(url).GetMCCQThueByFkeysAsync(Account, ACpass, username, password, pattern, fkeys);
            return rs.Body.GetMCCQThueByFkeysResult;
        }

        public async Task<string> ImportAndPublishInvMTTAsync(string url, string xml, string userservice, string passservice, string useradmin, string passadmin, string pattern, string serial)
        {
            var rs = await WebServiceHelper.PublishService(url).ImportAndPublishInvMTTAsync(useradmin, passadmin, xml, userservice, passservice, pattern, serial, 0);
            return rs.Body.ImportAndPublishInvMTTResult;
        }
        public async Task<string> ImportAndPublishInvAsync(string url, string xml, string userservice, string passservice, string useradmin, string passadmin, string pattern, string serial)
        {
            var rs = await WebServiceHelper.PublishService(url).ImportAndPublishInvAsync(useradmin, passadmin, xml, userservice, passservice, pattern, serial, 0);
            return rs.Body.ImportAndPublishInvResult;
        }
        public async Task<string> ImportAndPublishInvMTT32Async(string url, string xml, string userservice, string passservice, string useradmin, string passadmin, string pattern, string serial)
        {
            var rs = await WebServiceHelper.PublishService(url).ImportAndPublishInvMTT32Async(useradmin, passadmin, xml, userservice, passservice, pattern, serial, 0);
            return rs.Body.ImportAndPublishInvMTT32Result;
        }
        public async Task<string> ImportInvByPatternMTTAsync(string url, string xml, string userservice, string passservice, string useradmin, string passadmin, string pattern, string serial)
        {
            var rs = await WebServiceHelper.PublishService(url).ImportInvByPatternMTTAsync(useradmin, passadmin, xml, userservice, passservice, pattern, serial, 0);
            return rs.Body.ImportInvByPatternMTTResult;
        }
        // token phát hành 
        public async Task<string> PublishInvWithTokenAsync(string url, string xml, string userservice, string passservice, string useradmin, string passadmin, string pattern, string serial)
        {
            var rs = await WebServiceHelper.PublishService(url).publishInvWithTokenAsync(useradmin, passadmin, xml, userservice, passservice, pattern, serial);
            return rs.Body.publishInvWithTokenResult;
        }
        //lấy hash token
        public async Task<string> GetHashInvWithToken(string url, string xml, string userservice, string passservice, string useradmin, string passadmin, string serialCert, ENumTypePublishServiceEInvoice type, string invToken, string pattern, string serial, int convert = 0)
        {
            var rs = await WebServiceHelper.PublishService(url).getHashInvWithTokenAsync(useradmin, passadmin, xml, userservice, passservice, serialCert, (int)type,invToken, pattern, serial, convert);
            return rs.Body.getHashInvWithTokenResult;
        }

        public async Task<string> ImportInvByPatternAsync(string url, string xml, string userservice, string passservice, string useradmin, string passadmin, string pattern, string serial)
        {
            var rs = await WebServiceHelper.PublishService(url).ImportInvByPatternAsync(useradmin, passadmin, xml, userservice, passservice,pattern, serial, 0);
            return rs.Body.ImportInvByPatternResult;
        }

        public async Task<string> deleteInvoiceByFkey(string url, string lstFkey, string userservice, string passservice, string useradmin, string passadmin)
        {
            var rs = await WebServiceHelper.PublishService(url).deleteInvoiceByFkeyAsync(lstFkey,userservice, passservice, useradmin, passadmin);
            return rs.Body.deleteInvoiceByFkeyResult;
        }
    }
}
