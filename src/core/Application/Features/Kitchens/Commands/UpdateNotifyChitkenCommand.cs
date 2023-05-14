using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.ViewModel;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Kitchens.Commands
{
    public partial class UpdateNotifyChitkenCommand : NotifyKitChenModel, IRequest<Result<NotifyKitChenModel>>
    {

    }
    public class UpdateNotifyChitkenHandler : IRequestHandler<UpdateNotifyChitkenCommand, Result<NotifyKitChenModel>>
    {
        private readonly ILogger<UpdateNotifyChitkenHandler> _log;
        private readonly INotifyChitkenRepository _NotifyChitkenRepository;
        private readonly IMapper _mapper;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateNotifyChitkenHandler(
            ILogger<UpdateNotifyChitkenHandler> log,
            INotifyChitkenRepository NotifyChitkenRepository,
            IUnitOfWork unitOfWork, IMapper mapper)
        {
            _log = log;
            _NotifyChitkenRepository = NotifyChitkenRepository;

            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<NotifyKitChenModel>> Handle(UpdateNotifyChitkenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                switch (request.TypeNotifyKitchenOrder)
                {
                    case EnumTypeNotifyKitchenOrder.Orocessed:

                        if (request.UpdateFull && request.idChitken == null)
                        {
                            if (request.TypeNotifyKitchenOrder != EnumTypeNotifyKitchenOrder.Orocessed)
                            {
                                return await Result<NotifyKitChenModel>.FailAsync(HeperConstantss.ERR001);
                            }
                            var UpdateFull = await _NotifyChitkenRepository.UpdateNotifyAllStatusOrder(request.ComId, request.ListIdChitken, EnumTypeNotifyKitchenOrder.Orocessed, request.Status);
                            if (UpdateFull.Succeeded)
                            {
                                return Result<NotifyKitChenModel>.Success(UpdateFull.Message);
                            }
                            return Result<NotifyKitChenModel>.Fail(UpdateFull.Message);
                        }
                        else if (request.idChitken == null)
                        {
                            return await Result<NotifyKitChenModel>.FailAsync("Không tìm thấy món");
                        }
                        break;
                    case EnumTypeNotifyKitchenOrder.DELETEKITCHEN:
                        if (request.idChitken == null)
                        {
                            return await Result<NotifyKitChenModel>.FailAsync("Không tìm thấy món");
                        }

                        break; 
                    case EnumTypeNotifyKitchenOrder.Processing:
                        if (request.Id == null)
                        {
                            return await Result<NotifyKitChenModel>.FailAsync("Không tìm thấy món");
                        }
                        if (!request.IsProgress)//nếu là món nhận làm
                        {
                            request.Status = EnumStatusKitchenOrder.Processing;
                        }
                        else
                        {
                            request.Status = EnumStatusKitchenOrder.MOI;
                        }
                        var updateProcessing = await _NotifyChitkenRepository.UpdateNotifyProcessingFood(request.ComId,  request.Id.Value, request.Cashername, request.Status);
                        if (updateProcessing!=null)
                        {
                            NotifyKitChenModel notifyKitChenModel = new NotifyKitChenModel();
                            notifyKitChenModel.IdProduct = updateProcessing.IdProduct;
                            notifyKitChenModel.Id = updateProcessing.Id;
                            notifyKitChenModel.idChitken = updateProcessing.IdKitchen;
                            return await Result<NotifyKitChenModel>.SuccessAsync(notifyKitChenModel,HeperConstantss.SUS006);
                        }
                        return await Result<NotifyKitChenModel>.FailAsync(HeperConstantss.ERR012);
                      
                    case EnumTypeNotifyKitchenOrder.UPDATEBYFOOD:
                        if (request.IdProduct == null)
                        {
                            return await Result<NotifyKitChenModel>.FailAsync("Không tìm thấy món");
                        }
                        if (request.TypeNotifyKitChen==EnumTypeNotifyKitChen.NHA_BEP_2)
                        {
                            var updatebupro = await _NotifyChitkenRepository.UpdateNotifyDoneByProduct(request.ComId, request.IdProduct.Value, request.Quantity);
                            if (updatebupro.Succeeded)
                            {
                                NotifyKitChenModel notifyKitChenModel = new NotifyKitChenModel();
                                notifyKitChenModel.IdProduct = request.IdProduct;
                                notifyKitChenModel.ListIdChitken = updatebupro.Data.Select(x => x.IdKitchen).ToArray();
                                return await Result<NotifyKitChenModel>.SuccessAsync(notifyKitChenModel, HeperConstantss.SUS006);
                            }
                            return await Result<NotifyKitChenModel>.FailAsync(updatebupro.Message);
                        }
                        break;
                    case EnumTypeNotifyKitchenOrder.UPDATEBYTABLE:

                        if (request.IdOrder == null)
                        {
                            return await Result<NotifyKitChenModel>.FailAsync("Không tìm thấy đơn");
                        }
                        break;
                    default:
                        return await Result<NotifyKitChenModel>.FailAsync(HeperConstantss.ERR001);

                }
                var update = await _NotifyChitkenRepository.UpdateNotifyOrder(request.ComId, request.IdOrder, request.idChitken, request.IdProduct, request.UpdateOne, request.TypeNotifyKitchenOrder, request.Status);
                if (update.Succeeded)
                {
                    return Result<NotifyKitChenModel>.Success(update.Message);
                }
                return Result<NotifyKitChenModel>.Fail(update.Message);
            }
            catch (System.Exception e)
            {
                _log.LogError(e.ToString());
                return await Result<NotifyKitChenModel>.FailAsync(e.Message);
            }
        }
    }
}
