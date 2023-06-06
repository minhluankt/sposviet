using Application.Constants;
using Application.Enums;
using Application.Features.ConfigSystems.Query;
using Application.Features.Invoices.Commands;
using Application.Features.Invoices.Query;
using Application.Features.SupplierEInvoices.Query;
using Application.Hepers;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using Hangfire.MemoryStorage.Database;
using Domain.Identity;
using Infrastructure.Infrastructure.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Drawing.Drawing2D;
using Web.ManagerApplication.Abstractions;
using System.Reactive.Joins;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class InvoiceController : BaseController<InvoiceController>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly UserManager<ApplicationUser> _userManager;
        public InvoiceController(UserManager<ApplicationUser> userManager, IOptions<CryptoEngine.Secrets> config)
        {
            _config = config;
            _userManager = userManager;
        }
        [Authorize(Policy = "invoice.list")]
        public IActionResult IndexAsync()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetConfigSellInvoiceAsync()
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var _send = await _mediator.Send(new GetByKeyConfigSystemQuery(EnumConfigParameters.DELETEINVOICENOPAYMENT.ToString()) { ComId = currentUser.ComId });
            if (_send.Succeeded)
            {
                return new JsonResult(new { isValid = true, data = _send.Data });
            }
            return new JsonResult(new { isValid = false });
        }
       
        public async Task<IActionResult> SuppliersEInvoice(int SaleRetail=0)
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();
                //var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                var _send = await _mediator.Send(new GetAllSupplierEInvoiceQuery() { Comid = currentUser.ComId, IsManagerPatternEInvoices = true });

                if (_send.Succeeded)
                {
                    _send.Data.ForEach(x => x.SaleRetail = SaleRetail);
                    var html = await _viewRenderer.RenderViewToStringAsync("SuppliersEInvoice", _send.Data);
                    return new JsonResult(new { isValid = true, html = html, nodata = _send.Data.Count() > 0 });
                }
                _notify.Error("Chưa cấu hình hóa đơn điện tử");
                return new JsonResult(new { isValid = false });
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false });
            }
        }
     
        [HttpPost]
        public async Task<IActionResult> PublishEInvoiceMergeAsync(PublishInvoiceMergeModel model)
        {
            try
            {
                model.TypeSupplierEInvoice = ENumSupplierEInvoice.VNPT;
                if (string.IsNullOrEmpty(model.Buyer) && string.IsNullOrEmpty(model.CusName) && !model.IsRetailCustomer)
                {
                    _notify.Error("Tên người mua hoặc tên đơn vị bạn phải nhập một trong hai.");
                    return new JsonResult(new { isValid = false });
                }
                var currentUser = User.Identity.GetUserClaimLogin();

                if (model.ManagerPatternEInvoices == 0)
                {
                    _notify.Error("Vui lòng chọn mẫu số ký hiệu");
                    return new JsonResult(new { isValid = false });
                }
                if (model.IsCreateCustomer && string.IsNullOrEmpty(model.CusCode))
                {
                    _notify.Error("Để tạo khách hàng vào hệ thống, vui lòng điền mã khách hàng, mã khách hàng không được trùng trong hệ thống");
                    return new JsonResult(new { isValid = false });
                }
                if (model.VATRate == null)
                {
                    _notify.Error("Vui lòng chọn thuế xuất");
                    return new JsonResult(new { isValid = false });
                }
                model.IdCasher = currentUser.Id;
                model.CasherName = currentUser.FullName;
                model.TypeProduct = currentUser.IdDichVu;
                var _map = _mapper.Map<PublishEInvoiceMergeCommand>(model);
                _map.ComId = currentUser.ComId;
                var send = await _mediator.Send(_map);
                if (send.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("PublishEInvoice", send.Data);
                    return new JsonResult(new { isValid = true, html = html });
                }
                _notify.Error(send.Message);
                return new JsonResult(new { isValid = false });
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false });
            }
            
        }
        [Authorize(Policy = "invoice.publishEInvoiceMerge")]
        [HttpGet]
        public async Task<IActionResult> PublishEInvoiceMergeAsync(int[] lstid,bool IsDelete)
        {
            if (lstid==null || lstid.Count() == 0)
            {
                _notify.Error("Vui lòng chọn đơn");
                return new JsonResult(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            PublishEInvoiceMergeModel publishEInvoiceMergeModel = new PublishEInvoiceMergeModel();
            publishEInvoiceMergeModel.IsDelete = IsDelete;  
            var getinvoice = await _mediator.Send(new GetInvoiceArrayQuery() { ComId = currentUser.ComId,LstIdInvoice= lstid });
            if (getinvoice.Succeeded)
            {
                publishEInvoiceMergeModel.LstInvoiceCode = getinvoice.Message.Split(",");
                publishEInvoiceMergeModel.Invoice = getinvoice.Data;
                var _send = await _mediator.Send(new GetAllSupplierEInvoiceQuery() { Comid = currentUser.ComId, IsManagerPatternEInvoices = true });
                if (_send.Succeeded)
                {
                    publishEInvoiceMergeModel.SupplierEInvoice = _send.Data.FirstOrDefault();
                    if (publishEInvoiceMergeModel.SupplierEInvoice.ManagerPatternEInvoices.Count()==0)
                    {
                        _notify.Error("Chưa cấu hình mẫu số, ký hiệu hóa đơn, không thể phát hành!");
                        return new JsonResult(new { isValid = false });
                    }
                }
                else
                {
                    _notify.Error(_send.Message);
                    return new JsonResult(new { isValid = false });
                }
                var html = await _viewRenderer.RenderViewToStringAsync("PublishEInvoiceMerge", publishEInvoiceMergeModel);
                return new JsonResult(new { isValid = true, html = html });
            }
            _notify.Error(getinvoice.Message);
            return new JsonResult(new { isValid = false });
        }
        [HttpPost]
        [Authorize(Policy = "invoice.publishinvoice")]
        public async Task<IActionResult> PublishEInvoiceTokenAsync(string serialCert,string serial, string pattern, string dataxml)
        {
            if (string.IsNullOrEmpty(serialCert) || string.IsNullOrEmpty(serial)|| string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(dataxml))
            {
                _notify.Error("Dữ liệu truyền vào đã bị thay đổi");
                return new JsonResult(new { isValid = false });
            }

            var currentUser = User.Identity.GetUserClaimLogin();
            var send = await _mediator.Send(new PublishInvoiceCommand()
            {
                CasherName = currentUser.FullName,
                IdCarsher = currentUser.Id,
                pattern = pattern,
                serial = serial,
                serialCert = serialCert,
                dataxmlhash = dataxml,
                TypeSupplierEInvoice = ENumSupplierEInvoice.VNPT,
                TypeEventInvoice = EnumTypeEventInvoice.PublishEInvoiceTokenByHash
            });
            
            _notify.Error(send.Message);
            return new JsonResult(new { isValid = false });
        }
        [HttpPost]
        [Authorize(Policy = "invoice.publishinvoice")]
        public async Task<IActionResult> PublishEInvoiceAsync(int[] lstid, EnumTypeEventInvoice TypeEventInvoice,float? Vatrate,int idPattern)
        {
            try
            {
                if (TypeEventInvoice!=EnumTypeEventInvoice.PublishEInvoice && TypeEventInvoice != EnumTypeEventInvoice.CreateEInvoice)
                {
                    _notify.Error("Trạng thái không hợp lệ");
                    return new JsonResult(new { isValid = false });
                }
                if (lstid.Count() == 0)
                {
                    _notify.Error("Vui lòng chọn đơn");
                    return new JsonResult(new { isValid = false });
                }
                if (idPattern == 0)
                {
                    _notify.Error("Vui lòng chọn mẫu số ký hiệu");
                    return new JsonResult(new { isValid = false });
                }
                if (Vatrate == null)
                {
                    _notify.Error("Vui lòng chọn thuế xuất");
                    return new JsonResult(new { isValid = false });
                }
                var currentUser = User.Identity.GetUserClaimLogin();
                var send = await _mediator.Send(new PublishInvoiceCommand()
                {
                    CasherName = currentUser.FullName,
                    IdCarsher = currentUser.Id,
                    lstId = lstid,
                    VATRate = Vatrate.Value,
                    IdManagerPatternEInvoice = idPattern,
                    ComId = currentUser.ComId,
                    TypeSupplierEInvoice = ENumSupplierEInvoice.VNPT,
                    TypeEventInvoice = TypeEventInvoice
                });
                if (send.Succeeded)
                {
                    if (send.Data.TypeEventInvoice==EnumTypeEventInvoice.IsGetHashPublishEInvoice)
                    {
                        return new JsonResult(new { 
                            isValid = true,
                            data = send.Data.XmlByHashValue,
                            isGetHashToken = true ,
                            typeSupplierEInvoice = (int)send.Data.TypeSupplierEInvoice,
                            pattern = send.Data.Pattern ,
                            serialCert = send.Data.SerialCert , serial = send.Data.Serial});
                    }
                    var html = await _viewRenderer.RenderViewToStringAsync("PublishEInvoice", send.Data);
                    return new JsonResult(new { isValid = true,html = html, isGetHashToken = false });
                }
                _notify.Error(send.Message);
                return new JsonResult(new { isValid = false });
            }
            catch (Exception e)
            {
                _logger.LogError("PublishEInvoiceAsync");
                _logger.LogError(e.ToString());
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false });
            }

        }
        [EncryptedParameters("secret")]
        public async Task<IActionResult> CloneOrder(Guid? id)
        {
            if (id==null)
            {
                _notify.Error("Không tìm thấy hóa đơn!");
                return Json(new { isValid = false });
            }
            else
            {
                var currentUser = User.Identity.GetUserClaimLogin();
                var response = await _mediator.Send(new CloneOrderCommand() { 
                    ComId = currentUser.ComId,
                    Id = id,
                    CasherName= currentUser.FullName,
                    IdCasherName= currentUser.Id});
                if (response.Succeeded)
                {
                    _notify.Success("Sao chép thành công");
                    return Json(new { isValid = true,idTabel = response.Data.IdRoomAndTableGuid,idOder = response.Data.IdGuid });
                }
                _notify.Error($"{response.Message}");
                return new JsonResult(new { isValid = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoadAll(InvoiceModel model)
        {
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {
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
                int currentPage = skip >= 0 ? skip / pageSize : 0;
                model.Currentpage = currentPage + 1;
                // getting all Customer data
                var response = await _mediator.Send(new GetAllInvoiceQuery(currentUser.ComId)
                {
                    invoiceModel = model,
                    TypeProduct = currentUser.IdDichVu,
                    sortColumn = sortColumn,
                    sortColumnDirection = sortColumnDirection,
                    pageSize = pageSize,
                    skip = skip
                });
                if (response.Succeeded)
                {
                    return Json(new { draw = draw, recordsFiltered = response.Data.TotalItemCount, totalAmount = response.Data.TotalAmount, recordsTotal = response.Data.TotalItemCount, data = response.Data.Items });
                }
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = "" });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _notify.Error(ex.ToString());
                return Json(new { draw = draw, recordsFiltered = 0, recordsTotal = 0, data = "" });
            }

        }
        [EncryptedParameters("secret")]
        public async Task<ActionResult> DetailsAsync(Guid id)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var response = await _mediator.Send(new GetByIdInvoiceQuery() { ComId = currentUser.ComId, Id = id });
            if (response.Succeeded)
            {
                ViewBag.ListInvoice = null;
                if (response.Data.IsMerge && string.IsNullOrEmpty(response.Data.InvoiceCodePatern))//là thèn cha mới có, hóa đơn cho gộp
                {
                    var getisgop = await _mediator.Send(new GetInvoiceByCodeQuery() { ComId = currentUser.ComId, InvoiceCodePatern = response.Data.InvoiceCode });
                    if (getisgop.Succeeded)
                    {
                        ViewBag.ListInvoice = getisgop.Data.Select(x => x.InvoiceCode);
                    }
                }
                response.Data.Secret = CryptoEngine.Encrypt("id=" + response.Data.IdGuid, _config.Value.Key);
                return View(response.Data);
            }
            return View(new Invoice());
        }

        [EncryptedParameters("secret")]
        public async Task<ActionResult> PrintInvoiceAsync(Guid id)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var response = await _mediator.Send(new PrintInvoicePos() { ComId = currentUser.ComId, Id = id, IncludeRoomAndTable=true });
            if (response.Succeeded)
            {
                return Json(new { isValid = true, html = response.Data });
            }
            _notify.Error(response.Message);
            return Json(new { isValid = false });
        } 
        public async Task<ActionResult> GetInvoiceByDayAsync()
        {
           
            return Json(new { isValid = false });
        }
        [HttpPost]
        [EncryptedParameters("secret")]
        [Authorize(Policy = "invoice.cancel")]
        public async Task<ActionResult> CancelInvoiceAsync(Guid id, EnumTypeEventInvoice TypeEventInvoice,string Note, bool IsDeletePT = false)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            if (string.IsNullOrEmpty(Note))
            {
                _notify.Error("Vui lòng nhập lý do");
            }
            var response = await _mediator.Send(new UpdateInvoiceCommand() { CasherName= currentUser.FullName,
                IsDeletePT = IsDeletePT,
                ComId = currentUser.ComId, Id = id, Note= Note, TypeEventInvoice = TypeEventInvoice });
            if (response.Succeeded)
            {
                _notify.Success("Hủy bỏ thành công: " + response.Message);
                return Json(new { isValid = true });
            }
           
            _notify.Error(GeneralMess.ConvertStatusToString(response.Message));
            return Json(new { isValid = false });
        }
        [HttpPost]
        [EncryptedParameters("secret")]
        [Authorize(Policy = "invoice.deleteIsMerge")]
        public async Task<ActionResult> DeleteIsMergeAsync(Guid? id)
        {
            if (id==null)
            {
                _notify.Error("Không tìm thấy dữ liệu đầu vào");
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            var response = await _mediator.Send(new UpdateInvoiceCommand() { 
                CasherName= currentUser.FullName,
                ComId = currentUser.ComId, 
                Id = id,
                TypeEventInvoice = EnumTypeEventInvoice.DeleteIsMerge });
            if (response.Succeeded)
            {
                _notify.Success("Hủy bỏ thành công: " + response.Message);
                return Json(new { isValid = true });
            }
           
            _notify.Error(GeneralMess.ConvertStatusToString(response.Message));
            return Json(new { isValid = false });
        }
        
        [HttpPost]
        public async Task<ActionResult> UpdateCustomerAsync(Guid? id,int? idCus)
        {
            if (idCus == null)
            {
                _notify.Error("Dữ liệu khách hàng không hợp lệ");
                return Json(new { isValid = false });
            }
            if (id == null)
            {
                _notify.Error("Đơn hàng không tồn tại");
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            var response = await _mediator.Send(new UpdateInvoiceCommand() { 
                CasherName= currentUser.FullName,
                ComId = currentUser.ComId, 
                Id = id,
                IdCustomer = idCus,
                TypeEventInvoice = EnumTypeEventInvoice.UpdateCustomer });
            if (response.Succeeded)
            {
                _notify.Success(response.Message);
                return Json(new { isValid = true,note = response.Data.Note });
            }
           
            _notify.Error(GeneralMess.ConvertStatusToString(response.Message));
            return Json(new { isValid = false });
        }

        [Authorize(Policy = "invoice.cancel")]
        [HttpPost]
        public async Task<ActionResult> CancelInvoiceListAsync(int[] lstid, string Note, bool IsDeletePT = false)//IsDeletePT có xóa phiếu thu liên quan không
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            if (string.IsNullOrEmpty(Note))
            {
                _notify.Error("Vui lòng nhập lý do");
                return Json(new { isValid = false });
            }
            var response = await _mediator.Send(new UpdateInvoiceCommand() { CasherName= currentUser.FullName, IsDeletePT= IsDeletePT, ComId = currentUser.ComId, lstid= lstid, Note= Note, TypeEventInvoice = EnumTypeEventInvoice.Cancel });
            if (response.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("CancelInvoice", response.Data);
                return new JsonResult(new { isValid = true, html = html });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(response.Message));
            return Json(new { isValid = false });
        }
        [Authorize(Policy = "invoice.delete")]
        [HttpPost]
        public async Task<ActionResult> DeleteInvoiceListAsync(int[] lstid, string Note,bool IsDelete=false)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            if (string.IsNullOrEmpty(Note))
            {
                _notify.Error("Vui lòng nhập lý do");
                return Json(new { isValid = false });
            }
            var response = await _mediator.Send(new UpdateInvoiceCommand() {
                CasherName= currentUser.FullName, 
                ComId = currentUser.ComId,
                lstid= lstid, Note= Note,
                TypeEventInvoice = EnumTypeEventInvoice.Delete,
                IsDelete=IsDelete });
            if (response.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("CancelInvoice", response.Data);
                return new JsonResult(new { isValid = true, html = html });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(response.Message));
            return Json(new { isValid = false });
        }
    }
}
