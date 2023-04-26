using ApiHttpClient.Wrappers;
using Application.Interfaces.Repositories;
using Domain.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : BaseApiController<OrderController>
    {
        private readonly ICompanyAdminInfoRepository _companyRepository;
        public CompanyController(ICompanyAdminInfoRepository companyRepository)
        {
            this._companyRepository = companyRepository;
        }
        [HttpGet]
        [Route("search")]

        public IActionResult GetByIdAsync([FromQuery] int Id)
        {
            try
            {
                _logger.LogError($"Get all data Order");
                var getcompany = _companyRepository.GetCompany(Id);
                var json = new
                {
                    Id = getcompany.Id,
                    Name = getcompany.Name,
                    Taxcode = getcompany.CusTaxCode,
                    Address = getcompany.Address,
                    Domain = "http://fnb.sposviet.vn",
                };
                var rs = new ApiResponse("", json) { IsError = false };//k cần IsError vì đã để md, làm để nhớ thôi
                return Ok(rs);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}", e);
                 throw new ApiException(e);
            }
           
        }
    }
}
