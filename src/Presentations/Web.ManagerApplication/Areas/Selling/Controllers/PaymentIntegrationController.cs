using Application.Constants;
using Application.Enums;
using Application.Features.BankAccounts.Query;
using Application.Features.VietQRs.Commands;
using Application.Features.VietQRs.Query;
using Application.Hepers;
using Application.Providers;
using BankService.Model;
using Domain.Entities;
using Domain.ViewModel;
using Domain.XmlDataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Reflection;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class PaymentIntegrationController : BaseController<PaymentIntegrationController>
    {
        private IOptions<CryptoEngine.Secrets> _config;

        public PaymentIntegrationController(IOptions<CryptoEngine.Secrets> config)
        {
            _config = config;
        }

        [Authorize(Policy = "paymentIntegration.index")]
        public async Task<IActionResult> Index()
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            PaymentIntegrationModel model = new PaymentIntegrationModel();
            var send = await _mediator.Send(new GetAllVietQRQuery(currentUser.ComId));
            if (send.Succeeded)
            {
                model.VietQRs = send.Data;
                foreach (var item in model.VietQRs)
                {
                    item.secret = CryptoEngine.Encrypt("id=" + item.Id, _config.Value.Key);
                    InfoPayQrcode infoPayQrcode = new InfoPayQrcode()
                    {
                        accountName = item.BankAccount.AccountName,
                        accountNo = item.BankAccount.BankNumber,
                        acqId = item.BankAccount.BinVietQR,
                        template = item.Template,
                    };
                    var genqr = await _mediator.Send(new GenerateVietQRCommand() { infoPayQrcode = infoPayQrcode });
                    if (genqr.Succeeded)
                    {
                        item.qrDataURL = genqr.Data.qrDataURL;
                    }
                    
                }
            }
            return View(model);
        }
        public async Task<IActionResult> GenerateVietQRAsync(InfoPayQrcode model)
        {

            if (string.IsNullOrEmpty(model.accountName) || model.acqId == null || string.IsNullOrEmpty(model.accountNo))
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR000));
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            var send = await _mediator.Send(new GenerateVietQRCommand() { infoPayQrcode = model });
            if (send.Succeeded)
            {
                return new JsonResult(new { isValid = true, qrcodeurl = send.Data.qrDataURL, qrcodedata = send.Data.qrCode });
            }

            return new JsonResult(new { isValid = false, html = string.Empty });
        }
        private string GetDisplayName(object value)
        {
            var type = value.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException(string.Format("Type {0} is not an enum", type));
            }
            var field = type.GetField(value.ToString());
            if (field == null)
            {
                return value.ToString();
            }

            var attributes = field.GetCustomAttribute<DisplayAttribute>();
            return attributes != null ? attributes.Name : value.ToString();
        }
        private List<SelectListItem> GetSelectListItem(string? type)
        {
            if (string.IsNullOrEmpty(type))
            {
                type = EnumTemplateVietQR.print.ToString();
            }
            var select = Enum.GetValues(typeof(EnumTemplateVietQR)).Cast<EnumTemplateVietQR>()
                .OrderBy(x => x)
                .Select(x => new SelectListItem
                {
                    Text = GetDisplayName(x),
                    Value = x.ToString(),
                    Selected = x.ToString() == type
                }).ToList();
            select.Insert(0, new SelectListItem()
            {
                Text = "",
                Value = "",
            });
            return select;
        }
        [Authorize(Policy = "paymentIntegration.create")]
        public IActionResult AddVietQRAsync()
        {
            ViewBag.Selectlist = this.GetSelectListItem(string.Empty);
            return View(new VietQRModel());
        } 
        [Authorize(Policy = "paymentIntegration.create")]
        [EncryptedParameters("secret")]
        public async Task<IActionResult>  GetUpdateVietQR(int id)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var send = await _mediator.Send(new GetByIdVietQRQuery(currentUser.ComId,id));
            if (send.Succeeded)
            {
                ViewBag.Selectlist = this.GetSelectListItem(send.Data.Template);
                VietQRModel vietQRModel = new VietQRModel();
                vietQRModel.Id = id;
                vietQRModel.BinVietQR = send.Data.BankAccount?.BinVietQR;
                vietQRModel.AccountName = send.Data.BankAccount?.AccountName;
                vietQRModel.BankNumber = send.Data.BankAccount?.BankNumber;
                vietQRModel.BankName = send.Data.BankAccount?.BankName;
                vietQRModel.BankAddress = send.Data.BankAccount?.BankAddress;
                vietQRModel.Code = send.Data.BankAccount?.Code;
                vietQRModel.ShortName = send.Data.BankAccount?.ShortName;
                vietQRModel.template = send.Data.Template;

                InfoPayQrcode infoPayQrcode = new InfoPayQrcode()
                {
                    accountName = send.Data.BankAccount.AccountName,
                    accountNo = send.Data.BankAccount.BankNumber,
                    acqId = send.Data.BankAccount.BinVietQR,
                    template = send.Data.Template,
                };
                var genqr = await _mediator.Send(new GenerateVietQRCommand() { infoPayQrcode = infoPayQrcode });
                if (genqr.Succeeded)
                {
                    vietQRModel.qrDataURL = genqr.Data.qrDataURL;
                }
                return View("AddVietQR", vietQRModel);
            }
            _notify.Error(GeneralMess.ConvertStatusToString(send.Message));
            return null;
        } 
    
        [Authorize(Policy = "paymentIntegration.delete")]
        [HttpPost]
        [EncryptedParameters("secret")]
        public async Task<IActionResult> DeleteQRAsync(int? id)
        {
            if (id == null)
            {
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            var send = await _mediator.Send(new DeleteVietQRCommand(currentUser.ComId, id.Value));
            if (send.Succeeded)
            {
                _notify.Success(GeneralMess.ConvertStatusToString(send.Message));
                return new JsonResult(new { isValid = true, html = string.Empty });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(send.Message));
            return new JsonResult(new { isValid = false, html = string.Empty });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateVietQRAsync(VietQR vietQR)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelStateKey in ViewData.ModelState.Keys)
                {
                    var modelStateVal = ViewData.ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        var key = modelStateKey;
                        var errorMessage = error.ErrorMessage;
                        var exception = error.Exception;
                        _notify.Error(key + errorMessage);
                        // You may log the errors if you want
                    }
                }
                // return View(collection);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            vietQR.ComId = currentUser.ComId;
            var _map = _mapper.Map<UpdateVietQRCommand>(vietQR);
            var send = await _mediator.Send(_map);
            if (send.Succeeded)
            {
                string QrDataURL = string.Empty;
                InfoPayQrcode infoPayQrcode = new InfoPayQrcode()
                {
                    accountName = send.Data.BankAccount.AccountName,
                    accountNo = send.Data.BankAccount.BankNumber,
                    acqId = send.Data.BankAccount.BinVietQR,
                    template = send.Data.Template,
                };
                var genqr = await _mediator.Send(new GenerateVietQRCommand() { infoPayQrcode = infoPayQrcode });
                if (genqr.Succeeded)
                {
                    QrDataURL = genqr.Data.qrDataURL;
                }

                _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                return new JsonResult(new {
                    qrDataURL = QrDataURL,
                    isValid = true,
                    bin = send.Data.BankAccount?.BinVietQR,
                    shortName = send.Data.BankAccount?.ShortName,
                    bankNumber = send.Data.BankAccount?.BankNumber,
                    accountName = send.Data.BankAccount?.AccountName,
                    template = send.Data.Template,
                    secret = CryptoEngine.Encrypt("id=" + send.Data.Id, _config.Value.Key)
            });
        }
            _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR011));
            return new JsonResult(new { isValid = false, html = string.Empty });
        }
    }
}
