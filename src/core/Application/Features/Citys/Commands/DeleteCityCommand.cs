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

namespace Application.Features.Citys.Commands
{

    public class DeleteCityCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeleteCityHandler : IRequestHandler<DeleteCityCommand, Result<int>>
        {
            private readonly IRepositoryAsync<City> _Repository;
            private readonly IRepositoryAsync<Product> _ProductRepository;
            private readonly IRepositoryAsync<District> _DistrictRepository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteCityHandler(IRepositoryAsync<Product> ProductRepository,
                IRepositoryAsync<District> DistrictRepository,
                IRepositoryAsync<City> brandRepository,
                IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _DistrictRepository = DistrictRepository;
                _ProductRepository = ProductRepository;

                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteCityCommand command, CancellationToken cancellationToken)
            {
                var product = await _Repository.GetByIdAsync(command.Id);
                if (product != null)
                {
                    var check2 = _DistrictRepository.GetAll(m => m.idCity == command.Id).Count();
                    if (check2 > 0)
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR016);
                    }
                    await _Repository.DeleteAsync(product);
                    await _distributedCache.RemoveAsync(CityCacheKeys.ListKey);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success(product.Id);
                }
                return Result<int>.Fail(HeperConstantss.ERR012);
            }
        }
    }
}
