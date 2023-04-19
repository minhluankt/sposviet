using Microsoft.AspNetCore.Mvc;

namespace Web.ManagerApplication.Views.Shared.Components.FooterAdmin
{
    public class FooterAdminViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
