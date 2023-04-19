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

namespace Application.Features.Brands.Query
{ 
    public class GetAllBrandCacheQuery : IRequest<Result<List<Brand>>>
    {
        public GetAllBrandCacheQuery()
        {
        }
    }

    public class GetAllBrandCachedQueryHandler : IRequestHandler<GetAllBrandCacheQuery, Result<List<Brand>>>
    {

        private readonly IRepositoryCacheAsync<Brand> _BrandCache;
        private readonly IRepositoryAsync<Brand> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllBrandCachedQueryHandler(IRepositoryAsync<Brand> repositoryAsync,IRepositoryCacheAsync<Brand> BrandCache, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _BrandCache = BrandCache;
            _mapper = mapper;
        }

        public async Task<Result<List<Brand>>> Handle(GetAllBrandCacheQuery request, CancellationToken cancellationToken)
        {
           // var productList = await _BrandCache.GetCachedListAsync(BrandCacheKeys.ListKey);
            var productList = await _BrandCache.GetCachedListAsync(BrandCacheKeys.ListKey);

            return await Result<List<Brand>>.SuccessAsync(productList.OrderByDescending(m=>m.Name).ToList());
        }
    }
}
