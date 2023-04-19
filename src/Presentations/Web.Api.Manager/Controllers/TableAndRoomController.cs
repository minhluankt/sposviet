using Application.Features.RoomAndTables.Query;
using Application.Hepers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Newtonsoft.Json.Linq;
using ApiHttpClient.Wrappers;
using Spire.Doc.Documents;
using Web.Api.Manager.Model;
using Application.Features.Areas.Query;

namespace Web.Api.Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableAndRoomController : BaseApiController<TableAndRoomController>
    {
        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> GetAllAsync([FromQuery] ProductApiModelSearch model)
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();
                _logger.LogError($"Get all data product");

                var response = await _mediator.Send(new GetPageListQuery(currentUser.ComId)
                {
                    Comid = currentUser.ComId
                });

                if (response.Succeeded)
                {
                    // int skip = (model.PageNumber-1) * model.PageSize;
                    // var lstnew = response.Data.Skip(skip).Take(model.PageSize).ToList();
                    return Ok(new ApiResponse("", response.Data) { IsError = false });//k cần IsError vì đã để md, làm để nhớ thôi
                }
                var rs = new ApiResponse(response.Message, null) { IsError = true };//k cần IsError vì đã để md, làm để nhớ thôi
                return Ok(rs);
            }
            catch (Exception e)
            {
                var rs = new ApiResponse(e.Message, null) { IsError = true };//k cần IsError vì đã để md, làm để nhớ thôi
                return Ok(rs);
            }
           
        }
    }
}
