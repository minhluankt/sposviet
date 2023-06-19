using Application.CacheKeys;
using Application.Constants;
using Application.Enums;
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

    public partial class UpdateDefaultFoodOrderCommand : IRequest<Result<int>>
    {
        public Guid? Id { get; set; }
        public int ComId { get; set; }
        public int[] ListId { get; set; }
        public decimal? Quantity { get; set; }
        public EnumTypeUpdateDefaultFoodOrder TypeUpdateDefaultFoodOrder { get; set; }
    }
    public class UpdateDefaultFoodOrderHandler : IRequestHandler<UpdateDefaultFoodOrderCommand, Result<int>>
    {
    
        private readonly IDefaultFoodOrderRepository<DefaultFoodOrder> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateDefaultFoodOrderHandler(IDefaultFoodOrderRepository<DefaultFoodOrder> DefaultFoodOrderRepository,
            IUnitOfWork unitOfWork)
        {
            _Repository = DefaultFoodOrderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(UpdateDefaultFoodOrderCommand command, CancellationToken cancellationToken)
        {
      
            if (command.TypeUpdateDefaultFoodOrder==EnumTypeUpdateDefaultFoodOrder.UPDATE_FOOD)
            {
            
                if (command.ListId==null|| command.ListId.Count()==0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR000);
                }
                var up=  await _Repository.UpdateFoodAsync(command.ListId, command.ComId);
                if (up.Succeeded)
                {
                    return await Result<int>.SuccessAsync(up.Message);
                }
                return await Result<int>.FailAsync(up.Message);
            }
            else if (command.TypeUpdateDefaultFoodOrder==EnumTypeUpdateDefaultFoodOrder.DELETE_FOOD)
            {
                if (!command.Id.HasValue)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR000);
                }
                var up=  await _Repository.DeleteFoodAsync(command.Id.Value, command.ComId);
                if (up.Succeeded)
                {
                    return await Result<int>.SuccessAsync(up.Message);
                }
                return await Result<int>.FailAsync(up.Message);
            } 
            else if (command.TypeUpdateDefaultFoodOrder==EnumTypeUpdateDefaultFoodOrder.DELETE_MUTI_FOOD)
            {
                if (command.ListId==null ||command.ListId.Count()==0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR000);
                }
                var up=  await _Repository.DeleteFoodAsync(command.ListId, command.ComId);
                if (up.Succeeded)
                {
                    return await Result<int>.SuccessAsync(up.Message);
                }
                return await Result<int>.FailAsync(up.Message);
            }
            else
            {
                if (!command.Quantity.HasValue)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR000);
                }
                if (!command.Id.HasValue)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR000);
                }
                var up = await _Repository.UpdateQuantityFoodAsync(command.Id.Value, command.ComId, command.Quantity.Value);
                if (up.Succeeded)
                {
                    return await Result<int>.SuccessAsync(up.Message);
                }
                return await Result<int>.FailAsync(up.Message);
            }
        }
    }
}
