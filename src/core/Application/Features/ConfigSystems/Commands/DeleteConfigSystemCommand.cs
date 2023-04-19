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


namespace Application.Features.ConfigSystems.Commands
{

    public class DeleteConfigSystemCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeleteConfigSystemHandler : IRequestHandler<DeleteConfigSystemCommand, Result<int>>
        {
            private readonly IRepositoryAsync<ConfigSystem> _Repository;
            private readonly IRepositoryAsync<Product> _Product;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteConfigSystemHandler(IRepositoryAsync<Product> Product,
                IRepositoryAsync<ConfigSystem> brandRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _Product = Product;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteConfigSystemCommand command, CancellationToken cancellationToken)
            {
                var product = await _Repository.GetByIdAsync(command.Id);
                if (product != null)
                {


                    await _Repository.DeleteAsync(product);
                    // await _distributedCache.RemoveAsync(ConfigSystemCacheKeys.ListKey);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success(product.Id);
                }
                return Result<int>.Fail(HeperConstantss.ERR012);
            }
        }
    }
}

