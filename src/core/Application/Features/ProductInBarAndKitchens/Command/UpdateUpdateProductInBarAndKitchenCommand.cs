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

    public partial class UpdateProductInBarAndKitchenCommand : IRequest<Result<int>>
    {
        public EnumTypeUpdateProductInBarAndKitchen enumTypeUpdateProductIn { get; set; }
        public int IdBarAndKitchen { get; set; }
        public int? Id { get; set; }
        public int ComId { get; set; }
        public int[] ListId { get; set; }
    }
    public class UpdateProductInBarAndKitchenHandler : IRequestHandler<UpdateProductInBarAndKitchenCommand, Result<int>>
    {
    
        private readonly IProductInBarAndKitchenRepository _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateProductInBarAndKitchenHandler(IProductInBarAndKitchenRepository DefaultFoodOrderRepository,
            IUnitOfWork unitOfWork)
        {
            _Repository = DefaultFoodOrderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(UpdateProductInBarAndKitchenCommand command, CancellationToken cancellationToken)
        {

            if (command.enumTypeUpdateProductIn ==EnumTypeUpdateProductInBarAndKitchen.UPDATE_FOOD)
            {
                if (command.ListId == null || command.ListId.Count() == 0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR000);
                }
                var up = await _Repository.UpdateFoodInBarKitChenAsync(command.ListId, command.IdBarAndKitchen, command.ComId);
                if (up.Succeeded)
                {
                    return await Result<int>.SuccessAsync(up.Message);
                }
                return await Result<int>.FailAsync(up.Message);
            }

            else if (command.enumTypeUpdateProductIn == EnumTypeUpdateProductInBarAndKitchen.DELETE_FOOD)
            {
                if (!command.Id.HasValue)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR000);
                }
                var up = await _Repository.DeleteFoodAsync(command.Id.Value,command.IdBarAndKitchen, command.ComId);
                if (up.Succeeded)
                {
                    return await Result<int>.SuccessAsync(up.Message);
                }
                return await Result<int>.FailAsync(up.Message);
            }
            else if (command.enumTypeUpdateProductIn == EnumTypeUpdateProductInBarAndKitchen.DELETE_MUTI_FOOD)
            {
                if (command.ListId == null || command.ListId.Count() == 0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR000);
                }
                var up = await _Repository.DeleteFoodAsync(command.ListId, command.IdBarAndKitchen, command.ComId);
                if (up.Succeeded)
                {
                    return await Result<int>.SuccessAsync(up.Message);
                }
                return await Result<int>.FailAsync(up.Message);
            }
            return await Result<int>.FailAsync(HeperConstantss.ERR000);
        }
    }
}
