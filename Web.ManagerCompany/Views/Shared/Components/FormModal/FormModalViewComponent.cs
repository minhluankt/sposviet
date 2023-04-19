using Microsoft.AspNetCore.Mvc;

namespace Web.ManagerCompany.Views.Shared.Components.FormModal
{
    public class FormModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}