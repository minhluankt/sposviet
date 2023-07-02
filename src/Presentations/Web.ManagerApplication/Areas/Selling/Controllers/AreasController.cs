using Application.Constants;
using Application.Enums;
using Application.Features.Areas.Commands;
using Application.Features.Areas.Query;
using Application.Hepers;
using Application.Providers;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Areas.Selling.Models;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class AreasController : BaseController<AreasController>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly UserManager<ApplicationUser> _userManager;
        public AreasController(UserManager<ApplicationUser> userManager, IOptions<CryptoEngine.Secrets> config)
        {
            _config = config;
            _userManager = userManager;
        }
        [Authorize(Policy = "areas.index")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoadAll(AreasModel model)
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
                var response = await _mediator.Send(new GetAllAreaQuery(currentUser.ComId)
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
                        x.NumberTable = x.RoomAndTables.Count();
                    });
                    var json = response.Data.Select(x => new
                    {
                        id = x.Id,
                        name = x.Name,
                        idString = x.IdString,
                        numberTable = x.NumberTable,
                        status = x.Status,
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

        [Authorize(Policy = "area.edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(Domain.Entities.Area collection)
        {
            var getusser = User.Identity.GetUserClaimLogin();
            //var getusser = await _userManager.GetUserAsync(User);
            collection.ComId = getusser.ComId;
            if (collection.Active)
            {
                collection.Status = EnumStatusArea.DANG_HOAT_DONG;
            }
            else
            {
                collection.Status = EnumStatusArea.NGUNG_HOAT_DONG;
            }
            try
            {
                if (collection.Id == 0)
                {
                    var createProductCommand = _mapper.Map<CreateAreaCommand>(collection);
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
                    var updateProductCommand = _mapper.Map<UpdateAreaCommand>(collection);
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

        // GET: AreasController/Edit/5
        [Authorize(Policy = "area.create")]
        public async Task<ActionResult> Create()
        {
            var html = await _viewRenderer.RenderViewToStringAsync("_Create", new Domain.Entities.Area() { Active = true });
            return new JsonResult(new { isValid = true, html = html, title = "Thêm mới khu vực" });
        }
        [Authorize(Policy = "area.edit")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Edit(int id)
        {
            var getusser = User.Identity.GetUserClaimLogin();
          //  var getusser = await _userManager.GetUserAsync(User);
            var data = await _mediator.Send(new GetByIdAreaQuery(getusser.ComId) { Id = id });
            if (data.Succeeded)
            {
                if (data.Data.Status == EnumStatusArea.DANG_HOAT_DONG)
                {
                    data.Data.Active = true;
                }
                var html = await _viewRenderer.RenderViewToStringAsync("_Edit", data.Data);
                return new JsonResult(new { isValid = true, html = html, title = "Chỉnh sửa khu vực" });
            }
            return new JsonResult(new { isValid = false, html = string.Empty });
        }



        // POST: AreasController/Delete/5
        [Authorize(Policy = "area.delete")]
        [HttpPost]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                //var getusser = await _userManager.GetUserAsync(User);
                var getusser = User.Identity.GetUserClaimLogin();
                var deleteCommand = await _mediator.Send(new DeleteAreaCommand(getusser.ComId, id));
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
