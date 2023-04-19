using Microsoft.AspNetCore.Mvc;

namespace Web.ManagerApplication.Areas.Selling.Views.Shared.Components.HeaderSelling
{
    public class HeaderSellingViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
