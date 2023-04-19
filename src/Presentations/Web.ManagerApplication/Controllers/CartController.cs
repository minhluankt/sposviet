using Application.Constants;
using Application.Features.CompanyInfo.Query;
using Application.Features.Customers.Query;
using Application.Features.Products.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Controllers
{
    public class CartController : BaseController<AccountController>
    {
        private readonly IOrderRepository<Order> _orderRepository;
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository<Cart> _cartRepository;
        public CartController(IUserRepository userRepository,
            IOptions<CryptoEngine.Secrets> config, IOrderRepository<Order> orderRepository,
            ICartRepository<Cart> cartRepository)
        {
            _orderRepository = orderRepository;
            _config = config;
            _cartRepository = cartRepository;
            _userRepository = userRepository;
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        public async Task<IActionResult> GetQuantityCartByUserAsync()
        {
            var usercom = await _userRepository.GetUserAsync(User);
            if (usercom == null)
            {
                return new JsonResult(new { mess = GeneralMess.ConvertStatusToString(HeperConstantss.ERR033), isValid = false, login = true });
            }
            var addCart = await _cartRepository.GetQuantityCartByUserAsync(usercom.Id);
            return new JsonResult(new { isValid = true, data = addCart });

        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        public async Task<IActionResult> ChekoutCartAsync()
        {
            var usercom = await _userRepository.GetFullUserAsync(User);
            if (usercom == null)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR033));
                return RedirectToAction("Login");
            }
            var checkout = await _cartRepository.CheckOutCart(usercom);
            if (checkout.isSuccess)
            {
                var values = "OrderCode=" + checkout.Data.Order.OrderCode + "&idCustomer=" + usercom.Id;
                var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                return LocalRedirect("/Cart/OrderSuccess?secret=" + secret);
            }
            _notify.Error(GeneralMess.ConvertStatusToString(checkout.Message));
            return RedirectToAction("Payment");

        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        [EncryptedParameters("secret")]
        public async Task<IActionResult> OrderSuccessAsync(string OrderCode, int idCustomer)
        {
            if (string.IsNullOrEmpty(OrderCode))
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR033));
                return RedirectToAction("Login");
            }
            //var usercom = await _userRepository.GetFullUserAsync(User);
            OrderModelView model = new OrderModelView();
            model.Order = await _orderRepository.GetByIdOrderAndCustomerAsync(OrderCode, idCustomer);
            // model.Customer = usercom;
            return View(model);
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        public async Task<IActionResult> MycartAsync()
        {
            try
            {
                var usercom = await _userRepository.GetUserAsync(User);
                if (usercom == null)
                {
                    _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR033));
                    return RedirectToAction("Login");
                }

                var response = await _mediator.Send(new GetByCartCustomerQuery(usercom.Id));

                if (response.Succeeded)
                {
                    var customer = await _mediator.Send(new GetByIdCustomerQuery() { Id = usercom.Id });
                    if (customer.Succeeded)
                    {
                        response.Data.Customer = customer.Data;
                    }
                    else
                    {
                        response.Data.Customer = usercom;
                    }
                    return View(response.Data);
                }
                return View(response.Data);
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return RedirectToAction("/");
            }

        }
        public async Task<IActionResult> PaymentAsync()
        {
            try
            {
                var usercom = await _userRepository.GetUserAsync(User);
                if (usercom == null)
                {
                    _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR033));
                    return RedirectToAction("Login");
                }

                var response = await _mediator.Send(new GetByCartCustomerQuery(usercom.Id) { IsSelectCart = true });

                if (response.Succeeded)
                {
                    if (response.Data.CartModel.Amount == 0)
                    {
                        _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR036));
                        return RedirectToAction("Mycart");
                    }
                    var customer = await _mediator.Send(new GetByIdCustomerQuery() { Id = usercom.Id });
                    if (customer.Succeeded)
                    {
                        response.Data.Customer = customer.Data;
                    }
                    else
                    {
                        response.Data.Customer = usercom;
                    }

                    var infocompany = await _mediator.Send(new GetByIdCompanyInfoQuery());
                    if (infocompany.Succeeded)
                    {
                        response.Data.CompanyAdminInfo = infocompany.Data;
                    }

                    return View(response.Data);
                }
                return View(response.Data);
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return RedirectToAction("/");
            }

        }

        public async Task<IActionResult> UpdateItemCartAsync(int?[] IdItemCart, bool Select, bool removeAll = false, bool checkAll = false)
        {
            var usercom = await _userRepository.GetUserAsync(User);
            if (usercom == null)
            {
                return new JsonResult(new { login = true, mess = GeneralMess.ConvertStatusToString(HeperConstantss.ERR033), isValid = false });

            }
            var updateSelectCart = await _cartRepository.UpdateCartBySelectItemAsync(usercom.Id, IdItemCart, Select, removeAll, checkAll);
            if (updateSelectCart.isSuccess)
            {
                _notify.Success(GeneralMess.ConvertStatusToString(updateSelectCart.Message));
                return new JsonResult(new { data = updateSelectCart.Data, mess = GeneralMess.ConvertStatusToString(updateSelectCart.Message), isValid = updateSelectCart.isSuccess });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(updateSelectCart.Message));
            return new JsonResult(new { mess = GeneralMess.ConvertStatusToString(updateSelectCart.Message), isValid = false });
        }
        public async Task<IActionResult> RemoveItemCartAsync(int IdItemCart)
        {
            var usercom = await _userRepository.GetUserAsync(User);
            if (usercom == null)
            {
                return new JsonResult(new { login = true, mess = GeneralMess.ConvertStatusToString(HeperConstantss.ERR033), isValid = false });

            }
            var updateSelectCart = await _cartRepository.RemoveItemCartAsync(usercom.Id, IdItemCart);
            if (updateSelectCart.isSuccess)
            {
                _notify.Success(GeneralMess.ConvertStatusToString(updateSelectCart.Message));
                return new JsonResult(new { data = updateSelectCart.Data, mess = GeneralMess.ConvertStatusToString(updateSelectCart.Message), isValid = updateSelectCart.isSuccess });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(updateSelectCart.Message));
            return new JsonResult(new { mess = GeneralMess.ConvertStatusToString(updateSelectCart.Message), isValid = false });
        }

        public async Task<IActionResult> AddToCartAsync(CartModelView model)
        {
            try
            {
                var usercom = await _userRepository.GetUserAsync(User);
                if (usercom == null)
                {
                    // _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR033));
                    return new JsonResult(new { login = true, mess = GeneralMess.ConvertStatusToString(HeperConstantss.ERR033), isValid = false });

                }
                if (model.IdProduct == 0)
                {
                    _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR034));
                    return new JsonResult(new { mess = GeneralMess.ConvertStatusToString(HeperConstantss.ERR034), isValid = false });
                }
                var data = await _mediator.Send(new GetByIdProductQuery() { Id = model.IdProduct, IncludeCategoryProduct = false, IncludeUploadImgProducts = false });
                if (data.Succeeded)
                {
                    var addCart = await _cartRepository.AddOrUpdateToCartAsync(model.AddCart, data.Data, model.Quantity, usercom.Id);
                    if (addCart.isSuccess)
                    {
                        // _notify.Success(GeneralMess.ConvertStatusToString(addCart.Message));
                        return new JsonResult(new { data = addCart.Data, mess = GeneralMess.ConvertStatusToString(HeperConstantss.SUS011), isValid = addCart.isSuccess });
                    }
                    return new JsonResult(new { mess = GeneralMess.ConvertStatusToString(data.Message), isValid = false });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(data.Message));
                return new JsonResult(new { mess = GeneralMess.ConvertStatusToString(data.Message), isValid = false });
            }
            catch (Exception e)
            {

                return new JsonResult(new { mess = e.Message, isValid = false });
            }

        }
    }
}
