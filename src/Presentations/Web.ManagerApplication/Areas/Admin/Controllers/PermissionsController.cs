using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ManagerApplication.Abstractions;
using Domain.Entities;
using Application.Constants;
using System.Threading;
using Application.Features.Permissions.Commands;
using Application.Features.Permissions.Query;
using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Authorization;
using Web.ManagerApplication.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Infrastructure.Identity.Models;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    //[Authorize()]
    [Area("Admin")]

    public class PermissionsController : BaseController<PermissionsController>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IPermissionRepository<Domain.Entities.Permission> _permission;
        private readonly IUnitOfWork _IUnitOfWork;
        private readonly IRepositoryAsync<Domain.Entities.Permission> _repository;
        public PermissionsController(IPermissionRepository<Domain.Entities.Permission> permission,
            IUnitOfWork IUnitOfWork,
            RoleManager<ApplicationRole> roleManager,
            IRepositoryAsync<Domain.Entities.Permission> repository)
        {
            _roleManager = roleManager;
            _permission = permission;
            _repository = repository;
            _IUnitOfWork = IUnitOfWork;
        }




        // GET: PermissionsController
        [Breadcrumb("Danh sách", AreaName = "Admin")]
        [Authorize(Policy = "permissions.list")]
        public ActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> LoadAll()
        {
            try
            {
                var response = await _mediator.Send(new GetAllPermissionsCacheQuery());
                if (response.Succeeded)
                {
                    return PartialView("_ViewAll", response.Data);
                }
                return default;
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return default;
            }
        }

        // GET: PermissionsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PermissionsController/Create


        // POST: PermissionsController/Create
        [Authorize(Policy = "permissions.edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(Domain.Entities.Permission collection)
        {
            try
            {
                if (collection.Id == 0)
                {
                    var createProductCommand = _mapper.Map<CreatePermissionCommand>(collection);
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
                    var updateProductCommand = _mapper.Map<UpdatePermissionCommand>(collection);
                    var result = await _mediator.Send(updateProductCommand);
                    if (result.Succeeded) _notify.Success(HeperConstantss.SUS006);

                }
                var response = await _mediator.Send(new GetAllPermissionsCacheQuery());
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

        // GET: PermissionsController/Edit/5
        [Authorize(Policy = "permissions.create")]
        public async Task<ActionResult> Create()
        {
            var html = await _viewRenderer.RenderViewToStringAsync("_Create", new Domain.Entities.Permission());
            return new JsonResult(new { isValid = true, html = html });
        }
        [Authorize(Policy = "permissions.edit")]
        public async Task<ActionResult> Edit(int id)
        {
            var data = await _mediator.Send(new GetByIdPermissionQuery() { Id = id });
            if (data.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_Edit", data.Data);
                return new JsonResult(new { isValid = true, html = html });
            }
            return new JsonResult(new { isValid = false, html = string.Empty });
        }

        // POST: PermissionsController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit(Domain.Entities.Permission collection)
        //{
        //    try
        //    {
        //        await _permission.UpdateOnlyAsync(collection);
        //        await _IUnitOfWork.Commit(new CancellationToken());
        //        _notify.Success(HeperConstants.AddOk);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception e)
        //    {
        //        _notify.Error(e.ToString());
        //        return View(collection);
        //    }
        //}

        // POST: PermissionsController/Delete/5
        [Authorize(Policy = "permissions.delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var deleteCommand = await _mediator.Send(new DeletePermissionCommand { Id = id });
                if (deleteCommand.Succeeded)
                {
                    _notify.Success($"Permissions with Id {id} Deleted success.");
                    var response = await _mediator.Send(new GetAllPermissionsCacheQuery());
                    if (response.Succeeded)
                    {
                        //var viewModel = _mapper.Map<List<ProductViewModel>>(response.Data);
                        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                        return new JsonResult(new { isValid = true, html = html });
                    }
                    else
                    {
                        _notify.Error(response.Message);
                        return null;
                    }
                }
                else
                {
                    _notify.Error(deleteCommand.Message);
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
