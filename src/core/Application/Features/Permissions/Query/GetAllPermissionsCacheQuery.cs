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

namespace Application.Features.Permissions.Query
{
   
    public class GetAllPermissionsCacheQuery : IRequest<Result<List<Permission>>>
    {
        public GetAllPermissionsCacheQuery()
        {
        }
    }

    public class GetAllPermissionsCachedQueryHandler : IRequestHandler<GetAllPermissionsCacheQuery, Result<List<Permission>>>
    {
        private readonly IPermissionCacheRepository _permissionCache;
        private readonly IMapper _mapper;

        public GetAllPermissionsCachedQueryHandler(IPermissionCacheRepository permissionCache, IMapper mapper)
        {
            _permissionCache = permissionCache;
            _mapper = mapper;
        }

        public async Task<Result<List<Permission>>> Handle(GetAllPermissionsCacheQuery request, CancellationToken cancellationToken)
        {
            var productList = await _permissionCache.GetCachedListAsync();
            return Result<List<Permission>>.Success(productList);
        }
    }
}
