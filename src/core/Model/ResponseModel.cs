using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ResponseModel<T>
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
