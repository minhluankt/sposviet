using BankService.Model;
using HelperLibrary;
using Library;
using RestApiSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemVariable;

namespace BankService.VietQR
{
    public class VietQRService:IVietQRService
    {
        private IRestClientHelper restClientHelper;
        private string URIQR = "https://api.vietqr.io/v2/generate";
        public VietQRService(IRestClientHelper restClientHelper)
        {
            this.restClientHelper = restClientHelper;
        }
        public async Task<ApiResponseVietQR> GetQRCode(InfoPayQrcode model)
        {
            try
            {
                if (model.acqId==null)
                {
                    return new ApiResponseVietQR("Vui lòng nhập mã pin ngân hàng") { isError = true };
                }

                var header = new Dictionary<string, string>();
                header.Add("x-client-id", SystemVariableHelper.vietqrClientID);
                header.Add("x-api-key", SystemVariableHelper.vietqrAPIKey);
              
                if (model.amount == null)
                {
                    model.amount = string.Empty;
                }
                if (model.addInfo == null)
                {
                    model.addInfo = string.Empty;
                }
                var json = new
                {
                    addInfo = model.addInfo,
                    amount = model.amount,
                    template = model.template,
                    accountNo = model.accountNo,
                    accountName = model.accountName,
                    acqId = model.acqId.Value.ToString(),
                };

                var getapi = await restClientHelper.PostAsync(URIQR, json, header); 
                var data = ConvertSupport.ConverJsonToModel<ApiResponseVietQR>(getapi);
                if (data.code==0)
                {
                    data.isError = false;
                }
                return data;
            }
            catch (Exception e)
            {
                return new ApiResponseVietQR(e.Message) { isError = true };
            }
            
        }
    }
}
