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

namespace Application.Features.TypeSpecification.Commands
{

    public class DeleteTypeSpecificationsCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeleteTypeSpecificationsHandler : IRequestHandler<DeleteTypeSpecificationsCommand, Result<int>>
        {
            private readonly IRepositoryAsync<TypeSpecifications> _Repository;
            private readonly IRepositoryAsync<Specifications> _SpecificationsRepository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteTypeSpecificationsHandler(IRepositoryAsync<Specifications> SpecificationsRepository, IRepositoryAsync<TypeSpecifications> brandRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _SpecificationsRepository = SpecificationsRepository;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteTypeSpecificationsCommand command, CancellationToken cancellationToken)
            {
                var product = await _Repository.GetByIdAsync(command.Id);
                if (product != null)
                {
                    var check = _SpecificationsRepository.GetAll(m => m.idTypeSpecifications == command.Id).Count();
                    if (check > 0)
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR016);
                    }
                    await _Repository.DeleteAsync(product);
                    await _distributedCache.RemoveAsync(TypeSpecificationsCacheKeys.ListKey);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success(product.Id);
                }
                return Result<int>.Fail(HeperConstantss.ERR012);
            }
        }
    }
}
