using Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class ClaimsModel
    {
        public int ComId { get;set; }
        public string FullName { get;set; }
        public string UserName { get;set; }
        public string Id { get;set; }
        public bool IsAdmin { get;set; }
        public bool IsThuNgan { get;set; }
        public bool IsPhucVuPayment { get;set; }//phục vụ có thanh toán
        public bool IsBep { get;set; }
        public bool IsPhucVu { get;set; }
        public bool IsKeToan { get;set; }
        public EnumTypeProduct IdDichVu { get;set; }
    }
}
