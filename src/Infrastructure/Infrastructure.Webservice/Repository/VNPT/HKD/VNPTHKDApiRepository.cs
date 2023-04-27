using Application.CacheKeys;
using Application.Constants;
using Application.EInvoices.Interfaces.VNPT;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Domain.ApiModel.VNPT_HKD;
using Domain.ApiModel.VNPT_HKD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Infrastructure.Webservice.Repository.VNPT.HKD
{
    public class VNPTHKDApiRepository : IVNPTHKDApiRepository
    {
        private readonly IRestClientHelper _iRestClientHelper;
        private readonly IMemoryCacheRepository _iMemoryCacheRepository;
        public VNPTHKDApiRepository(IRestClientHelper iRestClientHelper, IMemoryCacheRepository iMemoryCacheRepository)
        {
            _iRestClientHelper = iRestClientHelper;
            _iMemoryCacheRepository = iMemoryCacheRepository;   
        }
        public async Task<ResponseLoginModel> Login(int ComId, string url, string user, string pass)
        {
            string uri = $"{url}{CommonConstants.URIApiHKD}/Account/Login";
            LoginHKDModel login = new LoginHKDModel()
            {
                username = user,
                password = pass
            };
            var call = await _iRestClientHelper.PostAsync(uri, login);
            var data =   Common.ConverJsonToModel<ResponseLoginModel>(call);
            if (data.success && !string.IsNullOrEmpty(data.Token))
            {
                _iMemoryCacheRepository.CacheRemoce(LoginTokenkey.Key(ComId));
                _iMemoryCacheRepository.CacheTrySetValue<string>(LoginTokenkey.Key(ComId), data.Token);
            }
            return data;
        }
        public  string GetTokenCache(int ComId)
        {
            return _iMemoryCacheRepository.CacheTryGetValue<string>(LoginTokenkey.Key(ComId));
        }

        public Task<ResponseLoginModel> XemHoaDon(string url, string token, int IdHoaDon)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseLoginModel> ThemHoaDon(string url, string token, InvoicesHKDModel data)
        {
            string uri = $"{url}{CommonConstants.URIApiHKD}/Account/Login";
        
            Dictionary<string, string> additionalHeaders = new Dictionary<string, string>();
            additionalHeaders.Add("Authorization",$"Bearer {token}");

            var call = await _iRestClientHelper.PostAsync(uri, data, additionalHeaders);
            var kq = Common.ConverJsonToModel<ResponseLoginModel>(call);
            return kq;
        }
    }
}
