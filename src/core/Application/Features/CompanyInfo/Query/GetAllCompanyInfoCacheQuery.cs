using Application.CacheKeys;
using Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;



namespace Application.Features.CompanyInfo.Query
{
    public class GetAllCompanyInfoCacheQuery : IRequest<Result<List<CompanyAdminInfo>>>
    {
        public int Id { get; set; }
        public GetAllCompanyInfoCacheQuery(int Ids)
        {
            Id = Ids;
        }
    }

    public class GetAllCompanyInfoCachedQueryHandler : IRequestHandler<GetAllCompanyInfoCacheQuery, Result<List<CompanyAdminInfo>>>
    {

        private readonly IRepositoryCacheAsync<CompanyAdminInfo> _CompanyInfoCache;
        private readonly IMapper _mapper;

        public GetAllCompanyInfoCachedQueryHandler(IRepositoryCacheAsync<CompanyAdminInfo> CompanyInfoCache, IMapper mapper)
        {
            _CompanyInfoCache = CompanyInfoCache;
            _mapper = mapper;
        }

        public async Task<Result<List<CompanyAdminInfo>>> Handle(GetAllCompanyInfoCacheQuery request, CancellationToken cancellationToken)
        {
            var productList = await _CompanyInfoCache.GetCachedListAsync(CompanyAdminInfoCacheKeys.ListKey);

            return Result<List<CompanyAdminInfo>>.Success(productList);
        }
    }
}
