using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using HelperLibrary;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.OrderTables.Commands
{
    public partial class UpdateOrderTableCommand : OrderTableModel, IRequest<Result<OrderTableModel>>
    {
        public Guid? idOldTableOrder { get; set; }//idtable old dùng cho change table của đơn

    }
    public class UpdateOrderTableHandler : IRequestHandler<UpdateOrderTableCommand, Result<OrderTableModel>>
    {
        private readonly IRepositoryAsync<Customer> _customerRepository;
        private readonly IOrderTableRepository _orderTableRepository;
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly ILogger<UpdateOrderTableHandler> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly IOrderTableRepository _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateOrderTableHandler(IOrderTableRepository brandRepository,
            IRepositoryAsync<Customer> customerRepository, ILogger<UpdateOrderTableHandler> logger,
             IFormFileHelperRepository fileHelper, IOrderTableRepository orderTableRepository,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _logger = logger;
            _customerRepository = customerRepository;
            _orderTableRepository = orderTableRepository;
            _fileHelper = fileHelper;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<OrderTableModel>> Handle(UpdateOrderTableCommand request, CancellationToken cancellationToken)
        {
            try
            {
                OrderTableModel orderTableModel = new OrderTableModel();
                Result<OrderTable> updatequantity = null;
                Customer customer = null;
                bool isRemoveRow = false;

                switch (request.TypeUpdate)
                {
                    case EnumTypeUpdatePos.UpdateQuantity:
                        updatequantity = await _orderTableRepository.UpdateItemOrderAsync(request.IdCustomer, request.CusCode, request.ComId, request.IdGuid.Value, request.IdOrderItem.Value, request.IdRoomAndTableGuid, request.IsBringBack, request.Quantity, request.CasherName, request.IdCasher, request.Note,  isRemoveRow, request.IsCancel);
                        break;
                    case EnumTypeUpdatePos.UpdateStaffOrder:
                        var up = await _orderTableRepository.UpdateStaffAsync(request.ComId, request.IdGuid.Value, request.IdCasher, request.CasherName);
                        if (up.Succeeded)
                        {
                            return await Result<OrderTableModel>.SuccessAsync(up.Message);
                        }
                        fix lỗi 0.5 hiển thị .5
                        else
                        {
                            return await Result<OrderTableModel>.FailAsync(up.Message);
                        }
                    case EnumTypeUpdatePos.CloneItemOrder:
                        if (request.IdGuid == null || request.IdOrderItem == null)
                        {
                            return await Result<OrderTableModel>.FailAsync("Không tìm thấy dữ liệu phù hợp khi cập nhật thay thế số lượng");
                        }
                        updatequantity = await _orderTableRepository.CloneItemAsync(request.ComId, request.IdGuid.Value, request.IdOrderItem.Value);
                        break;
                    case EnumTypeUpdatePos.ReplaceQuantity:
                        if (request.IdGuid == null || request.IdOrderItem == null)
                        {
                            return await Result<OrderTableModel>.FailAsync("Không tìm thấy dữ liệu phù hợp khi cập nhật thay thế số lượng");
                        }
                        updatequantity = await _orderTableRepository.UpdateAllQuantityOrderTable(request.ComId, request.IdGuid.Value, request.IdOrderItem.Value, request.Quantity);
                        break;
                    case EnumTypeUpdatePos.AddNoteOrder:

                        var addnote = await _orderTableRepository.AddNote(request.ComId, request.IdGuid.Value, request.Note);
                        if (addnote.Failed)
                        {
                            return await Result<OrderTableModel>.FailAsync(addnote.Message);
                        }
                        return await Result<OrderTableModel>.SuccessAsync(addnote.Message);
                    case EnumTypeUpdatePos.UpdateNoteAndTopping:
                        if (request.IdOrderItem == null || request.IdGuid == null)
                        {
                            return await Result<OrderTableModel>.FailAsync("Đơn hàng không tồn tại");
                        }
                        var addnoteandtopping = await _orderTableRepository.AddNoteAndToppingItemOrder(request.ComId,request.IdGuid.Value, request.IdOrderItem.Value, request.Note);
                        if (addnoteandtopping.Failed)
                        {
                            return await Result<OrderTableModel>.FailAsync(addnoteandtopping.Message);
                        }
                        return await Result<OrderTableModel>.SuccessAsync(addnoteandtopping.Message);
                    case EnumTypeUpdatePos.UpdateRoomOrTableInOrder:

                        if (request.IdGuid==null)
                        {
                            return await Result<OrderTableModel>.FailAsync(HeperConstantss.ERR012);
                        }
                        var updatetable = await _orderTableRepository.UpdateTableOrRoomOfOrder(request.ComId, request.IsBringBack,request.IdGuid.Value,request.idOldTableOrder, request.IdRoomAndTableGuid);
                        if (updatetable.Failed)
                        {
                            return await Result<OrderTableModel>.FailAsync(updatetable.Message);
                        }
                        var OrderTableModel = new OrderTableModel()
                        {
                            TableName = updatetable.Data.IsBringBack ? "Mang về" : updatetable.Data.RoomAndTable.Name,
                            IdRoomAndTableGuid = updatetable.Data.RoomAndTable?.IdGuid,
                            IsBringBack = updatetable.Data.IsBringBack,
                            Quantity = updatetable.Data.Quantity,
                        };
                        return await Result<OrderTableModel>.SuccessAsync(OrderTableModel, HeperConstantss.SUS006);
                    case EnumTypeUpdatePos.ConvertInvoice:
                        var updateinvoice = await _orderTableRepository.ConvertInvoice(request.ComId, request.IdGuid.Value, request.TypeProduct);
                        if (updateinvoice.Failed)
                        {
                            return await Result<OrderTableModel>.FailAsync(updateinvoice.Message);
                        }
                        return await Result<OrderTableModel>.SuccessAsync(updateinvoice.Message);

                    case EnumTypeUpdatePos.RemoveRowItem:
                        isRemoveRow = true;
                        updatequantity = await _orderTableRepository.UpdateItemOrderAsync(request.IdCustomer, request.CusCode, request.ComId, request.IdGuid.Value, request.IdOrderItem.Value, request.IdRoomAndTableGuid, request.IsBringBack, request.Quantity, request.CasherName, request.IdCasher, request.Note, isRemoveRow, request.IsCancel);
                        break;
                    case EnumTypeUpdatePos.ChangedCustomer:
                        OrderTableModel orderTable = new OrderTableModel();
                        if (request.TypeProduct == EnumTypeProduct.BAN_LE || request.TypeProduct == EnumTypeProduct.TAPHOA_SIEUTHI)
                        {
                            if (request.IsRemoveCustomer)
                            {
                                var removeCus = await _orderTableRepository.RemoveCustomerOrder(request.ComId, request.IdGuid.Value, request.TypeProduct);
                                if (removeCus.Succeeded)
                                {
                                    return Result<OrderTableModel>.Success(orderTable);
                                }
                                return Result<OrderTableModel>.Fail(removeCus.Message);
                            }
                            var checkCode = await _customerRepository.Entities.Where(x => x.Id == request.IdCustomer && x.Comid == request.ComId).SingleOrDefaultAsync();
                            if (checkCode == null)
                            {
                                return await Result<OrderTableModel>.FailAsync("Không tìm thấy khách hàng phù hợp");
                            }

                            await _orderTableRepository.UpdateCustomerOrder(request.ComId, request.IdGuid.Value, checkCode, request.TypeProduct);
                        }
                        else if (request.TypeProduct == EnumTypeProduct.AMTHUC)
                        {
                            if (!string.IsNullOrEmpty(request.CusCode))
                            {
                                var checkCode = await _customerRepository.Entities.Where(x => x.Code == request.CusCode && x.Comid == request.ComId).SingleOrDefaultAsync();
                                if (checkCode == null)
                                {
                                    return await Result<OrderTableModel>.FailAsync("Không tìm thấy khách hàng phù hợp");
                                }
                                customer = checkCode;
                                orderTable.Buyer = customer.Name;
                            }
                            if (!request.IdGuid.HasValue)
                            {
                                return await Result<OrderTableModel>.FailAsync("Đơn hàng không được bỏ trống khi thay đổi khách hàng");
                            }
                            await _orderTableRepository.UpdateCustomerOrder(request.ComId, request.IdGuid.Value, customer, request.TypeProduct);
                        }
                        else
                        {
                            return await Result<OrderTableModel>.FailAsync("Không tìm thấy trạng thái phù hợp");
                        }
                        return Result<OrderTableModel>.Success(orderTable);

                    case EnumTypeUpdatePos.RemoveOrder:
                        if (request.TypeProduct == EnumTypeProduct.BAN_LE)
                        {
                            var remove = await _orderTableRepository.RemoveOrder(request.ComId, request.IdGuid.Value, request.CasherName, request.IdCasher, request.TypeProduct);
                            if (remove.Failed)
                            {
                                return await Result<OrderTableModel>.FailAsync(HeperConstantss.ERR043);
                            }
                            
                            return await Result<OrderTableModel>.SuccessAsync(HeperConstantss.SUS007);
                        }
                        else
                        {
                            var remove = await _orderTableRepository.RemoveOrder(request.ComId, request.IdGuid.Value, request.CasherName, request.IdCasher, request.TypeProduct);
                            if (remove.Failed)
                            {
                                return await Result<OrderTableModel>.FailAsync(HeperConstantss.ERR043);
                            }
                            orderTableModel.IdRoomAndTableGuid = remove.Data.IdRoomAndTableGuid;
                            orderTableModel.IsBringBack = remove.Data.IsBringBack;
                            orderTableModel.IdGuid = request.IdGuid.Value;
                            // kèm báo bếp
                            if (remove.Data.NotifyOrderNewModels != null)
                            {
                                var genhtml = await _orderTableRepository.GenHtmlPrintBep(remove.Data.NotifyOrderNewModels, request.ComId);
                                if (genhtml.Succeeded)
                                {
                                    if (!string.IsNullOrEmpty(genhtml.Data))
                                    {
                                        orderTableModel.HtmlPrint = genhtml.Data;
                                    }
                                }
                            }
                            return await Result<OrderTableModel>.SuccessAsync(orderTableModel,HeperConstantss.SUS007);
                        }

                    case EnumTypeUpdatePos.Unknown:
                        return await Result<OrderTableModel>.FailAsync("Không đúng loại cập nhật");

                }


                if (updatequantity.Failed)
                {
                    return await Result<OrderTableModel>.FailAsync(updatequantity.Message);
                }
                
                orderTableModel = request;
                orderTableModel.IdGuid = updatequantity.Data.IdGuid;
                orderTableModel.CreateDate = updatequantity.Data.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss");
                orderTableModel.IdRoomAndTableGuid = request.IdRoomAndTableGuid;
                orderTableModel.OrderCode = updatequantity.Data.OrderTableCode;
                orderTableModel.IsBringBack = updatequantity.Data.IsBringBack;
                orderTableModel.IdOrder = updatequantity.Data.Id;
                orderTableModel.Buyer = updatequantity.Data.Buyer;
                orderTableModel.Amount = updatequantity.Data.Amonut;
                orderTableModel.Quantity = updatequantity.Data.OrderTableItems.Sum(x => x.Quantity);
                orderTableModel.OrderTableItems.AddRange(updatequantity.Data.OrderTableItems.OrderBy(x => x.Id).Select(x => new OrderTableItemModel() { Code = x.Code, Id = x.Id, IdGuid = x.IdGuid, IdProduct = x.IdProduct, Price = x.Price, Quantity = x.Quantity, QuantityNotifyKitchen = x.QuantityNotifyKitchen, IdOrderTable = x.IdOrderTable, Total = x.Amount, Name = x.Name , Note = x.Note, IsVAT = x.IsVAT }));
                // kèm báo bếp
                if (updatequantity.Data.NotifyOrderNewModels != null)
                {
                    var genhtml = await _orderTableRepository.GenHtmlPrintBep(updatequantity.Data.NotifyOrderNewModels, request.ComId);
                    if (genhtml.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(genhtml.Data))
                        {
                            orderTableModel.HtmlPrint=genhtml.Data;
                        }
                    }
                   
                }
                return Result<OrderTableModel>.Success(orderTableModel);

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
