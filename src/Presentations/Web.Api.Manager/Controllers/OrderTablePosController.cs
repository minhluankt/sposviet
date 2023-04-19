using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderTablePosController : BaseApiController<OrderTablePosController>
    {
        [HttpGet]
        public async Task<IActionResult> LoadDataOrder(int idtable)
        {
            return Ok(new { idtable });
        }
    }
}
