using ApiHttpClient.Wrappers;
using Application.Constants;
using Application.Features.ConfigSystems.Query;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace Web.Api.Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseApiController<ProductController>
    {
        private readonly IRepositoryAsync<CompanyAdminInfo> _companyrepository;
        private readonly IProductPepository<Product> _productRepository;
      
        private async Task<int> GetpageSiteAsync()
        {
            var getid = await _mediator.Send(new GetAllConfigQuery());
            if (getid.Succeeded)
            {
                var get = getid.Data.Where(m => m.Key == ParametersConfigSystem.pageSizeProductInCategory).SingleOrDefault();
                if (get != null)
                {
                    if (!string.IsNullOrEmpty(get.Value))
                    {
                        return int.Parse(get.Value);
                    }
                }
            }
            return 15;
        }
        public ProductController(IProductPepository<Product> productRepository, IRepositoryAsync<CompanyAdminInfo> companyrepository)
        {
            this._productRepository = productRepository;
            this._companyrepository = companyrepository;
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> GetAllAsync([FromQuery] ProductApiModelSearch model)
        {
            _logger.LogError($"Get all data product");
            var getData = await _productRepository.GetAllProductAsync(model);
            var rs = new ApiResponse("", getData) { IsError = false };//k cần IsError vì đã để md, làm để nhớ thôi
            return Ok(rs);
        }
        [HttpGet]
        [Route("listProductSell")]
        public async Task<IActionResult> GetAllProductSellAsync([FromQuery] ProductApiModelSearch model)
        {
            // string host = $"{this.Request.Scheme}://{this.Request.Host}/";
            //var getCompany = await _mediator.Send(new GetByIdCompanyInfoQuery());
            //if (getCompany.Succeeded)
            //{
            //    host = getCompany.Data.Website + "/";
            //}
            var getData = await _productRepository.GetAllProductAsync(model);
            var rs = new ApiResponse("", getData) { IsError = false };//k cần IsError vì đã để md, làm để nhớ thôi
            return Ok(rs);
        }


        [HttpGet]
        [Route("Get/{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogError($"Get all data product");
            var getData = _productRepository.GetById(id);
            string host = _companyrepository.GetFirstAsNoTracking().Website + "/";
            ProductResponseModel productModel = new ProductResponseModel()

            {
                Id = getData.Id,
                Name = getData.Name,
                Img = host + getData.Img,
                isPromotionRun = getData.isRunPromotion,
                isPromotion = getData.isPromotion,
                PriceSell = getData.isPromotion ? (getData.PriceDiscount > 0 ? getData.PriceDiscount : getData.Price - ((decimal)getData.Discount / 100 * getData.Price)) : 0,
                PriceSellRun = getData.isRunPromotion ? (getData.PriceDiscount > 0 ? getData.PriceDiscount : getData.Price - ((decimal)getData.Discount / 100 * getData.Price)) : 0,
                Price = getData.Price,
                Description = getData.Description
            };

            var rs = new ApiResponse("", productModel) { IsError = false };//k cần IsError vì đã để md, làm để nhớ thôi
            return Ok(rs);
        }
    }
}
