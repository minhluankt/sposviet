using Application.Constants;
using Application.Features.Areas.Query;
using Application.Features.Customers.Query;
using Application.Features.Products.Query;
using Application.Features.RoomAndTables.Commands;
using Application.Features.RoomAndTables.Query;
using Application.Providers;
using Domain.ViewModel;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Application.Hepers;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Areas.Selling.Models;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class RoomTableController : BaseController<ProductController>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly UserManager<ApplicationUser> _userManager;
        public RoomTableController(UserManager<ApplicationUser> userManager, IOptions<CryptoEngine.Secrets> config)
        {
            _config = config;
            _userManager = userManager;
        }
        [Authorize(Policy = "roomtable.list")]
        public async Task<IActionResult> IndexAsync()
        {
            //var currentUser = User.Identity.GetUserClaimLogin();
            var currentUser = HttpContext.User.Identity.GetUserClaimLogin();
            RoomTableModel model = new RoomTableModel();
            var response = await _mediator.Send(new GetAllAreaQuery(currentUser.ComId) { Paging = false });
            if (response.Succeeded)
            {
                model.SelectList = response.Data.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).ToList();
                //model.Areas = response.Data.ToList();
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> LoadAll(RoomTableModel model)
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
                int pageSize = length != null ? Convert.ToInt32(length) : 10;

                int skip = start != null ? Convert.ToInt32(start) : 0;

                int recordsTotal = 0;

                var currentUser = User.Identity.GetUserClaimLogin();
                // getting all Customer data  
                var response = await _mediator.Send(new GetAllRoomAndTableQuery(currentUser.ComId)
                {
                    Name = model.Name,
                    IdArea = model.IdArea,
                    Comid = currentUser.ComId,
                    Cache = false,
                    sortColumn = sortColumn,
                    sortColumnDirection = sortColumnDirection,
                });

                if (response.Succeeded)
                {
                    recordsTotal = int.Parse(response.Message);
                    var lstnew = response.Data.Skip(skip).Take(pageSize).ToList();
                    lstnew.ForEach(x =>
                    {
                        x.IdString = CryptoEngine.Encrypt("id=" + x.Id, _config.Value.Key);
                        x.AreaName = x.Area?.Name;
                    });
                    var json = lstnew.Select(x => new
                    {
                        id = x.Id,
                        active = x.Active,
                        idString = x.IdString,
                        name = x.Name,
                        createdOn = x.CreatedOn,
                        lastModifiedOn = x.LastModifiedOn,
                        areaName = x.AreaName
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
        [EncryptedParameters("secret")]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RoomAndTablesController/Create


        // POST: RoomAndTablesController/Create
        [Authorize(Policy = "roomtable.create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(Domain.Entities.RoomAndTable collection)
        {
            var getusser = User.Identity.GetUserClaimLogin();
            collection.ComId = getusser.ComId;
            try
            {
                if (collection.Id == 0)
                {
                    if (collection.IsCreateMuti)
                    {
                        collection.Name = collection.NameSelect;
                    }

                    var createProductCommand = _mapper.Map<CreateRoomAndTableCommand>(collection);
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
                    var updateProductCommand = _mapper.Map<UpdateRoomAndTableCommand>(collection);
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

        // GET: RoomAndTablesController/Edit/5
        [Authorize(Policy = "roomtable.create")]
        public async Task<ActionResult> Create()
        {
            var getusser = User.Identity.GetUserClaimLogin();
            RoomTableModel model = new RoomTableModel();
            model.Active = true;
            var response = await _mediator.Send(new GetAllAreaQuery(getusser.ComId) { Paging = false });
            ViewBag.SelectList = null;
            if (response.Succeeded)
            {
                ViewBag.SelectList = response.Data.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).ToList();

            }

            //var html = await _viewRenderer.RenderViewToStringAsync("_Create", new Domain.Entities.RoomAndTable() { Active = true });
            return View("_Create", new Domain.Entities.RoomAndTable() { Active = true });
        }
        [Authorize(Policy = "roomtable.edit")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Edit(int id)
        {
            var getusser = User.Identity.GetUserClaimLogin();
            var data = await _mediator.Send(new GetByIdRoomAndTableQuery(getusser.ComId) { Id = id });
            if (data.Succeeded)
            {
                RoomTableModel model = new RoomTableModel();
                model.Active = true;
                var response = await _mediator.Send(new GetAllAreaQuery(getusser.ComId) { Paging = false });
                ViewBag.SelectList = null;
                if (response.Succeeded)
                {
                    ViewBag.SelectList = response.Data.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name, Selected = x.Id == data.Data.IdArea }).ToList();

                }
                return View("_Edit", data.Data);
                //var html = await _viewRenderer.RenderViewToStringAsync("_Edit", data.Data);
                //return new JsonResult(new { isValid = true, html = html, title = "Chỉnh sửa bàn/phòng" });
            }
            return new JsonResult(new { isValid = false, html = string.Empty });
        }

        public async Task<JsonResult> GetTableJson()
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var data = await _mediator.Send(new GetAllAreaQuery(currentUser.ComId) { ComId = currentUser.ComId, Paging = false,IncludeOrdertable=true });
            if (data.Succeeded)
            {
                //var getlst = data.Data.SelectMany(x => x.RoomAndTables);
                var jspm = data.Data.SelectMany(x=>x.RoomAndTables).Select(x => new //RoomAndTableViewModel()
                {
                    tableName = x.Name,
                    idtable = x.IdGuid,
                    nameArea = x.Area.Name,
                    idArea = x.Area.Id,
                    numberProduct = x.OrderTables.Where(x=>x.Status==Application.Enums.EnumStatusOrderTable.DANG_DAT).Count(),
                }).OrderByDescending(x=>x.numberProduct).ToList();
                var GroupBycate = data.Data.Select(x => new
                {
                    name = x.Name,
                    id = x.Id,
                    countpro = x.RoomAndTables.Count()
                });
                return new JsonResult(new { isValid = true, jsonPro = jspm, jsoncate = GroupBycate });
            }
            return new JsonResult(new { isValid = false });
        }
        
        // POST: RoomAndTablesController/Delete/5
        [Authorize(Policy = "roomtable.delete")]
        [HttpPost]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var getusser = User.Identity.GetUserClaimLogin();

                var deleteCommand = await _mediator.Send(new DeleteRoomAndTableCommand(getusser.ComId, id));
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
