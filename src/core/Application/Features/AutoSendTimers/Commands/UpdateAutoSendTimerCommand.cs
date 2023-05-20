using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.AutoSendTimers.Commands
{

    public partial class UpdateAutoSendTimerCommand : AutoSendTimer, IRequest<Result<int>>
    {
    }
    public class UpdateAutoSendTimerHandler : IRequestHandler<UpdateAutoSendTimerCommand, Result<int>>
    {
        private readonly ILogger<UpdateAutoSendTimerHandler> _log;
        private readonly IAutoSendTimerRepository<AutoSendTimer> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateAutoSendTimerHandler(IAutoSendTimerRepository<AutoSendTimer> brandRepository,
            ILogger<UpdateAutoSendTimerHandler> log,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
            _log = log;
        }


        public async Task<Result<int>> Handle(UpdateAutoSendTimerCommand command, CancellationToken cancellationToken)
        {
           // await _unitOfWork.CreateTransactionAsync();
            try
            {
                var product = _mapper.Map<AutoSendTimer>(command);
                var up=  await _Repository.UpdateAsync(product);
                if (!up.Succeeded)
                {
                    _log.LogError("UpdateAutoSendTimerCommand Không tìm thấy Đơn vị  " + command.TypeSupplierEInvoice);
                    return await Result<int>.FailAsync("Không tìm thấy dữ liệu");
                }
                return await Result<int>.SuccessAsync(up.Data.Id,up.Message);
            }
            catch (Exception ex)
            {
               // await _unitOfWork.RollbackAsync();
                _log.LogError("UpdateAutoSendTimerCommand Create " + command.TypeSupplierEInvoice + "\n" + ex.ToString());
                return await Result<int>.FailAsync(ex.Message);
            }

        }
    }
}
