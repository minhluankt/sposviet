using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;

using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.OrderTables.Commands
{

    public class DeleteOrderTableCommand : IRequest<Result<int>>
    {
        public bool removeOrder { get; set; }
        public int Id { get; set; }
        public class DeleteOrderTableHandler : IRequestHandler<DeleteOrderTableCommand, Result<int>>
        {

            private readonly IRepositoryAsync<OrderTable> _Repository;
            private readonly IRepositoryAsync<Product> _ProductRepository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteOrderTableHandler(IRepositoryAsync<Product> ProductRepository,
                IRepositoryAsync<OrderTable> brandRepository,

                IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {

                _ProductRepository = ProductRepository;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteOrderTableCommand command, CancellationToken cancellationToken)
            {

                var product = await _Repository.GetByIdAsync(command.Id);

                return Result<int>.Fail(HeperConstantss.ERR012);
            }
        }
    }
}
