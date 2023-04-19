using Application.CacheKeys;
using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Abstractions.Repository;
using AspNetCoreHero.Results;
using AutoMapper;
using Dapper;
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
    public class DeletePermissionCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeletePermissionHandler : IRequestHandler<DeletePermissionCommand, Result<int>>
        {
            private readonly IRepositoryAsync<Permission> _Repository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private readonly IDapperRepository _dapperdb;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeletePermissionHandler(IRepositoryAsync<Permission> brandRepository, IDapperRepository dapperdb, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _dapperdb = dapperdb;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeletePermissionCommand command, CancellationToken cancellationToken)
            {
                try
                {
                    var product = await _Repository.GetByIdAsync(command.Id);
                    if (product != null)
                    {
                        DynamicParameters param = new DynamicParameters();

                        string sql = "SELECT * FROM RoleClaims";
                        List<RoleClaimsModel> list = _dapperdb.GetAll<RoleClaimsModel>(sql, param);
                        int getname = list.Count(m => m.ClaimValue == product.Code);

                        if (getname > 0)
                        {
                            return await Result<int>.FailAsync(HeperConstantss.ERR022);
                        }
                    }

                    await _Repository.DeleteAsync(product);
                    await _distributedCache.RemoveAsync(PermissionCacheKeys.ListKey);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success(product.Id);
                }
                catch (Exception e)
                {
                    return await Result<int>.FailAsync(e.Message);
                }
               
            }
        }
    }
    public class RoleClaimsModel
    {
        public string RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
