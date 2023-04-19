using Application.CacheKeys;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CompanyInfo.Query
{

    public class GetByIdCompanyInfoQuery : IRequest<Result<CompanyAdminInfo>>
    {
        public int? Id { get; set; }

        public class GetCompanyInfoByIdQueryHandler : IRequestHandler<GetByIdCompanyInfoQuery, Result<CompanyAdminInfo>>
        {
            private readonly IRepositoryAsync<CompanyAdminInfo> _repository;
            private readonly IRepositoryCacheAsync<CompanyAdminInfo> _repositorycache;

            public GetCompanyInfoByIdQueryHandler(IRepositoryAsync<CompanyAdminInfo> repository, IRepositoryCacheAsync<CompanyAdminInfo> repositorycache)
            {
                _repositorycache = repositorycache;
                _repository = repository;
            }
            public async Task<Result<CompanyAdminInfo>> Handle(GetByIdCompanyInfoQuery query, CancellationToken cancellationToken)
            {
                CompanyAdminInfo product = new CompanyAdminInfo();
                if (query.Id ==0 || query.Id==null)
                {
                    product = await _repositorycache.GetFirstAsync(CompanyAdminInfoCacheKeys.ListKey);
                }
                else
                {
                    product = await _repository.GetByIdAsync(query.Id.Value);
                }
                return Result<CompanyAdminInfo>.Success(product);
            }
        }
    }
}
