using Microsoft.AspNetCore.Mvc;

namespace Web.ManagerApplication.Areas.Admin.Views.Shared.Component.Sidebar
{
    public class SidebarSellingViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}