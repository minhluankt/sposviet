using Application.Constants;
using Application.Enums;
using Application.Features.Permissions.Query;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using Domain.Identity;
using Domain.ViewModel;
using Domain.Identity;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Hepers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Infrastructure.Repositories
{
    public class SignInManagerRepository : ISignInManagerRepository<ApplicationUser>
    {
        private readonly IMapper _mapper;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IPermissionCacheRepository _permissionCache;
        //  public  IUserClaimsPrincipalFactory<ApplicationUser> ClaimsFactory { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private HttpContext _context;
        private const string LoginProviderKey = "LoginProvider";
        private const string XsrfKey = "XsrfId";
        public HttpContext Context
        {
            get
            {
                var context = _context ?? _contextAccessor?.HttpContext;
                if (context == null)
                {
                    throw new InvalidOperationException("HttpContext must not be null.");
                }
                return context;
            }
            set
            {
                _context = value;
            }
        }
        public SignInManagerRepository(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IHttpContextAccessor contextAccessor, IMapper mapper,
            IPermissionCacheRepository permissionCache)
        {
            //if (claimsFactory == null)
            //{
            //    throw new ArgumentNullException(nameof(claimsFactory));
            //}
            //ClaimsFactory = claimsFactory;
            _userManager = userManager;
            _roleManager = roleManager;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _permissionCache = permissionCache;
        }
        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null)
        {
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items[LoginProviderKey] = provider;
            if (userId != null)
            {
                properties.Items[XsrfKey] = userId;
            }
            return properties;
        }
        public async Task RefreshSignInAsync(ApplicationUser user, string Scheme = CookieAuthenticationDefaults.AuthenticationScheme)
        {
            if (Scheme == "")
            {
                var auth = await Context.AuthenticateAsync();
                // var auth = await Context.AuthenticateAsync();
                var authenticationMethod = auth?.Principal?.FindFirstValue(ClaimTypes.AuthenticationMethod);
                await SignInAsync(user, auth?.Properties, authenticationMethod);
            }
            else
            {
                var auth = await Context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                // var auth = await Context.AuthenticateAsync();
                var authenticationMethod = auth?.Principal?.FindFirstValue(ClaimTypes.AuthenticationMethod);
                await SignInAsync(user, auth?.Properties, authenticationMethod);
            }

        }
        public virtual Task SignInAsync(ApplicationUser user, bool isPersistent, string authenticationMethod = null)
        {
            return SignInAsync(user, new AuthenticationProperties { IsPersistent = isPersistent }, authenticationMethod);
        }
        private async Task SignInAsync(ApplicationUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            await Context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //await Context.SignOutAsync();
            // var userPrincipal = await CreateUserPrincipalAsync(user);

            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.Id),
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim(ClaimUser.FULLNAME, user.FullName),
                                new Claim(ClaimUser.COMID, user.ComId.ToString()),
                                new Claim(ClaimUser.IDDICHVU, ((int)user.IdDichVu).ToString())
                            };
            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            var userRoles = await _userManager.GetRolesAsync(user);// Custom helper method to get list of user roles


            //xử lý quyền cho user check xem là gì 

            var getsup = userRoles.Any(x => x.ToLower() == PermissionUser.admin);
            if (user.IsStoreOwner || getsup)
            {
                claims.Add(new Claim(ClaimUser.IsAdmin, "1"));
            }

            var roles = await _roleManager.Roles.Where(x => x.ComId == user.ComId && userRoles.Contains(x.Name)).ToListAsync();
            var response = await _permissionCache.GetCachedListAsync();
            if (response.Count()>0)
            {
                var Claimsnew = new List<Claim>();
                foreach (var item in roles)
                {
                    if (await _userManager.IsInRoleAsync(user, item.Name))
                    {
                        var role = await _roleManager.FindByIdAsync(item.Id);
                        if (role!=null)
                        {
                            Claimsnew.AddRange(await _roleManager.GetClaimsAsync(role));
                        }
                       
                    }
                }
                var model = new PermissionViewModel();
                var allPermissions = new List<RoleClaimsViewModel>();
                allPermissions.GetAllPermissions(response);

                var claimsModel = _mapper.Map<List<RoleClaimsViewModel>>(Claimsnew);
                var allClaimValues = allPermissions.Select(a => a.Value).ToList();
                var roleClaimValues = claimsModel.Select(a => a.Value).ToList();
                var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
                List<string> permissions = new List<string>();
                foreach (var permission in allPermissions)
                {
                    if (authorizedClaims.Any(a => a == permission.Value))
                    {
                        permissions.Add(permission.Value);
                    }
                }
                foreach (var item in permissions.Distinct())
                {
                    if (item == PermissionUser.quanlyketoan && user.IdDichVu == EnumTypeProduct.AMTHUC)
                    {
                        claims.Add(new Claim(ClaimUser.IsKeToan, "1"));
                    }
                    else if (item == PermissionUser.phucvuthanhtoan && user.IdDichVu == EnumTypeProduct.AMTHUC)
                    {
                        claims.Add(new Claim(ClaimUser.IsPhucVuPayment, "1"));
                    } 
                    else if (item == PermissionUser.thunganpos && user.IdDichVu == EnumTypeProduct.AMTHUC)
                    {
                        claims.Add(new Claim(ClaimUser.IsThuNgan, "1"));
                    }
                    else if (item == PermissionUser.thunganSaleRetail && (user.IdDichVu == EnumTypeProduct.TAPHOA_SIEUTHI || user.IdDichVu == EnumTypeProduct.BAN_LE || user.IdDichVu == EnumTypeProduct.THOITRANG))
                    {
                        claims.Add(new Claim(ClaimUser.IsThuNgan, "1"));
                    }
                    else if (item == PermissionUser.beppos && user.IdDichVu == EnumTypeProduct.AMTHUC)
                    {
                        claims.Add(new Claim(ClaimUser.IsBep, "1"));
                    }
                    else if (item == PermissionUser.nhanvienphucvu)
                    {
                        claims.Add(new Claim(ClaimUser.IsPhucVu, "1"));
                    }
                }

            }


            // Add Role claims
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
            // Review: should we guard against CreateUserPrincipal returning null?
            if (authenticationMethod != null)
            {
                //userPrincipal.Identities.First().AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
                userPrincipal.Identities.Where(m => m.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme).First().AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            }
            await Context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                authenticationProperties ?? new AuthenticationProperties());
        }

        public async Task SignOutAsync(string schema = "")//string schema = CookieAuthenticationDefaults.AuthenticationScheme
        {
            if (!string.IsNullOrEmpty(schema))
            {
                await Context.SignOutAsync(schema);
            }
            await Context.SignOutAsync();
        }
        // public virtual async Task<ClaimsPrincipal> CreateUserPrincipalAsync(ApplicationUser user) => await ClaimsFactory.CreateAsync(user);


    }
}
