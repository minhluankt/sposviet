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

namespace Application.Features.TypeCategorys.Query
{ 
    public class GetAllTypeCategoryCacheQuery : IRequest<Result<List<TypeCategory>>>
    {
        public GetAllTypeCategoryCacheQuery()
        {
        }
    }

    public class GetAllTypeCategoryCachedQueryHandler : IRequestHandler<GetAllTypeCategoryCacheQuery, Result<List<TypeCategory>>>
    {

        private readonly IRepositoryCacheAsync<TypeCategory> _TypeCategoryCache;
        private readonly IMapper _mapper;

        public GetAllTypeCategoryCachedQueryHandler(IRepositoryCacheAsync<TypeCategory> TypeCategoryCache, IMapper mapper)
        {
            _TypeCategoryCache = TypeCategoryCache;
            _mapper = mapper;
        }

        public async Task<Result<List<TypeCategory>>> Handle(GetAllTypeCategoryCacheQuery request, CancellationToken cancellationToken)
        {
            var productList = await _TypeCategoryCache.GetCachedListAsync(TypeCategoryCacheKeys.ListKey);
            return Result<List<TypeCategory>>.Success(productList);
        }
    }
}
