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

namespace Application.Features.Specification.Query
{
    public class GetAllSpecificationsQuery : IRequest<Result<List<Specifications>>>
    {
        public int IdType { get; set; }
        public int? idBrand { get; set; }
        public GetAllSpecificationsQuery(int _IdType)
        {
            IdType = _IdType;
        }
    }

    public class GetAllSpecificationsCachedQueryHandler : IRequestHandler<GetAllSpecificationsQuery, Result<List<Specifications>>>
    {

        private readonly IRepositoryCacheAsync<Specifications> _SpecificationsCache;
        private readonly IRepositoryAsync<Specifications> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllSpecificationsCachedQueryHandler(IRepositoryAsync<Specifications> repositoryAsync, IRepositoryCacheAsync<Specifications> SpecificationsCache, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _SpecificationsCache = SpecificationsCache;
            _mapper = mapper;
        }

        public async Task<Result<List<Specifications>>> Handle(GetAllSpecificationsQuery request, CancellationToken cancellationToken)
        {
            var productList = _repositoryAsync.GetMulti(m => m.idTypeSpecifications == request.IdType);

            return await Result<List<Specifications>>.SuccessAsync(productList.ToList());
        }
    }
}
