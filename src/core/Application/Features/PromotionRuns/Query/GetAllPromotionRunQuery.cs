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
namespace Application.Features.PromotionRuns.Query
{

    public class GetAllPromotionRunQuery : IRequest<Result<IQueryable<PromotionRun>>>
    {
        public GetAllPromotionRunQuery()
        {
        }
    }

    public class GetAllPromotionRundQueryHandler : IRequestHandler<GetAllPromotionRunQuery, Result<IQueryable<PromotionRun>>>
    {

        private readonly IRepositoryCacheAsync<PromotionRun> _PromotionRun;
        private readonly IRepositoryAsync<PromotionRun> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllPromotionRundQueryHandler(IRepositoryAsync<PromotionRun> repositoryAsync, IRepositoryCacheAsync<PromotionRun> PromotionRun, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _PromotionRun = PromotionRun;
            _mapper = mapper;
        }

        public async Task<Result<IQueryable<PromotionRun>>> Handle(GetAllPromotionRunQuery request, CancellationToken cancellationToken)
        {
          
            var productList =  _repositoryAsync.GetAllQueryable();
            return await Result<IQueryable<PromotionRun>>.SuccessAsync(productList);
        }
    }
}
