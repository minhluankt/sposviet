using Application.Interfaces.Repositories;
using Domain.Identity;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Areas.Admin.Models;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserRoleController : BaseController<UserRoleController>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserManagerRepository<ApplicationUser> _usermanegerRepository;
        private readonly ISignInManagerRepository<ApplicationUser> _signInManagerrRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserRoleController(UserManager<ApplicationUser> userManager,
            IUserManagerRepository<ApplicationUser> usermanegerRepository,
            ISignInManagerRepository<ApplicationUser> signInManagerrRepository,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _usermanegerRepository = usermanegerRepository;
            _signInManagerrRepository = signInManagerrRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [Breadcrumb("Phân quyền Users", AreaName = "Admin")]
        public async Task<IActionResult> Index(string userId)
        {
            var viewModel = new List<UserRolesViewModel>();
            var user = await _userManager.FindByIdAsync(userId);
            ViewData["Title"] = $"{user.UserName} - Roles";
            ViewData["Caption"] = $"Manage {user.Email}'s Roles.";
            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                viewModel.Add(userRolesViewModel);
            }
            var model = new ManageUserRolesViewModel()
            {
                UserName = user.UserName,
                Name = $"{user.FirstName} {user.LastName}",
                UserId = userId,
                UserRoles = viewModel
            };

            return View(model);
        }

        public async Task<IActionResult> Update(string id, ManageUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            result = await _userManager.AddToRolesAsync(user, model.UserRoles.Where(x => x.Selected).Select(y => y.RoleName));

            // var currentUser = await _userManager.GetUserAsync(User);
            var currentUser = await _usermanegerRepository.GetUserAsync(User);
            if (currentUser.Id == id)
            {
                await _signInManagerrRepository.RefreshSignInAsync(currentUser);
            }
            //await _signInManager.RefreshSignInAsync(currentUser);


            await Infrastructure.Infrastructure.Identity.Seeds.DefaultSuperAdminUser.SeedAsync(_userManager, _roleManager);
            _notify.Success($"Updated Roles for User '{user.Email}'");
            return RedirectToAction("Index", new { userId = id });
        }
    }
}
