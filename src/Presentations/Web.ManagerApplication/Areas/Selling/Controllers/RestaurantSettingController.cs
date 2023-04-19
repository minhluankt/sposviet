using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class RestaurantSettingController : BaseController<RestaurantSettingController>
    {
        [Authorize(Policy = "selling.restaurantSetting")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
