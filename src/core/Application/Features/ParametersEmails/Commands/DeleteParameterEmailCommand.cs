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

namespace Application.Features.ParametersEmails.Commands
{
    public class DeleteParametersEmailCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeleteParametersEmailHandler : IRequestHandler<DeleteParametersEmailCommand, Result<int>>
        {
            private readonly IRepositoryAsync<ParametersEmail> _Repository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private readonly IDapperRepository _dapperdb;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteParametersEmailHandler(IRepositoryAsync<ParametersEmail> brandRepository, IDapperRepository dapperdb, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _dapperdb = dapperdb;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteParametersEmailCommand command, CancellationToken cancellationToken)
            {
                try
                {
                    var product = await _Repository.GetByIdAsync(command.Id);
                    if (product == null)
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR012);
                    }

                    await _Repository.DeleteAsync(product);
                    await _distributedCache.RemoveAsync(ParametersEmailCacheKeys.ListKey);
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
