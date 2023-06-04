using Application.Constants;
using Application.Enums;
using Application.Features.Banners.Query;
using Application.Features.CompanyInfo.Query;
using Application.Features.ConfigSystems.Query;
using Application.Features.OrderTablePos.Commands;
using Application.Features.OrderTablePos.Querys;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Areas.Selling.Models;
using Web.ManagerApplication.Models;

namespace Web.ManagerApplication.Controllers
{
    [Authorize]
    public class OrderStaffController : BaseController<OrderStaffController>
    {
        private readonly IPaymentMethodRepository _payment;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrderStaffController(UserManager<ApplicationUser> userManager, IPaymentMethodRepository payment)
        {
            _userManager = userManager;
            _payment = payment;
        }
        [Authorize(Policy = PermissionUser.nhanvienphucvu)]
        public async Task<IActionResult> IndexAsync()
        {
        
            HomeViewModel homeViewModel = new HomeViewModel();
            var getid = await _mediator.Send(new GetByIdCompanyInfoQuery());
            if (getid.Succeeded)
            {
                if (getid.Data != null)
                {
                    ViewBag.Website = getid.Data.Website;
                    ViewBag.Title = getid.Data.Title;
                    ViewBag.description = getid.Data.Description;
                    ViewBag.Keyword = getid.Data.Keyword;
                    if (!string.IsNullOrWhiteSpace(getid.Data.Image))
                    {
                        ViewBag.image = $"{SystemVariable.SystemVariableHelper.FolderUpload}{FolderUploadConstants.ComPany}/{getid.Data.Image}";
                    }
                    else
                    {
                        ViewBag.image = $"{FolderUploadConstants.ImgSeo}";
                    }
                }
                else
                {
                    ViewBag.image = $"{FolderUploadConstants.ImgSeo}";
                }
            }
            var response = await _mediator.Send(new GetAllBannerCacheQuery());
            if (response.Succeeded)
            {
                homeViewModel.Banners = response.Data;
            }

            var getConfigQuer = await _mediator.Send(new GetAllConfigQuery());
            if (getConfigQuer.Succeeded)
            {
                var getlayoutHeader = getConfigQuer.Data.Where(m => m.Key == ParametersConfigSystem.layoutHeader).SingleOrDefault();
                if (getlayoutHeader != null)
                {
                    if (!string.IsNullOrEmpty(getlayoutHeader.Value))
                    {
                        homeViewModel.layoutHeader = int.Parse(getlayoutHeader.Value);
                    }
                }
            }

            _logger.LogInformation("Hi There!");

            // _logger.LogInformation("Hi There!");
            return View(homeViewModel);
        }
        [Authorize(Policy = PermissionUser.phucvuthanhtoan)]
        public async Task<IActionResult> PaymentOrder(Guid? IdOrder)
        {
            if (IdOrder==null)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR043));
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            var update = await _mediator.Send(new GetByIdOrderTableQuery() { TypeProduct = currentUser.IdDichVu, Comid = currentUser.ComId, IdOrder = IdOrder, OutInvNo = false, OutRoom = true });
            if (update.Succeeded)
            {
                PaymentModelView paymentModelView = new PaymentModelView();
                paymentModelView.PaymentMethods = await _payment.GetAll(currentUser.ComId, true).ToListAsync();
                paymentModelView.OrderTable = update.Data?.SingleOrDefault();
                if (paymentModelView.OrderTable == null)
                {
                    _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR043));
                    return Json(new { isValid = false });
                }
                var html = await _viewRenderer.RenderViewToStringAsync("PaymentOrder", paymentModelView);
                return new JsonResult(new
                {
                    isValid = true,
                    data = html,
                    IdOrder = IdOrder,
                    title = $"Phiếu thanh toán {paymentModelView.OrderTable.OrderTableCode} {(paymentModelView.OrderTable.RoomAndTable != null ? paymentModelView.OrderTable.RoomAndTable.Name : "")}"
                });

            }
            _notify.Error(GeneralMess.ConvertStatusToString(update.Message));
            return Json(new { isValid = false });
        }
        
        [HttpPost]
        public async Task<IActionResult> CheckOutOrder(Guid? IdOrder,
            decimal discountPayment, int Idpayment, decimal Amount, decimal Total)
            {
                if (Idpayment == 0)
                {
                    _notify.Error("Vui lòng chọn hình thức thanh toán");
                    return Json(new { isValid = false });
                }
                if (IdOrder == null)
                {
                    _notify.Error("Đơn cần thanh toán không hợp lệ");
                    return Json(new { isValid = false });
                }
                var currentUser = User.Identity.GetUserClaimLogin();
                EnumTypeProduct enumType = EnumTypeProduct.AMTHUC;

                var update = await _mediator.Send(new CheckOutOrderStaffCommand()
                {
                    Cashername = currentUser.FullName,
                    IdCasher = currentUser.Id,
                    Idpayment = Idpayment,
                    ComId = currentUser.ComId,
                    TypeUpdate = enumType,
                    IdOrder = IdOrder.Value,
                    discountPayment = discountPayment,
                    Total = Total,
                    Amount = Amount
                });
                if (update.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(update.Message));
                    //if (string.IsNullOrEmpty(update.Data))
                    //{
                    //    _notify.Warning(GeneralMess.ConvertStatusToString(HeperConstantss.ERR048));
                    //}
                    return Json(new { isValid = true});
                }
                _notify.Error(GeneralMess.ConvertStatusToString(update.Message));

           
            return Json(new { isValid = false });


        }
    }
}
