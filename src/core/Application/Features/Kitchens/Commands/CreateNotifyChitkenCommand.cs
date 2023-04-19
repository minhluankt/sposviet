using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Kitchens.Commands
{
    public partial class CreateNotifyChitkenCommand : NotifyKitChenModel, IRequest<Result<NotifyKitChenModel>>
    {

    }
    public class CreateNotifyChitkenHandler : IRequestHandler<CreateNotifyChitkenCommand, Result<NotifyKitChenModel>>
    {
        private readonly ILogger<CreateNotifyChitkenHandler> _log;
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly INotifyChitkenRepository _NotifyChitkenRepository;
        private readonly IRepositoryAsync<Customer> _customerRepository;
        private readonly IRepositoryAsync<RoomAndTable> _roomAndTableRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateNotifyChitkenHandler(
            ILogger<CreateNotifyChitkenHandler> log,
            INotifyChitkenRepository NotifyChitkenRepository,
            IRepositoryAsync<RoomAndTable> roomAndTableRepository,
            IRepositoryAsync<Customer> customerRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _log = log;
            _NotifyChitkenRepository = NotifyChitkenRepository;
            _roomAndTableRepository = roomAndTableRepository;
            _customerRepository = customerRepository;
            _fileHelper = fileHelper;

            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<NotifyKitChenModel>> Handle(CreateNotifyChitkenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.IdOrder == null)
                {
                    return await Result<NotifyKitChenModel>.FailAsync("Không tìm thấy đơn");
                }
                var add = await _NotifyChitkenRepository.NotifyOrder(request.ComId, request.IdOrder.Value,request.Cashername);
                if (add.Succeeded)
                {
                    return Result<NotifyKitChenModel>.Success();
                }
                return Result<NotifyKitChenModel>.Fail(add.Message);
            }
            catch (System.Exception e)
            {
                _log.LogError(e.ToString());
                return await Result<NotifyKitChenModel>.FailAsync(e.Message);
            }
        }
    }
}
