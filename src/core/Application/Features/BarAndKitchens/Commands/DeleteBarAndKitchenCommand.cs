using Application.CacheKeys;
using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Abstractions.Repository;
using AspNetCoreHero.Results;
using AutoMapper;
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

namespace Application.Features.BarAndKitchens.Commands
{

    public class DeleteBarAndKitchenCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int ComId { get; set; }
        public class DeleteBarAndKitchenHandler : IRequestHandler<DeleteBarAndKitchenCommand, Result<int>>
        {
            private readonly IFormFileHelperRepository _fileHelper;
            private readonly IRepositoryAsync<BarAndKitchen> _Repository;
            private readonly IRepositoryAsync<Product> _ProductRepository;
            private readonly IRepositoryAsync<District> _DistrictRepository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteBarAndKitchenHandler(IRepositoryAsync<Product> ProductRepository,
                IRepositoryAsync<District> DistrictRepository,
                IRepositoryAsync<BarAndKitchen> brandRepository,
                   IFormFileHelperRepository fileHelper,
                IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _DistrictRepository = DistrictRepository;
                _ProductRepository = ProductRepository;
                _fileHelper = fileHelper;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteBarAndKitchenCommand command, CancellationToken cancellationToken)
            {
                var product = await _Repository.Entities.SingleOrDefaultAsync(x=>x.ComId==command.ComId && x.Id==command.Id);
                if (product != null)
                {
                    await _Repository.DeleteAsync(product);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success(product.Id);
                }
                return Result<int>.Fail(HeperConstantss.ERR012);
            }
        }
    }
}
