using Application.Constants;
using Application.Features.Banners.Commands;
using Application.Features.Banners.Query;
using Application.Hepers;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BannerController : BaseController<BannerController>
    {
        [Authorize(Policy = "banner.list")]
        public IActionResult Index()
        {
            _logger.LogInformation(User.Identity.Name + "--> Banner index");
            return View();
        }
        [Authorize(Policy = "banner.create")]
        public async Task<ActionResult> CreateOrEdit(int id = 0)
        {
            _logger.LogInformation(User.Identity.Name + "--> Banner CreateOrEdit");
            if (id == 0)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", new Banner());
                return new JsonResult(new { isValid = true, html = html });
            }
            else
            {
                var getid = await _mediator.Send(new GetByIdBannerQuery() { Id = id });
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
                var data = await _mediator.Send(new GetAllBannerCacheQuery());
                if (data.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", data.Data);
                    return new JsonResult(new { isValid = true, html = html });
                    //return View("_ViewAll", data.Data);
                }
                ////var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", new Domain.Entities.TypeSpecification());
                return new JsonResult(new { isValid = false, html = "" });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new JsonResult(new { isValid = false, html = "" });
            }
        }

        [Authorize(Policy = "banner.delete")]
        [HttpPost]
        public async Task<ActionResult> Delete(int Id)
        {
            _logger.LogInformation(User.Identity.Name + "--> Banner Delete");
            var delete = await _mediator.Send(new DeleteBannerCommand() { Id = Id });
            if (delete.Succeeded)
            {

                _notify.Success($"Xóa thành công Id: {Id}");
                var response = await _mediator.Send(new GetAllBannerCacheQuery());
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
            else
            {
                _notify.Error(delete.Message);
                return null;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(Banner model,IFormFile Img)
        {
            
            var collection = model;
            try
            {
                if (collection.Id == 0)
                {
                    if (Img == null || Img.Length == 0)
                    {
                        _notify.Error("Vui lòng chọn hình ảnh");
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }
                    var createProductCommand = _mapper.Map<CreateBannerCommand>(collection);
                    createProductCommand.Img = Img;
                    var result = await _mediator.Send(createProductCommand);
                    if (result.Succeeded)
                    {
                        collection.Id = result.Data;
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
                    var updateProductCommand = _mapper.Map<UpdateBannerCommand>(collection);
                    updateProductCommand.Img = Img;
                    var result = await _mediator.Send(updateProductCommand);
                    if (result.Succeeded) _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));

                }
                var response = await _mediator.Send(new GetAllBannerCacheQuery());
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
