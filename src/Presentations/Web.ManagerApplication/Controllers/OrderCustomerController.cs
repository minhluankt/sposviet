using Application.Constants;
using Application.Features.CompanyInfo.Query;
using Application.Features.Orders.Query;
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
    public class OrderCustomerController : BaseController<OrderCustomerController>
    {
        private INotifyUserRepository<NotifiUser> _notifyUserRepository;
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IStatusOrderRepository _statusrepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository<Order> _orderRepository;
        public OrderCustomerController(IOrderRepository<Order> orderRepository,
            INotifyUserRepository<NotifiUser> notifyUserRepository,
                  IStatusOrderRepository statusrepository, IOptions<CryptoEngine.Secrets> config,
            IUserRepository userRepository)
        {
            _notifyUserRepository = notifyUserRepository;
            _config = config;
            _statusrepository = statusrepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        [EncryptedParameters("secret")]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            return View(order);
        }
        [EncryptedParameters("secret")]
        public async Task<IActionResult> TrackingPublish(int id)
        {

            OrderModelIndex cartModel = new OrderModelIndex();
            var getdata = await _mediator.Send(new GetByIdOrderQuery() { Id = id, IncludeCustomer = true, IncludePayment = true });

            if (getdata.Succeeded)
            {
                var company = await _mediator.Send(new GetByIdCompanyInfoQuery());
                if (company.Succeeded)
                {
                    cartModel.Company = company.Data;
                }
                cartModel.Order = getdata.Data;
                cartModel.StatusOrders = await _statusrepository.GetAllByOrderAsync(cartModel.Order.Id);
                cartModel.Customer = cartModel.Order.Customer;

                var values = "code=" + cartModel.Order.OrderCode;
                var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                cartModel.secret = secret;
            }
            return View(cartModel);
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        [EncryptedParameters("secret")]
        public async Task<IActionResult> Cancel(int id, string content)
        {
            try
            {
                var usercom = await _userRepository.GetUserAsync(User);
                var order = await _orderRepository.CancelByIdAsync(id, content, usercom.Name, true);
                if (order.isSuccess)
                {

                    _notify.Success(GeneralMess.ConvertStatusToString(order.Message));
                    return new JsonResult(new { isValid = order.isSuccess, loadTable = order.isSuccess });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(order.Message));
                return new JsonResult(new { isValid = order.isSuccess });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false });
            }

        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        [EncryptedParameters("secret")]
        public async Task<IActionResult> TrackingAsync(string code, int idnotify = 0)
        {
            CartModelIndex cartModel = new CartModelIndex();
            if (string.IsNullOrEmpty(code))
            {
                return View(cartModel);
            }
            var user = await _userRepository.GetUserAsync(User);
            cartModel.Order = await _orderRepository.GetByIdOrderAndCustomerAsync(code, user.Id);
            if (cartModel.Order != null)
            {
                cartModel.StatusOrders = await _statusrepository.GetAllByOrderAsync(cartModel.Order.Id);
                var values = "id=" + cartModel.Order.Id;
                var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                cartModel.secret = secret;
            }

            cartModel.Customer = user;


            if (idnotify > 0)
            {
                await Task.Run(async () =>
                {
                    await _notifyUserRepository.UpdateReviewAsync(idnotify);
                });

            }
            return View(cartModel);
        }
    }
}
