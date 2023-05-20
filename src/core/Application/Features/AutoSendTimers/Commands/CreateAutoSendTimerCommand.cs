using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Hangfire.Common;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.AutoSendTimers.Commands
{

    public partial class CreateAutoSendTimerCommand : AutoSendTimer, IRequest<Result<int>>
    {

    }
    public class CreateAutoSendTimerHandler : IRequestHandler<CreateAutoSendTimerCommand, Result<int>>
    {
        private readonly ILogger<CreateAutoSendTimerCommand> _log;
        private readonly IAutoSendTimerRepository<AutoSendTimer> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateAutoSendTimerHandler(IAutoSendTimerRepository<AutoSendTimer> brandRepository,
            ILogger<CreateAutoSendTimerCommand> log,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
            _log = log;
        }

        public async Task<Result<int>> Handle(CreateAutoSendTimerCommand request, CancellationToken cancellationToken)
        {
            //await _unitOfWork.CreateTransactionAsync();
            try
            {
                var product = _mapper.Map<AutoSendTimer>(request);
                await _Repository.AddAsync(product);
                if (request.Active)
                {
                    RecurringJob.AddOrUpdate(product.JobId.ToString(),() => _Repository.StartJobEInvoiceAsync(product),Cron.Daily(request.Hour, request.Minute), TimeZoneInfo.Local);
                    // RecurringJob.Trigger(idJob); // chạy job ngay lập tức
                    return await Result<int>.SuccessAsync(product.Id,$"Hệ thống đã kích hoạt tính năng tự động gửi hóa đơn lên cơ quan thuế vào lúc: {request.Hour} giờ, {request.Minute} phút hằng ngày");
                }
                return await Result<int>.SuccessAsync(product.Id);
            }
            catch (Exception ex)
            {
               // await _unitOfWork.RollbackAsync();
                _log.LogError("CreateAutoSendTimerCommand Create " + request.TypeSupplierEInvoice + "\n" + ex.ToString());
                return await Result<int>.FailAsync(ex.Message);
            }

        }
    }
}
