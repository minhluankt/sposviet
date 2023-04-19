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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Customers.Commands
{

    public partial class UpdateInfoDeliveryCustomerCommand : Customer, IRequest<Result<int>>
    {
    }
    public class UpdateInfoDeliveryCustomerHandler : IRequestHandler<UpdateInfoDeliveryCustomerCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<UpdateInfoDeliveryCustomerCommand> _log;
        private readonly IRepositoryAsync<Customer> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateInfoDeliveryCustomerHandler(IRepositoryAsync<Customer> brandRepository, ILogger<UpdateInfoDeliveryCustomerCommand> log,
            IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;
            _Repository = brandRepository;

            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;

            _log = log;
        }

        public async Task<Result<int>> Handle(UpdateInfoDeliveryCustomerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _log.LogInformation("UpdateInfoDeliveryCustomerCommand update start: " + command.Name);
                Customer brand = await _Repository.GetByIdAsync(command.Id);
           
                if (brand == null)
                {
                    _log.LogError(HeperConstantss.ERR012 + "___" + command.Name);
                    return Result<int>.Fail(HeperConstantss.ERR012);
                }
                else
                {
                    if (brand.PhoneNumber != command.PhoneNumber && !string.IsNullOrEmpty(command.PhoneNumber))
                    {
                        int checktaxcodeandType = _Repository.Entities.Count(m => m.PhoneNumber == command.PhoneNumber && m.Id != command.Id);
                        if (checktaxcodeandType > 0)
                        {
                            _log.LogInformation("UpdateInfoDeliveryCustomerCommand update trùng phone: " + command.PhoneNumber);
                            return Result<int>.Fail(HeperConstantss.ERR006);
                        }

                    }
                    brand.PhoneNumber = command.PhoneNumber;
                    brand.Name = command.Name;
                    brand.Address = command.Address;
                    brand.IdCity = command.IdCity;
                    brand.IdDistrict = command.IdDistrict;
                    brand.IdWard = command.IdWard;
                    await _Repository.UpdateAsync(brand);
                    await _distributedCache.RemoveAsync(CustomerCacheKeys.ListKey);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    _log.LogInformation("UpdateInfoDeliveryCustomerCommand update end: " + command.PhoneNumber);
                    return Result<int>.Success(brand.Id, HeperConstantss.SUS006);
                }
            }
            catch (Exception e)
            {
                _log.LogError("UpdateInfoDeliveryCustomerCommand update" + command.PhoneNumber + "\n" + e.ToString());
                return Result<int>.Fail(e.Message);
            }
        }
    }
}
