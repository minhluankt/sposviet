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

namespace Application.Features.TypeSpecification.Query
{
    public class GetAllTypeSpecificationsCacheQuery : IRequest<Result<List<TypeSpecifications>>>
    {
        public GetAllTypeSpecificationsCacheQuery()
        {
        }
    }

    public class GetAllTypeSpecificationsCachedQueryHandler : IRequestHandler<GetAllTypeSpecificationsCacheQuery, Result<List<TypeSpecifications>>>
    {

        private readonly IRepositoryCacheAsync<TypeSpecifications> _TypeSpecificationsCache;
        private readonly IMapper _mapper;

        public GetAllTypeSpecificationsCachedQueryHandler(IRepositoryCacheAsync<TypeSpecifications> TypeSpecificationsCache, IMapper mapper)
        {
            _TypeSpecificationsCache = TypeSpecificationsCache;
            _mapper = mapper;
        }

        public async Task<Result<List<TypeSpecifications>>> Handle(GetAllTypeSpecificationsCacheQuery request, CancellationToken cancellationToken)
        {
            var productList = await _TypeSpecificationsCache.GetCachedListAsync(TypeSpecificationsCacheKeys.ListKey);
            return Result<List<TypeSpecifications>>.Success(productList);
        }
    }
}
