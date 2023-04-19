using Application.CacheKeys;
using Application.Interfaces.CacheRepositories;
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

namespace Application.Features.PromotionRuns.Query
{ 
    public class GetAllPromotionRunCacheQuery : IRequest<Result<List<PromotionRun>>>
    {
        public GetAllPromotionRunCacheQuery()
        {
        }
    }

    public class GetAllPromotionRunCachedQueryHandler : IRequestHandler<GetAllPromotionRunCacheQuery, Result<List<PromotionRun>>>
    {

        private readonly IRepositoryCacheAsync<PromotionRun> _PromotionRunCache;
        private readonly IRepositoryAsync<PromotionRun> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllPromotionRunCachedQueryHandler(IRepositoryAsync<PromotionRun> repositoryAsync,IRepositoryCacheAsync<PromotionRun> PromotionRunCache, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _PromotionRunCache = PromotionRunCache;
            _mapper = mapper;
        }

        public async Task<Result<List<PromotionRun>>> Handle(GetAllPromotionRunCacheQuery request, CancellationToken cancellationToken)
        {
           // var productList = await _PromotionRunCache.GetCachedListAsync(PromotionRunCacheKeys.ListKey);
            var productList = await _PromotionRunCache.GetCachedListAsync(PromotionRunCacheKeys.ListKey);

            return await Result<List<PromotionRun>>.SuccessAsync(productList.OrderByDescending(m=>m.Name).ToList());
        }
    }
}
