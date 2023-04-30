using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : BaseController<HomeController>
    {
        [Authorize(Policy = "admin.index")]
        public IActionResult Index()
        {
            _logger.LogInformation(User.Identity.Name + "--> Home index");
            return View();
        }
    }
}
