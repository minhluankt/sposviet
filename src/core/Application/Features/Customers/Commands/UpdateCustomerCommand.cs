using Application.CacheKeys;
using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using HelperLibrary;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.Customers.Commands
{

    public partial class UpdateCustomerCommand : Customer, IRequest<Result<int>>
    {
    }
    public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<UpdateCustomerCommand> _log;
        private readonly IRepositoryAsync<Customer> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateCustomerHandler(IRepositoryAsync<Customer> brandRepository, ILogger<UpdateCustomerCommand> log,
            IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;
            _Repository = brandRepository;

            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;

            _log = log;
        }

        public async Task<Result<int>> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // FormFileHelper _formFileHelper = new FormFileHelper(_hostingEnvironment);

                _log.LogInformation("UpdateCustomerCommand update start: " + command.Name);
                Customer brand = await _Repository.GetByIdAsync(command.Id);
                string imgold = string.Empty;
                string logoold = string.Empty;
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
                            _log.LogInformation("UpdateCustomerCommand update trùng phone: " + command.PhoneNumber);
                            return Result<int>.Fail(HeperConstantss.ERR006);
                        }

                    }
                    if (brand.Email != command.Email && !string.IsNullOrEmpty(command.Email))
                    {
                        int checktaxcodeandType = _Repository.Entities.Count(m => m.Email == command.Email && m.Id != command.Id);
                        if (checktaxcodeandType > 0)
                        {
                            _log.LogInformation("UpdateCustomerCommand update trùng email: " + command.Email);
                            return Result<int>.Fail(HeperConstantss.ERR005);
                        }

                    }
                    if (command.BirthDate != null)
                    {
                        brand.BirthDate = command.BirthDate;
                    }
                    if (!string.IsNullOrEmpty(command.PhoneNumber))
                    {
                        brand.PhoneNumber = command.PhoneNumber;
                        
                    }

                    brand.TypeCustomer = command.TypeCustomer;
                    brand.CCCD = command.CCCD;
                    brand.Nationality = command.Nationality;
                    brand.Passport = command.Passport;
                    brand.Buyer = command.Buyer;
                    brand.Sex = command.Sex;
                    brand.BirthDate = command.BirthDate;
                    brand.Taxcode = command.Taxcode;
                    brand.Name = command.Name;
                    brand.Address = command.Address;
                    if (!string.IsNullOrEmpty(command.Image))
                    {
                        imgold = brand.Image;
                        brand.Image = command.Image;
                    }
                    if (!string.IsNullOrEmpty(command.Logo))
                    {
                        logoold = brand.Logo;
                        brand.Logo = command.Logo;
                    }
                    //brand.Email = command.Email;
                    brand.PhoneNumber = command.PhoneNumber;
                    brand.CusBankName = command.CusBankName;
                    brand.CusBankNo = command.CusBankNo;

                    await _Repository.UpdateAsync(brand);
                    await _distributedCache.RemoveAsync(CustomerCacheKeys.ListKey);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    _log.LogInformation("UpdateCustomerCommand update end: " + command.PhoneNumber);
                    try
                    {
                        _log.LogInformation("UpdateCustomerCommand update Image start:" + command.PhoneNumber);
                        if (!string.IsNullOrEmpty(imgold))
                        {
                            _fileHelper.DeleteFile(imgold, FolderUploadConstants.Customer);
                        }
                        if (!string.IsNullOrEmpty(logoold))
                        {
                            _fileHelper.DeleteFile(logoold, FolderUploadConstants.Customer);
                        }
                    }
                    catch (Exception e)
                    {
                        _log.LogError("UpdateCustomerCommand update Image:" + command.PhoneNumber + "\n" + e.ToString());
                    }

                    return Result<int>.Success(brand.Id);
                }
            }
            catch (Exception e)
            {
                _log.LogError("UpdateCustomerCommand update" + command.PhoneNumber + "\n" + e.ToString());
                return Result<int>.Fail(e.Message);
            }
        }
    }
}
