﻿using Application.Constants;
using Application.Enums;
using Application.Features.ConfigSystems.Query;
using Application.Features.Invoices.Query;
using Application.Features.Kitchens.Commands;
using Application.Features.OrderTablePos.Commands;
using Application.Features.OrderTablePos.Querys;
using Application.Features.OrderTables.Commands;
using Application.Features.RoomAndTables.Query;
using Application.Features.SaleRetails.Commands;
using Application.Features.SupplierEInvoices.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.Entities;
using Domain.Identity;
using Domain.ViewModel;
using Infrastructure.Infrastructure.HubS;
using Domain.Identity;
using Infrastructure.Infrastructure.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.Web;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Areas.Selling.Models;
using Library;
using HelperLibrary;
using System.Drawing.Drawing2D;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class OrderTableController : BaseController<OrderTableController>
    {
        private SignalRHub dashboardHub;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPaymentMethodRepository _payment;
        public OrderTableController(UserManager<ApplicationUser> userManager, SignalRHub _dashboardHub,IPaymentMethodRepository payment)
        {
            _payment = payment;
            dashboardHub = _dashboardHub;
            _userManager = userManager;
        }
        [Authorize(Policy = "pos.order")]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> LoadDataOrderStaff(string idtable,Guid? idOrder=null)
        {
            //sự kiện kích thêm oder trên bàn, sự kiện thêm khách hàng,
            bool IsBringBack = false;
            Guid? idGuidtable = null;
            var currentUser = User.Identity.GetUserClaimLogin();
            if (idtable == "-1")
            {
                IsBringBack = true;
            }
            else
            {
                idGuidtable = Guid.Parse(idtable);
            }
            var update = await _mediator.Send(new GetByIdOrderTableQuery() {IdOrder= idOrder, TypeProduct = EnumTypeProduct.AMTHUC, IdRoomAndTable = idGuidtable, IncludeItem = true, IsBringBack = IsBringBack, Comid = currentUser.ComId }); ;
            if (update.Succeeded)
            {
                OrderTableViewModel orderTableViewModel = new OrderTableViewModel();
                orderTableViewModel.OrderTableCode = update.Message;
                orderTableViewModel.OrderTable = update.Data.FirstOrDefault();
                var html = await _viewRenderer.RenderViewToStringAsync("_TabLoadOrderStaff", orderTableViewModel);
                List<CustomerModelViewPos> customerModelViewPos = new List<CustomerModelViewPos>();
                List<ListNoteOrderModelViewPos> ListNoteOrderModelViewPos = new List<ListNoteOrderModelViewPos>();
                if (update.Data.Count() > 0)
                {
                   // customerModelViewPos.AddRange(update.Data.Where(x => x.Customer != null).Select(x => new CustomerModelViewPos() { IdOrder = x.IdGuid, CustomerCode = x.Customer.Code, CustomerName = x.Customer.Name }));
                    ListNoteOrderModelViewPos.AddRange(update.Data.Where(x => !string.IsNullOrEmpty(x.Note)).Select(x => new ListNoteOrderModelViewPos() { IdOrder = x.IdGuid, Note = x.Note }));

                }
                return new JsonResult(new
                {
                    isValid = true,
                    data = html,
                    active = orderTableViewModel.OrderTable!=null ? true : false,
                    dataCus = customerModelViewPos,
                    dataNote = ListNoteOrderModelViewPos
                });
                //return PartialView("_TabLoadOrder", update.Data);
            }
            _notify.Error(update.Message);
            return new JsonResult(new { isValid = false });
        }
        //[Authorize(Policy = "pos.order")]
    
     
        public async Task<IActionResult> LoadDataOrder(string idtable,Guid? idorder)
        {
            //sự kiện kích thêm oder trên bàn, sự kiện thêm khách hàng,
            bool IsBringBack = false;
            Guid? idGuidtable = null;
            var currentUser = User.Identity.GetUserClaimLogin();
            if (idtable == "-1")
            {
                IsBringBack = true;
            }
            else
            {
                idGuidtable = Guid.Parse(idtable);

            }

            var update = await _mediator.Send(new GetByIdOrderTableQuery() { TypeProduct = EnumTypeProduct.AMTHUC,
                IdOrder = idorder, 
                IdRoomAndTable = idGuidtable, 
                IncludeItem = true,
                IsBringBack = IsBringBack,
                Comid = currentUser.ComId }); ;
            if (update.Succeeded)
            {
                OrderTableViewModel orderTableViewModel = new OrderTableViewModel();
                orderTableViewModel.OrderTableCode = update.Message;
                orderTableViewModel.OrderTables = update.Data;
                var html = await _viewRenderer.RenderViewToStringAsync("_TabLoadOrder", orderTableViewModel);
                List<CustomerModelViewPos> customerModelViewPos = new List<CustomerModelViewPos>();
                List<ListNoteOrderModelViewPos> ListNoteOrderModelViewPos = new List<ListNoteOrderModelViewPos>();
                if (update.Data.Count() > 0)
                {
                    customerModelViewPos.AddRange(update.Data.Where(x => x.Customer != null).Select(x => new CustomerModelViewPos() { IdOrder = x.IdGuid, CustomerCode = x.Customer.Code, CustomerName = x.Customer.Name }));
                    ListNoteOrderModelViewPos.AddRange(update.Data.Where(x => !string.IsNullOrEmpty(x.Note)).Select(x => new ListNoteOrderModelViewPos() { IdOrder = x.IdGuid, Note = x.Note }));

                }
                return new JsonResult(new
                {
                    isValid = true,
                    data = html,
                    //idStaff = orderTableViewModel.OrderTables.FirstOrDefault()?.IdStaff,
                    active = orderTableViewModel.OrderTables.Count() > 0 ? true : false,
                    dataCus = customerModelViewPos,
                    dataNote = ListNoteOrderModelViewPos
                });
                //return PartialView("_TabLoadOrder", update.Data);
            }
            _notify.Error(update.Message);
            return new JsonResult(new { isValid = false });
        }
        public async Task<IActionResult> LoadDataByOrderInvoice(Guid idinvoice)
        {
            //sự kiện kích thêm oder trên bàn, sự kiện thêm khách hàng,
            bool IsBringBack = false;
            Guid? idGuidtable = null;
            var currentUser = User.Identity.GetUserClaimLogin();

            var update = await _mediator.Send(new GetByIdOrderTableQuery() { TypeProduct = EnumTypeProduct.BAN_LE, idinvoice = idinvoice, IdRoomAndTable = idGuidtable, IncludeItem = true, IsBringBack = false, Comid = currentUser.ComId }); ;
            if (update.Succeeded)
            {
                OrderTableViewModel orderTableViewModel = new OrderTableViewModel();
                orderTableViewModel.OrderTableCode = update.Message;
                orderTableViewModel.OrderTable = update.Data.SingleOrDefault();
                var html = await _viewRenderer.RenderViewToStringAsync("_TabLoadInvoiceOrder", orderTableViewModel);
                CustomerModelViewPos customerModelViewPos = new CustomerModelViewPos();
                ListNoteOrderModelViewPos ListNoteOrderModelViewPos = new ListNoteOrderModelViewPos();
                decimal Amount = 0;
                decimal Quantity = 0;
                if (orderTableViewModel.OrderTable != null)
                {
                    Amount = orderTableViewModel.OrderTable.Amonut;
                    Quantity = orderTableViewModel.OrderTable.Quantity;
                    if (orderTableViewModel.OrderTable.Customer != null)
                    {
                        customerModelViewPos.IdCustomer = orderTableViewModel.OrderTable.Customer.Id;
                        customerModelViewPos.CustomerCode = orderTableViewModel.OrderTable.Customer.Code;
                        customerModelViewPos.CustomerName = orderTableViewModel.OrderTable.Customer.Name;
                        customerModelViewPos.IdOrder = orderTableViewModel.OrderTable.IdGuid;
                        customerModelViewPos.IdCustomer = orderTableViewModel.OrderTable.Customer.Id;
                    }

                    ListNoteOrderModelViewPos.Note = orderTableViewModel.OrderTable.Note;
                    ListNoteOrderModelViewPos.IdOrder = orderTableViewModel.OrderTable.IdGuid;

                }
                string cn = "en-US"; //Vietnamese
                var _cultureInfo = new CultureInfo(cn);
                return new JsonResult(new
                {
                    amount = Amount,
                    quantity = Quantity,
                    isValid = true,
                    data = html,
                    active = orderTableViewModel.OrderTables.Count() > 0 ? true : false,
                    dataCus = customerModelViewPos,
                    dataNote = ListNoteOrderModelViewPos
                });
                //return PartialView("_TabLoadOrder", update.Data);
            }
            _notify.Error(update.Message);
            return new JsonResult(new { isValid = false });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStaffOrder(Guid? idtable,string idStaff)
        {
            if (idtable==null)
            {
                _notify.Error("Lỗi không tìm thấy đơn");
                return Json(new { isValid = false });
            }
            if (string.IsNullOrEmpty(idStaff))
            {
                _notify.Error("Vui lòng chọn nhân viên");
                return Json(new { isValid = false });
            }
            OrderTableModel model = new OrderTableModel();

            var currentUser = User.Identity.GetUserClaimLogin();
            model.ComId = currentUser.ComId;
            model.IdGuid = idtable.Value;
            model.IdCasher = idStaff;//dùng IdCasher sài nhé
           // var user = await _userManager.FindByIdAsync(idStaff);
            var user =  _userManager.Users.SingleOrDefault(x=>x.ComId==currentUser.ComId&&x.Id== idStaff);
            if (user==null)
            {
                _notify.Error("Lỗi không tìm thấy nhân viên");
                return Json(new { isValid = false });
            }
            model.CasherName = user.FullName;//dùng IdCasher sài nhé
            model.TypeUpdate = EnumTypeUpdatePos.UpdateStaffOrder;
            var ipdateOrderTableCommand = _mapper.Map<UpdateOrderTableCommand>(model);
            var UpdateQuantity = await _mediator.Send(ipdateOrderTableCommand);
            if (UpdateQuantity.Succeeded)
            {
                _notify.Success(UpdateQuantity.Message);
                return Json(new { isValid = true });
            }
            else
            {
                _notify.Error(UpdateQuantity.Message);
                return Json(new { isValid = false });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddOrderTableAsync(OrderTableModel model)
        {
           
            if (!string.IsNullOrEmpty(model.QuantityFloat))
            {
                string cn = "en-US"; //Vietnamese
                var _cultureInfo = new CultureInfo(cn);
                model.Quantity = decimal.Parse(model.QuantityFloat, _cultureInfo);
            }
            else 
            {
                _notify.Error("Số lượng không hợp lệ");
                return Json(new { isValid = false });
            }
            if (!model.IsBringBack && model.IdRoomAndTableGuid==null)
            {
                _notify.Error("Dữ liệu truyền vào không hợp lệ, vui lòng không hack, bàn hoặc mang về phải được chọn");
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            model.ComId = currentUser.ComId;
            model.IdCasher = currentUser.Id;
            model.CasherName = currentUser.FullName;
            // model.TypeProduct = currentUser.IdDichVu;
            model.TypeProduct = EnumTypeProduct.AMTHUC;

            if (model.TypeUpdate == EnumTypeUpdatePos.UpdateQuantity 
                || model.TypeUpdate == EnumTypeUpdatePos.CloneItemOrder 
                || model.TypeUpdate == EnumTypeUpdatePos.RemoveRowItem|| 
                model.TypeUpdate == EnumTypeUpdatePos.ReplaceQuantity)
            {
                if (model.IdGuid == null || model.IdOrderItem == null)
                {
                    _notify.Error("Dữ liệu truyền vào không hợp lệ, vui lòng không hack");
                    return Json(new { isValid = false });
                }
                if (model.TypeUpdate == EnumTypeUpdatePos.UpdateQuantity && model.Quantity == 0)
                {
                    _notify.Error("Số lượng không hợp lệ");
                    return Json(new { isValid = false });
                }
                else  if (model.TypeUpdate == EnumTypeUpdatePos.ReplaceQuantity && model.Quantity == 0)
                {
                    _notify.Error("Số lượng không hợp lệ");
                    return Json(new { isValid = false });
                } 
                else if (model.TypeUpdate == EnumTypeUpdatePos.CloneItemOrder && model.Quantity == 0)
                {
                    _notify.Error("Số lượng không hợp lệ");
                    return Json(new { isValid = false });
                }

                var ipdateOrderTableCommand = _mapper.Map<UpdateOrderTableCommand>(model);
                var UpdateQuantity = await _mediator.Send(ipdateOrderTableCommand);
                if (UpdateQuantity.Succeeded)
                {
                    //------------------báo cho các clinet khác cập nhật dataoder realtime
                    try
                    {
                        if (model.IsCancel)//nếu xd có hủy món khi đã báo bếp rồi thì sẽ update màn hình bếp
                        {
                            await dashboardHub.sendNotifyPos(EnumTypeSignalRHub.CHITKEN, EnumTypeSignalRHub.CHITKEN);//update
                        }
                       
                        await dashboardHub.LoadOrdertable(EnumTypePrint.RealtimeOrder, JsonConvert.SerializeObject(UpdateQuantity.Data));
                    }
                    catch (Exception e)
                    {

                        _logger.LogError(e.ToString());
                    }
                   
                    if (!string.IsNullOrEmpty(UpdateQuantity.Data.HtmlPrint))
                    {
                        //---------------------------báo bếp--------------------------------
                        try
                        {
                            var _send = await _mediator.Send(new GetByKeyConfigSystemQuery(EnumConfigParameters.PRINT_BAO_BEP.ToString()) { ComId = currentUser.ComId });
                            if (_send.Succeeded)
                            {
                                bool getvalue = Convert.ToBoolean(_send.Data.Value);
                                if (getvalue)
                                {
                                    var checkconfig = _send.Data.ConfigSystems.SingleOrDefault(x => x.Key == EnumConfigParameters.PRINT_KET_NOI.ToString());
                                    if (checkconfig != null)
                                    {
                                        if (Convert.ToBoolean(checkconfig.Value))
                                        {
                                            _notify.Success("Báo bếp thành công");
                                            await dashboardHub.PrintbaobepSposViet(currentUser.ComId, UpdateQuantity.Data.HtmlPrint);
                                          
                                            return Json(new { isValid = true, data = JsonConvert.SerializeObject(UpdateQuantity.Data), isbaobep = false });
                                        }
                                    }
                                    //---------------in bếp-------
                                    //return Json(new
                                    //{
                                    //    isValid = true,
                                    //    data = JsonConvert.SerializeObject(UpdateQuantity.Data),
                                    //    isbaobep = true,
                                    //    dataHtml = UpdateQuantity.Data.HtmlPrint
                                    //});
                                    //---------------xử lý gọi ngay chỗ màn báo bếp để in---------
                                    await dashboardHub.Printbaobep(UpdateQuantity.Data.HtmlPrint, "IN");
                                    return Json(new
                                    {
                                        isValid = true,
                                        data = JsonConvert.SerializeObject(UpdateQuantity.Data),
                                        isbaobep = false,
                                        dataHtml = UpdateQuantity.Data.HtmlPrint
                                    });

                                }
                                else
                                {
                                    return Json(new { isValid = true, data = JsonConvert.SerializeObject(UpdateQuantity.Data), isbaobep = false });
                                }
                            }
                            else
                            {
                                return Json(new { isValid = true, data = JsonConvert.SerializeObject(UpdateQuantity.Data), isbaobep = false });
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogInformation("Xử lý báo bếp");
                            _logger.LogError(e.ToString());
                        }
                        //----------------------------
                        
                    }
                    return Json(new { isValid = true, data = JsonConvert.SerializeObject(UpdateQuantity.Data), isbaobep = false });
                }
                _notify.Error(UpdateQuantity.Message);
                return Json(new { isValid = false, mess = UpdateQuantity.Message, isbaobep = false });
            }
            var update = await _mediator.Send(new CreateOrderTableCommand(model));
            if (update.Succeeded)
            {
                try
                {

                    //await dashboardHub.sendNotifyPos(EnumTypeSignalRHub.CHITKEN, EnumTypeSignalRHub.CHITKEN);//báo cho màn hình bếp update lại
                    //------------------báo cho các clinet khác cập nhật dataoder realtime
                    await dashboardHub.LoadOrdertable(EnumTypePrint.RealtimeOrder, JsonConvert.SerializeObject(update.Data));
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
               
                return Json(new { isValid = true, data = JsonConvert.SerializeObject(update.Data) });
            }
            _notify.Error(update.Message);
            return Json(new { isValid = false, mess = update.Message });
        }

        [HttpPost]
        public async Task<IActionResult> AddOrderInvoiceAsync(OrderTableModel model)
        {
            if (!string.IsNullOrEmpty(model.QuantityFloat))
            {
                string cn = "en-US"; //Vietnamese
                var _cultureInfo = new CultureInfo(cn);
                model.Quantity = decimal.Parse(model.QuantityFloat, _cultureInfo);
            }
            else
            {
                _notify.Error("Số lượng không hợp lệ");
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            model.ComId = currentUser.ComId;
            model.IdCasher = currentUser.Id;
            model.CasherName = currentUser.FullName;
            //model.TypeProduct = currentUser.IdDichVu;
            model.TypeProduct = EnumTypeProduct.BAN_LE;

            if (model.TypeUpdate == EnumTypeUpdatePos.UpdateQuantity || model.TypeUpdate == EnumTypeUpdatePos.ReplaceQuantity || model.TypeUpdate == EnumTypeUpdatePos.RemoveRowItem)
            {
                if (model.IdGuid == null || model.IdOrderItem == null)
                {
                    _notify.Error("Dữ liệu truyền vào không hợp lệ, vui lòng không hack");
                    return Json(new { isValid = false });
                }
                if ((model.TypeUpdate == EnumTypeUpdatePos.UpdateQuantity || model.TypeUpdate == EnumTypeUpdatePos.ReplaceQuantity) && model.Quantity == 0)
                {
                    _notify.Error("Số lượng không hợp lệ");
                    return Json(new { isValid = false });
                }

                var ipdateOrderTableCommand = _mapper.Map<UpdateOrderTableCommand>(model);
                var UpdateQuantity = await _mediator.Send(ipdateOrderTableCommand);
                if (UpdateQuantity.Succeeded)
                {
                    return Json(new { isValid = true, data = UpdateQuantity.Data });
                }
                _notify.Error(UpdateQuantity.Message);
                return Json(new { isValid = false, mess = UpdateQuantity.Message });
            }

            else if (model.TypeUpdate == EnumTypeUpdatePos.AddProduct)
            {
                var update = await _mediator.Send(new CreateOrderTableCommand(model));
                if (update.Succeeded)
                {
                    return Json(new { isValid = true, data = update.Data });
                }
                _notify.Error(update.Message);
                return Json(new { isValid = false, mess = update.Message });
            }
            return Json(new { isValid = false, mess = GeneralMess.ConvertStatusToString(HeperConstantss.ERR001) });

        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomOrderTableAsync(OrderTableModel model)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            //var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            model.ComId = currentUser.ComId;
            model.IdCasher = currentUser.Id;
            model.CasherName = currentUser.FullName;
            model.TypeProduct = currentUser.IdDichVu;
            if (model.TypeUpdate == EnumTypeUpdatePos.ChangedCustomer)
            {
                var ipdateOrderTableCommand = _mapper.Map<UpdateOrderTableCommand>(model);
                var UpdateQuantity = await _mediator.Send(ipdateOrderTableCommand);
                if (UpdateQuantity.Succeeded)
                {
                    return Json(new { isValid = true, data = UpdateQuantity.Data.Buyer });
                }
                _notify.Error(UpdateQuantity.Message);
                return Json(new { isValid = false, mess = UpdateQuantity.Message });
            }
            return Json(new { isValid = false, mess = "Không đúng type" });
        }

        public async Task<IActionResult> LoadCheckOrderInTable()//tifm đơn của bàn để active
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            try
            {
                var response = await _mediator.Send(new GetOrderTableAllTableQuery() { Comid = currentUser.ComId,enumTypeProduct=EnumTypeProduct.AMTHUC });
                if (response.Succeeded)
                {
                    return Json(new { isValid = true, data = response.Data });
                }
                _notify.Error("Hệ thống đang lỗi, vui lòng liên hệ admin " + response.Message);
                return Json(new { isValid = false });
            }
            catch (Exception e)
            {
                _logger.LogError(currentUser.ComId + "____" + e.ToString());
                _notify.Error("Hệ thống đang lỗi, vui lòng liên hệ admin <br/>" + e.Message);
                return Json(new { isValid = false });
            }
        }
        [HttpPost]
        public async Task<IActionResult> RemoveOder(EnumTypeUpdatePos TypeUpdate, Guid IdOrder)//xóa đơn
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            OrderTableModel model = new OrderTableModel();
            model.ComId = currentUser.ComId;
            model.TypeUpdate = TypeUpdate;
            model.IdGuid = IdOrder;
            model.CasherName = currentUser.FullName;
            model.IdCasher = currentUser.Id;
            model.TypeProduct = currentUser.IdDichVu;
            var ipdateOrderTableCommand = _mapper.Map<UpdateOrderTableCommand>(model);
            var UpdateQuantity = await _mediator.Send(ipdateOrderTableCommand);
            if (UpdateQuantity.Succeeded)
            {
                //------------------báo cho các clinet khác cập nhật dataoder realtime
                UpdateQuantity.Data.IdGuid = IdOrder;
                UpdateQuantity.Data.TypeUpdate = TypeUpdate;
                await dashboardHub.LoadOrdertable(EnumTypePrint.RealtimeOrder, JsonConvert.SerializeObject(UpdateQuantity.Data));

                if (!string.IsNullOrEmpty(UpdateQuantity.Data.HtmlPrint))
                {
                    await dashboardHub.sendNotifyPos(EnumTypeSignalRHub.CHITKEN, EnumTypeSignalRHub.CHITKEN);//báo cho màn hình bếp update lại
                    //---------------------------báo bếp--------------------------------

                    try
                    {
                        var _send = await _mediator.Send(new GetByKeyConfigSystemQuery(EnumConfigParameters.PRINT_BAO_BEP.ToString()) { ComId = currentUser.ComId });
                        if (_send.Succeeded)
                        {
                            bool getvalue = Convert.ToBoolean(_send.Data.Value);
                            if (getvalue)
                            {
                                var checkconfig = _send.Data.ConfigSystems.SingleOrDefault(x => x.Key == EnumConfigParameters.PRINT_KET_NOI.ToString());
                                if (checkconfig != null)
                                {
                                    if (Convert.ToBoolean(checkconfig.Value))
                                    {
                                       // _notify.Success("Báo bếp thành công");
                                        await dashboardHub.PrintbaobepSposViet(currentUser.ComId, UpdateQuantity.Data.HtmlPrint);

                                        return Json(new { isValid = true, isbaobep = false });
                                    }
                                }
                                return Json(new { isValid = true, isbaobep = true, dataHtml = UpdateQuantity.Data.HtmlPrint });
                            }
                            else
                            {
                                return Json(new { isValid = true,  isbaobep = false });
                            }
                        }
                        else
                        {
                            return Json(new { isValid = true,isbaobep = false });
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("Xử lý báo bếp");
                        _logger.LogError(e.ToString());
                    }
                    //----------------------------

                }

                _notify.Success(GeneralMess.ConvertStatusToString(UpdateQuantity.Message));
                return Json(new { isValid = true });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(UpdateQuantity.Message));
            return Json(new { isValid = false });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveOderInvoice(EnumTypeUpdatePos TypeUpdate, Guid IdOrder)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            OrderTableModel model = new OrderTableModel();
            model.ComId = currentUser.ComId;
            model.TypeUpdate = TypeUpdate;
            model.IdGuid = IdOrder;
            model.CasherName = currentUser.FullName;
            model.IdCasher = currentUser.Id;
            model.TypeProduct = currentUser.IdDichVu;
           // model.TypeProduct = EnumTypeProduct.BAN_LE;
            var ipdateOrderTableCommand = _mapper.Map<UpdateOrderTableCommand>(model);
            var UpdateQuantity = await _mediator.Send(ipdateOrderTableCommand);
            if (UpdateQuantity.Succeeded)
            {
                _notify.Success(GeneralMess.ConvertStatusToString(UpdateQuantity.Message));
                return Json(new { isValid = true });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(UpdateQuantity.Message));
            return Json(new { isValid = false });
        }
        [HttpPost]
        public async Task<IActionResult> AddNote(EnumTypeUpdatePos TypeUpdate, Guid IdOrder, string Note)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            OrderTableModel model = new OrderTableModel();
            model.ComId = currentUser.ComId;
            model.TypeUpdate = TypeUpdate;
            model.IdGuid = IdOrder;
            model.Note = Note;
            var ipdateOrderTableCommand = _mapper.Map<UpdateOrderTableCommand>(model);
            var updatenote = await _mediator.Send(ipdateOrderTableCommand);
            if (updatenote.Succeeded)
            {
                _notify.Success(GeneralMess.ConvertStatusToString(updatenote.Message));
                return Json(new { isValid = true });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(updatenote.Message));
            return Json(new { isValid = false });
        }  
        [HttpPost]
        public async Task<IActionResult> AddNoteAndToppingItemOrder(Guid? IdOrder,Guid? IdOrderItem, string Note) 
        {
            if (IdOrderItem==null || IdOrder==null)
            {
                _notify.Error("Đơn hàng đã bị xóa vui lòng thử lại");
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            OrderTableModel model = new OrderTableModel();
            model.ComId = currentUser.ComId;
            model.TypeUpdate = EnumTypeUpdatePos.UpdateNoteAndTopping;
            model.IdOrderItem = IdOrderItem;
            model.IdGuid = IdOrder ;
            model.Note = Note;
            var ipdateOrderTableCommand = _mapper.Map<UpdateOrderTableCommand>(model);
            var updatenote = await _mediator.Send(ipdateOrderTableCommand);
            if (updatenote.Succeeded)
            {
                _notify.Success(GeneralMess.ConvertStatusToString(updatenote.Message));
                return Json(new { isValid = true });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(updatenote.Message));
            return Json(new { isValid = false });
        }
        public async Task<IActionResult> LoadDataOrderByTableInSplit(Guid? idtable, Guid IdOrder, EnumTypeSpitOrder Type = EnumTypeSpitOrder.Graft, bool IsBringBack = false)
        {
            Guid? IdOrderNotIn = null;
            Guid? IdOrderIn = null;
            if (Type == EnumTypeSpitOrder.Graft) //ghép
            {
                IdOrderNotIn = IdOrder;
                IdOrderIn = null;
            }
            if (Type == EnumTypeSpitOrder.Separate) //tách
            {
                IdOrderNotIn = null;
                IdOrderIn = IdOrder;
            }
            SplitOrderModelView splitOrderModel = new SplitOrderModelView();
            splitOrderModel.TypeSpitOrder = Type;
            var currentUser = User.Identity.GetUserClaimLogin();
            var update = await _mediator.Send(new GetByIdOrderTableQuery() { IdRoomAndTable = idtable, IdOrder = IdOrderIn, IsBringBack = IsBringBack, IdOrderNotIn = IdOrderNotIn, Comid = currentUser.ComId });
            if (update.Succeeded)
            {
                if (Type == EnumTypeSpitOrder.Graft) //ghép
                {
                    splitOrderModel.OrderTables = update.Data;
                }
                if (Type == EnumTypeSpitOrder.Separate) //ghép
                {
                    splitOrderModel.OrderTable = update.Data.SingleOrDefault();
                }


                var htmlTable = await _viewRenderer.RenderViewToStringAsync("_TableInSplitOrder", splitOrderModel);
                return Json(new { isValid = true, html = htmlTable });
            }
            return Json(new { isValid = false });
        }
        public async Task<IActionResult> GetOrderStatusNew(EnumStatusOrderTable Status, Guid NotOrder)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var update = await _mediator.Send(new GetByIdOrderTableQuery()
            {
                IdOrderNotIn = NotOrder,
                Status = Status,
                Comid = currentUser.ComId,
                OutRoom = true
            });
            if (update.Succeeded)
            {
                var dt = update.Data.Select(x => new { id = x.IdGuid, text = $"{x.OrderTableCode} - {(x.RoomAndTable != null ? x.RoomAndTable.Name : x.IsBringBack ? "Mang về" : "Không tìm thấy bàn")}" });

                return Json(new { isValid = true, Json = dt.ToArray() });
            }
            return Json(new { isValid = false });
        }

        [HttpGet]
        public async Task<IActionResult> SplitOrder(EnumTypeUpdatePos TypeUpdate, Guid IdOrder, bool IsBringBack)//tách đơn
        {
            try
            {
                SplitOrderModelView splitOrderModel = new SplitOrderModelView();
                if (TypeUpdate == EnumTypeUpdatePos.SplitOrder)
                {
                    var currentUser = User.Identity.GetUserClaimLogin();
                    var update = await _mediator.Send(new GetByIdOrderTableQuery() { IdOrder = IdOrder, OutRoom = true, IsBringBack = IsBringBack, Comid = currentUser.ComId });

                    splitOrderModel.OrderTable = update.Data.SingleOrDefault();

                    return View(splitOrderModel);
                }
                return View(splitOrderModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> SplitOrder(SplitOrderModel model)//tách đơn
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();

                var dtModel = JsonConvert.DeserializeObject<List<DetailtSpitModel>>(model.json);
                model.lstOrder = dtModel;
                model.CasherName = currentUser.FullName;
                model.IdCasher = currentUser.Id.ToString();
                if (model.TypeUpdate == EnumTypeSpitOrder.Graft || model.TypeUpdate == EnumTypeSpitOrder.Separate)
                {
                    var map = _mapper.Map<SplitOrderCommand>(model);
                    map.ComId = currentUser.ComId;
                    var send = await _mediator.Send(map);
                    if (send.Succeeded)
                    {
                        await dashboardHub.sendNotifyPos(EnumTypeSignalRHub.CHITKEN, EnumTypeSignalRHub.CHITKEN);//tách bàn tách đơn đều báo
                        _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS015));
                        return Json(new { isValid = true });
                    }
                    _notify.Error(GeneralMess.ConvertStatusToString(send.Message));
                    return Json(new { isValid = false });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR044));
                return Json(new { isValid = false });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Json(new { isValid = false });
            }

        }

        [HttpPost]
        public async Task<IActionResult> NotifyKitChen(NotifyKitChenModel model)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            model.ComId = currentUser.ComId;
            model.Cashername = currentUser.FullName;
            model.IdStaff = currentUser.Id;
            var createNotifyChitkenCommand = _mapper.Map<CreateNotifyChitkenCommand>(model);
            var send = await _mediator.Send(createNotifyChitkenCommand);
            if (send.Succeeded)
            {
                if (send.Data=="ERR")
                {
                    _notify.Error(send.Message);
                    return Json(new { isValid = true, isNotPrint = true });
                }
            
                await dashboardHub.sendNotifyPos(EnumTypeSignalRHub.CHITKEN, EnumTypeSignalRHub.CHITKEN);//thông báo màn hình bếp
                //await dashboardHub.PrintbaobepSposViet(currentUser.ComId, send.Data);
                // return Json(new { isValid = true, isNotPrint=true });
                var _send = await _mediator.Send(new GetByKeyConfigSystemQuery(EnumConfigParameters.PRINT_BAO_BEP.ToString()) { ComId = currentUser.ComId });
                if (_send.Succeeded)
                {
                    bool getvalue = Convert.ToBoolean(_send.Data.Value);
                    if (getvalue)
                    {
                        var checkconfig = _send.Data.ConfigSystems.SingleOrDefault(x => x.Key == EnumConfigParameters.PRINT_KET_NOI.ToString());
                        if (checkconfig!=null)
                        {
                            if (Convert.ToBoolean(checkconfig.Value))
                            {
                                _notify.Success("Thông báo bếp thành công!");
                                await dashboardHub.PrintbaobepSposViet(currentUser.ComId, send.Data);
                                 return Json(new { isValid = true, isNotPrint=true });
                            }
                        }
                        _notify.Success("Thông báo bếp thành công!");
                        //return Json(new { isValid = true, html = send.Data, isNotPrint = false }); dành cho khi trả kết quả view rồi tiếp gọi lên lại
                        //---------------xử lý gọi ngay chỗ màn báo bếp để in---------
                        await dashboardHub.Printbaobep(send.Data,"IN");
                        return Json(new { isValid = true, html = send.Data, isNotPrint = true });
                    }
                    else
                    {
                        return Json(new { isValid = true, html = send.Data, isNotPrint = true });
                    }
                }
                else
                {
                    return Json(new { isValid = true, html = send.Data, isNotPrint = true });
                }
            }
            _notify.Error(GeneralMess.ConvertStatusToString(send.Message));
            return Json(new { isValid = false });
        }
        [HttpGet]
        public async Task<IActionResult> Payment(EnumTypeUpdatePos TypeUpdate, Guid IdOrder,bool vat,bool isStopService)
        {
       
            try
            {
                if (TypeUpdate == EnumTypeUpdatePos.Payment)
                {
                    var _orderTable = new OrderTable();
                    var currentUser = User.Identity.GetUserClaimLogin();
                    if (isStopService)//nếu có sản phẩm nào là hàng hóa tính giờ
                    {
                        var get = await _mediator.Send(new UpdateServiceFoodByPaymentCommand()
                        {
                            ComId = currentUser.ComId,
                            IdOrder = IdOrder,
                        });
                        if (get.Succeeded)
                        {
                            _orderTable = get.Data;
                        }
                        else
                        {
                            _notify.Error(GeneralMess.ConvertStatusToString(get.Message));
                            return Json(new { isValid = false });
                        }
                    }
                    else
                    {
                        var get = await _mediator.Send(new GetByIdOrderTableQuery()
                        {
                            TypeProduct = currentUser.IdDichVu,
                            Comid = currentUser.ComId,
                            IdOrder = IdOrder,
                            IsStopService = isStopService,
                            OutInvNo = false,
                            OutRoom = true
                        });
                        if (get.Succeeded)
                        {
                            _orderTable = get.Data?.FirstOrDefault();
                        }
                        else
                        {
                            _notify.Error(GeneralMess.ConvertStatusToString(get.Message));
                            return Json(new { isValid = false });
                        }
                    }
                    PaymentModelView paymentModelView = new PaymentModelView();
                    //---------lấy config có tính chiết khấu sau thuế không
                    var _sendDISCOUNT_PRICE_AFTER_TAX = await _mediator.Send(new GetByKeyConfigSystemQuery(EnumConfigParameters.DISCOUNT_PRICE_AFTER_TAX.ToString()) { ComId = currentUser.ComId });
                    if (_sendDISCOUNT_PRICE_AFTER_TAX.Succeeded)
                    {
                        paymentModelView.IsDiscountAfterTax = (_sendDISCOUNT_PRICE_AFTER_TAX.Data.Value=="true");
                    }
                    //-----------
                    var _send = await _mediator.Send(new GetAllSupplierEInvoiceQuery() { Comid = currentUser.ComId, IsManagerPatternEInvoices = true });
                  
                    paymentModelView.PaymentMethods = _payment.GetAll(currentUser.ComId, true).ToList();
                    paymentModelView.SupplierEInvoiceModel = _send.Data?.FirstOrDefault();
                    paymentModelView.OrderTable = _orderTable;
                    paymentModelView.VatMTT = vat; 

                    if (paymentModelView.OrderTable == null)
                    {
                        _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR043));
                        return Json(new { isValid = false });
                    }
                    if (paymentModelView.OrderTable.OrderTableItems.Where(x => x.IsVAT).Select(x => x.VATRate).Distinct().Count() > 1)
                    {
                        _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR050));
                    }
                    var html = await _viewRenderer.RenderViewToStringAsync("_Payment", paymentModelView);
                    return new JsonResult(new
                    {
                        isValid = true,
                        data = html,
                        IdOrder = IdOrder,
                        title = $"Phiếu thanh toán {paymentModelView.OrderTable.OrderTableCode} {(paymentModelView.OrderTable.RoomAndTable != null ? paymentModelView.OrderTable.RoomAndTable.Name : "")}"
                    });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR001));
                return Json(new { isValid = false });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                _notify.Error(e.Message);
                return Json(new { isValid = false });
            }
           
        }
        [HttpGet]
        public async Task<IActionResult> LoadAllDataOrderAsync()
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var update = await _mediator.Send(new GetAllOrderTableQuery() {Comid= currentUser.ComId });
            if (update.Succeeded)
            {
                return new JsonResult(new
                { 
                    data = ConvertSupport.ConverModelToJson(update.Data),
                    isValid = true,
                });
            }
            return new JsonResult(new
            {
                isValid = false,
            });
        } 
            
            [HttpGet]
        public async Task<IActionResult> GetHistoryOrder(Guid IdOrder)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var update = await _mediator.Send(new GetHistoryQuery() { IdOrder = IdOrder,Comid= currentUser.ComId });
            if (update.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_HistoryOrder", update.Data);
                return new JsonResult(new
                {
                    isValid = true,
                    html = html
                });
            }
            return new JsonResult(new
            {
                isValid = false,
            });
        } 
        [HttpGet]
        public async Task<IActionResult> ChangeTableInOrder(Guid IdOrder,Guid? IdRoomAndTable,bool isbringback = false)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            ChangeTableInOrderModel model = new ChangeTableInOrderModel();

            var gettablle= await _mediator.Send(new GetAllRoomAndTableQuery(currentUser.ComId) { });
            
            if (gettablle.Succeeded)
            {
                model.RoomAndTables = gettablle.Data;
            }
            var update = await _mediator.Send(new GetOrderOneQuery() { IdOrder = IdOrder,IdRoomAndTable= IdRoomAndTable, Comid = currentUser.ComId, TypeProduct = currentUser.IdDichVu });
            if (update.Succeeded)
            {
                model.OrderTable = update.Data;
                var htmldt = await _viewRenderer.RenderViewToStringAsync("_ChangeTableInOrder", model);
                return new JsonResult(new
                {
                    isValid = true,
                    html = htmldt
                });
            }
            return new JsonResult(new
            {
                isValid = false,
            });
        }
        [HttpPost]
        public async Task<IActionResult> ChangeTableInOrder(ChangeTableInOrderModel model)
        {
            var currentUser = User.Identity.GetUserClaimLogin();

            if ((model.TypeSelectTable == 1 && model.IdTable==null) || model.IdOrder == null)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR000));
                return new JsonResult(new
                {
                    isValid = false,
                });
            }
            bool IsBringBack = false;
            if (model.TypeSelectTable==0)
            {
                IsBringBack = true;
            }
            var UpdateQuantity = await _mediator.Send(new UpdateOrderTableCommand()
            {
                ComId= currentUser.ComId,
                IdRoomAndTableGuid = model.IdTable,
                IdGuid = model.IdOrder.Value,
                TypeUpdate = model.TypeUpdate,
                IsBringBack = IsBringBack,
                idOldTableOrder = model.OldIdTable,
            });
            if (UpdateQuantity.Succeeded)
            {
               
                _notify.Success(GeneralMess.ConvertStatusToString(UpdateQuantity.Message));
                return Json(new { isValid = true, 
                    tableName = UpdateQuantity.Data.TableName,
                    quantity = UpdateQuantity.Data.Quantity,
                    idTable = (UpdateQuantity.Data.IsBringBack?"-1": UpdateQuantity.Data.IdRoomAndTableGuid.ToString()),
                    IsBringback = UpdateQuantity.Data.IsBringBack });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(UpdateQuantity.Message));
            return new JsonResult(new
            {
                isValid = false,
            });
        }
        [EncryptedParameters("secret")]
        public async Task<ActionResult> PrintOrderTaleAsync(bool vat, Guid? id,int Vatrate)//in tạm tính
        {
            if (id==null)
            {
                _notify.Error("Chưa có đơn để in!");
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            var response = await _mediator.Send(new PrintOrderTaleQuery() { Comid = currentUser.ComId,VATRate= Vatrate, IdOrder = id ,vat= vat , casherName = currentUser.FullName});
            if (response.Succeeded)
            {
                return Json(new { isValid = true, html = response.Data });
            }
            _notify.Error(response.Message);
            return Json(new { isValid = false });
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePriceItemTableAsync(Guid? IdOrder, Guid? IdItemOrder, decimal Discount, decimal DiscountAmount, decimal Price, decimal PriceAdjust)
        {
         
            if (IdOrder == null)
            {
                _notify.Error("Không tìm thấy đơn!");
                return Json(new { isValid = false });
            }

            var currentUser = User.Identity.GetUserClaimLogin();
            OrderTableModel orderTableModel = new OrderTableModel();
            orderTableModel.TypeUpdate = EnumTypeUpdatePos.UpdatePriceAndDiscountItemOrder;
            orderTableModel.ComId = currentUser.ComId;
            orderTableModel.IdGuid = IdOrder.Value;
            orderTableModel.IdOrderItem = IdItemOrder;
            orderTableModel.PriceOld = PriceAdjust;
            orderTableModel.Price = Price;
            orderTableModel.Discount = Discount;
            orderTableModel.DiscountAmount = DiscountAmount;
            var ipdateOrderTableCommand = _mapper.Map<UpdateOrderTableCommand>(orderTableModel);
            var _send = await _mediator.Send(ipdateOrderTableCommand);
            if (_send.Succeeded)
            {
                _notify.Success("Cập nhật thành công!");
                // tính tổng tiền và giảm giá trên từng món: hàm 
                return Json(new { isValid = true,
                    amount = _send.Data.Amount,
                    quantity = _send.Data.Quantity,
                    isServiceDate = _send.Data.IsServiceDate,
                    price= _send.Data.Price,
                    priceold= _send.Data.PriceOld
                });
            }
            else
            {
                _notify.Error(GeneralMess.ConvertStatusToString(_send.Message));
                return Json(new { isValid = false });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateFoodServiceStatusAsync(int? IdOrder,Guid? IdItemOrder,bool IsTop)
        {
            if (IdOrder == null)
            {
                _notify.Error("Không tìm thấy đơn!");
                return Json(new { isValid = false });
            }

            var currentUser = User.Identity.GetUserClaimLogin();
            OrderTableModel orderTableModel = new OrderTableModel();
            orderTableModel.TypeUpdate = EnumTypeUpdatePos.UpdateStatusFoodService;
            orderTableModel.ComId = currentUser.ComId;
            orderTableModel.IdOrder = IdOrder.Value;
            orderTableModel.IdOrderItem = IdItemOrder;
            orderTableModel.IsCancel = IsTop;
            orderTableModel.DateCreateService = DateTime.Now;//dùng lấy giờ dừng
            var ipdateOrderTableCommand = _mapper.Map<UpdateOrderTableCommand>(orderTableModel);
            var _send= await _mediator.Send(ipdateOrderTableCommand);
            if (_send.Succeeded)
            {
               // tính tổng tiền và giảm giá trên từng món: hàm 
                return Json(new { isValid = true,date= orderTableModel.DateCreateService });
            }
            else
            {
                _notify.Error(GeneralMess.ConvertStatusToString(_send.Message));
                return Json(new { isValid = false });
            }
        } 
        [HttpPost]
        public async Task<IActionResult> UpdateDateTimeFoodServiceAsync(int? IdOrder,Guid? IdItemOrder,bool IsStartDate,string date)
        {
          
            if (IdOrder == null)
            {
                _notify.Error("Không tìm thấy đơn!");
                return Json(new { isValid = false });
            }
            var DateCreateService = LibraryCommon.ConvertStringToDateTime(date, "dd/MM/yyyy HH:mm");
            if (!DateCreateService.HasValue)
            {
                _notify.Error("Thời gian bạn chọn không hợp lệ, vui lòng chọn lại!");
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            OrderTableModel orderTableModel = new OrderTableModel();
            orderTableModel.TypeUpdate = EnumTypeUpdatePos.UpdateDateTimeFoodService;
            orderTableModel.ComId = currentUser.ComId;
            orderTableModel.IdOrder = IdOrder.Value;
            orderTableModel.IdOrderItem = IdItemOrder;
            orderTableModel.IsStartDate = IsStartDate;
            orderTableModel.DateCreateService = DateCreateService;//dùng lấy giờ 
            var ipdateOrderTableCommand = _mapper.Map<UpdateOrderTableCommand>(orderTableModel);
            var _send= await _mediator.Send(ipdateOrderTableCommand);
            if (_send.Succeeded)
            {
                _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                // tính tổng tiền và giảm giá trên từng món: hàm 
                return Json(new { isValid = true,date= orderTableModel.DateCreateService });
            }
            else
            {
                _notify.Error(GeneralMess.ConvertStatusToString(_send.Message));
                return Json(new { isValid = false });
            }
        }
       
        [HttpPost]
        public async Task<IActionResult> PaymentSaleRatailtAsync(string jsonData)
        {
            OrderInvoicePaymentSaleRetailModel model = Common.ConverJsonToModel<OrderInvoicePaymentSaleRetailModel>(jsonData);
            //------- validate xuất hóa đơn
            if (model.VATMTT)
            {
                if (model.IdPattern == null || model.IdPattern == 0)
                {
                    _notify.Error("Vui lòng chọn mẫu số ký hiệu để xuất hóa đơn");
                    return Json(new { isValid = false });
                }
                if (model.VATRate == null || model.VATRate == 0)
                {
                    _notify.Error("Vui lòng chọn thuế suất hóa đơn");
                    return Json(new { isValid = false });
                }
            }
            if (model.IdPaymentMethod == 0)
            {
                _notify.Error("Vui lòng chọn hình thức thanh toán");
                return Json(new { isValid = false });

            }
            if (model == null)
            {
                _notify.Error("Đơn cần thanh toán không hợp lệ");
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            EnumTypeProduct enumType = EnumTypeProduct.BAN_LE;
            model.EnumTypeProduct = enumType;
            model.ComId = currentUser.ComId;
            model.IdCasher = currentUser.Id;
            model.Cashername = currentUser.FullName;

            if (!model.VATMTT)
            {
                model.VATRate = (int)NOVAT.NOVAT;
            }
            var _map = _mapper.Map<CheckOutOrderInvoiceCommand>(model);
            var update = await _mediator.Send(_map);
            if (update.Succeeded)
            {
                _notify.Success(GeneralMess.ConvertStatusToString(update.Message));
                if (string.IsNullOrEmpty(update.Data))
                {
                    _notify.Warning(GeneralMess.ConvertStatusToString(HeperConstantss.ERR048));
                }
                return Json(new { isValid = true, data = HttpUtility.HtmlDecode(update.Data) });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(update.Message));
            return Json(new { isValid = false });
        }
        [HttpPost]
        public async Task<IActionResult> CheckOutOrder(EnumTypeUpdatePos TypeUpdate, Guid? IdOrder,
            decimal discountPayment, decimal discount, decimal discountOther, decimal? cuspayAmount, int Idpayment,bool vat,
            int? Vatrate,int? ManagerPatternEInvoices,decimal VATAmount,decimal Amount,decimal Total)
        {

            //đã thanh toán thành công nhưng khi load lại đơn thì chưa kích vào tab od-new, check chỉ cần kích vào nữa là ok
            if (TypeUpdate == EnumTypeUpdatePos.CheckOutOrder)
            {
                if (Idpayment == 0)
                {
                    _notify.Error("Vui lòng chọn hình thức thanh toán");
                    return Json(new { isValid = false });

                }
                if (IdOrder == null)
                {
                    _notify.Error("Đơn cần thanh toán không hợp lệ");
                    return Json(new { isValid = false });
                }
                var currentUser = User.Identity.GetUserClaimLogin();
                EnumTypeProduct enumType = EnumTypeProduct.AMTHUC;
                if (currentUser.IdDichVu == EnumTypeProduct.BAN_LE || currentUser.IdDichVu == EnumTypeProduct.TAPHOA_SIEUTHI)
                {
                    enumType = EnumTypeProduct.BAN_LE;
                }
             
                var update = await _mediator.Send(new CheckOutOrderCommand()
                {
                    Cashername = currentUser.FullName,
                    IdCasher = currentUser.Id,
                    Idpayment = Idpayment,
                    ComId = currentUser.ComId,
                    TypeUpdate = enumType,
                    IdOrder = IdOrder.Value,
                    discountPayment = discountPayment,
                    discountOther = discountOther,
                    discount = discount,
                    vat = vat,
                    Vatrate = Vatrate,
                    VATAmount = VATAmount,
                    Amount = Amount,
                    Total = Total,
                    ManagerPatternEInvoices = ManagerPatternEInvoices,
                    cuspayAmount = cuspayAmount
                });
                if (update.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(update.Message));
                    if (string.IsNullOrEmpty(update.Data))
                    {
                        _notify.Warning(GeneralMess.ConvertStatusToString(HeperConstantss.ERR048));
                    }
                    return Json(new { isValid = true, data = HttpUtility.HtmlDecode(update.Data) });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(update.Message));

            }
            else
            {
                _notify.Error("Không đúng loại yêu cầu");
            }
            return Json(new { isValid = false });


        }
    }
}
