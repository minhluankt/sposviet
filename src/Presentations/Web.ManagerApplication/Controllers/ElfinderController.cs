using Microsoft.AspNetCore.Mvc;

namespace Web.ManagerApplication.Controllers
{
    [Route("Elfinder")]
    public class ElfinderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("browse")]
        public IActionResult Browse(int ckeditor = 1, string id = "")
        {
            ViewBag.ckeditor = ckeditor;
            ViewBag.idelement = id;
            return View();
        }
    }
}
