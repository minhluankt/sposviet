using Application.Constants;
using Application.Enums;
using Application.Features.Banners.Query;
using Application.Features.CompanyInfo.Query;
using Application.Features.ConfigSystems.Query;
using Application.Features.NotificationNewsEmails.Commands;
using Application.Hepers;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Models;

namespace Web.ManagerApplication.Controllers
{
    [Authorize]
    public class HomeController : BaseController<HomeController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [Authorize(Policy = PermissionUser.nhanvienphucvu)]
        public async Task<IActionResult> IndexAsync()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (await _userManager.IsInRoleAsync(currentUser, Roles.SuperAdmin.ToString()) || await _userManager.IsInRoleAsync(currentUser, Roles.quanly.ToString()))
            {
                return Redirect("/Selling/Dashboard");
            }
            if (await _userManager.IsInRoleAsync(currentUser, Roles.SuperAdmin.ToString()) || await _userManager.IsInRoleAsync(currentUser, Roles.quanly.ToString()))
            {
                return Redirect("/OrderStaff");
            }
            return View();
        }
        public async Task<IActionResult> SendMailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR002));
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            var createProductCommand = new CreateNotificationNewsEmailCommand() { Email = email };
            var result = await _mediator.Send(createProductCommand);
            if (result.Succeeded)
            {
                _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS009));
                return new JsonResult(new { isValid = true, html = string.Empty });
            }
            else
            {
                _notify.Error(GeneralMess.ConvertStatusToString(result.Message));
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}