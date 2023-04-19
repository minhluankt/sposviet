using Application.CacheKeys;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class PermissionRepository : RepositoryAsync<Permission>,  IPermissionRepository<Permission>
    {
        private readonly IRepositoryAsync<Permission> _repository;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IDistributedCache _distributedCache;
        public PermissionRepository(IDbFactory dbFactory, IRepositoryAsync<Permission> repository, IUnitOfWork UnitOfWork, IDistributedCache distributedCache) : base(dbFactory)
        {
            _distributedCache = distributedCache;
            _repository = repository;
            _UnitOfWork = UnitOfWork;
        }

        //public async Task UpdateOnlyAsync(Permission entity)
        //{
           
        //    Permission find = await _repository.GetByIdAsync(entity.Id);
        //    find.Name = entity.Name;
        //    find.Code = find.Code;
        //}
    }
}
