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

namespace Application.Features.Citys.Query
{
    public class GetAllCityCacheQuery : IRequest<Result<List<City>>>
    {
        public GetAllCityCacheQuery()
        {
        }
    }

    public class GetAllCityCachedQueryHandler : IRequestHandler<GetAllCityCacheQuery, Result<List<City>>>
    {

        private readonly IRepositoryCacheAsync<City> _CityCache;
        private readonly IMapper _mapper;

        public GetAllCityCachedQueryHandler(IRepositoryCacheAsync<City> CityCache, IMapper mapper)
        {
            _CityCache = CityCache;
            _mapper = mapper;
        }

        public async Task<Result<List<City>>> Handle(GetAllCityCacheQuery request, CancellationToken cancellationToken)
        {
            var productList = await _CityCache.GetCachedListAsync(CityCacheKeys.ListKey);
            return Result<List<City>>.Success(productList);
        }
    }
}
