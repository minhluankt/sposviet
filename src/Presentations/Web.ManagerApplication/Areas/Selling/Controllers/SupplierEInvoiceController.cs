using Application.Constants;
using Application.Enums;
using Application.Features.SupplierEInvoices.Commands;
using Application.Features.SupplierEInvoices.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class SupplierEInvoiceController : BaseController<SupplierEInvoiceController>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEInvoiceRepository<EInvoice> _einvoice;
        public SupplierEInvoiceController(UserManager<ApplicationUser> userManager,
            IOptions<CryptoEngine.Secrets> config,
            IEInvoiceRepository<EInvoice> einvoice)
        {
            _config = config;
            _einvoice = einvoice;
            _userManager = userManager;
        }
        private string GetDisplayName(object value)
        {
            var type = value.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException(string.Format("Type {0} is not an enum", type));
            }

            // Get the enum field.
            var field = type.GetField(value.ToString());
            if (field == null)
            {
                return value.ToString();
            }

            // Gets the value of the Name property on the DisplayAttribute, this can be null.
            var attributes = field.GetCustomAttribute<DisplayAttribute>();
            return attributes != null ? attributes.Name : value.ToString();
        }
        private List<SelectListItem> GetSelectListItem(ENumTypeSeri type = ENumTypeSeri.HSM)
        {
            var select = Enum.GetValues(typeof(ENumTypeSeri)).Cast<ENumTypeSeri>().OrderBy(x => (Convert.ToInt32(x))).Where(x => (Convert.ToInt32(x)) >= 0).Select(x => new SelectListItem
            {
                Text = GetDisplayName(x),
                Value = Convert.ToInt32(x).ToString(),
                Selected = x == type
            }).ToList();
            select.Insert(0, new SelectListItem()
            {
                Text = "",
                Value = "",
            });
            return select;
        }

        [Authorize(Policy = "SupplierEInvoice.list")]
        public async Task<IActionResult> Index()
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            List<SupplierEInvoiceModel> supp = new List<SupplierEInvoiceModel>();
            var _send = await _mediator.Send(new GetAllSupplierEInvoiceQuery() { Comid= currentUser .ComId});
            supp = _send.Data;
           
            return View(supp);
        }
        [Authorize(Policy = "SupplierEInvoice.create")]
        public async Task<ActionResult> CreateAsync(ENumSupplierEInvoice TypeSupplierEInvoice)
        {
            try
            {
                _logger.LogInformation(User.Identity.Name + "--> SupplierEInvoice create");
                var html = await _viewRenderer.RenderViewToStringAsync("_Create", new SupplierEInvoice() { TypeSupplierEInvoice = TypeSupplierEInvoice, Selectlist = GetSelectListItem() });
                return new JsonResult(new { isValid = true, html = html });
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false });
            }

            // return View("_Create");
        }
        [Authorize(Policy = "SupplierEInvoice.create")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> CheckConnectWebserviceAsync(int id,SupplierEInvoice model)
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();
                if (id>0&& string.IsNullOrEmpty(model.DomainName))
                {
                    var data = await _mediator.Send(new GetByIdSupplierEInvoiceQuery() { Id = id, ComId = currentUser.ComId });
                    if (data.Succeeded)
                    {
                        var check = await _einvoice.CheckConnectWebserviceAsync(data.Data.DomainName, data.Data.UserNameService, data.Data.PassWordService, data.Data.UserNameAdmin, data.Data.PassWordAdmin);
                        if (check.Succeeded)
                        {
                            _notify.Success(check.Message);
                        }
                        else
                        {
                            _notify.Error(check.Message);
                        }
                        return new JsonResult(new { isValid = true });
                    }
                    return new JsonResult(new { isValid = false });
                }
                else
                {
                    var check = await _einvoice.CheckConnectWebserviceAsync(model.DomainName, model.UserNameService, model.PassWordService, model.UserNameAdmin, model.PassWordAdmin);
                    if (check.Succeeded)
                    {
                        _notify.Success(check.Message);
                    }
                    else
                    {
                        _notify.Error(check.Message);
                    }
                    return new JsonResult(new { isValid = true });
                }
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false });
            }

            // return View("_Create");
        }

        [EncryptedParameters("secret")]
        [Authorize(Policy = "SupplierEInvoice.edit")]
        public async Task<ActionResult> EditAsync(int id)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            _logger.LogInformation(User.Identity.Name + "--> edit detailt");
            var data = await _mediator.Send(new GetByIdSupplierEInvoiceQuery() { Id = id, ComId = currentUser.ComId });
            if (data.Succeeded)
            {
                data.Data.Selectlist = GetSelectListItem(data.Data.TypeSeri);
                var html = await _viewRenderer.RenderViewToStringAsync("_Edit", data.Data);
                return new JsonResult(new { isValid = true, html = html });
            }
            _notify.Error(data.Message);
            return new JsonResult(new { isValid = false, html = string.Empty });
        } 
        [EncryptedParameters("secret")]
        [Authorize(Policy = "SupplierEInvoice.edit")]
        public async Task<ActionResult> RemoveAsync(int id)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            _logger.LogInformation(User.Identity.Name + "--> SupplierEInvoice RemoveAsync");
            var data = await _mediator.Send(new DeleteSupplierEInvoiceCommand() { Id = id, ComId = currentUser.ComId });
            if (data.Succeeded)
            {
                return new JsonResult(new { isValid = true });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(data.Message));
            return new JsonResult(new { isValid = false, html = string.Empty });
        }
        [Authorize(Policy = "SupplierEInvoice.create")]
        [HttpPost]
        public async Task<ActionResult> OnPostCreateOrEditAsync(SupplierEInvoice model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var secret = string.Empty;
                    var secrettype = string.Empty;
                    var currentUser = User.Identity.GetUserClaimLogin();
                    model.ComId = currentUser.ComId;

                    if (model.Id == 0)
                    {
                        var createProductCommand = _mapper.Map<CreateSupplierEInvoiceCommand>(model);
                        var result = await _mediator.Send(createProductCommand);
                        if (result.Succeeded)
                        {
                            var values = "id=" + result.Data.Id;
                             secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                            var valuestype = "TypeSupplierEInvoice=" + result.Data.TypeSupplierEInvoice;
                            secrettype = CryptoEngine.Encrypt(valuestype, _config.Value.Key);
                            _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS008));
                        }
                        else
                        {
                            _notify.Error(result.Message);
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                    }
                    else
                    {
                        var createProductCommand = _mapper.Map<UpdateSupplierEInvoiceCommand>(model);
                        var result = await _mediator.Send(createProductCommand);
                        if (result.Succeeded)
                        {
                            var values = "id=" + result.Data;
                            secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                            var valuestype = "TypeSupplierEInvoice=" + model.TypeSupplierEInvoice;
                            secrettype = CryptoEngine.Encrypt(valuestype, _config.Value.Key);
                            _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                        }
                        else
                        {
                            _notify.Error(result.Message);
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                    }
                    return new JsonResult(new { isValid = true,closeSwal = true, secret = secret , secrettype = secrettype });
                }
                catch (Exception ex)
                {
                    _notify.Error(ex.Message);
                    return View();
                }
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", model);
                return new JsonResult(new { isValid = false, html = html });
            }

        }
    }
}
