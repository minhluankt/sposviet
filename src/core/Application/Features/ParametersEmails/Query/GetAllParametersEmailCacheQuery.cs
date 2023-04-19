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

namespace Application.Features.ParametersEmails.Query
{
   
    public class GetAllParametersEmailsCacheQuery : IRequest<Result<List<ParametersEmail>>>
    {
        public GetAllParametersEmailsCacheQuery()
        {
        }
    }

    public class GetAllParametersEmailsCachedQueryHandler : IRequestHandler<GetAllParametersEmailsCacheQuery, Result<List<ParametersEmail>>>
    {
        private readonly IRepositoryCacheAsync<ParametersEmail> _ParametersEmailCache;
        private readonly IMapper _mapper;

        public GetAllParametersEmailsCachedQueryHandler(IRepositoryCacheAsync<ParametersEmail> ParametersEmailCache, IMapper mapper)
        {
            _ParametersEmailCache = ParametersEmailCache;
            _mapper = mapper;
        }

        public async Task<Result<List<ParametersEmail>>> Handle(GetAllParametersEmailsCacheQuery request, CancellationToken cancellationToken)
        {
            var productList = await _ParametersEmailCache.GetCachedListAsync(ParametersEmailCacheKeys.ListKey);
            return Result<List<ParametersEmail>>.Success(productList);
        }
    }
}
