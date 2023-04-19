using Application.CacheKeys;
using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.Districts.Commands
{

    public class DeleteDistrictCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeleteDistrictHandler : IRequestHandler<DeleteDistrictCommand, Result<int>>
        {
            private readonly IRepositoryAsync<District> _Repository;
            private readonly IRepositoryAsync<Customer> _CustomerRepository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteDistrictHandler(
                IRepositoryAsync<Customer> CustomerRepository,
                IRepositoryAsync<District> DistrictRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _CustomerRepository = CustomerRepository;
                _Repository = DistrictRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteDistrictCommand command, CancellationToken cancellationToken)
            {
                var product = await _Repository.GetByIdAsync(command.Id);
                if (product != null)
                {
                    var check = _CustomerRepository.GetAll(m => m.IdDistrict == command.Id).Count();
                    if (check > 0)
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR016);
                    }
                    await _Repository.DeleteAsync(product);
                    await _distributedCache.RemoveAsync(DistrictCacheKeys.ListKey);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success(product.Id);
                }
                return Result<int>.Fail(HeperConstantss.ERR012);
            }
        }
    }
}

