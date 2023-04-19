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
using X.PagedList;

namespace Application.Features.ConfigSystems.Query
{

    public class GetByKeyConfigSystemQuery : IRequest<Result<ConfigSystem>>
    {
        public string Key { get; set; }
        public int ComId { get; set; }
        public GetByKeyConfigSystemQuery(string _key)
        {
            Key = _key;
        }
    }
    public class GetConfigSystemByIdQueryHandler : IRequestHandler<GetByKeyConfigSystemQuery, Result<ConfigSystem>>
    {
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IRepositoryCacheAsync<ConfigSystem> _repositorycache;
        private readonly IRepositoryAsync<ConfigSystem> _repository;
        public GetConfigSystemByIdQueryHandler(IRepositoryAsync<ConfigSystem> repository, IUnitOfWork unitOfWork,
            IDistributedCache distributedCache,
    IRepositoryCacheAsync<ConfigSystem> repositorycache)
        {
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCache;
            _repositorycache = repositorycache;
            _repository = repository;
        }
        public async Task<Result<ConfigSystem>> Handle(GetByKeyConfigSystemQuery query, CancellationToken cancellationToken)
        {
            var product = await _repository.Entities.AsNoTracking().Where(m => m.Key.ToLower() == query.Key.ToLower() && m.ComId== query.ComId).SingleOrDefaultAsync();
            if (product == null)
            {
                //var model = new ConfigSystem() { Key = query.Key, Value = String.Empty };
                //await _repository.AddAsync(model);
                //await _unitOfWork.SaveChangesAsync();
                //await _distributedCache.RemoveAsync(ConfigSystemCacheKeys.key);
                return await Result<ConfigSystem>.FailAsync();
            }
            if (string.IsNullOrEmpty(product.Parent))
            {
                var ConfigSystems = await _repository.Entities.AsNoTracking().Where(m => m.Parent.ToLower() == query.Key.ToLower() && m.ComId == query.ComId).ToListAsync();
                product.ConfigSystems = ConfigSystems;
            }
            return await Result<ConfigSystem>.SuccessAsync(product);
        }
    }

}
