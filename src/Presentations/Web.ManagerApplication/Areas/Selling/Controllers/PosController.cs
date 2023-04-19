using Application.Constants;
using Application.Enums;
using Application.Features.PosSellings.Query;
using Application.Features.Products.Query;
using Application.Hepers;
using Application.Providers;
using Domain.ViewModel;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Model;
using SystemVariable;
using Web.ManagerApplication.Abstractions;


namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class PosController : BaseController<PosController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IOptions<CryptoEngine.Secrets> _config;
        public PosController(UserManager<ApplicationUser> userManager, IOptions<CryptoEngine.Secrets> config)
        {
            _userManager = userManager;
            _config = config;
        }
        [Authorize(Policy = PermissionUser.thunganpos)]
        public async Task<IActionResult> Index()
        {
            var user = User.Identity.GetUserClaimLogin();
            PosModel posModel = new PosModel();
            var _get = await _mediator.Send(new GetAllPosSellingQuery(user.ComId) { TypeProduct = user.IdDichVu });
            if (_get.Succeeded)
            {
                posModel = _get.Data;
            }
            return View(posModel);
        }
        public async Task<IActionResult> SearchProductPos(string text, bool iSsell,  EnumTypeProductCategory type = EnumTypeProductCategory.PRODUCT)
        {
            var user = User.Identity.GetUserClaimLogin();
           // var user = await _userManager.GetUserAsync(User);

            var response = await _mediator.Send(new QueryProductAutoComplete()
            {
                iSsell = iSsell,
                enumTypeProductCategory= type,
                typeProduct = user.IdDichVu,
                ComId = user.ComId,
                text = text,
                Take = 15,
                code = text
            });
            //var listkq = response.Data.Where(x=>!x.StopBusiness).OrderBy(x => x.Name).Take(15).Select(x => new AutocompleteProductPosModel
            //{
            //    Id = x.Id,
            //    Name = x.Name,
            //    Code = x.Code,
            //    Img = !string.IsNullOrEmpty(x.Img) ? x.Img : SystemVariableHelper.UrlImgPos,
            //    Price = x.Price.ToString("F3"),
            //    Quantity = x.Quantity.ToString("F3"),
            //    Length = x.Name.Length,
            //}).ToList();
            return Json(response.Data);
        }
    }
}
