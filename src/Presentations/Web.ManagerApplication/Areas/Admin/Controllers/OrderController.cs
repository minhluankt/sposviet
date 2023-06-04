using Application.Constants;
using Application.Enums;
using Application.Features.Orders.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.Entities;
using Domain.Identity;
using Domain.ViewModel;
using Domain.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : BaseController<OrderController>
    {
        private readonly IUserManagerRepository<ApplicationUser> _userManager;
        private readonly IStatusOrderRepository _statusrepository;
        private readonly IOrderRepository<Order> _orderRepository;
        public OrderController(IOrderRepository<Order> orderRepository, IUserManagerRepository<ApplicationUser> userManager,
              IStatusOrderRepository statusrepository
            )
        {
            _userManager = userManager;
            _statusrepository = statusrepository;
            _orderRepository = orderRepository;
        }
        [EncryptedParameters("secret")]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            return View(order);
        }
        public IActionResult Index(OrderViewModel model)
        {
            return View(model);
        }
        public async Task<IActionResult> LoadAllOrder(OrderViewModel model)
        {
            //var usercom = await _userRepository.GetUserAsync(User);
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                // Skip number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();

                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction (asc, desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;

                int skip = start != null ? Convert.ToInt32(start) : 0;

                int recordsTotal = 0;
                DateTime? FromDate = null;
                DateTime? ToDate = null;

                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                {
                    FromDate = Common.ConvertStringToDateTime(model.FromDate);
                    ToDate = Common.ConvertStringToDateTime(model.ToDate);
                }
                // getting all Customer data  
                var response = await _mediator.Send(new GetOrderQueryQuery()
                {
                    IdCustomer = model.IdCustomer,
                    FromDate = FromDate,
                    ToDate = ToDate,
                    codeOrder = model.OrderCode,
                    sortColumn = sortColumn,
                    sortColumnDirection = sortColumnDirection,
                    pageSize = pageSize,
                    skip = skip
                });
                if (response.Succeeded)
                {
                    return Json(new { draw = draw, recordsFiltered = response.Data.TotalRow, recordsTotal = response.Data.TotalRow, data = response.Data.Orders });
                }

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = "" });

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [EncryptedParameters("secret")]
        public async Task<ActionResult> UpdateStatus(int id)
        {
            _logger.LogInformation(User.Identity.Name + "--> Order detailt");
            var data = await _mediator.Send(new GetByIdOrderQuery() { Id = id });
            if (data.Succeeded)
            {
                OrderStatusModel orderStatusModel = new OrderStatusModel();
                orderStatusModel.StatusOrders = await _statusrepository.GetAllByOrderAsync(id);
                orderStatusModel.Order = data.Data;
                var html = await _viewRenderer.RenderViewToStringAsync("UpdateStatus", orderStatusModel);
                return new JsonResult(new { isValid = true, html = html });
            }
            return new JsonResult(new { isValid = false, html = string.Empty });
        }
        [EncryptedParameters("secret")]
        [HttpPost]
        public async Task<ActionResult> UpdateStatus(int idorder, int status, string note)
        {
            var user = await _userManager.GetUserAsync(User);
            _logger.LogInformation(User.Identity.Name + "--> Order UpdateStatus");
            var update = await _orderRepository.UpdateStatusAsync(idorder, (EnumStatusOrder)status, note, user.FullName);
            if (update.isSuccess)
            {
                _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                return new JsonResult(new {statusName = GeneralMess.ConvertStatusOrder((EnumStatusOrder)status), isValid = update.isSuccess, note = update.Data.Note, idnote = update.Data.Id, fullname = update.Data.FullNameUpdate, date = update.Data.CreateDate.ToString("dd/MM/yyyy HH:mm:ss") });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR011));
            return new JsonResult(new { isValid = update.isSuccess, html = string.Empty });
        }

        [HttpPost]
        [EncryptedParameters("secret")]
        public async Task<IActionResult> Cancel(int id, string content)
        {
            try
            {
                var usercom = await _userManager.GetUserAsync(User);
                var order = await _orderRepository.CancelByIdAsync(id, content, usercom.FullName,false);
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
    }
}
