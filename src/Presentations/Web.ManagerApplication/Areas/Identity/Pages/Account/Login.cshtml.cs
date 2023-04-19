// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Application.Constants;
using Application.Enums;
using Application.Features.ActivityLog.Commands.AddLog;
using Application.Features.Permissions.Query;
using Application.Interfaces.Repositories;
using AutoMapper;
using Infrastructure.Infrastructure.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.WsTrust;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.Claims;
using Telegram.Bot.Types;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Areas.Admin.Models;
using Web.ManagerApplication.Helper;

namespace Web.ManagerApplication.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : BasePageModel<LoginModel>
    {
        private readonly IUserManagerRepository<ApplicationUser> _userManagerRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ISignInManagerRepository<ApplicationUser> _signInManagerRepository;
        private readonly ILogger<LoginModel> _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger, RoleManager<ApplicationRole> roleManager, IMapper mapper,
            ISignInManagerRepository<ApplicationUser> signInManagerRepository, IUserManagerRepository<ApplicationUser> userManagerRepository,
            UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            _userManagerRepository = userManagerRepository;
            _signInManagerRepository = signInManagerRepository;
            _mapper = mapper;
            _roleManager = roleManager;
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
           // [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            public string UserName { get; set; }

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
            await _signInManagerRepository.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
                if (IsValidEmail(Input.UserName))
                {
                    var userCheck = await _userManager.FindByEmailAsync(Input.UserName);
                    if (userCheck != null)
                    {
                        userName = userCheck.UserName;
                    }
                }
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        return RedirectToPage("./Deactivated");
                    }
                    else if (!user.EmailConfirmed && user.Level != 2)
                    {
                        _notyf.Error("Email Not Confirmed.");
                        ModelState.AddModelError(string.Empty, "Email Not Confirmed.");
                        return Page();
                    }
                    else
                    {
                        // var result = await _signInManager.PasswordSignInAsync(userName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                        var result = await _signInManager.CheckPasswordSignInAsync(user, Input.Password, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
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
                            //else
                            //{
                            //    _notyf.Error("Invalid login attempt.");
                            //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                            //    return Page();
                            //}
                            await _signInManagerRepository.SignOutAsync(CookieAuthenticationCustomer.AuthenticationScheme);
                            await _signInManagerRepository.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                            await _signInManagerRepository.RefreshSignInAsync(user);

                            await _mediator.Send(new AddActivityLogCommand() { userId = user.Id, Action = "Logged In" });
                            _logger.LogInformation("User logged in.");
                            _notyf.Success($"Đăng nhập thành công {userName}.");

                            if (user.IdDichVu == EnumTypeProduct.AMTHUC 
                                || user.IdDichVu == EnumTypeProduct.BAN_LE 
                                || user.IdDichVu == EnumTypeProduct.TAPHOA_SIEUTHI 
                                || user.IdDichVu == EnumTypeProduct.VATLIEU_XAYDUNG 
                                || user.IdDichVu == EnumTypeProduct.HOTEL_BIDA
                                || user.IdDichVu == EnumTypeProduct.THOITRANG)
                            {
                                var getrolebyuser = await _userManager.GetRolesAsync(user);
                                var getsup = getrolebyuser.Any(x=>x.ToLower()== PermissionUser.admin);
                                if (user.IsStoreOwner || getsup)
                                {
                                    returnUrl = "/Selling/Dashboard";
                                    return LocalRedirect(returnUrl);
                                }
                                var response = await _mediator.Send(new GetAllPermissionsCacheQuery());
                                if (response.Succeeded)
                                {
                                    var roles = await _roleManager.Roles.Where(x => x.ComId == user.ComId).ToListAsync();
                                    var viewModel = new List<UserRolesViewModel>();
                                    var Claims = new List<Claim>();
                                    foreach (var item in roles)
                                    {
                                        if (await _userManager.IsInRoleAsync(user, item.Name))
                                        {
                                            var role = await _roleManager.FindByIdAsync(item.Id);
                                            Claims.AddRange(await _roleManager.GetClaimsAsync(role));
                                            //var userRolesViewModel = new UserRolesViewModel
                                            //{
                                            //    RoleName = item.Name,
                                            //    RoleId = item.Id
                                            //};
                                            //viewModel.Add(userRolesViewModel);
                                        }
                                    }
                                    var model = new PermissionViewModel();
                                    var allPermissions = new List<RoleClaimsViewModel>();
                                    allPermissions.GetAllPermissions(response.Data);

                                    var claimsModel = _mapper.Map<List<RoleClaimsViewModel>>(Claims);
                                    var allClaimValues = allPermissions.Select(a => a.Value).ToList();
                                    var roleClaimValues = claimsModel.Select(a => a.Value).ToList();
                                    var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
                                    List<string> permissions = new List<string>();
                                    foreach (var permission in allPermissions)
                                    {
                                        if (authorizedClaims.Any(a => a == permission.Value))
                                        {
                                            //permission.Selected = true;
                                            permissions.Add(permission.Value);
                                        }
                                    }
                                    foreach (var item in permissions)
                                    {
                                        if (item == PermissionUser.quanlyketoan)
                                        {
                                            returnUrl = "/Selling/Dashboard";
                                            break;
                                        }
                                        else if (item== PermissionUser.thunganpos && user.IdDichVu == EnumTypeProduct.AMTHUC)
                                        {
                                            returnUrl = "/Selling/Pos";
                                            break;
                                        }
                                        else if (item == PermissionUser.thunganSaleRetail && (user.IdDichVu == EnumTypeProduct.TAPHOA_SIEUTHI || user.IdDichVu == EnumTypeProduct.BAN_LE || user.IdDichVu == EnumTypeProduct.THOITRANG))
                                        {
                                            returnUrl = "/Selling/SaleRetail";
                                            break;
                                        }
                                        else if (item == PermissionUser.nhanvienphucvu && (user.IdDichVu == EnumTypeProduct.TAPHOA_SIEUTHI || user.IdDichVu == EnumTypeProduct.BAN_LE || user.IdDichVu == EnumTypeProduct.THOITRANG))
                                        {
                                            returnUrl = "/";
                                            break;
                                        }
                                    }

                                }
                                //var lstrole = _roleManager.Roles.Where(x => x.ComId == user.ComId).ToList();
                                //foreach (var role in lstrole)
                                //{
                                //    if (await _userManager.IsInRoleAsync(user, PermissionUser.quanlyketoan))
                                //    {
                                //        returnUrl = "/Selling/Dashboard";
                                //        break;
                                //    }
                                //    if (await _userManager.IsInRoleAsync(user, PermissionUser.thunganpos) && user.IdDichVu == EnumTypeProduct.AMTHUC)
                                //    {
                                //        returnUrl = "/Selling/Pos";
                                //        break;
                                //    }
                                //    else if (await _userManager.IsInRoleAsync(user, PermissionUser.thunganSaleRetail) && (user.IdDichVu == EnumTypeProduct.TAPHOA_SIEUTHI || user.IdDichVu == EnumTypeProduct.BAN_LE || user.IdDichVu == EnumTypeProduct.THOITRANG))
                                //    {
                                //        returnUrl = "/Selling/SaleRetail";
                                //        break;
                                //    }
                                //    if (await _userManager.IsInRoleAsync(user, PermissionUser.nhanvienphucvu) && (user.IdDichVu == EnumTypeProduct.TAPHOA_SIEUTHI || user.IdDichVu == EnumTypeProduct.BAN_LE || user.IdDichVu == EnumTypeProduct.THOITRANG))
                                //    {
                                //        returnUrl = "/";
                                //        break;
                                //    }
                                //}

                                return LocalRedirect(returnUrl);
                            }

                            if (string.IsNullOrEmpty(returnUrl.Replace("/", "")))
                            {
                                returnUrl = "/";
                            }
                            return LocalRedirect(returnUrl);
                        }
                        await _mediator.Send(new AddActivityLogCommand() { userId = user.Id, Action = "Log-In Failed" });
                       
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

        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
