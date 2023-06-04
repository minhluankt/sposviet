using Application.Constants;
using Application.Features.Customers.Commands;
using Application.Features.Customers.Query;
using Application.Features.Supplierss.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using HelperLibrary;
using Domain.Identity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Model;
using Newtonsoft.Json;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Extensions;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class SuppliersController : BaseController<SuppliersController>
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        public SuppliersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetJsonDataSuppliers()
        {
            var user = User.Identity.GetUserClaimLogin();
            var response = await _mediator.Send(new GetAllSuppliersQuery(user.ComId));
            if (response.Succeeded)
            {
                var listkq = response.Data.OrderBy(x => x.Name).Select(x => new
                {
                    id = x.Id,
                    text = x.Name
                }).ToList();
                if (listkq.Count() > 0)
                {
                    var data = JsonConvert.SerializeObject(listkq);
                    return Content(data);
                }
            }
            return Content("[]");
        }
    }
}
