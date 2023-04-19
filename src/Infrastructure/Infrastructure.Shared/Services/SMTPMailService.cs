using Application.DTOs.Mail;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Shared;
using Domain.Entities;
using Domain.ViewModel;
using Infrastructure.Infrastructure.DbContexts;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MimeKit;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SystemVariable;
using static Application.Hepers.Validate;

namespace Infrastructure.Shared.Services
{
    public class SMTPMailService : IMailService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;
        //  public MailSettings _mailSettings { get; }IOptions<MailSettings> mailSettings
        private IEmailRepository<MailSettings> _mailSetting;
        private IEmailHistoryRepository<Mailhistory> _mailhistory;
        public ILogger<SMTPMailService> _logger { get; }

        [Obsolete]
        public SMTPMailService(IEmailRepository<MailSettings> mailSettings,
            IHostingEnvironment hostingEnvironment, IServiceScopeFactory serviceScopeFactory,
            IEmailHistoryRepository<Mailhistory> mailhistory, ILogger<SMTPMailService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _hostingEnvironment = hostingEnvironment;
            _mailhistory = mailhistory;
            _mailSetting = mailSettings;
            _logger = logger;
        }

        public async Task SendAsync(MailRequest model)
        {
            if (model.To.Length == 0)
            {
                _logger.LogError("Gửi mail thất bại, không có người nhận");
                return;
            }
            else
            {
                MailSettings _mailSettings = await _mailSetting.GetMailSettingCacheAsync();
                _mailSettings.Password = EncryptionHelper.Decrypt(_mailSettings.Password, SystemVariableHelper.publicKey);
                try
                {
                    if (model.To == null || model.To.Length == 0)
                    {
                        return;
                    }
                    int i = 0;
                    MimeMessage mMessage = new MimeMessage();
                    foreach (var item in model.To)
                    {
                        if (Validator.EmailIsValid(item))
                        {
                            MailboxAddress mail = MailboxAddress.Parse(item);
                            mMessage.To.Add(mail);
                            i++;
                        }
                    }
                    if (model.CC != null)
                    {
                        foreach (var item in model.CC)
                        {
                            if (Validator.EmailIsValid(item))
                            {
                                MailboxAddress mail = MailboxAddress.Parse(item);
                                mMessage.To.Add(mail);
                                i++;
                            }
                        }
                    }

                    if (model.BCC != null)
                    {
                        foreach (var item in model.BCC)
                        {
                            if (Validator.EmailIsValid(item))
                            {
                                MailboxAddress mail = MailboxAddress.Parse(item);
                                mMessage.To.Add(mail);
                                i++;
                            }
                        }
                    }

                    if (i > 0)
                    {
                        mMessage.Subject = model.Title;
                        mMessage.Body = model.Body;

                    }

                    mMessage.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From));
                    Mailhistory mailhistory = new Mailhistory
                    {
                        From = _mailSettings.UserName,
                        FullNameUserSend = model.FullNameUserSend,
                        IdTypeUserSend = model.IdTypeUserSend,
                        CreatedBy = model.IdTypeUserSend.ToString(),
                        IdUserSend = model.IdUserSend,
                        CC = model.CC != null ? JsonConvert.SerializeObject(model.CC) : "",
                        BCC = model.BCC != null ? JsonConvert.SerializeObject(model.BCC) : "",
                        To = JsonConvert.SerializeObject(model.To),
                        Status = false,
                        SendMuti = true,
                        Title = model.Title,
                        CreateAgain = model.CreateAgain != null ? model.CreateAgain : null,
                        Subject = model.Subject,
                        Body = mMessage.HtmlBody,
                    };
                    Mailhistory UpdateMailhistory = await _mailhistory.AddAsync(mailhistory);

                    using (var client = new SmtpClient())
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                        //if (SystemVariableHelper.EnableProxyData == 1 && SystemVariableHelper.EnableProxySendMail())
                        //{

                        //    System.Net.NetworkCredential objNetworkCredential = new System.Net.NetworkCredential(SystemVariableHelper.UserNameProxy, SystemVariableHelper.PassWordProxy);
                        //    MailKit.Net.Proxy.HttpProxyClient objHttpProxyClient = new HttpProxyClient(SystemVariableHelper.DomainProxy, SystemVariableHelper.PortProxy, objNetworkCredential);
                        //    client.ProxyClient = objHttpProxyClient;
                        //}
                        await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.Auto);
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        await client.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
                        await client.SendAsync(mMessage);
                        await client.DisconnectAsync(true);
                    }
                    try
                    {
                        await _mailhistory.UpdateStatusAsync(UpdateMailhistory.Id);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Update status trạng thái email thất bại id:! {UpdateMailhistory.Id}, title {UpdateMailhistory.Title}");
                        _logger.LogError(e.ToString());
                    }

                    _logger.LogInformation($"Send mail thành công id:! {UpdateMailhistory.Id}, title {UpdateMailhistory.Title}");
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }
            }
        }

        public async Task<ResponseModel<string>> SendAgainAsync(int id, string emailnew)
        {
            var data = await _mailhistory.GetByIdAsync(id);
            if (string.IsNullOrEmpty(emailnew))
            {
                emailnew = data.CC;
            }
            var send = this.SendMailWithResult(emailnew, data.Title, data.Body);
            if (send.isSuccess)
            {
                _mailhistory.UpdateStatus(id, true);
                return new ResponseModel<string> { isSuccess = send.isSuccess, Message = send.Message };
            }
            return new ResponseModel<string> { isSuccess = send.isSuccess, Message = send.Message };
        }

        public void SendEmailOne(MailRequest model, MailSettings mailSettings = null, bool CreateScope = false)
        {

            if (string.IsNullOrEmpty(model.EmailTo))
            {
                _logger.LogError("Gửi mail thất bại, không có người nhận");
                return;
            }
            else
            {
                MailSettings _mailSettings = mailSettings;
                if (CreateScope && _mailSettings == null)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                        _mailSettings = db.MailSetting.FirstOrDefault();
                        db.Dispose();
                    }
                }
                else if (mailSettings == null)
                {
                    _mailSettings = _mailSetting.GetMailSetting();
                }
                _mailSettings.Password = EncryptionHelper.Decrypt(_mailSettings.Password, SystemVariableHelper.publicKey);
                try
                {
                    var builder = new BodyBuilder();
                    builder.HtmlBody = model.Content;
                    MimeEntity aContent = builder.ToMessageBody();

                    MimeMessage mMessage = new MimeMessage();
                    MailboxAddress mail = MailboxAddress.Parse(model.EmailTo);
                    mMessage.To.Add(mail); ;
                    mMessage.Subject = model.Subject;
                    mMessage.Body = aContent;
                    mMessage.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From));

                    string[] tomailjson = model.EmailTo.Split(',');
                    Mailhistory mailhistory = new Mailhistory
                    {
                        From = _mailSettings.UserName,
                        FullNameUserSend = model.FullNameUserSend,
                        To = JsonConvert.SerializeObject(tomailjson),

                        Status = false,
                        Title = model.Title,
                        CreatedOn = DateTime.Now,
                        CreatedBy = model.FullNameUserSend,
                        CreateAgain = model.CreateAgain != null ? model.CreateAgain : null,
                        Subject = model.Subject,
                        Body = mMessage.HtmlBody,
                    };

                    if (CreateScope)
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                            db.Mailhistory.Add(mailhistory);
                            db.SaveChanges();
                            db.Dispose();
                        }
                    }
                    else
                    {
                        mailhistory = _mailhistory.Add(mailhistory);
                    }




                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.Auto);
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        client.Authenticate(_mailSettings.UserName, _mailSettings.Password);
                        client.Send(mMessage);
                        client.Disconnect(true);
                    }

                    try
                    {
                        if (CreateScope)
                        {
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                                var data = db.Mailhistory.Find(mailhistory.Id);
                                data.Status = true;
                                data.LastModifiedOn = DateTime.Now;
                                db.Update(data);
                                db.SaveChanges();
                                db.Dispose();
                            }
                        }
                        else
                        {
                            _mailhistory.UpdateStatus(mailhistory.Id);
                        }

                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Update status trạng thái email thất bại id:! {mailhistory.Id}, title {mailhistory.Title}");
                        _logger.LogError(e.ToString());
                    }
                    _logger.LogInformation($"Send mail thành công!: " + model.EmailTo);
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }
            }
        }
        public async Task SendEmailOneAsync(MailRequest model)
        {

            if (string.IsNullOrEmpty(model.EmailTo))
            {
                _logger.LogError("Gửi mail thất bại, không có người nhận");
                return;
            }
            else
            {
                MailSettings _mailSettings = await _mailSetting.GetMailSettingCacheAsync();
                _mailSettings.Password = EncryptionHelper.Decrypt(_mailSettings.Password, SystemVariableHelper.publicKey);
                try
                {
                    var builder = new BodyBuilder();
                    builder.HtmlBody = model.Content;
                    MimeEntity aContent = builder.ToMessageBody();

                    MimeMessage mMessage = new MimeMessage();
                    MailboxAddress mail = MailboxAddress.Parse(model.EmailTo);
                    mMessage.To.Add(mail); ;
                    mMessage.Subject = model.Subject;
                    mMessage.Body = aContent;
                    mMessage.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From));
                    string[] tomailjson = model.EmailTo.Split(',');
                    Mailhistory mailhistory = new Mailhistory
                    {
                        From = _mailSettings.UserName,
                        FullNameUserSend = model.FullNameUserSend,
                        To = JsonConvert.SerializeObject(tomailjson),
                        Status = false,
                        Title = model.Title,
                        CreatedOn = DateTime.Now,
                        CreatedBy = model.FullNameUserSend,
                        CreateAgain = model.CreateAgain != null ? model.CreateAgain : null,
                        Subject = model.Subject,
                        Body = mMessage.HtmlBody,
                    };
                    Mailhistory UpdateMailhistory = await _mailhistory.AddAsync(mailhistory);


                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                        //if (SystemVariableHelper.EnableProxyData == 1 && SystemVariableHelper.EnableProxySendMail())
                        //{

                        //    System.Net.NetworkCredential objNetworkCredential = new System.Net.NetworkCredential(SystemVariableHelper.UserNameProxy, SystemVariableHelper.PassWordProxy);
                        //    MailKit.Net.Proxy.HttpProxyClient objHttpProxyClient = new HttpProxyClient(SystemVariableHelper.DomainProxy, SystemVariableHelper.PortProxy, objNetworkCredential);
                        //    client.ProxyClient = objHttpProxyClient;
                        //}
                        await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.Auto);
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        await client.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
                        await client.SendAsync(mMessage);
                        await client.DisconnectAsync(true);
                    }

                    try
                    {
                        await _mailhistory.UpdateStatusAsync(UpdateMailhistory.Id);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Update status trạng thái email thất bại id:! {UpdateMailhistory.Id}, title {UpdateMailhistory.Title}");
                        _logger.LogError(e.ToString());
                    }
                    _logger.LogInformation($"Send mail thành công!");
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }
            }
        }

        public string GetTemplate(EmailParametersModel model, string content)
        {
            try
            {
                var template = HttpUtility.HtmlDecode(content);
                //using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))//"EmailTemplate/Publish.txt", FileMode.Open, FileAccess.Read))
                //{
                //    using (var reader = new StreamReader(file))
                //    {
                //        template = reader.ReadToEnd();
                //    }
                //}


                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    var getiinfocompany = db.CompanyAdminInfo.FirstOrDefault();
                    model.hotline = !string.IsNullOrEmpty(getiinfocompany.Hotline) ? getiinfocompany.Hotline : "0949906004";
                    model.website = getiinfocompany.Website;
                    if (!string.IsNullOrEmpty(model.urldonhang))
                    {
                        model.urldonhang = getiinfocompany.Website + "/" + model.urldonhang;
                    }
                    db.Dispose();
                }

                var arrtoken = new Dictionary<string, string>()
                {
                    { "{noidungcapnhatdonhang}",model.noidungcapnhatdonhang},
                    { "{madonhang}",model.madonhang},
                    { "{soluongsanpham}",model.soluongsanpham},
                    { "{tongtiendonhang}",model.tongtiendonhang},
                    { "{tenkhachhang}", model.tenkhachhang },
                    { "{trangthaidonhang}", model.trangthaidonhang },
                    { "{diachigiaohang}", model.diachigiaohang },
                    { "{sodienthoai}", model.sodienthoai },
                    { "{hotline}", model.hotline },
                    { "{urldonhang}", model.urldonhang },
                    { "{website}", model.website },
                    { "{emailCustomer}", model.emailCustomer },
                    { "{ngaythangnam}", model.ngaythangnam }

                };
                // builder.HtmlBody = BuildTemplate(template, arrtoken);
                return BuildTemplate(template, arrtoken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
        protected static string BuildTemplate(string html, Dictionary<string, string> arrtoken)
        {
            if (string.IsNullOrEmpty(html)) return html;
            foreach (var token in arrtoken)
            {
                html = html.Replace(token.Key, token.Value);
            }
            return html;
        }

        public ResponseModel<string> SendMailWithResult(string emailto, string title, string content)
        {
            if (string.IsNullOrEmpty(emailto))
            {
                _logger.LogError("Gửi mail thất bại, không có người nhận");
                return new ResponseModel<string> { Message = "Gửi mail thất bại, không có người nhận" };
            }
            else
            {
                MailSettings _mailSettings = _mailSettings = _mailSetting.GetMailSetting();
                _mailSettings.Password = EncryptionHelper.Decrypt(_mailSettings.Password, SystemVariableHelper.publicKey);
                try
                {
                    var builder = new BodyBuilder();
                    builder.HtmlBody = content;
                    MimeEntity aContent = builder.ToMessageBody();

                    MimeMessage mMessage = new MimeMessage();
                    MailboxAddress mail = MailboxAddress.Parse(emailto);
                    mMessage.To.Add(mail);
                    mMessage.Subject = title;
                    mMessage.Body = aContent;
                    mMessage.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From));

                    using (var client = new SmtpClient())
                    {

                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.Auto);
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        client.Authenticate(_mailSettings.UserName, _mailSettings.Password);
                        client.Send(mMessage);
                        client.Disconnect(true);
                    }
                    try
                    {
                        string[] tomailjson = emailto.Split(',');
                        Mailhistory mailhistory = new Mailhistory
                        {
                            From = _mailSettings.UserName,
                            FullNameUserSend = _mailSettings.UserName,
                            To = JsonConvert.SerializeObject(tomailjson),
                            Status = true,
                            Title = title,
                            CreatedOn = DateTime.Now,
                            CreatedBy = "SystemMail",
                            Subject = title,
                            Body = content,
                        };
                        mailhistory = _mailhistory.Add(mailhistory);

                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation($"Lưu lịch sử gửi mail thất bại!");
                    }
                    _logger.LogInformation($"Send mail thành công!");
                    return new ResponseModel<string> { isSuccess = true, Message = "Gửi mail thành công" };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                    return new ResponseModel<string> { isSuccess = false, Message = ex.Message };
                }
            }
        }

        public string GetTemplateTitle(EmailParametersModel model, string content)
        {
            try
            {
                var template = HttpUtility.HtmlDecode(content);
                var arrtoken = new Dictionary<string, string>()
                {
                    { "{ngaythangnam}", model.ngaythangnam },
                    { "{madonhang}", model.madonhang }
                };
                return BuildTemplate(template, arrtoken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return String.Empty;
            }
        }
    }
}