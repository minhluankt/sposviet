using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Products.Query
{

    public class GetByIdProductQuery : IRequest<Result<Product>>
    {

        public int Comid { get; set; }
        public int Id { get; set; }
        public bool IsView { get; set; }
        public bool IncludeCategoryProduct { get; set; } = true;
        public bool IncludeUploadImgProducts { get; set; } = true;
        public bool IncludeCommnet { get; set; }
        public bool IncludeComboProduct { get; set; }

        public class GetProductByIdQueryHandler : IRequestHandler<GetByIdProductQuery, Result<Product>>
        {
            private readonly IRepositoryAsync<Product> _repository;
            private readonly IProductPepository<Product> _productrepository;
            public GetProductByIdQueryHandler(IRepositoryAsync<Product> repository, IProductPepository<Product> productrepository)
            {
                _productrepository = productrepository;
                _repository = repository;
            }
            public async Task<Result<Product>> Handle(GetByIdProductQuery query, CancellationToken cancellationToken)
            {
                var product = _repository.Entities.Where(m => m.Id == query.Id);
                if (query.Comid > 0)
                {
                    product = product.Where(x => x.ComId == query.Comid);
                }
                if (query.IncludeCategoryProduct)
                {
                    product = product.Include(m => m.CategoryProduct).ThenInclude(x => x.CategoryChild);
                }
                if (query.IncludeCategoryProduct)
                {
                    product = product.Include(m => m.UploadImgProducts);
                }
                if (query.IncludeComboProduct)
                {
                    product = product.Include(m => m.ComponentProducts);
                }
                if (query.IncludeCommnet)
                {
                    product = product.Include(m => m.Comments).ThenInclude(x => x.Customer);
                } 
                var ProductData = await product.SingleOrDefaultAsync();

                if (ProductData == null)
                {
                    return await Result<Product>.FailAsync(HeperConstantss.ERR012);
                }
                if (query.IsView)
                {
                    var task = Task.Run(() =>
                    {
                        _productrepository.UpdateReView(ProductData.Id);
                    });
                }
                return await Result<Product>.SuccessAsync(ProductData);
            }
        }
    }
}
