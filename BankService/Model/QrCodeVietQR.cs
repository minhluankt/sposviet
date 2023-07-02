using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankService.Model
{
    public class InfoPayQrcode
    {
        public string accountNo { get; set; }//so tên ngân
        public string accountName { get; set; }// tên ngân
        public int? acqId { get; set; }//mã pin ngân hàng
        public string addInfo { get; set; }// nội dung chuyển tiền
        public string amount { get; set; }
        public string template { get; set; } = "compact";
    }
    public class BankAccountModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public int bin { get; set; }
        public string shortName { get; set; }
        public string logo { get; set; }
        public int transferSupported { get; set; }
        public int lookupSupported { get; set; }
        public string short_name { get; set; }
        public int support { get; set; }
        public int isTransfer { get; set; }
        public string swift_code { get; set; }

    }
    public class VietQRData
    {
        public string qrCode { get; set; }
        public string qrDataURL { get; set; }
    }
}
