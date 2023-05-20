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

namespace Application.Features.AutoSendTimers.Commands
{
    public class DeleteAutoSendTimerCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int ComId { get; set; }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
        public class DeleteAutoSendTimerHandler : IRequestHandler<DeleteAutoSendTimerCommand, Result<int>>
        {
            private readonly IFormFileHelperRepository _fileHelper;
            private readonly IAutoSendTimerRepository<AutoSendTimer> _Repository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private readonly ILogger<DeleteAutoSendTimerCommand> _log;
            private readonly IHostingEnvironment _hostingEnvironment;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteAutoSendTimerHandler(IAutoSendTimerRepository<AutoSendTimer> brandRepository,
                ILogger<DeleteAutoSendTimerCommand> log, IHostingEnvironment hostingEnvironment,
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
            public async Task<Result<int>> Handle(DeleteAutoSendTimerCommand command, CancellationToken cancellationToken)
            {
                try
                {
                    _log.LogInformation("DeleteAutoSendTimerCommand start");
                    var product = await _Repository.DeleteAsync(command.Id, command.ComId, command.TypeSupplierEInvoice);
                    if (!product.Succeeded)
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR012);
                    }
                    return await Result<int>.SuccessAsync(product.Message);
                }
                catch (Exception e)
                {
                    _log.LogError("DeleteAutoSendTimerCommand Exception: " + e.ToString());
                    return await Result<int>.FailAsync(e.Message);
                }

            }
        }
    }
}
