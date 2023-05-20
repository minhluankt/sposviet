using Microsoft.AspNetCore.Authorization;
using Web.ManagerApplication.Abstractions;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Application.Features.ManagerPatternEInvoices.Commands;
using Application.Features.TemplateInvoices.Query;
using Application.Features.ManagerPatternEInvoices.Query;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.DataProtection;


namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class ManagerPatternEInvoiceController : BaseController<ManagerPatternEInvoiceController>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEInvoiceRepository<EInvoice> _einvoice;
        public ManagerPatternEInvoiceController(UserManager<ApplicationUser> userManager,
            IOptions<CryptoEngine.Secrets> config,
            IEInvoiceRepository<EInvoice> einvoice)
        {
            _config = config;
            _einvoice = einvoice;
            _userManager = userManager;
        }
      
        [Authorize(Policy = "selling.managerPatternEInvoice")]
        [EncryptedParameters("secret")]
        public async Task<IActionResult> Index(int type)
        {
            ENumSupplierEInvoice TypeSupplierEInvoice = (ENumSupplierEInvoice)type;
            //var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUser = User.Identity.GetUserClaimLogin();

            var _send = await _mediator.Send(new GetAllSupplierEInvoiceQuery() { TypeSupplierEInvoice= TypeSupplierEInvoice, Comid = currentUser.ComId,IsManagerPatternEInvoices=false });
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
        [EncryptedParameters("secret")]
        [Authorize(Policy = "managerPatternEInvoice.edit")]
        public async Task<ActionResult> EditAsync(int id, ENumSupplierEInvoice TypeSupplierEInvoice)
        {
            if (TypeSupplierEInvoice == ENumSupplierEInvoice.NONE)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR012));
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            _logger.LogInformation(User.Identity.Name + "--> managerPatternEInvoice edit ");
            var data = await _mediator.Send(new GetByIdManagerPatternEInvoiceQuery() { TypeSupplierEInvoice= TypeSupplierEInvoice, Id = id, ComId = currentUser.ComId });
            if (data.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_Edit", data.Data);
                return new JsonResult(new { isValid = true, html = html });
            }
            _notify.Error(data.Message);
            return new JsonResult(new { isValid = false, html = string.Empty });
        }
        public async Task<IActionResult> LoadAll(ManagerPatternEInvoice model)
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

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
                // getting all templateInvoice data  
                var response = await _mediator.Send(new GetAllManagerPatternEInvoiceQuery()
                {
                    keyword= searchValue,
                    Comid = currentUser.ComId,
                    TypeSupplierEInvoice = model.TypeSupplierEInvoice,
                    sortColumn = sortColumn,
                    sortColumnDirection = sortColumnDirection,
                    pageSize = pageSize,
                    skip = skip
                });
                if (response.Succeeded)
                {
                    recordsTotal = response.Data.Count();
                    var lst = response.Data.Skip(skip).Take(pageSize).ToList();
                   
                    foreach (var item in lst)
                    {
                        var values = "id=" + item.Id;
                        var secret1 = CryptoEngine.Encrypt(values, _config.Value.Key);
                        item.screct = secret1;
                        item.CreatedBy =  _userManager.FindByIdAsync(item.CreatedBy).Result?.FullName;
                    }
                    var datakq = lst.Select(x => new { pattern = x.Pattern, serial = x.Serial, selected = x.Selected, secret = x.screct, createdBy = x.CreatedBy, id = x.Id, createdOn = x.CreatedOn });
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = datakq });
                }

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = "" });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }

        }
        [EncryptedParameters("secret")]
        [Authorize(Policy = "managerPatternEInvoice.create")]
        public async Task<ActionResult> CreateAsync(int id, ENumSupplierEInvoice TypeSupplierEInvoice)
        {
            if (TypeSupplierEInvoice == ENumSupplierEInvoice.NONE)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR012));
                return new JsonResult(new { isValid = false });
            }
            try
            {
                _logger.LogInformation(User.Identity.Name + "--> managerPatternEInvoice create");
                var html = await _viewRenderer.RenderViewToStringAsync("_Create", new ManagerPatternEInvoice() {IdSupplierEInvoice=id, TypeSupplierEInvoice= TypeSupplierEInvoice ,Active=true});
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
        [Authorize(Policy = "managerPatternEInvoice.edit")]
        [HttpPost]
        public async Task<ActionResult> RemoveAsync(int id, ENumSupplierEInvoice TypeSupplierEInvoice)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            _logger.LogInformation(User.Identity.Name + "--> SupplierEInvoice RemoveAsync");
            if (TypeSupplierEInvoice==ENumSupplierEInvoice.NONE)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR012));
                return new JsonResult(new { isValid = false });
            }
            var data = await _mediator.Send(new DeleteManagerPatternEInvoiceCommand() { Id = id, ComId = currentUser.ComId ,TypeSupplierEInvoice= TypeSupplierEInvoice });
            if (data.Succeeded)
            {
                _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS007));
                return new JsonResult(new { isValid = true, loadTable = true});
            }
            _notify.Error(GeneralMess.ConvertStatusToString(data.Message));
            return new JsonResult(new { isValid = false, html = string.Empty });
        }
        [Authorize(Policy = "managerPatternEInvoice.create")]
        [HttpPost]
        public async Task<ActionResult> OnPostCreateOrEditAsync(ManagerPatternEInvoice model)
        {
            if (ModelState.IsValid)
            {
                var secret = string.Empty;
                try
                {
                    model.Pattern = model.Pattern?.Replace(" ","");
                    model.Serial = model.Serial?.Replace(" ","");
                    var currentUser = User.Identity.GetUserClaimLogin();
                    model.ComId = currentUser.ComId;

                    if (model.Id == 0)
                    {
                        var createProductCommand = _mapper.Map<CreateManagerPatternEInvoiceCommand>(model);
                        var result = await _mediator.Send(createProductCommand);
                        if (result.Succeeded)
                        {
                            var values = "id=" + result.Data;
                            secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                            model.Id = result.Data;
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
                        var createProductCommand = _mapper.Map<UpdateManagerPatternEInvoiceCommand>(model);
                        var result = await _mediator.Send(createProductCommand);
                        if (result.Succeeded)
                        {
                            var values = "id=" + result.Data;
                            secret = CryptoEngine.Encrypt(values, _config.Value.Key);

                            _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                        }
                        else
                        {
                            _notify.Error(result.Message);
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                    }
                    return new JsonResult(new { isValid = true, loadTable = true, closeSwal = true });
                }
                catch (Exception ex)
                {
                    _notify.Error(ex.Message);
                    return View();
                }
            }
            else
            {
                return new JsonResult(new { isValid = false });
            }

        }
    }
}
