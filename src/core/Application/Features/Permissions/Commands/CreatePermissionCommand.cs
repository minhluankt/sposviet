using Application.CacheKeys;
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
    public partial class CreatePermissionCommand : IRequest<Result<int>>
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
    public class CreatePermissionHandler : IRequestHandler<CreatePermissionCommand, Result<int>>
    {
        private readonly IRepositoryAsync<Permission> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreatePermissionHandler(IRepositoryAsync<Permission> brandRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        {
            var fidn =  _Repository.Entities.Where(m=>m.Code==request.Code).SingleOrDefault();
            if (fidn!=null)
            {
                return await Result<int>.FailAsync("Mã quyền đã tồn tại!");
            }
            var product = _mapper.Map<Permission>(request);
            await _Repository.AddAsync(product);
            await _distributedCache.RemoveAsync(PermissionCacheKeys.ListKey);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(product.Id);
        }
    }
}
