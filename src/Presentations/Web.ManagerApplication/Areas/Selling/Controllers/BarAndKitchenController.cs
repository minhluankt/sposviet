using Application.Constants;
using Application.Enums;
using Application.Features.Banners.Query;
using Application.Features.BarAndKitchens.Commands;
using Application.Features.BarAndKitchens.Query;
using Application.Features.Permissions.Commands;
using Application.Features.Permissions.Query;
using Application.Features.TemplateInvoices.Commands;
using Application.Features.TemplateInvoices.Query;
using Application.Hepers;
using Application.Providers;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class BarAndKitchenController : BaseController<BarAndKitchenController>
    {
        // GET: BarAndKitchenController
        [Authorize(Policy = "barAndKitchen.list")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: BarAndKitchenController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BarAndKitchenController/Create
        [Authorize(Policy = "barAndKitchen.create")]
        public async Task<ActionResult> CreateAsync()
        {
            try
            {
                _logger.LogInformation(User.Identity.Name + "--> templateInvoice create");
                var htmlview = await _viewRenderer.RenderViewToStringAsync("_Create", new BarAndKitchen() { Active = true });
                return new JsonResult(new { isValid = true, html = htmlview, title = "Thêm nhà bếp" });
            }
            catch (Exception e)
            {
                _notify.Error(e.ToString());
                return new JsonResult(new { isValid = false });
            }
        }
        [EncryptedParameters("secret")]
        [Authorize(Policy = "barAndKitchen.edit")]
        public async Task<ActionResult> EditAsync(int id)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            _logger.LogInformation(User.Identity.Name + "--> edit detailt");
            var data = await _mediator.Send(new GetByIdBarAndKitchenQuery() { Id = id, Comid = currentUser.ComId });
            if (data.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_Edit", data.Data);
                return new JsonResult(new { isValid = true, html = html, title = "Chỉnh sửa bếp" });
            }
            return new JsonResult(new { isValid = false, html = string.Empty });
        }
        
        [HttpPost]
        public async Task<ActionResult> OnPostCreateOrEditAsync(BarAndKitchen model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var currentUser = User.Identity.GetUserClaimLogin();
                    model.ComId = currentUser.ComId;

                    if (model.Id == 0)
                    {

                        var createProductCommand = _mapper.Map<CreateBarAndKitchenCommand>(model);
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
                        var createProductCommand = _mapper.Map<UpdateBarAndKitchenCommand>(model);
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
                    var response = await _mediator.Send(new GetAllBarAndKitchenQuery() { ComId = currentUser.ComId });
                    if (response.Succeeded)
                    {
                        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                        return new JsonResult(new { isValid = true, html = html, loadDataTable = true, closeSwal = true });
                    }
                    return new JsonResult(new { isValid = true, loadDataTable = true, html = "", closeSwal = true });
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
        public async Task<IActionResult> LoadAll()
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();
                var response = await _mediator.Send(new GetAllBarAndKitchenQuery() { ComId= currentUser.ComId});
                if (response.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                    return new JsonResult(new { isValid = true, html = html });
                }
                return default;
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return default;
            }
        }
        // POST: BarAndKitchenController/Create
        

       
        // GET: BarAndKitchenController/Delete/5
        [Authorize(Policy = "barAndKitchen.delete")]
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();
                var deleteCommand = await _mediator.Send(new DeleteBarAndKitchenCommand { Id = id,ComId= currentUser.ComId });
                if (deleteCommand.Succeeded)
                {
                    _notify.Success($"Xóa thành công.");
                    var response = await _mediator.Send(new GetAllBarAndKitchenQuery() { ComId = currentUser.ComId });
                    if (response.Succeeded)
                    {
                        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                        return new JsonResult(new { isValid = true, html = html });
                    }
                    else
                    {
                        _notify.Error(response.Message);
                        return new JsonResult(new { isValid = false});
                    }
                }
                else
                {
                    _notify.Error(deleteCommand.Message);
                    return new JsonResult(new { isValid = false });
                }
            }
            catch(Exception e)
            {
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false });
            }
        }
        // POST: BarAndKitchenController/Delete/5
       
    }
}
