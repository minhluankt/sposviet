// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Application.Features.ActivityLog.Commands.AddLog;
using Application.Interfaces.Repositories;
using Infrastructure.Infrastructure.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Web.ManagerCompany.Abstractions;

namespace Web.ManagerCompany.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : BasePageModel<LoginModel>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ISignInManagerRepository<ApplicationUser> _signInManagerRepository;
        private readonly ILogger<LoginModel> _logger;
        private readonly IMediator _mediator;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            ISignInManagerRepository<ApplicationUser> signInManagerRepository,
            UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            _signInManagerRepository = signInManagerRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            public string UserName { get; set; }

            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var userName = Input.UserName;
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        return RedirectToPage("./Deactivated");
                    }
                    else if (!user.EmailConfirmed)
                    {
                        _notyf.Error("Email Not Confirmed.");
                        ModelState.AddModelError(string.Empty, "Email Not Confirmed.");
                        return Page();
                    }
                    else
                    {
                        var result = await _signInManager.PasswordSignInAsync(userName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                        //var result = await _signInManager.CheckPasswordSignInAsync(user, Input.Password, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            await _signInManager.SignOutAsync();
                            await _signInManager.RefreshSignInAsync(user);

                            await _mediator.Send(new AddActivityLogCommand() { userId = user.Id, Action = "Logged In" });
                            _logger.LogInformation("User logged in.");
                            _notyf.Success($"Logged in as {userName}.");

                            if (string.IsNullOrEmpty(returnUrl.Replace("/", "")))
                            {
                                returnUrl = "/Home";
                            }
                            return LocalRedirect(returnUrl);
                        }
                        await _mediator.Send(new AddActivityLogCommand() { userId = user.Id, Action = "Log-In Failed" });
                        if (result.RequiresTwoFactor)
                        {
                            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                        }
                        if (result.IsLockedOut)
                        {
                            _notyf.Warning("User account locked out.");
                            _logger.LogWarning("User account locked out.");
                            return RedirectToPage("./Lockout");
                        }
                        else
                        {
                            _notyf.Error("Invalid login attempt.");
                            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                            return Page();
                        }
                    }
                }
                else
                {
                    _notyf.Error("Email / Username Not Found.");
                    ModelState.AddModelError(string.Empty, "Email / Username Not Found.");
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
