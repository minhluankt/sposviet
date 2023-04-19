using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Constants
{
    public static class CookieAuthenticationCustomer
    {
        public const string AuthenticationScheme = "CookiesCustomer";
        public static readonly string ReturnUrlParameter;
    } 
    public static class CookieAuthenticationExternal
    {
        public const string AuthenticationScheme = "Cookies.External";
        public static readonly string ReturnUrlParameter;
    }
    public class IdentityCookieConstants
    {
        public const string IdentityApplicationScheme = "IdentityCookies";
        public static readonly string ReturnUrlParameter;
    }
}
