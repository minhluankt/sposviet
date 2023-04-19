using Application.CacheKeys;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Districts.Query
{
    public class GetAllDistrictCacheQuery : IRequest<Result<List<District>>>
    {
        public bool GetCache { get; set; }  
        public GetAllDistrictCacheQuery()
        {
        }
    }

    public class GetAllDistrictCachedQueryHandler : IRequestHandler<GetAllDistrictCacheQuery, Result<List<District>>>
    {

        private readonly IRepositoryCacheAsync<District> _DistrictCache;
        private readonly IRepositoryAsync<District> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllDistrictCachedQueryHandler(IRepositoryAsync<District> repositoryAsync, IRepositoryCacheAsync<District> DistrictCache, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _DistrictCache = DistrictCache;
            _mapper = mapper;
        }

        public async Task<Result<List<District>>> Handle(GetAllDistrictCacheQuery request, CancellationToken cancellationToken)
        {
            if (request.GetCache)
            {
                var productLists = await _DistrictCache.GetCachedListAsync(DistrictCacheKeys.ListKey);
                return await Result<List<District>>.SuccessAsync(productLists.OrderByDescending(m => m.Name).ToList());
            }
            // var productList = await _DistrictCache.GetCachedListAsync(DistrictCacheKeys.ListKey);
            var productList = _repositoryAsync.GetListInclude(m => m.Include(m => m.City));

            return await Result<List<District>>.SuccessAsync(productList.OrderByDescending(m => m.Name).ToList());
        }
    }
}
