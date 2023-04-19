using Application.Constants;
using Application.Features.TemplateInvoices.Commands;
using Application.Features.TemplateInvoices.Query;
using Application.Hepers;
using Application.Providers;
using Domain.Entities;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            return View();
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
        [Authorize(Policy = "templateInvoice.create")]
        public async Task<ActionResult> CreateAsync()
        {
            try
            {
                _logger.LogInformation(User.Identity.Name + "--> templateInvoice create");
                var html = await _viewRenderer.RenderViewToStringAsync("_Create", new TemplateInvoice());
                return new JsonResult(new { isValid = true, html = html });
            }
            catch (Exception e)
            {

                throw;
            }

            // return View("_Create");
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

                var html = await _viewRenderer.RenderViewToStringAsync("_Edit", data.Data);
                return new JsonResult(new { isValid = true, html = html });
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
