using Application.Constants;
using Application.EInvoices.Interfaces.VNPT;
using Application.Enums;
using Application.Features.AutoSendTimers.Query;
using Application.Features.ManagerPatternEInvoices.Query;
using Application.Features.SupplierEInvoices.Commands;
using Application.Features.SupplierEInvoices.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using Hangfire.MemoryStorage.Database;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Web.ManagerApplication.Abstractions;
using X.PagedList;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class AutoTimerController : BaseController<AutoTimerController>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEInvoiceRepository<EInvoice> _einvoice;
        private readonly IVNPTHKDApiRepository _apiHkd;
        public AutoTimerController(UserManager<ApplicationUser> userManager,
            IOptions<CryptoEngine.Secrets> config, IVNPTHKDApiRepository apiHkd,
            IEInvoiceRepository<EInvoice> einvoice)
        {
            _apiHkd = apiHkd;
            _config = config;
            _einvoice = einvoice;
            _userManager = userManager;
        }
      

        [Authorize(Policy = "AutoTimer.list")]
        [EncryptedParameters("secret")]
        public async Task<IActionResult> EInvoiceAsync(int type)//cấu hình tự động gửi hóa đơn
        {
            ENumSupplierEInvoice TypeSupplierEInvoice = (ENumSupplierEInvoice)type;
            var currentUser = User.Identity.GetUserClaimLogin();

            var _send = await _mediator.Send(new GetAllSupplierEInvoiceQuery() { TypeSupplierEInvoice = TypeSupplierEInvoice, Comid = currentUser.ComId, IsManagerPatternEInvoices = false });
            SupplierEInvoiceModel SupplierEInvoice = null;
            if (TypeSupplierEInvoice == ENumSupplierEInvoice.NONE)
            {
                SupplierEInvoice = _send.Data.FirstOrDefault();
            }
            else
            {
                SupplierEInvoice = _send.Data.SingleOrDefault();
            }
            if (SupplierEInvoice == null)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR012));
                return RedirectToAction("Index", "SupplierEInvoice");
            }
            return View(SupplierEInvoice);
        }
        public async Task<IActionResult> LoadAll(ENumSupplierEInvoice TypeSupplierEInvoice)
        {
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
                // Skip number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();

                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction (asc, desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;

                int skip = start != null ? Convert.ToInt32(start) : 0;

                int recordsTotal = 0;

                var currentUser = User.Identity.GetUserClaimLogin();
                int currentPage = skip >= 0 ? skip / pageSize : 0;
                currentPage = currentPage + 1;
                // getting all templateInvoice data  
                var response = await _mediator.Send(new GetAllAutoSendTimerQuery()
                {
                    currentPage = currentPage,
                    Comid = currentUser.ComId,
                    TypeSupplierEInvoice = TypeSupplierEInvoice,
                    sortColumn = sortColumn,
                    sortColumnDirection = sortColumnDirection,
                    pageSize = pageSize,
                    skip = skip
                });
                if (response.Succeeded)
                {
                    return Json(new { draw = draw, recordsFiltered = response.Data.TotalItemCount, recordsTotal = response.Data.TotalItemCount, data = response.Data.Items });
                }
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = "" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json(new { draw = draw, recordsFiltered = 0, recordsTotal = 0, data = "" });
            }

        }

        [Authorize(Policy = "AutoTimer.create")]
        public async Task<ActionResult> CreateAsync(ENumSupplierEInvoice TypeSupplierEInvoice)
        {
            try
            {
                _logger.LogInformation(User.Identity.Name + "--> SupplierEInvoice create");
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", new AutoSendTimer() { TypeSupplierEInvoice = TypeSupplierEInvoice});
                return new JsonResult(new { isValid = true, html = html });
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false });
            }

            // return View("_Create");
        }
        
        [EncryptedParameters("secret")]
        [Authorize(Policy = "AutoTimer.edit")]
        public async Task<ActionResult> EditAsync(int id)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            _logger.LogInformation(User.Identity.Name + "--> edit detailt");
            var data = await _mediator.Send(new GetByIdSupplierEInvoiceQuery() { Id = id, ComId = currentUser.ComId });
            if (data.Succeeded)
            {
              
                var html = await _viewRenderer.RenderViewToStringAsync("_Edit", data.Data);
                return new JsonResult(new { isValid = true, html = html });
            }
            _notify.Error(data.Message);
            return new JsonResult(new { isValid = false, html = string.Empty });
        }
        [EncryptedParameters("secret")]
        [Authorize(Policy = "AutoTimer.edit")]
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
        [Authorize(Policy = "AutoTimer.create")]
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
                            var valuestype = "type=" + (int)result.Data.TypeSupplierEInvoice;
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
                            var valuestype = "type=" + (int)model.TypeSupplierEInvoice;
                            secrettype = CryptoEngine.Encrypt(valuestype, _config.Value.Key);
                            _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                        }
                        else
                        {
                            _notify.Error(result.Message);
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                    }
                    return new JsonResult(new { isValid = true, closeSwal = true, secret = secret, secrettype = secrettype });
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
