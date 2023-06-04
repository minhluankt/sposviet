using Application.Constants;
using Application.Enums;
using Application.Features.Areas.Commands;
using Application.Features.Areas.Query;
using Application.Features.Invoices.Commands;
using Application.Features.Products.Query;
using Application.Features.PromotionRuns.Query;
using Application.Features.PurchaseOrders.Commands;
using Application.Features.PurchaseOrders.Query;
using Application.Hepers;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using Domain.Identity;
using Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Model;
using Newtonsoft.Json.Linq;
using SystemVariable;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Areas.Selling.Models;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class PurchaseOrderController : BaseController<PurchaseOrderController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public PurchaseOrderController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [Authorize(Policy = "purchaseOrder.list")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Policy = "purchaseOrder.list")]
        public IActionResult IndexReturn()
        {
            return View();
        }
        [EncryptedParameters("secret")]
      
        public async Task<ActionResult> ViewAsync(int id, EnumTypePurchaseOrder Type)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var response = await _mediator.Send(new GetByIdPurchaseOrderQuery(currentUser.ComId) { TypePurchaseOrder = Type,Id=id });
            if (response.Succeeded)
            {
                string view = "_Detail";
                if (Type==EnumTypePurchaseOrder.TRA_HANG_NHAP)
                {
                    view = "_DetailReturn";
                }
                var html = await _viewRenderer.RenderViewToStringAsync(view, response.Data);
                return new JsonResult(new {
                    isValid = true,
                    html = html,
                    idData = response.Data.Id,
                    idSuppliers = (response.Data.IdSuppliers==null?0: response.Data.IdSuppliers),
                    idPayment = (response.Data.IdPayment==null?0: response.Data.IdPayment),
                    purchaseOrderCode = response.Data.PurchaseOrderCode });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(response.Message));
            return Json(new { isValid = false });
        }
        [HttpPost]
        public async Task<IActionResult> LoadAll(EnumTypePurchaseOrder Type,string code)
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                // Skip number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();

                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction (asc, desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;

                int skip = start != null ? Convert.ToInt32(start) : 0;

                int recordsTotal = 0;

                var currentUser = User.Identity.GetUserClaimLogin();
                // getting all Customer data  
                var response = await _mediator.Send(new GetAllPurchaseOrderQuery(currentUser.ComId)
                {   Comid= currentUser.ComId,
                    Code = code,
                    Type = Type,
                    TypeProduct = currentUser.IdDichVu,
                    sortColumn = sortColumn,
                    sortColumnDirection = sortColumnDirection,
                    pageSize = pageSize,
                    skip = skip
                }) ;
                if (response.Succeeded)
                {
                   
                    return Json(new { draw = draw, recordsFiltered = response.Data.Count, recordsTotal = response.Data.Count, data = response.Data.Data });
                }

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = "" });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }

        }
        [Authorize(Policy = "purchaseOrder.create")]

        public ActionResult Create()
        {
            _logger.LogInformation(User.Identity.Name + "-->PurchaseOrder  CreateOrEdit");
            return View();
        } 
        public async Task<IActionResult> PurchaseReturns(int? id)
        {
            PurchaseReturnsModel purchaseReturnsModel = new PurchaseReturnsModel();
            _logger.LogInformation(User.Identity.Name + "-->PurchaseOrder  PurchaseReturns");
            if (id!=null)
            {
                var currentUser = User.Identity.GetUserClaimLogin();
                var response = await _mediator.Send(new GetByIdPurchaseOrderQuery(currentUser.ComId) { TypePurchaseOrder = EnumTypePurchaseOrder.NHAP_HANG, Id = id.Value });
                if (response.Succeeded)
                {
                    
                    purchaseReturnsModel.PurchaseOrder = response.Data;
                    if (response.Data!=null)
                    {
                        var responsePurchaseOrder = await _mediator.Send(new GetByCodePurchaseOrderQuery(currentUser.ComId) { TypePurchaseOrder = EnumTypePurchaseOrder.TRA_HANG_NHAP, Code = response.Data.PurchaseNo });//kiểm tra bên trả hàng có trả hàng chưa
                        if (responsePurchaseOrder.Succeeded)
                        {
                            purchaseReturnsModel.PurchaseOrderItems = responsePurchaseOrder.Data.SelectMany(x=>x.ItemPurchaseOrders).GroupBy(x=>x.Code).Select(x=> new PurchaseOrderItemModel()
                            {
                                Code = x.Key,
                                Name = x.First().Name,
                                Quantity = x.Sum(x=>x.Quantity),
                                Price = x.First().Price,
                                Total = x.Sum(x => x.Total),
                                Discount = x.Sum(x => x.Discount),
                                DiscountAmount = x.Sum(x => x.DiscountAmount),
                                Unit = x.First().Unit
                            }).ToList();
                            List<ItemPurchaseOrder> lstremove= new List<ItemPurchaseOrder>();
                            foreach (var item in purchaseReturnsModel.PurchaseOrder.ItemPurchaseOrders)
                            {
                                var checkcode = purchaseReturnsModel.PurchaseOrderItems.SingleOrDefault(x=>x.Code==item.Code);
                                if (checkcode!=null)
                                {
                                    if (checkcode.Quantity >= item.Quantity)
                                    {
                                        purchaseReturnsModel.PurchaseOrder.ItemPurchaseOrders.Remove(item);
                                    }
                                    else
                                    {
                                        item.Quantity = item.Quantity - checkcode.Quantity;
                                    }
                                }   
                             
                            }
                        }
                    }
                    return View(purchaseReturnsModel);
                }
            }
            return View(purchaseReturnsModel);
        }
        [HttpPost] 
        public async Task<IActionResult> Create(PurchaseOrderModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.JsonItem))
                {
                    _notify.Error("Chưa có hàng hóa cần nhập hàng");
                    return Json(new { isValid = false });
                }
                var items = ConvertSupport.ConverJsonToModel<List<ItemPurchaseOrder>>(model.JsonItem);
                if (items.Count()==0)
                {
                    _notify.Error("Chưa có hàng hóa cần nhập hàng 2");
                    return Json(new { isValid = false });
                }
                var currentUser = User.Identity.GetUserClaimLogin();
                var map = new CreatePurchaseOrderCommand()
                {
                    IdPayment = model.IdPayment,
                    IdSuppliers = model.IdSuppliers,
                    Status = model.Status,
                    Comid = currentUser.ComId,
                    Total = model.Total,
                    DisCountAmount = model.DiscountAmount,
                    AmountSuppliers = model.AmountSuppliers,
                    Amount = model.Amount,
                    DebtAmount = model.DebtAmount,
                    Quantity = model.Quantity,
                    Note = model.Note,
                    Type = model.Type,
                    Carsher = currentUser.FullName,
                    IdPurchaseOrder = model.IdPurchaseOrder,
                    CreateDate = model.CreateDate!=null? model.CreateDate.Value:DateTime.Now,
                    ItemPurchaseOrders = items
                };
                

                var _send = await _mediator.Send(map);
                if (_send.Succeeded)
                {
                    _logger.LogInformation(User.Identity.Name + "-->PurchaseOrder  CreateOrEdit");
                    return Json(new { isValid = true });
                }
                _notify.Error(_send.Message);
                _logger.LogInformation(User.Identity.Name + "-->PurchaseOrder  CreateOrEdit");
                return Json(new { isValid = false });
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return Json(new { isValid = false });
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> Update(PurchaseOrderModel model)//dành cho xem chi tiết nhập hàng update lại thôi nhé
        {
            try
            {
                if (model.CreateDate==null)
                {
                    _notify.Error("Vui lòng chọn ngày trả hàng");
                    return Json(new { isValid = false });
                }
                var currentUser = User.Identity.GetUserClaimLogin();
                var map = new UpdatePurchaseOrderCommand()
                {
                    Id = model.Id,
                    IdPayment = model.IdPayment,
                    IdSuppliers = model.IdSuppliers,
                    Comid = currentUser.ComId,
                    Note = model.Note,
                    Type = model.Type,
                    Carsher = currentUser.FullName,
                    CreateDate = model.CreateDate.Value
                };

                var _send = await _mediator.Send(map);
                if (_send.Succeeded)
                {
                    _logger.LogInformation(User.Identity.Name + "-->PurchaseOrder  Update");
                    return Json(new { isValid = true });
                }
                _notify.Error(_send.Message);
                _logger.LogInformation(User.Identity.Name + "-->PurchaseOrder  Update");
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
