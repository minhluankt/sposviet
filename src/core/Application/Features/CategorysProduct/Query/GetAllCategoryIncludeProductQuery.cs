
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CategorysProduct.Query
{

    public class GetAllCategoryIncludeProductQuery : IRequest<Result<IEnumerable<CategoryProduct>>>
    {
        public int?[] lstIdCategory { get; set; }
        public int taskProduct { get; set; }
        public bool checkActiveProduct { get; set; }
        public GetAllCategoryIncludeProductQuery()
        {
           
        }
    }

    public class GetAllCategoryIncludeProductdQueryHandler : IRequestHandler<GetAllCategoryIncludeProductQuery, Result<IEnumerable<CategoryProduct>>>
    {

        private readonly ICategoryProductRepository<CategoryProduct> _CategoryIncludeProduct;
        private readonly IMapper _mapper;

        public GetAllCategoryIncludeProductdQueryHandler(ICategoryProductRepository<CategoryProduct> CategoryIncludeProduct, IMapper mapper)
        {
            _CategoryIncludeProduct = CategoryIncludeProduct;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<CategoryProduct>>> Handle(GetAllCategoryIncludeProductQuery request, CancellationToken cancellationToken)
        {
            var productList =  _CategoryIncludeProduct.GetListIncludeProduct(request.lstIdCategory, request.checkActiveProduct, request.taskProduct);
            if (productList == null)
            {
                return await Result<IEnumerable<CategoryProduct>>.FailAsync();
            }
            return await Result<IEnumerable<CategoryProduct>>.SuccessAsync(productList);
        }
    }
}
