using ApiHttpClient.Wrappers;
using Application.Features.CategorysProduct.Query;
using Application.Features.CompanyInfo.Query;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Manager.Model;

namespace Web.Api.Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryProductController : BaseApiController<CategoryProductController>
    {
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var response = await _mediator.Send(new GetAllCategoryProductCacheQuery() { IsLevelParent = true });
                if (response.Succeeded)
                {
                    string host = $"{this.Request.Scheme}://{this.Request.Host}/";
                    var getCompany = await _mediator.Send(new GetByIdCompanyInfoQuery());
                    if (getCompany.Succeeded)
                    {
                        host = getCompany.Data.Website + "/";
                    }


                    List<CategoryProductModel> listData = new List<CategoryProductModel>();
                    listData = response.Data.Select(x => new CategoryProductModel() { Icon = (string.IsNullOrEmpty(x.Icon) ? x.Icon : host + x.Icon), Name = x.Name, Id = x.Id }).ToList();
                    return Ok(new ApiResponse("", listData) { IsError = false });
                }
                return Ok(new ApiResponse("Không tìm thấy dữ liệu") { IsError = true });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Ok(new ApiResponse(e.Message) { IsError = true, StatusCode = 500 });
            }

        }
    }
}
