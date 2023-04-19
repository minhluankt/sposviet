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

namespace Application.Features.Banners.Query
{
    public class GetAllBannerCacheQuery : IRequest<Result<List<Banner>>>
    {
        public GetAllBannerCacheQuery()
        {
        }
    }

    public class GetAllBannerCachedQueryHandler : IRequestHandler<GetAllBannerCacheQuery, Result<List<Banner>>>
    {

        private readonly IRepositoryCacheAsync<Banner> _BannerCache;
        private readonly IMapper _mapper;

        public GetAllBannerCachedQueryHandler(IRepositoryCacheAsync<Banner> BannerCache, IMapper mapper)
        {
            _BannerCache = BannerCache;
            _mapper = mapper;
        }

        public async Task<Result<List<Banner>>> Handle(GetAllBannerCacheQuery request, CancellationToken cancellationToken)
        {
            var productList = await _BannerCache.GetCachedListAsync(BannerCacheKeys.ListKey);
            return Result<List<Banner>>.Success(productList);
        }
    }
}
