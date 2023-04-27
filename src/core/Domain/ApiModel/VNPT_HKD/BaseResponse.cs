using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ApiModel.VNPT_HKD
{
    public class BaseResponse<T>
    {
        public bool success { get; set; } = false;
        public string[] errors { get; set; }
        public T data { get; set; }
    }
}
