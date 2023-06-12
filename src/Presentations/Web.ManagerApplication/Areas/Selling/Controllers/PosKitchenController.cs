using Application.Constants;
using Application.Enums;
using Application.Features.KitchenPos.Querys;
using Application.Features.Kitchens.Commands;
using Application.Features.Kitchens.Querys;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.ViewModel;
using Infrastructure.Infrastructure.HubS;
using Domain.Identity;
using Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.ManagerApplication.Abstractions;
using Spire.Doc;
using HelperLibrary;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class PosKitchenController : BaseController<PosKitchenController>
    {
        private SignalRHub dashboardHub;
        private readonly INotifyChitkenRepository _notifyChitkenRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private IOptions<CryptoEngine.Secrets> _config;
        public PosKitchenController(UserManager<ApplicationUser> userManager, SignalRHub _dashboardHub,
            INotifyChitkenRepository notifyChitkenRepository,
            IOptions<CryptoEngine.Secrets> config)
        {
            dashboardHub = _dashboardHub;
            _userManager = userManager;
            _notifyChitkenRepository = notifyChitkenRepository;
            _config = config;
        }
        [Authorize(Policy = "posKitchen.order")]
        public IActionResult IndexAsync()
        {
            return View(new KitChenModel());
        } 
        [Authorize(Policy = "posKitchen.order")]
        public IActionResult KitchenIndexAsync()
        {
            return View(new KitChenModel());
        }
        [Authorize]
        public async Task<IActionResult> GetInfoByIdAsync(int? Id)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            if (Id == null)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR012));
                return new JsonResult(new
                {
                    isValid = false
                });
            }
            var getAll = await _mediator.Send(new GetKitchenQuery() { Comid = currentUser.ComId,IdKitken=Id.Value});
            if (getAll.Succeeded)
            {
                return new JsonResult(new
                {
                    isValid = true,
                    data = getAll.Data
                });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(getAll.Message));
            return new JsonResult(new
            {
                isValid = false
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetFoodDataByRoomAsync()
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var getAll = await _mediator.Send(new GetOrderChitkenTableQuery() { Comid = currentUser.ComId, Status = EnumStatusKitchenOrder.MOI });
            if (getAll.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("ChitkenByRoom", getAll.Data);
                return new JsonResult(new
                {
                    isValid = true,
                    data = html
                });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(getAll.Message));
            return new JsonResult(new
            {
                isValid = false
            });


        }
        [HttpGet]
        public async Task<IActionResult> GetFoodConfirmationAsync(Guid? idorder)
        {
            if (idorder==null)
            {
                _notify.Error("Đơn đã bị xóa, hoặc đã thanh toán");
                return new JsonResult(new
                {
                    isValid = false
                });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            var getAll = await _mediator.Send(new GetFoodConfirmationOrderQuery() { Comid = currentUser.ComId,idOrder = idorder.Value, Status = EnumStatusKitchenOrder.MOI });
            if (getAll.Succeeded)
            {
                if (getAll.Data.Count()==0)
                {
                    _notify.Warning("Bàn chưa có món cần chế biến");
                    return new JsonResult(new
                    {
                        isValid = false
                    });
                }
                var html = await _viewRenderer.RenderViewToStringAsync("_FoodConfirmationOrder", getAll.Data);
                return new JsonResult(new
                {
                    isValid = true,
                    data = html
                });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(getAll.Message));
            return new JsonResult(new
            {
                isValid = false
            });
        }
        [HttpGet]
        public async Task<IActionResult> DataFoodNewAsync()
        {
            var currentUser = User.Identity.GetUserClaimLogin(); 
            var getAll = await _mediator.Send(new GetOrderChitkenQuery() { Comid = currentUser.ComId, Status = EnumStatusKitchenOrder.MOI, OrderByDateReady = true });
            if (getAll.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_OrderChitkenNew", getAll.Data);
                return new JsonResult(new
                {
                    isValid = true,
                    data = html
                });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(getAll.Message));
            return new JsonResult(new
            {
                isValid = false
            });
        }
        [HttpGet]
       // [Authorize(Policy = "posKitchen.order")]
        public async Task<IActionResult> DataFoodReadyAsync()
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var getAll = await _mediator.Send(new GetOrderChitkenQuery() { Comid = currentUser.ComId, Status = EnumStatusKitchenOrder.READY, OrderByDateReady = true });
            if (getAll.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_OrderChitkenReady", getAll.Data);
                return new JsonResult(new
                {
                    isValid = true,
                    data = html
                });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(getAll.Message));
            return new JsonResult(new
            {
                isValid = false
            });
        }
   

        public async Task<IActionResult>  GetKitkenCancel(int?[] lstIdItemOrder, decimal Quantity)
        {
            if (lstIdItemOrder == null ||  lstIdItemOrder.Count()==0)
            {
                _notify.Error(GeneralMess.ConvertStatusToString("Vui lòng chọn món!"));
                return Json(new { isValid = false });
            } 
            if (Quantity <= 0)
            {
                _notify.Error(GeneralMess.ConvertStatusToString("Số lượng phải lớn hơn 0"));
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            var getdata = await _mediator.Send(new GetKitchenListQuery() { IsCancel=true, Comid = currentUser.ComId, lstIdItemOrder = lstIdItemOrder });
            if (getdata.Succeeded)
            {
                if (getdata.Data.Count() == 0)
                {
                   // _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR012));
                    return Json(new { isValid = true, notityBar = false });
                }
                decimal _quant = getdata.Data.Sum(x=>x.Quantity);
                if (_quant< Quantity)
                {
                    Quantity = _quant;

                }

                var json = getdata.Data.DistinctBy(x=>x.IdItemOrder).Select(x => new {
                    Id = x.Id,
                    Quantity = Quantity.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                    Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    Name = x.ProName,
                    Cashername = x.Cashername,
                    RoomTableName = x.RoomTableName,
                    Note = x.Note,
                    StaffName = currentUser.FullName,
                    IdStaffName = currentUser.Id,
                }).ToList();
                await dashboardHub.StaffAlertBep(currentUser.ComId, ConvertSupport.ConverObjectToJsonString(json), EnumTypeNotifyKitchenBar.CANCEL);
                return Json(new { isValid = true, notityBar = true });
            }
            return Json(new { isValid = false, notityBar = false });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatusFoodInStaffAsync(NotifyKitChenModel model)//cho nhân viên phục vụ
        {
           try
            {
                var today = DateTime.Now;
                var currentUser = User.Identity.GetUserClaimLogin();
                if (model.IsCancel)//danfh cho hury mons
                {
                    var getdata = await _mediator.Send(new GetKitchenListQuery() { Comid=currentUser.ComId,lstId=model.lstIdChiken});
                    if (getdata.Succeeded)
                    {
                        if (getdata.Data.Count()==0)
                        {
                            _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR012));
                            return Json(new { isValid = false, notityBar = false });
                        }
                      
                        var json= getdata.Data.Select(x=>new {Id = x.Id,
                            Name =x.ProName,
                            Cashername = x.Cashername,
                            RoomTableName = x.RoomTableName,
                            Note = x.Note,
                            StaffName= currentUser.FullName,
                            IdStaffName= currentUser.Id,
                            Date = today.ToString("dd/MM/yyyy HH:mm:ss")
                        });
                        await dashboardHub.StaffAlertBep(currentUser.ComId, ConvertSupport.ConverObjectToJsonString(json), EnumTypeNotifyKitchenBar.CANCEL);
                        return Json(new { isValid = true, notityBar = true });
                    }
                    return Json(new { isValid = false, notityBar = false });
                }
                else
                { //dành cho done món
                    model.ComId = currentUser.ComId;
                    model.Cashername = currentUser.FullName;
                   // var map = _mapper.Map<StaffUpdateFoodCommand>(model);
                    var getAll = await _mediator.Send(new StaffUpdateFoodCommand() {
                        ComId = currentUser.ComId,
                        Cashername = currentUser.FullName,
                        lstIdChiken = model.lstIdChiken });
                    if (getAll.Succeeded)
                    {
                        var json = getAll.Data.Select(x => new {
                            Id = x.Id,
                            Name = x.ProName,
                            Cashername = x.Cashername,
                            RoomTableName = x.RoomTableName,
                            Note = x.Note,
                            StaffName = currentUser.FullName,
                            IdStaffName = currentUser.Id,
                            Date = today.ToString("dd/MM/yyyy HH:mm:ss")
                        });
                        await dashboardHub.StaffAlertBep(currentUser.ComId, ConvertSupport.ConverObjectToJsonString(json),EnumTypeNotifyKitchenBar.DONE);
                        _notify.Success(GeneralMess.ConvertStatusToString(getAll.Message));
                        return Json(new { isValid = true });
                    }
                    _notify.Error(GeneralMess.ConvertStatusToString(getAll.Message));
                    return Json(new { isValid = false });
                }
                
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return Json(new { isValid = false });
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateStatusFoodInStaffKitchenAsync(NotifyKitChenModel model)//cho nhân viên bếp
        {
           try {

                var currentUser = User.Identity.GetUserClaimLogin();
                model.ComId = currentUser.ComId;
                model.Cashername = currentUser.FullName;
                var map = _mapper.Map<StaffUpdateFoodCommand>(model);
                var getAll = await _mediator.Send(map);
                if (getAll.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(getAll.Message));
                    return Json(new { isValid = true });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(getAll.Message));
                return Json(new { isValid = false });
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return Json(new { isValid = false });
            }
        }
         [HttpPost]
        public async Task<IActionResult> UpdateProcessingFoodAsync(NotifyKitChenModel model)//nhận món làm hoặc done món từ bếp màn hình nhà bếp 2
        {
            try
            {
               
                var currentUser = User.Identity.GetUserClaimLogin();
                model.ComId = currentUser.ComId;
                model.Cashername = currentUser.FullName;
                model.TypeNotifyKitChen = EnumTypeNotifyKitChen.NHA_BEP_2;//LOẠI NHÀ BẾP2
                var map = _mapper.Map<UpdateNotifyChitkenCommand>(model);
                var getAll = await _mediator.Send(map);
                if (getAll.Succeeded)
                {
                    if (model.TypeNotifyKitchenOrder==EnumTypeNotifyKitchenOrder.Processing)
                    {
                        if (!string.IsNullOrEmpty(model.IdStaff))
                        {
                            var json = new
                            {
                                Id = getAll.Data.IdOrderItem,
                                ProName = getAll.Data.ProName,
                                RoomTableName = getAll.Data.RoomTableName,
                                IsProgress = !model.IsProgress,
                            };
                            await dashboardHub.FeedbackBepToStaff(model.IdStaff, ConvertSupport.ConverObjectToJsonString(json),4);
                        }
                    }
                   
                    _notify.Success(GeneralMess.ConvertStatusToString(getAll.Message));
                    return Json(new { isValid = true });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(getAll.Message));
                return Json(new { isValid = false });
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return Json(new { isValid = false });
            }

        }

        [HttpPost]
        public async Task<IActionResult> NotifyOrderOrocessedAsync(NotifyKitChenModel model)
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();
                model.ComId = currentUser.ComId;
                var map = _mapper.Map<UpdateNotifyChitkenCommand>(model);
                var getAll = await _mediator.Send(map);
                if (getAll.Succeeded)
                {
                    await dashboardHub.sendNotifyPos(EnumTypeSignalRHub.CHITKEN, EnumTypeSignalRHub.CHITKEN);//báo cho màn hình bếp update lại
                    _notify.Success(GeneralMess.ConvertStatusToString(getAll.Message));
                    return Json(new { isValid = true });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(getAll.Message));
                return Json(new { isValid = false });
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return Json(new { isValid = false });
            }

        }
        [HttpPost]
        public async Task<IActionResult> DeleteKitchenAsync(NotifyKitChenModel model)
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();
                model.ComId = currentUser.ComId;
                var map = _mapper.Map<UpdateNotifyChitkenCommand>(model);
                var getAll = await _mediator.Send(map);
                if (getAll.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(getAll.Message));
                    return Json(new { isValid = true });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(getAll.Message));
                return Json(new { isValid = false });
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return Json(new { isValid = false });
            }

        }
    }
}
