using Application.CacheKeys;
using Application.Constants;
using Application.Hepers;
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

namespace Application.Features.DefaultFoodOrders.Commands
{

    public partial class UpdateDefaultFoodOrderCommand : DefaultFoodOrder, IRequest<Result<int>>
    {

    }
    public class UpdateDefaultFoodOrderHandler : IRequestHandler<UpdateDefaultFoodOrderCommand, Result<int>>
    {
    
        private readonly IRepositoryAsync<DefaultFoodOrder> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateDefaultFoodOrderHandler(IRepositoryAsync<DefaultFoodOrder> DefaultFoodOrderRepository,
            IUnitOfWork unitOfWork)
        {
            _Repository = DefaultFoodOrderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(UpdateDefaultFoodOrderCommand command, CancellationToken cancellationToken)
        {
            var DefaultFoodOrder = await _Repository.GetByIdAsync(command.Id);
            if (DefaultFoodOrder == null)
            {
                return await Result<int>.FailAsync(HeperConstantss.ERR012);
            }
            else
            {
                DefaultFoodOrder.Quantity = command.Quantity;
                await _Repository.UpdateAsync(DefaultFoodOrder);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(DefaultFoodOrder.Id);
            }
        }
    }
}
