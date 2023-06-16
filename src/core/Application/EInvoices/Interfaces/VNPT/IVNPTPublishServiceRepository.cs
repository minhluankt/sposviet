using Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.EInvoices.Interfaces.VNPT
{
    public interface IVNPTPublishServiceRepository
    {
        Task<string> ImportAndPublishInvAsync(string url, string xml, string userservice, string passservice, string useradmin, string passadmin, string pattern, string serial);
        Task<string> ImportAndPublishInvMTT32Async(string url, string xml, string userservice, string passservice, string useradmin, string passadmin, string pattern, string serial);
        Task<string> ImportInvByPatternMTTAsync(string url, string xml, string userservice, string passservice, string useradmin, string passadmin, string pattern, string serial);
        Task<string> ImportInvByPatternAsync(string url, string xml, string userservice, string passservice, string useradmin, string passadmin, string pattern, string serial);
         Task<string> GetMCCQThueByFkeys(string Account, string ACpass, string username, string password, string pattern, string fkeys, string url);
         Task<string> ImportAndPublishInvMTTAsync(string url,string xml, string userservice, string passservice,string useradmin, string passadmin,string pattern,string serial);
        Task<string> PublishInvWithTokenAsync(string url, string xml, string userservice, string passservice, string useradmin, string passadmin, string pattern, string serial);
        Task<string> GetHashInvWithToken(string url, string xml, string userservice, string passservice, string useradmin, string passadmin, string serialCert, ENumTypePublishServiceEInvoice type, string invToken, string pattern, string serial, int convert = 0);
        Task<string> deleteInvoiceByFkey(string url, string lstFkey, string userservice, string passservice, string useradmin, string passadmin);
    }
}
