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
namespace Application.Features.Brands.Query
{

    public class GetAllBrandQuery : IRequest<Result<List<Brand>>>
    {
        public bool IncludeProduct { get; set; }
        public GetAllBrandQuery()
        {
        }
    }

    public class GetAllBranddQueryHandler : IRequestHandler<GetAllBrandQuery, Result<List<Brand>>>
    {

        private readonly IRepositoryCacheAsync<Brand> _Brand;
        private readonly IRepositoryAsync<Brand> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllBranddQueryHandler(IRepositoryAsync<Brand> repositoryAsync, IRepositoryCacheAsync<Brand> Brand, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _Brand = Brand;
            _mapper = mapper;
        }

        public async Task<Result<List<Brand>>> Handle(GetAllBrandQuery request, CancellationToken cancellationToken)
        {
            // var productList = await _Brand.GetCachedListAsync(BrandKeys.ListKey);
          
            if (request.IncludeProduct)
            {
                await Result<List<Brand>>.SuccessAsync(await _repositoryAsync.GetAllQueryable().Include(m => m.Products).ToListAsync());
            }
            var productList = await _repositoryAsync.GetAllAsync();
            return await Result<List<Brand>>.SuccessAsync(productList);
        }
    }
}
