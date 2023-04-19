using Application.Constants;
using Application.Features.Units.Commands;
using Application.Features.Units.Query;
using Application.Hepers;
using Application.Providers;
using Domain.Entities;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Units.Selling.Controllers
{
    [Area("Selling")]
    public class UnitController : BaseController<UnitController>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly UserManager<ApplicationUser> _userManager;
        public UnitController(UserManager<ApplicationUser> userManager, IOptions<CryptoEngine.Secrets> config)
        {
            _config = config;
            _userManager = userManager;
        }
        [Authorize(Policy = "units.index")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoadAll(Unit model)
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
                var response = await _mediator.Send(new GetAllUnitQuery(currentUser.ComId)
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
                    response.Data.ForEach(x =>
                    {
                        x.IdString = CryptoEngine.Encrypt("id=" + x.Id, _config.Value.Key);
                        x.useCount = x.Products.Count();
                    });
                    var json = response.Data.Select(x => new
                    {
                        id = x.Id,
                        fullName = x.FullName,
                        name = x.Name,
                        idString = x.IdString,
                        numberTable = x.useCount,
                        createdOn = x.CreatedOn,
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

        [Authorize(Policy = "units.edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(Domain.Entities.Unit collection)
        {
            var getusser = User.Identity.GetUserClaimLogin();
            //var getusser = await _userManager.GetUserAsync(User);
            collection.ComId = getusser.ComId;
            try
            {
                if (collection.Id == 0)
                {
                    var createProductCommand = _mapper.Map<CreateUnitCommand>(collection);
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
                    var updateProductCommand = _mapper.Map<UpdateUnitCommand>(collection);
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

        // GET: UnitsController/Edit/5
        [Authorize(Policy = "units.create")]
        public async Task<ActionResult> Create()
        {
            var html = await _viewRenderer.RenderViewToStringAsync("_Create", new Domain.Entities.Unit() {  });
            return new JsonResult(new { isValid = true, html = html, title = "Thêm mới đơn vị tính" });
        }
        [Authorize(Policy = "units.edit")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Edit(int id)
        {
            var getusser = User.Identity.GetUserClaimLogin();
            //  var getusser = await _userManager.GetUserAsync(User);
            var data = await _mediator.Send(new GetByIdUnitQuery(getusser.ComId) { Id = id });
            if (data.Succeeded)
            {
              
                var html = await _viewRenderer.RenderViewToStringAsync("_Edit", data.Data);
                return new JsonResult(new { isValid = true, html = html, title = "Chỉnh sửa đơn vị tính" });
            }
            return new JsonResult(new { isValid = false, html = string.Empty });
        }



        // POST: UnitsController/Delete/5
        [Authorize(Policy = "units.delete")]
        [HttpPost]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                //var getusser = await _userManager.GetUserAsync(User);
                var getusser = User.Identity.GetUserClaimLogin();
                var deleteCommand = await _mediator.Send(new DeleteUnitCommand(getusser.ComId, id));
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
