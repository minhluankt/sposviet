using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface ISignInManagerRepository<T>
    {
        Task RefreshSignInAsync(T user, string Scheme = CookieAuthenticationDefaults.AuthenticationScheme);
        Task SignOutAsync(string schema = "");//= CookieAuthenticationDefaults.AuthenticationScheme

        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null);

    }
}
