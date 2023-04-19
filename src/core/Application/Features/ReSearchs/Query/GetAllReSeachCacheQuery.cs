using Application.CacheKeys;
using Application.Constants;
using Application.Enums;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using HelperLibrary;
using MediatR;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ReSearchs.Query
{

    public class GetAllReSearchCacheQuery : IRequest<Result<List<ReSearch>>>
    {
        public bool history { get; set; }
        public bool showhistorylayout { get; set; }
        public string keyword { get; set; }
        public int? take { get; set; }
        public GetAllReSearchCacheQuery()
        {
        }
    }

    public class GetAllReSearchCachedQueryHandler : IRequestHandler<GetAllReSearchCacheQuery, Result<List<ReSearch>>>
    {
        private readonly IReSearchRepository _reSearchCacheRepository;


        public GetAllReSearchCachedQueryHandler(
            IReSearchRepository ReSearchCacheRepository)
        {
            _reSearchCacheRepository = ReSearchCacheRepository;
      
        }

        public async Task<Result<List<ReSearch>>> Handle(GetAllReSearchCacheQuery request, CancellationToken cancellationToken)
        {
            if (request.history)
            {
                return await Result<List<ReSearch>>.SuccessAsync(await _reSearchCacheRepository.GetHistoriAsync(ProductEnumcs.Procuct,  request.take, request.showhistorylayout));

            }
            var productList = await _reSearchCacheRepository.SearchAsync(request.keyword, ProductEnumcs.Procuct);
          
            return await Result<List<ReSearch>>.SuccessAsync(productList);
        }
    }
}
