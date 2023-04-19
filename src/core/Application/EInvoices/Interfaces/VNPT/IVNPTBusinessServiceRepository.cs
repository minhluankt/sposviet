using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.EInvoices.Interfaces.VNPT
{
    public interface IVNPTBusinessServiceRepository
    {
        Task<string> cancelInvNoPayAsync(string useradmin, string passadmin, string fkey, string userservice, string passservice, string url);
        Task<string> SendInvMTTFkeyAsync(string useradmin, string passadmin, string lstfkey, string userservice, string passservice, string pattern, string serial, string serialCert, string url);
        Task<string> GetHashInvMTTFkeyByTokenAsync(string useradmin, string passadmin, string lstfkey, string userservice, string passservice, string pattern, string serial, string serialCert, string url);
        Task<string> SendInvMTTFkeyByTokenAsync(string useradmin, string passadmin, string xml, string userservice, string passservice, string pattern, string serial, string url);
    }
}
