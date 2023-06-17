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


namespace Application.Features.DefaultFoodOrders.Commands
{

    public class DeleteDefaultFoodOrderCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeleteDefaultFoodOrderHandler : IRequestHandler<DeleteDefaultFoodOrderCommand, Result<int>>
        {
            private readonly IRepositoryAsync<DefaultFoodOrder> _Repository;
            private readonly IRepositoryAsync<Customer> _CustomerRepository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteDefaultFoodOrderHandler(
                IRepositoryAsync<Customer> CustomerRepository,
                IRepositoryAsync<DefaultFoodOrder> DefaultFoodOrderRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _CustomerRepository = CustomerRepository;
                _Repository = DefaultFoodOrderRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteDefaultFoodOrderCommand command, CancellationToken cancellationToken)
            {
                var product = await _Repository.GetByIdAsync(command.Id);
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

