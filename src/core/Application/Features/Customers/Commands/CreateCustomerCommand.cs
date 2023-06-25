using Application.CacheKeys;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Customers.Commands
{

    public partial class CreateCustomerCommand : Customer, IRequest<Result<int>>
    {

    }
    public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Result<int>>
    {
        private readonly ILogger<CreateCustomerCommand> _log;
        private readonly IRepositoryAsync<Customer> _Repository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IManagerIdCustomerRepository _managerIdCustomerRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateCustomerHandler(IRepositoryAsync<Customer> brandRepository, IManagerIdCustomerRepository managerIdCustomerRepository,
            ILogger<CreateCustomerCommand> log, ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _customerRepository = customerRepository;
            _managerIdCustomerRepository = managerIdCustomerRepository;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
            _log = log;
        }

        public async Task<Result<int>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                _log.LogInformation("CreateCustomerCommand  start " + request.PhoneNumber);

                if (!string.IsNullOrEmpty(request.PhoneNumber))
                {
                    var fidn = await _Repository.Entities.AsNoTracking().Where(m => m.PhoneNumber == request.PhoneNumber && m.Comid == request.Comid).FirstOrDefaultAsync();
                    if (fidn != null)
                    {
                        _log.LogError("CreateCustomerCommand Create: Đã tồn tại PhoneNumber " + request.PhoneNumber);
                        return await Result<int>.FailAsync("Đã tồn tại số điện thoại!");
                    }
                }

                if (!string.IsNullOrEmpty(request.Email))
                {
                    var fidnmail = await _Repository.Entities.AsNoTracking().Where(m => m.Email == request.Email && m.Comid == request.Comid).FirstOrDefaultAsync();
                    if (fidnmail != null)
                    {
                        _log.LogError("CreateCustomerCommand Create: Đã tồn tại email " + request.Email);
                        return await Result<int>.FailAsync("Đã tồn tại công ty!");
                    }
                }
                var product = _mapper.Map<Customer>(request);
                var update = await _customerRepository.Crreate(product);
                if (update.Succeeded)
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    await _unitOfWork.RollbackAsync();
                    return await Result<int>.FailAsync(update.Message);
                }

                _log.LogInformation("CreateCustomerCommand  end " + request.PhoneNumber);
                return await Result<int>.SuccessAsync(product.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _log.LogError("CreateCustomerCommand Create " + request.PhoneNumber + "\n" + ex.ToString());
                return await Result<int>.FailAsync(ex.Message);
            }

        }
    }
}
