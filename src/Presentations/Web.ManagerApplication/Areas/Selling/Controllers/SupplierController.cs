using Application.Constants;
using Application.Enums;
using Application.Features.PurchaseOrders.Query;
using Application.Features.Supplierss.Commands;
using Application.Features.Supplierss.Query;
using Application.Hepers;
using Application.Providers;
using Domain.Entities;
using Infrastructure.Infrastructure.Identity.Models;
using Joker.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Supplierss.Selling.Controllers
{
    [Area("Selling")]
    public class SupplierController : BaseController<SupplierController>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly UserManager<ApplicationUser> _userManager;
        public SupplierController(UserManager<ApplicationUser> userManager, IOptions<CryptoEngine.Secrets> config)
        {
            _config = config;
            _userManager = userManager;
        }
        [EncryptedParameters("secret")]
        public async Task<ActionResult> DetailAsync(int id)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var response = await _mediator.Send(new GetByIdSuppliersQuery(currentUser.ComId) { Id = id });
            if (response.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("Detail", response.Data);
                return new JsonResult(new
                {
                    isValid = true,
                    html = html
                });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(response.Message));
            return Json(new { isValid = false });
        }
        [Authorize(Policy = "suppliers.list")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoadAll(Suppliers model)
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
                if (!string.IsNullOrEmpty(searchValue))
                {
                    model.Name = searchValue;
                }
                //Paging Size (10, 20, 50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 10;

                int skip = start != null ? Convert.ToInt32(start) : 0;

                int recordsTotal = 0;
                var currentUser = User.Identity.GetUserClaimLogin();
                // var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                // getting all Customer data  
                var response = await _mediator.Send(new GetAllSuppliersQuery(currentUser.ComId)
                {
                    Name = model.Name,
                    Comid = currentUser.ComId,
                    sortColumn = sortColumn,
                    sortColumnDirection = sortColumnDirection,
                    pageSize = pageSize,
                    skip = skip
                });
                if (response.Succeeded)
                {
                    if (recordsTotal == 0)
                    {
                        recordsTotal = int.Parse(response.Message);
                    }
                   var lstdt =  response.Data.ToList();
                    lstdt.ForEach(x =>
                     {
                         x.secret = CryptoEngine.Encrypt("id=" + x.Id, _config.Value.Key);
                     });
                    var json = lstdt.Select(x => new
                    {
                        id = x.Id,
                        name = x.Name,
                        amount = x.Amount,
                        code = x.CodeSupplier,
                        phonenumber = x.Phonenumber,
                        email = x.Email,
                        secret = x.secret,
                        createdOn = x.CreatedOn,
                        lastModifiedOn = x.LastModifiedOn
                    });

                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = json });
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

        [Authorize(Policy = "suppliers.edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(Suppliers collection)
        {
            var getusser = User.Identity.GetUserClaimLogin();
            //var getusser = await _userManager.GetUserAsync(User);
            collection.ComId = getusser.ComId;
            try
            {
                if (collection.Id == 0)
                {
                    var createProductCommand = _mapper.Map<CreateSupplierCommand>(collection);
                    var result = await _mediator.Send(createProductCommand);
                    if (result.Succeeded)
                    {
                        collection.Id = result.Data;
                        _notify.Success(HeperConstantss.SUS008);

                    }
                    else
                    {
                        _notify.Error(result.Message);
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }
                }
                else
                {
                    var updateProductCommand = _mapper.Map<UpdateSuppliersCommand>(collection);
                    var result = await _mediator.Send(updateProductCommand);
                    if (result.Succeeded) _notify.Success(HeperConstantss.SUS006);

                }
                return new JsonResult(new { isValid = true, loadTable = true, closeSwal = true });
            }
            catch (Exception e)
            {
                _notify.Error(e.ToString());
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
        }

        // GET: SupplierssController/Edit/5
        [Authorize(Policy = "suppliers.create")]
        public async Task<ActionResult> Create()
        {
            var html = await _viewRenderer.RenderViewToStringAsync("_Create", new Domain.Entities.Suppliers());
            return new JsonResult(new { isValid = true, html = html, title = "Thêm mới nhà cung cấp" });
        }
        [Authorize(Policy = "suppliers.edit")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Edit(int id)
        {
            var getusser = User.Identity.GetUserClaimLogin();
            //  var getusser = await _userManager.GetUserAsync(User);
            var data = await _mediator.Send(new GetByIdSuppliersQuery(getusser.ComId) { Id = id });
            if (data.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_Edit", data.Data);
                return new JsonResult(new { isValid = true, html = html, title = "Chỉnh sửa nhà cung cấp" });
            }
            return new JsonResult(new { isValid = false, html = string.Empty });
        }



        // POST: SupplierssController/Delete/5
        [Authorize(Policy = "suppliers.delete")]
        [HttpPost]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                //var getusser = await _userManager.GetUserAsync(User);
                var getusser = User.Identity.GetUserClaimLogin();
                var deleteCommand = await _mediator.Send(new DeleteSuppliersCommand(getusser.ComId, id));
                if (deleteCommand.Succeeded)
                {
                    return new JsonResult(new { isValid = true, loadTable = true });
                }
                else
                {
                    _notify.Error(deleteCommand.Message);
                    return new JsonResult(new { isValid = false });
                }
            }
            catch (Exception ex)
            {
                _notify.Error(ex.Message);
                return new JsonResult(new { isValid = false });
            }
        }
    }
}
