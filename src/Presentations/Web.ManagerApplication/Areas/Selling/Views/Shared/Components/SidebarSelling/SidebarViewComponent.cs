using Microsoft.AspNetCore.Mvc;

namespace Web.ManagerApplication.Areas.Selling.Views.Shared.Component.Sidebar
{
    public class SidebarSellingViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}