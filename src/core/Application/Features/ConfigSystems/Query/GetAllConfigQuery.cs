using Application.CacheKeys;
using Application.Constants;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Application.Features.ConfigSystems.Query
{
 
    public class GetAllConfigQuery : IRequest<Result<List<ConfigSystem>>>
    {
      public int ComId { get; set; }
      public string Key { get; set; }
    }
    public class GetAllConfigQueryHandler : IRequestHandler<GetAllConfigQuery, Result<List<ConfigSystem>>>
    {
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IRepositoryCacheAsync<ConfigSystem> _repositorycache;
        private readonly IRepositoryAsync<ConfigSystem> _repository;
        public GetAllConfigQueryHandler(IRepositoryAsync<ConfigSystem> repository, IUnitOfWork unitOfWork,
            IDistributedCache distributedCache,
    IRepositoryCacheAsync<ConfigSystem> repositorycache)
        {
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCache;
            _repositorycache = repositorycache;
            _repository = repository;
        }
        public async Task<Result<List<ConfigSystem>>> Handle(GetAllConfigQuery query, CancellationToken cancellationToken)
        {
            var product = await _repository.Entities.Where(x=>x.ComId== query.ComId).ToListAsync();
            if (product == null)
            {
                return await Result<List<ConfigSystem>>.FailAsync();
            }
            return await Result<List<ConfigSystem>>.SuccessAsync(product);
        }
    }
}
