using Application.CacheKeys;
using Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CategorysProduct.Query
{

    public class GetAllCategoryProductCacheQuery : IRequest<Result<List<CategoryProduct>>>
    {
        public bool IsLevelParent { get; set; }
        public int? Comid { get; set; }
        public bool IsPos { get; set; }
        public GetAllCategoryProductCacheQuery()
        {
        }
    }

    public class GetAllCategoryCachedQueryHandler : IRequestHandler<GetAllCategoryProductCacheQuery, Result<List<CategoryProduct>>>
    {

        private readonly IRepositoryCacheAsync<CategoryProduct> _CategoryCache;
        private readonly IMapper _mapper;

        public GetAllCategoryCachedQueryHandler(IRepositoryCacheAsync<CategoryProduct> CategoryCache, IMapper mapper)
        {
            _CategoryCache = CategoryCache;
            _mapper = mapper;
        }

        public async Task<Result<List<CategoryProduct>>> Handle(GetAllCategoryProductCacheQuery request, CancellationToken cancellationToken)
        {
            var productList = await _CategoryCache.GetCachedListAsync(CategoryCacheKeys.ListProductKey);
            if (request.IsLevelParent)
            {
                productList = productList.Where(x => x.IdLevel == 0).ToList();
            }
            if (request.Comid!=null)
            {
                productList = productList.Where(x => x.ComId == request.Comid).ToList();
            }
            productList = productList.Where(x => x.IsPos == request.IsPos).ToList();
            // var mappedProducts = _mapper.Map<List<Category>>(productList);
            return Result<List<CategoryProduct>>.Success(productList);
        }
    }
}
