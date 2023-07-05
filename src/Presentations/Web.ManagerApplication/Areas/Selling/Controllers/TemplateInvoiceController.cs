using Application.Constants;
using Application.Enums;
using Application.Features.RoomAndTables.Commands;
using Application.Features.TemplateInvoices.Commands;
using Application.Features.TemplateInvoices.Query;
using Application.Features.VietQRs.Query;
using Application.Hepers;
using Application.Providers;
using Domain.Entities;
using Domain.Identity;
using Hangfire.MemoryStorage.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class TemplateInvoiceController : BaseController<TemplateInvoiceController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public TemplateInvoiceController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [EncryptedParameters("secret")]
        public async Task<IActionResult> DetailtAsync(int id)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var data = await _mediator.Send(new GetByIdTemplateInvoiceQuery() { Id = id, ComId = currentUser.ComId });
            if (data.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_Detailt", data.Data);
                return new JsonResult(new { isValid = true, html = html });
            }
            return new JsonResult(new { isValid = false, html = string.Empty });
        }
        [Authorize(Policy = "templateinvoice.list")]
        public IActionResult Index()
        {
            ViewBag.Selectlist = this.GetSelectListItem(); 
            return View();
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
        private List<SelectListItem> GetSelectListItem(EnumTypeTemplatePrint type = EnumTypeTemplatePrint.NONE)
        {
            var select = Enum.GetValues(typeof(EnumTypeTemplatePrint)).Cast<EnumTypeTemplatePrint>()
                .OrderBy(x => (Convert.ToInt32(x)))
                .Where(x => (Convert.ToInt32(x)) > 0)
                .Select(x => new SelectListItem
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
        public async Task<IActionResult> LoadAll(TemplateInvoice model)
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
                var response = await _mediator.Send(new GetAllTemplateInvoiceQuery()
                {
                    Name = model.Name,
                    TypeTemplatePrint = model.TypeTemplatePrint,
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
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = response.Data });
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
        [Authorize(Policy = "templateInvoice.delete")]
        [HttpPost]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var getusser = User.Identity.GetUserClaimLogin();

                var deleteCommand = await _mediator.Send(new DeleteTemplateInvoiceCommand() { ComId=getusser.ComId,Id=id});
                if (deleteCommand.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(deleteCommand.Message));
                    return new JsonResult(new { isValid = true, loadTable = true });
                }
                else
                {
                    _notify.Error(GeneralMess.ConvertStatusToString(deleteCommand.Message));
                    return new JsonResult(new { isValid = false });
                }
            }
            catch (Exception ex)
            {
                _notify.Error(ex.Message);
                return new JsonResult(new { isValid = false });
            }
        }
        [Authorize(Policy = "templateInvoice.create")]
        public async Task<ActionResult> CreateAsync()
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();
                TemplateInvoice templateInvoice = new TemplateInvoice() { TypeTemplatePrint = EnumTypeTemplatePrint.NONE, Selectlist = this.GetSelectListItem(), Active = true };


                _logger.LogInformation(User.Identity.Name + "--> templateInvoice create");
                var send = await _mediator.Send(new GetByIdVietQRQuery(currentUser.ComId, null) { IsGetFirst = true });
                if (send.Succeeded)
                {
                    templateInvoice.IsRegisterQrCodeVietQR = true;
                }
                var htmlview = await _viewRenderer.RenderViewToStringAsync("_Create", templateInvoice);
                return new JsonResult(new { isValid = true, html = htmlview, title="Thêm mới mẫu in" });
            }
            catch (Exception e)
            {
                _notify.Error(e.ToString());
                return new JsonResult(new { isValid = false });
            }
        }
        [EncryptedParameters("secret")]
        [Authorize(Policy = "templateInvoice.edit")]
        public async Task<ActionResult> EditAsync(int id)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            _logger.LogInformation(User.Identity.Name + "--> edit detailt");
            var data = await _mediator.Send(new GetByIdTemplateInvoiceQuery() { Id = id, ComId = currentUser.ComId });
            if (data.Succeeded)
            {
                data.Data.Selectlist = this.GetSelectListItem(data.Data.TypeTemplatePrint);

                var send = await _mediator.Send(new GetByIdVietQRQuery(currentUser.ComId, null) { IsGetFirst=true});
                if (send.Succeeded)
                {
                    data.Data.IsRegisterQrCodeVietQR = true;
                }
                var html = await _viewRenderer.RenderViewToStringAsync("_Edit", data.Data);
                return new JsonResult(new { isValid = true, html = html, title = "Chỉnh sửa mẫu in" });
            }
            return new JsonResult(new { isValid = false, html = string.Empty });
        }
        [Authorize(Policy = "templateInvoice.create")]
        [HttpPost]
        public async Task<ActionResult> OnPostCreateOrEditAsync(TemplateInvoice model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var currentUser = User.Identity.GetUserClaimLogin();
                    model.ComId = currentUser.ComId;

                    if (!model.IsShowQrCodeVietQR)
                    {
                        model.HtmlQrCodeVietQR = null;
                    }

                    if (model.Id == 0)
                    {

                        var createProductCommand = _mapper.Map<CreateTemplateInvoiceCommand>(model);
                        var result = await _mediator.Send(createProductCommand);
                        if (result.Succeeded)
                        {
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
                        var createProductCommand = _mapper.Map<UpdateTemplateInvoiceCommand>(model);
                        var result = await _mediator.Send(createProductCommand);
                        if (result.Succeeded)
                        {
                            model.Id = result.Data;
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
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", model);
                return new JsonResult(new { isValid = false, html = html });
            }

        }
    }
}
