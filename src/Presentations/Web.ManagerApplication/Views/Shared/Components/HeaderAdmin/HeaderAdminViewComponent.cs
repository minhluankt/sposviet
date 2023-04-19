using Microsoft.AspNetCore.Mvc;

namespace Web.ManagerApplication.Views.Shared.Components.HeaderAdmin
{
    public class HeaderAdminViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
