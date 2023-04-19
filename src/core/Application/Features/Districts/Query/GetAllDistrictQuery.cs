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

    public class GetAllDistrictQuery : IRequest<Result<List<District>>>
    {
        public int idCity { get; set; }
        public GetAllDistrictQuery()
        {
        }
    }

    public class GetAllDistrictdQueryHandler : IRequestHandler<GetAllDistrictQuery, Result<List<District>>>
    {

        private readonly IRepositoryCacheAsync<District> _District;
        private readonly IRepositoryAsync<District> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllDistrictdQueryHandler(IRepositoryAsync<District> repositoryAsync, IRepositoryCacheAsync<District> District, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _District = District;
            _mapper = mapper;
        }

        public async Task<Result<List<District>>> Handle(GetAllDistrictQuery request, CancellationToken cancellationToken)
        {
            // var productList = await _District.GetCachedListAsync(DistrictKeys.ListKey);
            var productList = await _repositoryAsync.GetMulti(m => m.idCity == request.idCity).ToListAsync();

            return await Result<List<District>>.SuccessAsync(productList);
        }
    }
}
