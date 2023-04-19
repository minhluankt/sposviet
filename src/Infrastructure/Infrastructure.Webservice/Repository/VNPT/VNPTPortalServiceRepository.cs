using Application.EInvoices.Interfaces.VNPT;
using Infrastructure.Webservice.Webservice.VNPT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Webservice.Repository.VNPT
{
   
    public class VNPTPortalServiceRepository : IVNPTPortalServiceRepository
    {

        public async Task<string> getInvViewFkeyNoPayAsync(string fkey, string userservice, string passservice, string url)
        {
            var rs = await WebServiceHelper.PortalService(url).getInvViewFkeyNoPayAsync(fkey, userservice, passservice);
            return rs.Body.getInvViewFkeyNoPayResult;
        }
        public async Task<string> convertForStoreFkeyAsync(string fkey, string userservice, string passservice, string url)
        {
            var rs = await WebServiceHelper.PortalService(url).convertForStoreFkeyAsync(fkey, userservice, passservice);
            return rs.Body.convertForStoreFkeyResult;
        }

        public async Task<string> listInvByCusAsync(string cuscode, string fromdate, string todate, string userservice, string passservice, string url)
        {
            var rs = await WebServiceHelper.PortalService(url).listInvByCusAsync(cuscode, fromdate, todate, userservice, passservice);
            return rs.Body.listInvByCusResult;
        }
        public async Task<string> downloadInvFkeyNoPayAsync(string fkey, string userservice, string passservice, string url)
        {
            var rs = await WebServiceHelper.PortalService(url).downloadInvFkeyNoPayAsync(fkey, userservice, passservice);
            return rs.Body.downloadInvFkeyNoPayResult;
        }


        public async Task<string> getCusAsync(string cusCode, string userservice, string passservice, string url)
        {
            var rs = await WebServiceHelper.PortalService(url).getCusAsync(cusCode, userservice, passservice);
            return rs.Body.getCusResult;
        }

        public async Task<string> listInvByCusFkeyAsync(string fkey, string fromdate, string todate, string userservice, string passservice, string url)
        {
            var rs = await WebServiceHelper.PortalService(url).listInvByCusFkeyAsync(fkey, fromdate, todate,userservice, passservice);
            return rs.Body.listInvByCusFkeyResult;
        }
    }
}
