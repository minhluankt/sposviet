using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CompanyInfo.Query
{
    public class GetAllCompanyQuery : IRequest<Result<IQueryable<CompanyAdminInfo>>>
    {
        public int? Id { get; set; }

        public class GetAllCompanyQueryHandler : IRequestHandler<GetAllCompanyQuery, Result<IQueryable<CompanyAdminInfo>>>
        {
            private readonly IRepositoryAsync<CompanyAdminInfo> _repository;
            private readonly IRepositoryCacheAsync<CompanyAdminInfo> _repositorycache;

            public GetAllCompanyQueryHandler(IRepositoryAsync<CompanyAdminInfo> repository, IRepositoryCacheAsync<CompanyAdminInfo> repositorycache)
            {
                _repositorycache = repositorycache;
                _repository = repository;
            }
            public async Task<Result<IQueryable<CompanyAdminInfo>>> Handle(GetAllCompanyQuery query, CancellationToken cancellationToken)
            {
                var product = _repository.GetAllQueryable();
                return Result<IQueryable<CompanyAdminInfo>>.Success(product);
            }
        }
    }
}
