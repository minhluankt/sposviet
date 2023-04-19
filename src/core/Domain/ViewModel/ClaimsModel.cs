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
        public EnumTypeProduct IdDichVu { get;set; }
    }
}
