using Application.DTOs.Mail;
using Domain.Entities;
using Domain.ViewModel;
using Model;
using System.Threading.Tasks;

namespace Application.Interfaces.Shared
{
    public interface IMailService
    {
        Task SendEmailOneAsync(MailRequest model);
        void SendEmailOne(MailRequest model, MailSettings mailSettings = null, bool CreateScope = false);
        Task<ResponseModel<string>> SendAgainAsync(int id, string emailnew);
        ResponseModel<string> SendMailWithResult(string emailto,string title,string content);
        Task SendAsync(MailRequest request);
        string GetTemplate(EmailParametersModel model, string content);
        string GetTemplateTitle(EmailParametersModel model, string content);
    }
}