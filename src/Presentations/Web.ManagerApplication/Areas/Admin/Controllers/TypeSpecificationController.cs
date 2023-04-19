using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.ManagerApplication.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Logging;

using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Authorization;
using Application.Constants;
using Application.Features.TypeSpecification.Query;
using Application.Features.TypeSpecification.Commands;
using Microsoft.Extensions.Localization;
using Application.Hepers;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class TypeSpecificationController : BaseController<TypeSpecificationController>
    {
        private IStringLocalizer<SharedResource> _localizer;
        private readonly IRepositoryAsync<TypeSpecifications> _repository;
        public TypeSpecificationController(IStringLocalizer<SharedResource> localizer,
        IRepositoryAsync<TypeSpecifications> repository)
        {
            _localizer = localizer;
            _repository = repository;

        }
        [Breadcrumb("Danh sách phân loại danh mục")]
        [Authorize(Policy = "typespecification.list")]
        public IActionResult Index()
        {
            _logger.LogInformation(User.Identity.Name + "--> typespecification Index");
            return View();
        }
        public async Task<ActionResult> CreateOrEdit(int id = 0)
        {
            _logger.LogInformation(User.Identity.Name + "--> typespecification CreateOrEdit");
            if (id == 0)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", new TypeSpecifications());
                return new JsonResult(new { isValid = true, html = html });
            }
            else
            {
                var getid = await _mediator.Send(new GetByIdTypeSpecificationsQuery() { Id = id });
                if (getid.Succeeded)
                {

                    var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", getid.Data);
                    return new JsonResult(new { isValid = true, html = html });

                }
                _notify.Error(getid.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

        }
        public async Task<IActionResult> LoadAll()
        {
            try
            {
                var data = await _mediator.Send(new GetAllTypeSpecificationsCacheQuery());
                if (data.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", data.Data);
                    return new JsonResult(new { isValid = true, html = html });
                    //return View("_ViewAll", data.Data);
                }
                _logger.LogError(_localizer.GetString("NotData").Value);
                ////var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", new Domain.Entities.TypeSpecification());
                return new JsonResult(new { isValid = false, html = "" });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new JsonResult(new { isValid = false, html = "" });
            }
        }
        [HttpPost]
        public async Task<ActionResult> Delete(int Id)
        {
            _logger.LogInformation(User.Identity.Name + "--> typespecification Delete");
            var delete = await _mediator.Send(new DeleteTypeSpecificationsCommand() { Id = Id });
            if (delete.Succeeded)
            {

                _notify.Success($"Xóa thành công Id: {Id}");
                var response = await _mediator.Send(new GetAllTypeSpecificationsCacheQuery());
                if (response.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                    return new JsonResult(new { isValid = true, html = html, LoadDataTable = true });
                }
                else
                {
                    _notify.Error(GeneralMess.ConvertStatusToString(response.Message));
                    return new JsonResult(new { isValid = false });
                }
            }
            else
            {
                _notify.Error(GeneralMess.ConvertStatusToString(delete.Message));
                return new JsonResult(new { isValid = false });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(TypeSpecifications model)
        {
            if (string.IsNullOrEmpty(model.Name.Trim()))
            {
                _notify.Error("Tên không được để trống");
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            var collection = model;
            try
            {
                if (collection.Id == 0)
                {
                    var createProductCommand = _mapper.Map<CreateTypeSpecificationsCommand>(collection);
                    var result = await _mediator.Send(createProductCommand);
                    if (result.Succeeded)
                    {
                        collection.Id = result.Data;
                        _notify.Success(_localizer.GetString("AddOk").Value);
                    }
                    else
                    {
                        _notify.Error(result.Message);
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }
                }
                else
                {
                    var updateProductCommand = _mapper.Map<UpdateTypeSpecificationsCommand>(collection);
                    var result = await _mediator.Send(updateProductCommand);
                    if (result.Succeeded) _notify.Success(_localizer.GetString("EditOk").Value);

                }
                var response = await _mediator.Send(new GetAllTypeSpecificationsCacheQuery());
                if (response.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                    return new JsonResult(new { isValid = true, html = html, LoadDataTable = true });
                }
                else
                {
                    _notify.Error(response.Message);
                    return null;
                }
            }
            catch (Exception e)
            {
                _notify.Error(e.ToString());
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
        }
    }
}
