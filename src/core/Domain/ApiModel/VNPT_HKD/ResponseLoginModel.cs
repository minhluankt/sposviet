using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ApiModel.VNPT_HKD
{
    public class ResponseLoginModel: BaseResponse<string>
    {
        public string Token { get; set; }
        public string refreshToken { get; set; }
    }
}
