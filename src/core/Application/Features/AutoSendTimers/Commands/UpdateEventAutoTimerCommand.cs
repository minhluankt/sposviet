using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.AutoSendTimers.Commands
{

    public partial class UpdateEventAutoTimerCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public int ComId { get; set; }
        public bool IsStart { get; set; }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
    }
    public class UpdateEventAutoTimerHandler : IRequestHandler<UpdateEventAutoTimerCommand, IResult>
    {
        private readonly ILogger<UpdateEventAutoTimerHandler> _log;
        private readonly IAutoSendTimerRepository<AutoSendTimer> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateEventAutoTimerHandler(IAutoSendTimerRepository<AutoSendTimer> brandRepository,
            ILogger<UpdateEventAutoTimerHandler> log,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
            _log = log;
        }


        public async Task<IResult> Handle(UpdateEventAutoTimerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (command.IsStart)
                {
                    return await _Repository.StartJobAsync(command.Id, command.ComId, command.TypeSupplierEInvoice);
                }
                else
                {
                    return await _Repository.DeleteJobAsync(command.Id, command.ComId, command.TypeSupplierEInvoice);
                }

            }
            catch (Exception ex)
            {
                // await _unitOfWork.RollbackAsync();
                _log.LogError("UpdateEventAutoTimerCommand Create " + command.TypeSupplierEInvoice + "\n" + ex.ToString());
                return await Result<int>.FailAsync(ex.Message);
            }

        }
    }

}
