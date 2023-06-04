// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Application.Constants;
using Application.Features.ActivityLog.Commands.AddLog;
using Application.Interfaces.Repositories;
using Application.Interfaces.Shared;
using Domain.Identity;
using Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Identity.Pages.Account
{
    public class LogoutModel : BasePageModel<LogoutModel>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator _mediator;
        private readonly ISignInManagerRepository<ApplicationUser> _signInManagerRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IAuthenticatedUserService _userService;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
            ISignInManagerRepository<ApplicationUser> signInManagerRepository,
            ILogger<LogoutModel> logger, IMediator mediator, IAuthenticatedUserService userService)
        {
            _userManager = userManager;
            _signInManagerRepository = signInManagerRepository;
            _signInManager = signInManager;
            _logger = logger;
            _mediator = mediator;
            _userService = userService;
        }
        public async Task<IActionResult> OnGet(string returnUrl = null)
        {
            // var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            await _mediator.Send(new AddActivityLogCommand() { userId = _userService.UserId, Action = "Logged Out" });
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationCustomer.AuthenticationScheme);
            await _signInManagerRepository.SignOutAsync();

            _logger.LogInformation("User logged out in cutomer.");
            //if (currentUser.IdDichVu == EnumTypeProduct.AMTHUC)
            //{
            //    return RedirectToPage("/");
            //}
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage("/");
            }
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            await _mediator.Send(new AddActivityLogCommand() { userId = _userService.UserId, Action = "Logged Out" });
            await _signInManager.SignOutAsync();
            // await HttpContext.SignOutAsync(CookieAuthenticationCustomer.AuthenticationScheme);
            await _signInManagerRepository.SignOutAsync();
            _notyf.Information("User logged out.");
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
