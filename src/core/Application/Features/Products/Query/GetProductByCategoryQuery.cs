using Application.CacheKeys;
using Application.Interfaces.CacheRepositories;
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
using System.Linq.Dynamic.Core;
using Application.Interfaces.Repositories;

namespace Application.Features.Products.Query
{
    public class GetProductByCategoryQuery : IRequest<Result<IEnumerable<Product>>>
    {
        public int? idcategory { get; set; }
        public int[] lstIdcategory { get; set; }
        public GetProductByCategoryQuery()
        {
        }
    }
    public class GetProductByCategoryQueryHandler : IRequestHandler<GetProductByCategoryQuery, Result<IEnumerable<Product>>>
    {

        private readonly IRepositoryCacheAsync<Product> _ProductCache;
        private readonly IProductPepository<Product> _Product;
        private readonly IMapper _mapper;

        public GetProductByCategoryQueryHandler(IRepositoryCacheAsync<Product> ProductCache, IMapper mapper, IProductPepository<Product> Product)
        {
            _Product = Product;
            _ProductCache = ProductCache;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<Product>>> Handle(GetProductByCategoryQuery request, CancellationToken cancellationToken)
        {
            int count = 0;
            IEnumerable<Product> productList;
            productList = _Product.GetProductbyListCategoryId(request.lstIdcategory).OrderByDescending(m => m.Id);

            return await Result<IEnumerable<Product>>.SuccessAsync(productList, count.ToString());
        }
    }
}
