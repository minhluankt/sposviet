using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.OrderTablePos.Commands
{

    public partial class SplitOrderCommand : SplitOrderModel, IRequest<Result<OrderTableModel>>
    {

    }
    public class SplitOrderHandler : IRequestHandler<SplitOrderCommand, Result<OrderTableModel>>
    {
        private readonly IRepositoryAsync<Customer> _customerRepository;
        private readonly IOrderTableRepository _orderTableRepository;
        private readonly ILogger<SplitOrderHandler> _logger;
        private readonly IOrderTableRepository _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public SplitOrderHandler(IOrderTableRepository brandRepository,
            IRepositoryAsync<Customer> customerRepository, ILogger<SplitOrderHandler> logger,
             IFormFileHelperRepository fileHelper, IOrderTableRepository orderTableRepository,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _logger = logger;
            _customerRepository = customerRepository;
            _orderTableRepository = orderTableRepository;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<OrderTableModel>> Handle(SplitOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                switch (request.TypeUpdate)
                {
                    case EnumTypeSpitOrder.Graft:
                        List<Guid> guids = new List<Guid>();
                        guids = request.lstOrder.Where(x => x.idOrder != null).Select(x => x.idOrder.Value).ToList();
                        var split = await _orderTableRepository.SplitOrderAsync(request.ComId, request.IdOrderOld.Value, guids, request.TypeUpdate, request.CasherName, request.IdCasher);
                        if (split.Failed)
                        {
                            return await Result<OrderTableModel>.FailAsync(split.Message);
                        }
                        return await Result<OrderTableModel>.SuccessAsync(split.Message);

                    case EnumTypeSpitOrder.Separate:
                        if (request.IdOrderOld == null)
                        {
                            return await Result<OrderTableModel>.FailAsync("Đơn cần tách không tồn tại");
                        }
                        if (request.IsNewOrder && request.IdTable == null && !request.IsBringBack)
                        {
                            return await Result<OrderTableModel>.FailAsync("Đơn tách vào không tồn tại");
                        }
                        

                        var split2 = await _orderTableRepository.SplitOrderSeparateAsync(request.ComId, request.IdOrderOld.Value, request.lstOrder, request.IsNewOrder, request.IsBringBack, request.IdOrderNew, request.IdTable, request.CasherName, request.IdCasher);
                        if (split2.Failed)
                        {
                            return await Result<OrderTableModel>.FailAsync(split2.Message);
                        }
                        return await Result<OrderTableModel>.SuccessAsync(split2.Message);


                }
                return await Result<OrderTableModel>.FailAsync("lỗi không đúng type");
                //  return await Result<OrderTableModel>.FailAsync("Không đúng loại cập nhật");
            }
            catch (System.Exception e)
            {
                _logger.LogError("COMID: " + request.ComId + "___" + e.ToString());
                return await Result<OrderTableModel>.FailAsync(e.Message);
            }
        }
    }
}
