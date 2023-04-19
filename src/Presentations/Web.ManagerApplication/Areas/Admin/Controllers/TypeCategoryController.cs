using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.ManagerApplication.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Authorization;
using Application.Features.TypeCategorys.Query;
using Application.Constants;
using Web.ManagerApplication.Areas.Admin.Models.Categorys;
using Application.Features.TypeCategorys.Commands;
using Microsoft.Extensions.Localization;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TypeCategoryController : BaseController<TypeCategoryController>
    {
        private IStringLocalizer<SharedResource> _localizer;
        private readonly IRepositoryAsync<TypeCategory> _repository;
        public TypeCategoryController(
        IRepositoryAsync<TypeCategory> repository, IStringLocalizer<SharedResource> localizer)
        {
            _repository = repository;
            _localizer = localizer;

        }
        [Breadcrumb("Danh sách phân loại danh mục")]
        [Authorize(Policy = "typecategory.list")]
        public IActionResult Index()
        {
            _logger.LogInformation(User.Identity.Name + "--> TypeCategory Index");
            return View();
        }
        public async Task<ActionResult> CreateOrEdit(int id = 0)
        {
            _logger.LogInformation(User.Identity.Name + "--> TypeCategory CreateOrEdit ");
            if (id == 0)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", new TypeCategoryViewModel());
                return new JsonResult(new { isValid = true, html = html });
            }
            else
            {
                var getid = await _mediator.Send(new GetByIdTypeCategoryQuery() { Id = id });
                if (getid.Succeeded)
                {
                    var model = new TypeCategoryViewModel
                    {
                        Name = getid.Data.Name,
                        View = getid.Data.View,

                    };
                    var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", model);
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
                var data = await _mediator.Send(new GetAllTypeCategoryCacheQuery());
                if (data.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", data.Data);
                    return new JsonResult(new { isValid = true, html = html });
                    //return View("_ViewAll", data.Data);
                }
                _logger.LogError(_localizer.GetString("NotData").Value);
                ////var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", new Domain.Entities.TypeCategory());
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
            var delete = await _mediator.Send(new DeleteTypeCategoryCommand() { Id = Id });
            if (delete.Succeeded)
            {

                _notify.Success($"Xóa thành công Id: {Id}");
                var response = await _mediator.Send(new GetAllTypeCategoryCacheQuery());
                if (response.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                    return new JsonResult(new { isValid = true, html = html, });
                }
                else
                {
                    _notify.Error(response.Message);
                    return null;
                }
            }
            else
            {
                _notify.Error(delete.Message);
                return null;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(TypeCategoryViewModel model)
        {
            var collection = model;
            try
            {
                if (collection.Id == 0)
                {
                    var createProductCommand = _mapper.Map<CreateTypeCategoryCommand>(collection);
                    createProductCommand.IsActive = true;
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
                    var updateProductCommand = _mapper.Map<UpdateTypeCategoryCommand>(collection);
                    var result = await _mediator.Send(updateProductCommand);
                    if (result.Succeeded) _notify.Success(_localizer.GetString("EditOk").Value);

                }
                var response = await _mediator.Send(new GetAllTypeCategoryCacheQuery());
                if (response.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                    return new JsonResult(new { isValid = true, html = html });
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
