using Application.Constants;
using Application.Interfaces.Repositories;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class SignInManagerRepository : ISignInManagerRepository<ApplicationUser>
    {

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
        public SignInManagerRepository(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor)
        {
            //if (claimsFactory == null)
            //{
            //    throw new ArgumentNullException(nameof(claimsFactory));
            //}
            //ClaimsFactory = claimsFactory;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
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
