using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Application.Constants;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Application.Interfaces.Repositories;
using Application.Enums;
using Model;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace Application.Providers
{
    public interface IUserManager
    {
        Task SignIn(HttpContext httpContext, CookieCustomerUser user, bool isPersistent = false);
        SignInResult PasswordSignIn(Customer user, string pass, bool isPersistent = false);
        Task SignOut(HttpContext httpContext = null);
        Task RefreshSignInAsync(CookieCustomerUser user, HttpContext httpContext);
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null);
        Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
        Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null);
    }

   

    public class UserManager : IUserManager
    {
        private readonly ILogger<UserManager> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private HttpContext _context;
        private IAuthenticationSchemeProvider _schemes;
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
        public UserManager(IAuthenticationSchemeProvider schemes, ILogger<UserManager> logger,


            IHttpContextAccessor contextAccessor)
        {
        
            _contextAccessor = contextAccessor;
            _logger = logger;
            _schemes = schemes;
        }
        public async Task RefreshSignInAsync(CookieCustomerUser user, HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationCustomer.AuthenticationScheme);
            await SignIn(httpContext, user,true);
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

        public async Task SignIn(HttpContext httpContext, CookieCustomerUser user, bool isPersistent = false)
        {
            string authenticationScheme = CookieAuthenticationCustomer.AuthenticationScheme;

            // Generate Claims from DbEntity
            var claims = GetUserClaims(user);

            // Add Additional Claims from the Context
            // which might be useful
            // claims.Add(httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name));

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, authenticationScheme);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                // AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.
                //ExpiresUtc = DateTimeOffset.Now.AddDays(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.
                IsPersistent = isPersistent,
                // Whether the authentication session is persisted across 
                // multiple requests. Required when setting the 
                // ExpireTimeSpan option of CookieAuthenticationOptions 
                // set with AddCookie. Also required when setting 
                // ExpiresUtc.
                // IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.
                //RedirectUri = "~/Account/Login"
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await httpContext.SignInAsync(authenticationScheme, claimsPrincipal, authProperties);
        }

        public async Task SignOut(HttpContext httpContext = null)
        {
            if (httpContext==null)
            {
                await Context.SignOutAsync(CookieAuthenticationCustomer.AuthenticationScheme);
            }
            else
            {
                await httpContext.SignOutAsync(CookieAuthenticationCustomer.AuthenticationScheme);
            }
        }
      

        private List<Claim> GetUserClaims(CookieCustomerUser user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            if (!string.IsNullOrEmpty(user.EmailAddress))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.EmailAddress));
            }

            claims.Add(new Claim("Typeuser", user.TypeUser.ToString()));
            if (!string.IsNullOrEmpty(user.Image))
            {
                claims.Add(new Claim(ConfigCustomerLogin.Image, user.Image));
            }
            return claims;
        }

        public async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            var schemes = await _schemes.GetAllSchemesAsync();
            return schemes.Where(s => !string.IsNullOrEmpty(s.DisplayName));
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null)
        {
            var auth = await Context.AuthenticateAsync(IdentityConstants.ExternalScheme);
            var items = auth?.Properties?.Items;
            if (auth?.Principal == null || items == null || !items.ContainsKey(LoginProviderKey))
            {
                return null;
            }

            if (expectedXsrf != null)
            {
                if (!items.ContainsKey(XsrfKey))
                {
                    return null;
                }
                var userId = items[XsrfKey] as string;
                if (userId != expectedXsrf)
                {
                    return null;
                }
            }

            var providerKey = auth.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var provider = items[LoginProviderKey] as string;
            if (providerKey == null || provider == null)
            {
                return null;
            }

            var providerDisplayName = (await GetExternalAuthenticationSchemesAsync()).FirstOrDefault(p => p.Name == provider)?.DisplayName
                                      ?? provider;
            return new ExternalLoginInfo(auth.Principal, provider, providerKey, providerDisplayName)
            {
                AuthenticationTokens = auth.Properties.GetTokens()
            };
        }

        public SignInResult PasswordSignIn(Customer user,string pass, bool isPersistent = false)
        {
            try
            {
                if (user.Password == Hasher.GenerateHash(pass, user.Salt))
                {
                    return SignInResult.Success;
                }

                return SignInResult.Failed;
            }
            catch (Exception e)
            {
                _logger.LogInformation("Lỗi PasswordSignInAsync");
                _logger.LogError(e.ToString());
                return SignInResult.Failed;
            }
          
        }

       
    }
}
