using ApiHttpClient.Wrappers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : BaseApiController<OrderController>
    {
        private readonly IOrderRepository<Order> _orderRepository;

        public OrderController(IOrderRepository<Order> orderRepository)
        {
            this._orderRepository = orderRepository;
        }
        [HttpGet]
        [Route("search")]

        public async Task<IActionResult> GetAllAsync([FromQuery] OrderViewModel model)
        {
            _logger.LogError($"Get all data Order");
            var rs = new ApiResponse("", await _orderRepository.GetAllOrderAsync(model)) { IsError = false };//k cần IsError vì đã để md, làm để nhớ thôi
            return Ok(rs);
        }
    }
}
