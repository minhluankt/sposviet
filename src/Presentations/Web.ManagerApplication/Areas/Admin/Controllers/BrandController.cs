using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.ManagerApplication.Abstractions;
using Microsoft.Extensions.Logging;

using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Authorization;
using Application.Constants;
using Application.Features.Brands.Query;
using Application.Features.Brands.Commands;
using Microsoft.Extensions.Localization;
using Domain.Entities;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : BaseController<BrandController>
    {
        private readonly IRepositoryAsync<Brand> _repository;
        public BrandController(
        IRepositoryAsync<Brand> repository)
        {
            _repository = repository;

        }
        [Breadcrumb("Danh sách hãng")]
        [Authorize(Policy = "Brand.list")]
        public IActionResult Index()
        {
            _logger.LogInformation(User.Identity.Name + "--> Brand Index");
            return View();
        }
        [Authorize(Policy = "Brand.createoredit")]
        public async Task<ActionResult> CreateOrEdit(int id = 0)
        {
            _logger.LogInformation(User.Identity.Name + "--> Brand CreateOrEdit");
            if (id == 0)
            {
                var model = new Brand();
                var data = await _mediator.Send(new GetAllBrandCacheQuery());
                if (data.Succeeded)
                {
                    if (data.Data.Count()>0)
                    {
                        model.Sort = data.Data.Max(x => x.Sort) + 1;
                    }
                   
                }
                    var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", model);
                return new JsonResult(new { isValid = true, html = html });
            }
            else
            {
                var getid = await _mediator.Send(new GetByIdBrandQuery() { Id = id });
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
                var data = await _mediator.Send(new GetAllBrandCacheQuery());
                if (data.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", data.Data);
                    return new JsonResult(new { isValid = true, html = html });
                    //return View("_ViewAll", data.Data);
                }
                _logger.LogError(_localizer.GetString("NotData").Value);
                ////var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", new Domain.Entities.Brand());
                return new JsonResult(new { isValid = false, html = "" });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new JsonResult(new { isValid = false, html = "" });
            }
        }
        [Authorize(Policy = "Brand.delete")]
        [HttpPost]
        public async Task<ActionResult> Delete(int Id)
        {
            _logger.LogInformation(User.Identity.Name + "--> Brand Delete");
            var delete = await _mediator.Send(new DeleteBrandCommand() { Id = Id });
            if (delete.Succeeded)
            {

                _notify.Success($"Xóa thành công Id: {Id}");
                var response = await _mediator.Send(new GetAllBrandCacheQuery());
                if (response.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                    return new JsonResult(new { isValid = true, html = html, LoadDataTable = true });
                }
                else
                {
                    _notify.Error(response.Message);
                    return new JsonResult(new { isValid = false });
                }
            }
            else
            {
                _notify.Error(delete.Message);
                return new JsonResult(new { isValid = false });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(Brand model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                _notify.Error("Tên không được để trống");
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            var collection = model;
            try
            {
                if (collection.Id == 0)
                {
                    var createProductCommand = _mapper.Map<CreateBrandCommand>(collection);
                    var result = await _mediator.Send(createProductCommand);
                    if (result.Succeeded)
                    {
                        collection.Id = result.Data;
                        _notify.Success(_localizer.GetString("AddCartOk").Value);
                    }
                    else
                    {
                        _notify.Error(result.Message);
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }
                }
                else
                {
                    var updateProductCommand = _mapper.Map<UpdateBrandCommand>(collection);
                    var result = await _mediator.Send(updateProductCommand);
                    if (result.Succeeded) _notify.Success(_localizer.GetString("EditOk").Value);

                }
                var response = await _mediator.Send(new GetAllBrandCacheQuery());
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
