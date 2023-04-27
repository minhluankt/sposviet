using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ApiModel.VNPT_HKD.Model
{
    public class InvoicesHKDModel
    {
        public int IdHoaDon { get; set; }
        public ThongTinHoaDon ThongTinHoaDon { get; set; }
    }
    public class ThongTinHoaDon
    {
        public int IdMauSoHoaDonDaDangKy { get; set; }
    }
}
