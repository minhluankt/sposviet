using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.SadminEIMS.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class DeactivatedModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}