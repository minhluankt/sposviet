using Application.Constants;
using Application.Enums;
using Application.Features.BankAccounts.Query;
using Application.Features.VietQRs.Commands;
using Application.Features.VietQRs.Query;
using Application.Hepers;
using BankService.Model;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Reflection;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class PaymentIntegrationController : BaseController<PaymentIntegrationController>
    {
        public async Task<IActionResult> Index()
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            PaymentIntegrationModel model = new PaymentIntegrationModel();
            var send = await _mediator.Send(new GetAllVietQRQuery(currentUser.ComId));
            if (send.Succeeded)
            {
                model.VietQRs = send.Data;
               
            }
            return View(model);
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
        private List<SelectListItem> GetSelectListItem(EnumTemplateVietQR type = EnumTemplateVietQR.print)
        {
            var select = Enum.GetValues(typeof(EnumTemplateVietQR)).Cast<EnumTemplateVietQR>()
                .OrderBy(x => x)
                .Select(x => new SelectListItem
                {
                    Text = GetDisplayName(x),
                    Value = x.ToString(),
                    Selected = x == type
                }).ToList();
            select.Insert(0, new SelectListItem()
            {
                Text = "",
                Value = "",
            });
            return select;
        }
        public IActionResult AddVietQR()
        {
            ViewBag.Selectlist = this.GetSelectListItem();
            return View();
        } 
        public async Task<IActionResult> GenerateVietQRAsync(InfoPayQrcode model)
        {
            
            if (string.IsNullOrEmpty(model.accountName)|| model.acqId==null || string.IsNullOrEmpty(model.accountNo))
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR000));
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            var send = await _mediator.Send(new GenerateVietQRCommand() { infoPayQrcode=model});
            if (send.Succeeded)
            {
                return new JsonResult(new { isValid = true, qrcodeurl = send.Data.qrDataURL, qrcodedata = send.Data.qrCode });
            }

            return new JsonResult(new { isValid = false, html = string.Empty });
        }
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
                return new JsonResult(new { isValid = true, data = send.Data });
            }

            return new JsonResult(new { isValid = false, html = string.Empty });
        }
    }
}
