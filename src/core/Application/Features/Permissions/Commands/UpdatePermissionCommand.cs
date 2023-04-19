using Application.CacheKeys;
using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Abstractions.Repository;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.Permissions.Commands
{

    public partial class UpdatePermissionCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
    public class UpdatePermissionHandler : IRequestHandler<UpdatePermissionCommand, Result<int>>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IPermissionRepository<Permission> _Permission;
        private readonly IRepositoryAsync<Permission> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdatePermissionHandler(IRepositoryAsync<Permission> brandRepository, 
            IPermissionRepository<Permission> Permission,
            IUnitOfWork unitOfWork,IDistributedCache distributedCach)
        {
            _Permission = Permission;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdatePermissionCommand command, CancellationToken cancellationToken)
        {
            var brand = await _Repository.GetByIdAsync(command.Id);
            if (brand == null)
            {
                return Result<int>.Fail(HeperConstantss.ERR012);
            }
            else
            {
                brand.Name = command.Name;
                await _Repository.UpdateAsync(brand);
                await _distributedCache.RemoveAsync(PermissionCacheKeys.ListKey);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(brand.Id);
            }
        }
    }
}
