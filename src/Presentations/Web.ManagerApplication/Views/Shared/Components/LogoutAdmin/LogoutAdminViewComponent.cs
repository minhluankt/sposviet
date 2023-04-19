using Microsoft.AspNetCore.Mvc;
namespace Web.ManagerApplication.Views.Shared.Components.LogoutAdmin
{
    public class LogoutAdminViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
