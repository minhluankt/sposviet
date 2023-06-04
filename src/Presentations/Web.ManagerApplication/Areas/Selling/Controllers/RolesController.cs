
using Application.Constants;
using Application.Features.Permissions.Query;
using Application.Hepers;
using Domain.Identity;
using Domain.ViewModel;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NStandard;
using SmartBreadcrumbs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Areas.Admin.Models;
using Web.ManagerApplication.Helper;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("selling")]

    public class RolesController : BaseController<RolesController>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: RolesController
        [Breadcrumb("Danh sách nhóm quyền", AreaName = "Admin")]
        [Authorize(Policy = "role.list")]
        public ActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> LoadAll()
        {
            var user = User.Identity.GetUserClaimLogin();
            var roles = await _roleManager.Roles.Where(x => x.ComId == user.ComId).ToListAsync();
            var model = _mapper.Map<IEnumerable<RoleViewModel>>(roles);
            return PartialView("_ViewAll", model);
        }


        [Breadcrumb("Quản lý nhóm quyền", AreaName = "Admin")]
        public async Task<ActionResult> GetRole(string roleId)
        {
            var model = new PermissionViewModel();
            var response = await _mediator.Send(new GetAllPermissionsCacheQuery());
            if (response.Succeeded)
            {
                var allPermissions = new List<RoleClaimsViewModel>();
                allPermissions.GetAllPermissions(response.Data);
                var role = await _roleManager.FindByIdAsync(roleId);
                model.RoleId = roleId;
                model.RoleName = role.Name;
                var claims = await _roleManager.GetClaimsAsync(role);
                var claimsModel = _mapper.Map<List<RoleClaimsViewModel>>(claims);
                var allClaimValues = allPermissions.Select(a => a.Value).ToList();
                var roleClaimValues = claimsModel.Select(a => a.Value).ToList();
                var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
                foreach (var permission in allPermissions)
                {
                    if (authorizedClaims.Any(a => a == permission.Value))
                    {
                        permission.Selected = true;
                    }
                    //if (authorizedClaims.Any(a => a == permission.Value))
                    //{
                    //    permission.Selected = true;
                    //}
                }
                model.RoleClaims = _mapper.Map<List<RoleClaimsViewModel>>(allPermissions);
                ViewData["Title"] = $"Permissions for {role.Name} Role";
                ViewData["Caption"] = $"Manage {role.Name} Role Permissions.";
                // _notify.Success($"Updated Claims / Permissions for Role {role.Name}");
            }

            return View(model);
        }
        [Authorize(Policy = "role.edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(PermissionViewModel model)
        {
            var selectedClaims = model.RoleClaims.Where(a => a.Selected).ToList();
            var response = await _mediator.Send(new GetAllPermissionsCacheQuery());
            var allPermissions = new List<RoleClaimsViewModel>();
            if (response.Succeeded)
            {
                allPermissions.GetAllPermissions(response.Data);
            }
            var allClaimValues = allPermissions.Select(a => a.Value).ToList();

            var role = await _roleManager.FindByIdAsync(model.RoleId);
            //Remove all Claims First
            var claims = await _roleManager.GetClaimsAsync(role);
            var claimsModel = _mapper.Map<List<RoleClaimsViewModel>>(claims);
            var roleClaimValues = claimsModel.Select(a => a.Value).ToList();
            // lấy ra các cái trùng giữa cái mới và cái đã có
            var listCoincide = selectedClaims.Select(a => a.Value).Intersect(roleClaimValues).ToList();
            // lấy ra cái đã bỏ đi khỏi list cái cũ để xóa
            var listNot = claims.Where(m => !listCoincide.Contains(m.Value)).ToList();
            // lấy ra cái mới không có trong list cũ để thêm vào (lấy ra cái k có trong cái list trùng listCoincide)
            var listNew = selectedClaims.Where(m => !listCoincide.Contains(m.Value)).ToList();
            foreach (var claim in listNot)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            foreach (var claim in listNew)
            {
                await _roleManager.AddPermissionClaim(role, claim.Value);
            }
            _notify.Success($"Cập nhậy quyền  {role.Name} thành công");
            //var user = User.Identity.GetUserClaimLogin();
            //await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction("getrole", new { roleId = model.RoleId });
        }
        // GET: RolesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RolesController/Create
        public async Task<IActionResult> OnGetCreateOrEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Create", new RoleViewModel()) });
            else
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null) _notify.Error("Unexpected Error. Role not found!");
                var roleviewModel = _mapper.Map<RoleViewModel>(role);
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Edit", roleviewModel) });
            }
        }
        [HttpPost]
        public async Task<IActionResult> OnPostCreateOrEdit(RoleViewModel role)
        {
            var userlogin = User.Identity.GetUserClaimLogin();
            if (ModelState.IsValid && role.Name.ToLower() != "superadmin")
            {
                if (string.IsNullOrEmpty(role.Id))
                {
                    var datamd = new ApplicationRole(role.Name) { Name = role.Name, Code = Common.ConvertToSlugNoSpage(role.Name.ToUpper()) };
                    datamd.ComId = userlogin.ComId;
                    var cr=   await _roleManager.CreateAsync(datamd);
                    if (cr.Succeeded)
                    {
                        _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS008));
                    }
                    else
                    {
                        _notify.Error(string.Join(',', cr.Errors.ToList()));
                        return new JsonResult(new { isValid = false });
                    }
                    
                }
                else
                {
                    var existingRole = await _roleManager.FindByIdAsync(role.Id);
                    existingRole.Name = role.Name;
                    existingRole.Code = Common.ConvertToSlugNoSpage(role.Name.ToUpper());
                    await _roleManager.UpdateAsync(existingRole);
                    _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                }

                var roles = await _roleManager.Roles.Where(x => x.ComId == userlogin.ComId).ToListAsync();
                var mappedRoles = _mapper.Map<IEnumerable<RoleViewModel>>(roles);
                var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", mappedRoles);
                return new JsonResult(new { isValid = true, html = html });
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync<RoleViewModel>("_CreateOrEdit", role);
                return new JsonResult(new { isValid = false, html = html });
            }
        }
        // POST: RolesController/Create
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

        // GET: RolesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RolesController/Edit/5
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
        [Authorize(Policy = "role.delete")]
        [HttpPost]
        public async Task<JsonResult> OnPostDelete(string id)
        {
            var userlogin = User.Identity.GetUserClaimLogin();
            var existingRole = await _roleManager.FindByIdAsync(id);
            if (existingRole.Name != "SuperAdmin" && existingRole.Name != "Basic")
            {
                //TODO Check if Any Users already uses this Role
                bool roleIsNotUsed = true;
                var allUsers = await _userManager.Users.ToListAsync();
                foreach (var user in allUsers)
                {
                    if (await _userManager.IsInRoleAsync(user, existingRole.Name))
                    {
                        roleIsNotUsed = false;
                    }
                }
                if (roleIsNotUsed)
                {
                    await _roleManager.DeleteAsync(existingRole);
                    _notify.Success($"Role {existingRole.Name} deleted.");
                }
                else
                {
                    _notify.Error("Quyền này đã được sử dụng cho nhân viên bạn không được phép xóa");
                }
            }
            else
            {
                _notify.Error($"Bạn không được xóa quyền SuperAdmin {existingRole.Name} Role.");
            }
            var roles = await _roleManager.Roles.Where(x=>x.ComId== userlogin.ComId).ToListAsync();
            var mappedRoles = _mapper.Map<IEnumerable<RoleViewModel>>(roles);
            var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", mappedRoles);
            return new JsonResult(new { isValid = true, html = html });
        }
    }
}
