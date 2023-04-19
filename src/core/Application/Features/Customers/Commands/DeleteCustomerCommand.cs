using Application.CacheKeys;
using Application.Constants;
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
    public class DeleteCustomerCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, Result<int>>
        {
            private readonly IFormFileHelperRepository _fileHelper;
            private readonly IRepositoryAsync<Customer> _Repository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private readonly ILogger<DeleteCustomerCommand> _log;
            private readonly IHostingEnvironment _hostingEnvironment;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteCustomerHandler(IRepositoryAsync<Customer> brandRepository,
                ILogger<DeleteCustomerCommand> log, IHostingEnvironment hostingEnvironment,
                IFormFileHelperRepository fileHelper,
                IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _fileHelper = fileHelper;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _log = log; _hostingEnvironment = hostingEnvironment;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteCustomerCommand command, CancellationToken cancellationToken)
            {
                try
                {
                    _log.LogInformation("DeleteCustomerCommand start");
                    var product = await _Repository.GetByIdAsync(command.Id);
                    await _Repository.DeleteAsync(product);
                    await _distributedCache.RemoveAsync(CustomerCacheKeys.ListKey);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    _log.LogInformation("DeleteCustomerCommand end: " + product.Name);
                    try
                    {
                        // FormFileHelper _formFileHelper = new FormFileHelper(_hostingEnvironment);
                        _log.LogInformation("UpdateCustomerCommand delete Image start:" + product.PhoneNumber);
                        if (!string.IsNullOrEmpty(product.Image))
                        {
                            _fileHelper.DeleteFile(product.Image, FolderUploadConstants.Customer);
                        }
                        if (!string.IsNullOrEmpty(product.Logo))
                        {
                            _fileHelper.DeleteFile(product.Logo, FolderUploadConstants.Customer);
                        }
                    }
                    catch (Exception e)
                    {
                        _log.LogError("UpdateCustomerCommand update Image:" + product.PhoneNumber + "\n" + e.ToString());
                    }

                    return await Result<int>.SuccessAsync(product.Id);
                }
                catch (Exception e)
                {
                    _log.LogError("DeleteCustomerCommand Exception: " + e.ToString());
                    return await Result<int>.FailAsync(e.Message);
                }

            }
        }
    }
}
