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
using Application.Features.PromotionRuns.Query;
using Application.Features.PromotionRuns.Commands;
using Microsoft.Extensions.Localization;
using Domain.Entities;
using Application.Hepers;
using Application.Enums;
using Application.Features.Products.Query;
using Web.ManagerApplication.Areas.Admin.Models;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
   
    [Area("Admin")]
    public class PromotionRunController : BaseController<PromotionRunController>
    {
        private readonly IRepositoryAsync<PromotionRun> _repository;
        public PromotionRunController(
        IRepositoryAsync<PromotionRun> repository)
        {
            _repository = repository;

        }
        [Breadcrumb("Danh sách chương trình khuyến mãi")]
        [Authorize(Policy = "PromotionRun.list")]
        public IActionResult Index()
        {
            _logger.LogInformation(User.Identity.Name + "--> PromotionRun Index");
            return View();
        }
        [Authorize(Policy = "PromotionRun.createoredit")]
        public async Task<ActionResult> CreateOrEdit(int id = 0)
        {
            _logger.LogInformation(User.Identity.Name + "--> PromotionRun CreateOrEdit");

            if (id == 0)
            {
                var model = new PromotionRun();
                model.StartDate = DateTime.Now;
                model.EndDate = DateTime.Now.AddDays(1);
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", model);
                return new JsonResult(new { isValid = true, html = html });
            }
            else
            {
                var getid = await _mediator.Send(new GetByIdPromotionRunQuery() { Id = id });
                if (getid.Succeeded)
                {
                    if (getid.Data.Status> (int)StatusPromotionRun.Processing &&getid.Data.IsCancelEvent)
                    {
                        _notify.Error("Chương trình đã kết thúc hoặc đã bị hủy không được chỉnh sửa");
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }

                    var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", getid.Data);
                    return new JsonResult(new { isValid = true, html = html });

                }
                _notify.Error(getid.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

        }


         [Authorize(Policy = "PromotionRun.CloneSite")]
        public async Task<ActionResult> CloneSite(int id)
        {
            _logger.LogInformation(User.Identity.Name + "--> PromotionRun CloneSite");
            var getid = await _mediator.Send(new GetByIdPromotionRunQuery() { Id = id });
            if (getid.Succeeded)
            {
                var model = _mapper.Map<PromotionRunViewModel>(getid.Data);
                if (getid.Data.Status < (int)StatusPromotionRun.Done)
                {
                    _notify.Error("Chương trình chưa kết thúc không được nhân bản");
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }

                var SearchProductQuery = await _mediator.Send(new SearchProductQuery() { IdPromotionRun = id });
                if (SearchProductQuery.Succeeded)
                {
                    model.Products  = SearchProductQuery.Data.ToList();
                }
                model.IsActive = false;
                var html = await _viewRenderer.RenderViewToStringAsync("_CloneSite", model);
                return new JsonResult(new { isValid = true, html = html });

            }
            _notify.Error(getid.Message);
            return new JsonResult(new { isValid = false, html = string.Empty });
        }
           [Authorize(Policy = "PromotionRun.CloneSite")]
           [HttpPost]
        public async Task<ActionResult> CloneSite(PromotionRunViewModel model)
        {
            _logger.LogInformation(User.Identity.Name + "--> PromotionRun CloneSite");

            if (model.EndDate<=DateTime.Now.AddMinutes(10))
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR031));
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            if (string.IsNullOrEmpty(model.JsonProduct))
            {
                var getid = await _mediator.Send(new GetByIdPromotionRunQuery() { Id = model.Id });
                if (!getid.Succeeded)
                {
                    _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR030));
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
            }
            var createProductCommand = _mapper.Map<CreatePromotionRunCommand>(model);
            createProductCommand.IsCloneEvent = true;
            var result = await _mediator.Send(createProductCommand);
            if (result.Succeeded)
            {
                _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS008));
                // ok thì đi tiếp
            }
            else
            {
                _notify.Error(result.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            ///ok thì load lại danh sách
            var response = await _mediator.Send(new GetAllPromotionRunCacheQuery());
            if (response.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                return new JsonResult(new { isValid = true, html = html, LoadDataTable = true });
            }
            else
            {
                _notify.Error(response.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
        }



        public async Task<IActionResult> LoadAll()
        {
            try
            {
                var data = await _mediator.Send(new GetAllPromotionRunCacheQuery());
                if (data.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", data.Data);
                    return new JsonResult(new { isValid = true, html = html });
                    //return View("_ViewAll", data.Data);
                }
                _logger.LogError(_localizer.GetString("NotData").Value);
                ////var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", new Domain.Entities.PromotionRun());
                return new JsonResult(new { isValid = false, html = "" });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new JsonResult(new { isValid = false, html = "" });
            }
        }
        [Authorize(Policy = "PromotionRun.delete")]
        [HttpPost]
        public async Task<ActionResult> Delete(int Id)
        {
            _logger.LogInformation(User.Identity.Name + "--> PromotionRun Delete");
            var delete = await _mediator.Send(new DeletePromotionRunCommand() { Id = Id });
            if (delete.Succeeded)
            {

                _notify.Success($"Xóa thành công Id: {Id}");
                var response = await _mediator.Send(new GetAllPromotionRunCacheQuery());
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
        public async Task<ActionResult> OnPostCreateOrEdit(PromotionRun model)
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
                    var createProductCommand = _mapper.Map<CreatePromotionRunCommand>(collection);
                    var result = await _mediator.Send(createProductCommand);
                    if (result.Succeeded)
                    {
                        collection.Id = result.Data;
                        _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS008));
                    }
                    else
                    {
                        _notify.Error(GeneralMess.ConvertStatusToString(result.Message));
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }
                }
                else
                {
                    var updateProductCommand = _mapper.Map<UpdatePromotionRunCommand>(collection);
                    var result = await _mediator.Send(updateProductCommand);
                    if (result.Succeeded) _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));

                }
                var response = await _mediator.Send(new GetAllPromotionRunCacheQuery());
                if (response.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                    return new JsonResult(new { isValid = true, html = html, LoadDataTable = true });
                }
                else
                {
                    _notify.Error(GeneralMess.ConvertStatusToString(response.Message));
                    return new JsonResult(new { isValid = false, html = string.Empty });
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
