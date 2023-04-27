using Domain.ApiModel.VNPT_HKD;
using Domain.ApiModel.VNPT_HKD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.EInvoices.Interfaces.VNPT
{
    public interface IVNPTHKDApiRepository
    {
        string GetTokenCache(int ComId);
        public Task<ResponseLoginModel> Login(int ComId, string url, string user, string pass);
        public Task<ResponseLoginModel> XemHoaDon(string url, string token,int IdHoaDon);
        public Task<ResponseLoginModel> ThemHoaDon(string url, string token, InvoicesHKDModel data);
    }
}
