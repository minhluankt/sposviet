using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.EInvoices.Interfaces.VNPT
{
    public interface IVNPTPortalServiceRepository
    {
        Task<string> convertForStoreFkeyAsync(string fkey, string userservice, string passservice, string url);
        Task<string> downloadInvFkeyNoPayAsync(string fkey, string userservice, string passservice, string url);
        Task<string> getCusAsync(string cusCode, string userservice, string passservice, string url);
        Task<string> getInvViewFkeyNoPayAsync(string fkey, string userservice, string passservice, string url);
        Task<string> getNewInvViewFkeyAsync(string fkey, string userservice, string passservice, string url);
        Task<string> listInvByCusAsync(string cuscode, string fromdate, string todate, string userservice, string passservice, string url);
        Task<string> listInvByCusFkeyAsync(string fkey, string fromdate, string todate, string userservice, string passservice, string url);
    }

}
