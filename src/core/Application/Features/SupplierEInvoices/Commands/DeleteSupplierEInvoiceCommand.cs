using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.SupplierEInvoices.Commands
{
    public class DeleteSupplierEInvoiceCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int ComId { get; set; }
        public class DeleteSupplierEInvoiceHandler : IRequestHandler<DeleteSupplierEInvoiceCommand, Result<int>>
        {
            private readonly IFormFileHelperRepository _fileHelper;
            private readonly ISupplierEInvoiceRepository<SupplierEInvoice> _Repository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private readonly ILogger<DeleteSupplierEInvoiceCommand> _log;
            private readonly IHostingEnvironment _hostingEnvironment;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteSupplierEInvoiceHandler(ISupplierEInvoiceRepository<SupplierEInvoice> brandRepository,
                ILogger<DeleteSupplierEInvoiceCommand> log, IHostingEnvironment hostingEnvironment,
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
            public async Task<Result<int>> Handle(DeleteSupplierEInvoiceCommand command, CancellationToken cancellationToken)
            {
                try
                {
                    _log.LogInformation("DeleteSupplierEInvoiceCommand start");
                    var product = await _Repository.DeleteAsync(command.Id, command.ComId);
                    if (!product.Succeeded)
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR012);
                    }
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return await Result<int>.SuccessAsync();
                }
                catch (Exception e)
                {
                    _log.LogError("DeleteSupplierEInvoiceCommand Exception: " + e.ToString());
                    return await Result<int>.FailAsync(e.Message);
                }

            }
        }
    }
}
