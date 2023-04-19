

using BankService;
using Microsoft.AspNetCore.Http;

namespace VNPay.Services;
public interface IVnPayService
{
    string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
    PaymentResponseModel PaymentExecute(IQueryCollection collections);
}