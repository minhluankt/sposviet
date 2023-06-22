using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using Domain.XmlDataModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.OrderTablePos.Commands
{
    public partial class CreateOrderTableCommand : IRequest<Result<OrderTableModel>>
    {
        public OrderTableModel orderTableModel { get; set; }
        public CreateOrderTableCommand(OrderTableModel model)
        {
            orderTableModel = model;
        }
    }
    public class CreateOrderTableHandler : IRequestHandler<CreateOrderTableCommand, Result<OrderTableModel>>
    {
        private readonly ILogger<CreateOrderTableHandler> _log;
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IOrderTableRepository _orderTableRepository;
        private readonly IRepositoryAsync<Customer> _customerRepository;
        private readonly IRepositoryAsync<OrderTable> _Repository;
        private readonly IRepositoryAsync<RoomAndTable> _roomAndTableRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateOrderTableHandler(IRepositoryAsync<OrderTable> brandRepository,
            ILogger<CreateOrderTableHandler> log,
            IOrderTableRepository orderTableRepository,
            IRepositoryAsync<RoomAndTable> roomAndTableRepository,
            IRepositoryAsync<Customer> customerRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _log = log;
            _orderTableRepository = orderTableRepository;
            _roomAndTableRepository = roomAndTableRepository;
            _customerRepository = customerRepository;
            _fileHelper = fileHelper;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<OrderTableModel>> Handle(CreateOrderTableCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Result<OrderTable> update = null;
                bool IsNewOrder = true;
                OrderTable orderTable = new OrderTable();
                orderTable.Id = request.orderTableModel.IdOrder;
                orderTable.TypeProduct = request.orderTableModel.TypeProduct;
                orderTable.ComId = request.orderTableModel.ComId;
                if (request.orderTableModel.IdGuid != null)
                {
                    IsNewOrder = false;
                    orderTable.IdGuid = request.orderTableModel.IdGuid.Value;
                }

                OrderTableItem orderTableItem = new OrderTableItem();
                orderTableItem.DateCreateService = request.orderTableModel.DateCreateService;// 
                orderTableItem.IdProduct = request.orderTableModel.IdProduct;//
                orderTableItem.Code = request.orderTableModel.ProductCode;// 

                if (!IsNewOrder)//nếu là có rồi thì thôi vào việc luôn
                {
                    update = await _orderTableRepository.AddOrUpdateOrderTable(IsNewOrder, orderTable, orderTableItem);
                    if (update.Failed)
                    {
                        return await Result<OrderTableModel>.FailAsync(update.Message);
                    }
                }
                else if (IsNewOrder)
                {
                
                    orderTable.IdRoomAndTableGuid = request.orderTableModel.IdRoomAndTableGuid;

                    if (!string.IsNullOrEmpty(request.orderTableModel.CusCode))
                    {
                        Customer customer = await _customerRepository.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.Code == request.orderTableModel.CusCode && x.Comid == request.orderTableModel.ComId);
                        if (customer == null)
                        {
                            return await Result<OrderTableModel>.FailAsync("Không tìm thấy khách hàng đã chọn");
                        }
                        orderTable.IdCustomer = customer.Id;
                        orderTable.Buyer = customer.Name;
                        orderTable.CusCode = customer.Code;
                        orderTable.IsRetailCustomer = false;
                    }
                    else
                    {
                        orderTable.IdCustomer = null;
                        orderTable.IsRetailCustomer = true;
                        orderTable.Buyer = "Khách lẻ";
                    }
                    if (request.orderTableModel.IdRoomAndTableGuid != null)
                    {
                        var table = await _roomAndTableRepository.Entities.SingleOrDefaultAsync(x => x.IdGuid == request.orderTableModel.IdRoomAndTableGuid && x.ComId == request.orderTableModel.ComId);
                        if (table == null)
                        {
                            return await Result<OrderTableModel>.FailAsync("Không tìm thấy bàn phù hợp");
                        }
                        orderTable.IdRoomAndTable = table.Id;
                    }

                    orderTable.IsBringBack = request.orderTableModel.IsBringBack;//mang về
                    orderTable.IdStaff = request.orderTableModel.IdCasher;//nhân viên phục vụ
                    orderTable.StaffName = request.orderTableModel.CasherName;//nhân viên phục vụ
                    orderTable.Quantity = request.orderTableModel.Quantity;


                    if (orderTable.IsBringBack && orderTable.IdRoomAndTable > 0)
                    {
                        return await Result<OrderTableModel>.FailAsync("Không hỗ trợ đơn vừa mang về vừa là bàn");
                    }


                    update = await _orderTableRepository.AddOrUpdateOrderTable(IsNewOrder, orderTable, orderTableItem);
                    if (update.Failed)
                    {
                        return await Result<OrderTableModel>.FailAsync(update.Message);
                    }
                }
                // xử lý sau khi đã cập nhật
                OrderTableModel orderTableModel = new OrderTableModel();
                orderTableModel = request.orderTableModel;
                orderTableModel.CreateDate = update.Data.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss");
                orderTableModel.IsBringBack = update.Data.IsBringBack;
                orderTableModel.IdRoomAndTableGuid = update.Data.IdRoomAndTableGuid;
                //------------ xử lý lấy bàn
                if (update.Data.IdRoomAndTableGuid != null)
                {
                    var table = await _roomAndTableRepository.Entities.AsNoTracking().Include(x=>x.Area).SingleOrDefaultAsync(x => x.IdGuid == update.Data.IdRoomAndTableGuid);
                    orderTableModel.TableName = table.Name;
                    orderTableModel.AreaName = table.Area?.Name;
                }
               
                //--------xử lý giờ
                DateTime today = DateTime.Now;
                TimeSpan value = today.Subtract(update.Data.CreatedOn);
                orderTableModel.TimeNumber = value.TotalSeconds;
                //--------xử lý giờ

                orderTableModel.IdGuid = update.Data.IdGuid;
                orderTableModel.IdOrder = update.Data.Id;
                orderTableModel.OrderCode = update.Data.OrderTableCode;
                orderTableModel.Amount = update.Data.Amonut;
                orderTableModel.Buyer = update.Data.Buyer;
                orderTableModel.Quantity = update.Data.OrderTableItems.Sum(x => x.Quantity);
                orderTableModel.OrderTableItems.AddRange(update.Data.OrderTableItems.Select(x => new OrderTableItemModel() { 
                    Code = x.Code, 
                    Id = x.Id,
                    IdGuid = x.IdGuid,
                    IdProduct = x.IdProduct, 
                    Price = x.Price,
                    Quantity = x.Quantity,
                    QuantityNotifyKitchen = x.QuantityNotifyKitchen,
                    IdOrderTable = x.IdOrderTable, 
                    Total = x.Amount,
                    Name = x.Name,
                    Note = x.Note,
                    IsServiceDate = x.IsServiceDate,
                    DateCreateService = x.DateCreateService,
                    DateEndService = x.DateEndService,
                    IsVAT = x.IsVAT }));

                return Result<OrderTableModel>.Success(orderTableModel);


            }
            catch (System.Exception e)
            {
                _log.LogError(e.ToString());
                return await Result<OrderTableModel>.FailAsync(e.Message);
            }

        }
    }
}
