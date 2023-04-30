using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class ErrorController : Controller
    {
        [AllowAnonymous]
        public IActionResult ExpiredStort()
        {
            return View("Index");
        }
    }
}
