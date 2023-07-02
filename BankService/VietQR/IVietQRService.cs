using BankService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankService.VietQR
{
    public interface IVietQRService
    {
        Task<ApiResponseVietQR> GetQRCode(InfoPayQrcode model);
    }
}
