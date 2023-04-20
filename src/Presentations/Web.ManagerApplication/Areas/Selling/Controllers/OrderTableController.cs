using Application.Constants;
using Application.Enums;
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
using Domain.ViewModel;
using Infrastructure.Infrastructure.Identity.Models;
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

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class OrderTableController : BaseController<OrderTableController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPaymentMethodRepository _payment;
        public OrderTableController(UserManager<ApplicationUser> userManager, IPaymentMethodRepository payment)
        {
            _payment = payment;
            _userManager = userManager;
        }
        [Authorize(Policy = "pos.order")]
        public IActionResult Index()
        {
            return View();
        }

        //[Authorize(Policy = "pos.order")]


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
                orderTableViewModel.OrderTable = update.Data.SingleOrDefault();
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


        public async Task<IActionResult> LoadDataOrder(string idtable)
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
            var update = await _mediator.Send(new GetByIdOrderTableQuery() { TypeProduct = EnumTypeProduct.AMTHUC, IdRoomAndTable = idGuidtable, IncludeItem = true, IsBringBack = IsBringBack, Comid = currentUser.ComId }); ;
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

            if (model.TypeUpdate == EnumTypeUpdatePos.UpdateQuantity || model.TypeUpdate == EnumTypeUpdatePos.RemoveRowItem|| model.TypeUpdate == EnumTypeUpdatePos.ReplaceQuantity)
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
                 if (model.TypeUpdate == EnumTypeUpdatePos.ReplaceQuantity && model.Quantity == 0)
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



            var update = await _mediator.Send(new CreateOrderTableCommand(model));
            if (update.Succeeded)
            {
                return Json(new { isValid = true, data = update.Data });
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
        public async Task<IActionResult> RemoveOder(EnumTypeUpdatePos TypeUpdate, Guid IdOrder)
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
            var createNotifyChitkenCommand = _mapper.Map<CreateNotifyChitkenCommand>(model);
            var send = await _mediator.Send(createNotifyChitkenCommand);
            if (send.Succeeded)
            {
                _notify.Success("Thông báo bếp thành công!");
                return Json(new { isValid = true });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(send.Message));
            return Json(new { isValid = false });
        }

        [HttpGet]
        public async Task<IActionResult> Payment(EnumTypeUpdatePos TypeUpdate, Guid IdOrder,bool vat)
        {
            if (TypeUpdate == EnumTypeUpdatePos.Payment)
            {
                var currentUser = User.Identity.GetUserClaimLogin();
                var update = await _mediator.Send(new GetByIdOrderTableQuery() { TypeProduct = currentUser.IdDichVu, Comid = currentUser.ComId, IdOrder = IdOrder, OutInvNo = false, OutRoom = true });
                if (update.Succeeded)
                {
                    var _send = await _mediator.Send(new GetAllSupplierEInvoiceQuery() { Comid = currentUser.ComId, IsManagerPatternEInvoices = true });
                    PaymentModelView paymentModelView = new PaymentModelView();
                    paymentModelView.PaymentMethods = _payment.GetAll(currentUser.ComId, true).ToList() ;
                    paymentModelView.SupplierEInvoiceModel = _send.Data?.FirstOrDefault();
                    paymentModelView.OrderTable = update.Data.SingleOrDefault();
                    paymentModelView.VatMTT = vat;
                    if (paymentModelView.OrderTable == null)
                    {
                        _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR043));
                        return Json(new { isValid = false });
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
                _notify.Error(GeneralMess.ConvertStatusToString(update.Message));

            }
            return Json(new { isValid = false });
        }
        [HttpGet]
        public async Task<IActionResult> GetHistoryOrder(Guid IdOrder)
        {
          
            var update = await _mediator.Send(new GetHistoryQuery() { IdOrder = IdOrder });
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
                return Json(new { isValid = true, tableName = UpdateQuantity.Data.TableName,idTable = (UpdateQuantity.Data.IsBringBack?"-1": UpdateQuantity.Data.IdRoomAndTableGuid.ToString()),IsBringback = UpdateQuantity.Data.IsBringBack });
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
            var response = await _mediator.Send(new PrintOrderTaleQuery() { Comid = currentUser.ComId,VATRate= Vatrate, IdOrder = id ,vat= vat });
            if (response.Succeeded)
            {
                return Json(new { isValid = true, html = response.Data });
            }
            _notify.Error(response.Message);
            return Json(new { isValid = false });
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
            decimal discountPayment, decimal discount, decimal? cuspayAmount, int Idpayment,bool vat,
            int? Vatrate,int? ManagerPatternEInvoices,decimal VATAmount,decimal Amount)
        {
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
                    discount = discount,
                    vat = vat,
                    Vatrate = Vatrate,
                    VATAmount = VATAmount,
                    Amount = Amount,
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
