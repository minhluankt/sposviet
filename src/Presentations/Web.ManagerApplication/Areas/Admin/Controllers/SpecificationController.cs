using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Constants;
using System.Threading;
using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Application.Hepers;

using Microsoft.AspNetCore.Mvc.Rendering;
using Application.Features.TypeSpecification.Query;
using Newtonsoft.Json;
using Microsoft.Extensions.Localization;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication;
using Web.ManagerApplication.Areas.Admin.Models;
using Application.Features.Specification.Query;
using Application.Features.Specification.Commands;
using Application.Features.Specificationss.Commands;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SpecificationController : BaseController<SpecificationController>
    {
        private readonly ITypeSpecificationRepository _typespec;
        private readonly IRepositoryAsync<Specifications> _repository;
        private IStringLocalizer<SharedResource> _localizer;
        public SpecificationController(ITypeSpecificationRepository typespec, IStringLocalizer<SharedResource> localizer,
            IRepositoryAsync<Specifications> repository)
        {
            _localizer = localizer;
            _typespec = typespec;
            _repository = repository;
        }
        public IActionResult getData(string code, int idBrand, int? idselectd)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var idtype = _typespec.GetByCode(code.ToLower()).Id;
                var allUsersExceptCurrentUser = from d in _repository.Entities.Where(m => m.idTypeSpecifications == idtype) select new { id = d.Id, text = d.Name, selected = d.Id == idselectd };
                var json = allUsersExceptCurrentUser.ToArray();
                var data = JsonConvert.SerializeObject(allUsersExceptCurrentUser);
                return Content(data);
            }

            return Content("");
        }
        [HttpPost]
        public IActionResult getDataArr(string code, int?[] idBrand, int?[] idselectd)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var idtype = _typespec.GetByCode(code.ToLower()).Id;
                var allUsersExceptCurrentUser = from d in _repository.Entities.Where(m => m.idTypeSpecifications == idtype) select new { id = d.Id, text = d.Name, selected = idselectd.Contains(d.Id) };
                var json = allUsersExceptCurrentUser.ToArray();
                var data = JsonConvert.SerializeObject(allUsersExceptCurrentUser);
                return Content(data);
            }
            return Content("");
        }
        [HttpPost]
        public IActionResult getDataSelectMuti(string code, int idBrand, int[] idselectd)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var idtype = _typespec.GetByCode(code.ToLower()).Id;
                var allUsersExceptCurrentUser = from d in _repository.Entities.Where(m => m.idTypeSpecifications == idtype) select new { id = d.Id, text = d.Name, selected = idselectd.Contains(d.Id) };
                var json = allUsersExceptCurrentUser.ToArray();
                var data = JsonConvert.SerializeObject(allUsersExceptCurrentUser);
                return Content(data);
            }

            return Content("");
        }
        // GET: SpecificationssController
        [Breadcrumb("Danh sách thông số kỹ thuật")]
        [Authorize(Policy = "specification.list")]
        public async Task<IActionResult> Index(int IdType = 0, int idbrand = 0)
        {
            _logger.LogInformation(User.Identity.Name + "--> Specification index");
            SpecificationsViewModel model = new SpecificationsViewModel();
            var data = await _mediator.Send(new GetAllTypeSpecificationsCacheQuery());
            if (data.Succeeded)
            {
                model.idTypeSpecifications = IdType;
                model.listtype = data.Data;
            }
            return View(model);
        }

        public async Task<IActionResult> LoadAll(int IdType)
        {
            try
            {
                var response = await _mediator.Send(new GetAllSpecificationsQuery(IdType) { });
                if (response.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                    return new JsonResult(new { isValid = true, html = html });

                }
                return new JsonResult(new { isValid = false });
            }
            catch (Exception e)
            {
                _logger.LogError(default, e);
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = true });
            }
        }
        // GET: SpecificationssController/Details/5
        public ActionResult Details(int id)
        {

            return View();
        }

        // GET: SpecificationssController/Create
        [Authorize(Policy = "specification.create")]
        public async Task<ActionResult> CreateOrEdit(int IdType, int? idBrand, int id = 0)
        {
            _logger.LogInformation(User.Identity.Name + "--> Specification CreateOrEdit");

            if (id == 0)
            {
                var model = new SpecificationsViewModel
                {

                    idTypeSpecifications = IdType
                };
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", model);
                return new JsonResult(new { isValid = true, html = html });
                //  return View("_Create", model);
            }
            else
            {
                var getid = await _mediator.Send(new GetByIdSpecificationQuery() { Id = id });
                if (getid.Succeeded)
                {
                    //var model = new SpecificationsViewModel
                    //{
                    //    idTypeSpecifications = IdType,
                    //    Name = getid.Data.Name,
                    //    Id = id
                    //};
                    var model = _mapper.Map<SpecificationsViewModel>(getid.Data);
                    var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", model);
                    return new JsonResult(new { isValid = true, html = html });
                    // return View("_Edit", model);
                }
                _notify.Error(getid.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(SpecificationsViewModel collection)
        {
            //var collection = new Specifications
            //{
            //    Name = model.Name,
            //    Id = model.Id,
            //    idTypeSpecifications = model.idTypeSpecifications
            //};

            try
            {
                if (collection.Id == 0)
                {
                    var createProductCommand = _mapper.Map<CreateSpecificationsCommand>(collection);
                    var result = await _mediator.Send(createProductCommand);
                    if (result.Succeeded)
                    {
                        collection.Id = result.Data;
                        _notify.Success(_localizer.GetString("AddOk").Value);
                        // return new JsonResult(new { isValid = true, html = string.Empty });
                    }
                    else
                    {
                        _notify.Error(result.Message);
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }
                }
                else
                {
                    var updateProductCommand = _mapper.Map<UpdateSpecificationsCommand>(collection);
                    var result = await _mediator.Send(updateProductCommand);
                    if (result.Succeeded)
                    {
                        _notify.Success(_localizer.GetString("EditOk").Value);
                        // return new JsonResult(new { isValid = true, html = string.Empty });
                    }
                    else
                    {
                        _notify.Error(result.Message);
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }
                }
                var listdata = await _mediator.Send(new GetAllSpecificationsQuery(collection.idTypeSpecifications));
                if (listdata.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", listdata.Data);
                    return new JsonResult(new { isValid = true, html = html, loadDataTable = true });
                }
                _notify.Error(_localizer.GetString("LoadDataErr").Value);
                return new JsonResult(new { isValid = false, html = string.Empty });

            }
            catch (Exception e)
            {
                _notify.Error(e.ToString());
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
        }

        // POST: SpecificationssController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SpecificationssController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SpecificationssController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SpecificationssController/Delete/5
        [HttpPost]
        [Authorize(Policy = "specification.delete")]
        public async Task<ActionResult> Delete(int Id, int IdType)
        {
            _logger.LogInformation(User.Identity.Name + "--> Specification Delete");
            var delete = await _mediator.Send(new DeleteSpecificationsCommand() { Id = Id });
            if (delete.Succeeded)
            {

                _notify.Success(_localizer.GetString("DeleteOk").Value);
                var response = await _mediator.Send(new GetAllSpecificationsQuery(IdType));
                if (response.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                    return new JsonResult(new { isValid = true, html = html, loadDataTable = true });
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
    }
}
