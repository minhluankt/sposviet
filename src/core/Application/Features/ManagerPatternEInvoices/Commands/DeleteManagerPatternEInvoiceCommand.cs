using Application.Constants;
using Application.Enums;
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

namespace Application.Features.ManagerPatternEInvoices.Commands
{
    public class DeleteManagerPatternEInvoiceCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int ComId { get; set; }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
        public class DeleteManagerPatternEInvoiceHandler : IRequestHandler<DeleteManagerPatternEInvoiceCommand, Result<int>>
        {
            private readonly IFormFileHelperRepository _fileHelper;
            private readonly IManagerPatternEInvoiceRepository<ManagerPatternEInvoice> _Repository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private readonly ILogger<DeleteManagerPatternEInvoiceCommand> _log;
            private readonly IHostingEnvironment _hostingEnvironment;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteManagerPatternEInvoiceHandler(IManagerPatternEInvoiceRepository<ManagerPatternEInvoice> brandRepository,
                ILogger<DeleteManagerPatternEInvoiceCommand> log, IHostingEnvironment hostingEnvironment,
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
            public async Task<Result<int>> Handle(DeleteManagerPatternEInvoiceCommand command, CancellationToken cancellationToken)
            {
                try
                {
                    _log.LogInformation("DeleteManagerPatternEInvoiceCommand start");
                    var product = await _Repository.DeleteAsync(command.Id, command.ComId, command.TypeSupplierEInvoice);
                    if (!product.Succeeded)
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR012);
                    }
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return await Result<int>.SuccessAsync();
                }
                catch (Exception e)
                {
                    _log.LogError("DeleteManagerPatternEInvoiceCommand Exception: " + e.ToString());
                    return await Result<int>.FailAsync(e.Message);
                }

            }
        }
    }
}
