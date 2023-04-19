using Microsoft.AspNetCore.Mvc;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/file-manager")]
    public class FileManagerController : Controller
    {
       // https://xuanthulab.net/asp-net-core-mvc-tich-hop-trinh-quan-ly-file-vao-website.html
      
        public IActionResult Index()
        {
            return View();
        }
    }
}
