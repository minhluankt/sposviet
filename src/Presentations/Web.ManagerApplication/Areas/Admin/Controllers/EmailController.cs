using Application.DTOs.Mail;
using Application.Features.Mail.Commands;
using Application.Features.Mail.Querys;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Shared;
using Domain.Entities;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MimeKit;
using Newtonsoft.Json;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Areas.Admin.Models;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmailController : BaseController<EmailController>
    {
        private IStringLocalizer<SharedResource> _localizer;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _IUnitOfWork;
        private readonly IMailService _IMailService;
        private readonly IRepositoryAsync<MailSettings> _repository;
        public EmailController(IUnitOfWork IUnitOfWork,
            IStringLocalizer<SharedResource> localizer,
            IMailService IMailService, UserManager<ApplicationUser> Usermanager, SignInManager<ApplicationUser> signInManager,
        IRepositoryAsync<MailSettings> repository)
        {
            _localizer = localizer;
            _userManager = Usermanager;
            _signInManager = signInManager;
            _IMailService = IMailService;
            _repository = repository;
            _IUnitOfWork = IUnitOfWork;

        }
        [Authorize(Policy = "email.setting")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Policy = "email.history")]
        public async Task<IActionResult> HistoryAsync()
        {
            _logger.LogInformation(User.Identity.Name + "--> mailsetting History");
            var getid = await _mediator.Send(new GetAllEmailHistoryQuery());
            if (getid.Succeeded)
            {
                if (getid.Data == null)
                {
                    return View();
                }
                //   var datamodel = _mapper.Map<EmailHistory>(getid.Data);

                return View(getid.Data);
            }
            return View();
        }
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var getid = await _mediator.Send(new GetByIdMailHistoryQuery() { Id = id });
            if (getid.Succeeded)
            {
                return Json(new { email = getid.Data.To, isValid = true });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(getid.Message));
            return Json(new { isValid = false });
        }
        public async Task<IActionResult> SendEmailAsync(string email, int id)
        {
            var getid = await _mediator.Send(new GetByIdMailHistoryQuery() { Id = id });
            if (getid.Succeeded)
            {
                MailRequest mailRequest = new MailRequest();
                mailRequest.To = email.Split(',');
                mailRequest.Subject = getid.Data.Subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = getid.Data.Body;
                MimeEntity aContent = builder.ToMessageBody();
                mailRequest.Body = aContent;
                await _IMailService.SendAsync(mailRequest);
                _notify.Success("SendEmailOK");
                return new JsonResult(new { isValid = true, html = string.Empty });
            }

            return new JsonResult(new { isValid = false, html = string.Empty });
        }
        public async Task<IActionResult> DetailtAsync(int id)
        {
            _logger.LogInformation(User.Identity.Name + "--> mailsetting DetailtAsync");
            var getid = await _mediator.Send(new GetByIdMailHistoryQuery() { Id = id });
            if (getid.Succeeded)
            {
                if (!string.IsNullOrEmpty(getid.Data.To))
                {

                    string[] to = JsonConvert.DeserializeObject<string[]>(getid.Data.To);

                    if (to.Length == 1)
                    {
                        getid.Data.To = "";
                        getid.Data.To = to[0];
                    }
                    else
                    {
                        getid.Data.To = "";
                        foreach (var item in to)
                        {
                            getid.Data.To += item + ",";
                        }
                        var datalast = getid.Data.To.Remove(getid.Data.To.Length - 1);
                        getid.Data.To = datalast;
                    }

                }

                var html = await _viewRenderer.RenderViewToStringAsync("_Detailt", getid.Data);
                return new JsonResult(new { isValid = true, html = html });
            }
            return View();
        }

        public async Task<IActionResult> TestEmailAsync(string ToEmail, string Content, string Title)
        {
            _logger.LogInformation(User.Identity.Name + "--> mailsetting TestEmailAsync");
            try
            {
                //var user = await _userManager.GetUserAsync(HttpContext.User);
                //var builder = new BodyBuilder();
                //builder.HtmlBody = Content;
                //MimeEntity aContent = builder.ToMessageBody();

                //List<string> list = new List<string>();
                //list.Add(ToEmail);
                //string[] listmail = list.ToArray();
                //MailRequest mailRequest = new MailRequest
                //{
                //    To = listmail,
                //    Title = Title ?? "Test email",
                //    Subject = Title ?? "Test email",
                //    Body = aContent,
                //    IdTypeUserSend = (int)CommonEnum.User,
                //    IdUserSend = user.Id,
                //    FullNameUserSend = user.FullName,
                //};
                //await _IMailService.SendAsync(mailRequest);
                var send = _IMailService.SendMailWithResult(ToEmail, Title, Content);
                if (send.isSuccess)
                {
                    _notify.Success(_localizer.GetString("SendEmailOK").Value);
                }
                else
                {
                    _notify.Error(send.Message);
                }
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

        }
        [Authorize(Policy = "email.setting")]
        public async Task<IActionResult> SettingAsync(int Id)
        {
            _logger.LogInformation(User.Identity.Name + "--> mailsetting Setting");
            var getid = await _mediator.Send(new GetByIdMailSettingQuery() { Id = Id });
            if (getid.Succeeded)
            {
                if (getid.Data == null)
                {
                    return View("_CreateOrEdit", new MailSettingViewModel());
                }
                var datamodel = _mapper.Map<MailSettingViewModel>(getid.Data);

                return View("_CreateOrEdit", datamodel);
            }
            _notify.Error(_localizer.GetString("NotData").Value);
            return RedirectToAction("/admin");
        }
        [HttpPost]
        [Authorize(Policy = "email.setting")]
        public async Task<ActionResult> OnPostCreateOrEditAsync(int id, MailSettingViewModel model)
        {
            _logger.LogInformation(User.Identity.Name + "--> mailsetting CreateOrEdit");
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id == 0)
                    {
                        var createProductCommand = _mapper.Map<CreateMailSettingCommand>(model);
                        if (string.IsNullOrEmpty(model.Password))
                        {
                            _notify.Error("Vui lòng nhập mật khẩu email");
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                        var result = await _mediator.Send(createProductCommand);
                        if (result.Succeeded)
                        {
                            model.Id = result.Data;
                            _notify.Success(_localizer.GetString("AddOk").Value);

                            return new JsonResult(new { isValid = true, html = string.Empty });
                        }
                        else
                        {
                            _notify.Error(result.Message);
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.Password) && model.ChangePass)
                        {
                            _notify.Error("Vui lòng nhập mật khẩu email");
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                        var createProductCommand = _mapper.Map<UpdateMailSettingCommand>(model);
                        var result = await _mediator.Send(createProductCommand);

                        if (result.Succeeded)
                        {
                            model.Id = result.Data;
                            _notify.Success(_localizer.GetString("EditOk").Value);

                            return new JsonResult(new { isValid = true, html = string.Empty }); ;
                        }
                        else
                        {
                            _notify.Error(result.Message);
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                    }

                }
                catch (Exception ex)
                {
                    _notify.Error(ex.Message);
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
            }
            else
            {
                //var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", model);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

        }
    }
}
