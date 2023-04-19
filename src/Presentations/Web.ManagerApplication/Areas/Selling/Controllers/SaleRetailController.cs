using Application.Constants;
using Application.Features.OrderTables.Commands;
using Application.Features.PosSellings.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class SaleRetailController : BaseController<SaleRetailController>
    {
        private readonly IPaymentMethodRepository _payment;
        private readonly UserManager<ApplicationUser> _userManager;
        private IOptions<CryptoEngine.Secrets> _config;
        public SaleRetailController(UserManager<ApplicationUser> userManager, IPaymentMethodRepository payment, IOptions<CryptoEngine.Secrets> config)
        {
            _payment = payment;
            _userManager = userManager;
            _config = config;
        }
        [Authorize(Policy = PermissionUser.thunganSaleRetail)]
        public async Task<IActionResult> Index()
        {
            var user = User.Identity.GetUserClaimLogin();
            if (user.IdDichVu!=Application.Enums.EnumTypeProduct.TAPHOA_SIEUTHI && user.IdDichVu != Application.Enums.EnumTypeProduct.BAN_LE)
            {
                return BadRequest();
            }
            PosModel posModel = new PosModel();
            var _get = await _mediator.Send(new GetAllPosSellingQuery(user.ComId) { TypeProduct = user.IdDichVu, Comid = user.ComId });
            if (_get.Succeeded)
            {
                posModel = _get.Data;
                posModel.PaymentMethods =  _payment.GetAll(user.ComId, true).ToList();
            }
            return View(posModel);
        }
        [Authorize(Policy = "saleRetail.order")]
        [HttpPost]
        public async Task<IActionResult> ConvertInvoiceAsync(Guid? IdOrder)
        {
            var user = User.Identity.GetUserClaimLogin();
            var UpdateQuantity = await _mediator.Send(new UpdateOrderTableCommand() { ComId = user.ComId, IdGuid = IdOrder.Value });
            if (UpdateQuantity.Succeeded)
            {
                return Json(new { isValid = true, data = UpdateQuantity.Data });
            }
            _notify.Error(UpdateQuantity.Message);
            return Json(new { isValid = false });
        }
    }
}
