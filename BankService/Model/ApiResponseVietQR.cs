using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BankService.Model
{
    public class ApiResponseVietQR
    {
        public int code { get; set; }
        public string desc { get; set; }
        public bool isError { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public object data { get; set; }

        [JsonConstructor]
        public ApiResponseVietQR(string message, object result = null, int statusCode = 200, string apiVersion = "1.0.0.0")
        {
            this.code = statusCode;
            this.desc = message;
            this.data = result;
            this.isError = false;
        }
    }
}
