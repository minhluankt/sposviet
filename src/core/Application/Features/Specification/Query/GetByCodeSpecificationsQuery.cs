using Application.CacheKeys;
using Application.Constants;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Specification.Query
{
    public class GetByCodeSpecificationsQuery : IRequest<Result<List<Specifications>>>
    {
        public string Code { get; set; }

        public class ByCodeQueryHandler : IRequestHandler<GetByCodeSpecificationsQuery, Result<List<Specifications>>>
        {
            private readonly IRepositoryAsync<TypeSpecifications> _typerepository;
            private readonly IRepositoryAsync<Specifications> _repository;
            private readonly IRepositoryCacheAsync<Specifications> _repositorycache;
            public ByCodeQueryHandler(IRepositoryAsync<Specifications> repository,
                IRepositoryCacheAsync<Specifications> repositorycache,
                IRepositoryAsync<TypeSpecifications> typerepository)
            {
                _repositorycache = repositorycache;
                _typerepository = typerepository;
                _repository = repository;
            }
            public async Task<Result<List<Specifications>>> Handle(GetByCodeSpecificationsQuery query, CancellationToken cancellationToken)
            {
                var getry = await _typerepository.Entities.SingleOrDefaultAsync(m => m.Code == query.Code);
                if (getry==null)
                {
                    return await Result<List<Specifications>>.SuccessAsync(new List<Specifications>());
                }

                var product = await _repositorycache.GetCachedListAsync(SpecificationsCacheKeys.ListKey);
                if (product.Count() > 0)
                {
                    product = product.Where(m => m.idTypeSpecifications == getry.Id).ToList();
                }
                if (product == null || product.Count()==0)
                {
                    return await Result<List<Specifications>>.SuccessAsync(new List<Specifications>());
                    //return await Result<List<Specifications>>.FailAsync(HeperConstantss.ERR012);
                    
                }
                return await Result<List<Specifications>>.SuccessAsync(product);
            }
        }
    }
}
