using Application.Constants;
using Application.Enums;
using Application.Features.EInvoices.Commands;
using Application.Features.EInvoices.Query;
using Application.Features.Invoices.Commands;
using Application.Features.Invoices.Query;
using Application.Features.ManagerPatternEInvoices.Query;
using Application.Features.SupplierEInvoices.Query;
using Application.Hepers;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using Infrastructure.Infrastructure.Identity.Models;
using Infrastructure.Infrastructure.Migrations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using SelectPdf;
using System;
using Telegram.Bot.Types.Payments;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class EInvoiceController : BaseController<EInvoiceController>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly UserManager<ApplicationUser> _userManager;
        public EInvoiceController(UserManager<ApplicationUser> userManager, IOptions<CryptoEngine.Secrets> config)
        {
            _config = config;
            _userManager = userManager;
        }
        [Authorize(Policy = "einvoice.list")]
        public IActionResult Index()
        {
           
            return View();
        }
        [Authorize]
        public async Task<IActionResult> DashboardEinvoice(InvoiceModel model)
        {
            if (model.Date == null)
            {
                model.Date = DateTime.Now;
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            var response = await _mediator.Send(new GetDashboardQuery(currentUser.ComId)
            {
              Date= model.Date
            }); 
            if (!response.Succeeded)
            {
                _logger.LogError(GeneralMess.ConvertStatusToString(response.Message));
                _notify.Error(response.Message);
                return new JsonResult(new { isValid = false });
            }
            return Json(new { isValid = true, data = response.Data });
        }
        [Authorize]

        [EncryptedParameters("secret")]
        public async Task<IActionResult> ViewInvoice(int id)
        {
            if (id == 0)
            {
                _notify.Error("Vui lòng chọn hóa đơn");
                _logger.LogError("Vui lòng chọn hóa đơn");
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            //var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            // getting all templateInvoice data  
            var response = await _mediator.Send(new ViewInvoiceQuery()
            {
                Carsher = currentUser.FullName,
                IdCarsher = currentUser.Id,
                ComId = currentUser.ComId,
                IdEInvoice = id
            });
            if (!response.Succeeded)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(response.Message));
                _logger.LogError(GeneralMess.ConvertStatusToString(response.Message));
            }
            return Json(new { fkey =id,isValid = response.Succeeded,html = response.Data});
        }
        [EncryptedParameters("secret")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ExportXML(int id)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var response = await _mediator.Send(new ViewInvoiceQuery()
            {
                Carsher = currentUser.FullName,
                IdCarsher = currentUser.Id,
                ComId = currentUser.ComId,
                IdEInvoice = id,
                TypeEventEinvoicePosrtal = ENumTypeEventEinvoicePosrtal.ExportXML
            });
            if (!response.Succeeded)
            {
                _logger.LogError(GeneralMess.ConvertStatusToString(response.Message));
                _notify.Error(response.Message);
                return new JsonResult(new { isValid = false });
            }
            return Json(new { isValid = true,data = response.Data });
        }
        [Authorize(Policy = "einvoice.syncInvoice")]

        public async Task<IActionResult> SyncInvoice(int[] lstid, EnumTypeSyncEInvoice type)
        {
            if (lstid.Count()==0)
            {
                _logger.LogError("Vui lòng chọn hóa đơn");
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            //var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            // getting all templateInvoice data  
            var response = await _mediator.Send(new SycnInvoiceCommand()
            {
                Carsher = currentUser.FullName,
                TypeSyncEInvoice = type,
                IdCarsher = currentUser.Id,
                ComId = currentUser.ComId,
              lstId = lstid
            });
            if (response.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("SyncInvoice", response.Data);
                return new JsonResult(new { isValid = true, html = html });
            }
            _notify.Error(response.Message);
            return new JsonResult(new { isValid = false });
        }
        
        [HttpPost]
        [Authorize]
        //  public async Task<FileResult> ExportPDF(int id)
        [EncryptedParameters("secret")]
        public async Task<IActionResult> ExportPDF(int id)
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();

                var response = await _mediator.Send(new ViewInvoiceQuery()
                {
                    Carsher = currentUser.FullName,
                    IdCarsher = currentUser.Id,
                    ComId = currentUser.ComId,
                    IdEInvoice = id,
                    TypeEventEinvoicePosrtal = ENumTypeEventEinvoicePosrtal.ExportPDF
                });
                if (!response.Succeeded)
                {
                    _logger.LogError(GeneralMess.ConvertStatusToString(response.Message));
                    _notify.Error(response.Message);
                    return new JsonResult(new { isValid = false });
                }
                string htmlString = response.Data;
                string baseUrl = "";
                string pdf_page_size = "A4";
                PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                    pdf_page_size, true);

                string pdf_orientation = "Portrait";
                PdfPageOrientation pdfOrientation =
                    (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 954;
                //try
                //{
                //    webPageWidth = Convert.ToInt32(collection["TxtWidth"]);
                //}
                //catch { }

                int webPageHeight = 0;
                //try
                //{
                //    webPageHeight = Convert.ToInt32(collection["TxtHeight"]);
                //}
                //catch { }

                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();

                // set converter options
                converter.Options.MaxPageLoadTime = 120;
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                converter.Options.WebPageFixedSize = false;
                converter.Options.MarginLeft = 1;
                converter.Options.MarginRight = 1;
                converter.Options.MarginTop = 2;
                converter.Options.MarginBottom = 1;
                converter.Options.AutoFitWidth = SelectPdf.HtmlToPdfPageFitMode.ShrinkOnly;
                converter.Options.AutoFitHeight = SelectPdf.HtmlToPdfPageFitMode.NoAdjustment;
                // create a new pdf document converting an url
                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);

                // save pdf document
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();

                string base64 = Convert.ToBase64String(pdf, 0, pdf.Length);
                return new JsonResult(new { isValid = true, base64 = base64 });
                // FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                //fileResult.FileDownloadName = "Document.pdf";
                //  return fileResult;
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false });
            }
            
        }
        public async Task<IActionResult> GetHashTokenAsync(int[] lstid)
        {
            try
            {
                if (lstid.Count() == 0 || lstid == null)
                {
                    _logger.LogError("Vui lòng chọn hóa đơn");
                    return Json(new { isValid = false });
                }
                var currentUser = User.Identity.GetUserClaimLogin();
                var response = await _mediator.Send(new GetHashTokenQuery()
                {
                    ComId = currentUser.ComId,
                    lstid = lstid
                });
                if (response.Succeeded)
                {
                    return new JsonResult(new { isValid = true, hash = response.Data });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(response.Message));
                return new JsonResult(new { isValid = false });
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false });
            }
            
        }
        [Authorize(Policy = "einvoice.SendCQT")]
        public async Task<IActionResult> SendCQT(int[] lstid)
        {
            if (lstid.Count()==0)
            {
                _notify.Error("Vui lòng chọn hóa đơn");
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            var response = await _mediator.Send(new SendCQTCommand()
            {
                Carsher = currentUser.FullName,
                IdCarsher = currentUser.Id,
                ComId = currentUser.ComId,
              lstId = lstid
            });
            if (response.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("SendCQT", response.Data);
                return new JsonResult(new { isValid = true, html = html });
            }
            _notify.Error(response.Message);
            return new JsonResult(new { isValid = false });
        }
        [Authorize(Policy = "einvoice.SendCQT")]
        public async Task<IActionResult> SendCQTToken(string data,int[] lstid)
        {
            if (string.IsNullOrEmpty(data) || lstid == null)
            {
                _notify.Error("Vui lòng chọn hóa đơn");
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            var response = await _mediator.Send(new SendCQTCommand()
            {
                TypeSeri = ENumTypeSeri.TOKEN,
                Carsher = currentUser.FullName,
                IdCarsher = currentUser.Id,
                ComId = currentUser.ComId,
                lstId = lstid,
                dataSign = data,
            }) ;
            if (response.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("SendCQT", response.Data);
                return new JsonResult(new { isValid = true, html = html });
            }
            _notify.Error(response.Message);
            return new JsonResult(new { isValid = false });
        }
        [Authorize(Policy = "einvoice.cancel")]
        public async Task<IActionResult> CancelEInvoiceAsync(int[] lstid)
        {
            if (lstid.Count()==0)
            {
                _notify.Error("Vui lòng chọn hóa đơn");
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            //var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            // getting all templateInvoice data  
            var response = await _mediator.Send(new CancelEInvoiceCommand()
            {
              ComId = currentUser.ComId,
              lstId = lstid
            });
            if (response.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("CancelEInvoice", response.Data);
                return new JsonResult(new { isValid = true, html = html });
            }
            _notify.Error(response.Message);
            return new JsonResult(new { isValid = false });
        }
        public async Task<IActionResult> PrintMutiEInvoiceAsync(int[] lstid, ENumTypePrint typePrint)
        {
            if (lstid.Count()==0)
            {
                _logger.LogError("Vui lòng chọn hóa đơn");
                return Json(new { isValid = false });
            }else if (lstid.Count()>20)
            {
                _logger.LogError("Số lượng hóa đơn chọn không được vượt quá 20 hóa đơn!");
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            //var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            // getting all templateInvoice data  
            var response = await _mediator.Send(new ViewInvoiceQuery()
            {
                Carsher = currentUser.FullName,
                IdCarsher = currentUser.Id,
                ComId = currentUser.ComId,
                typePrint = typePrint,
                lstid = lstid,
                TypeEventEinvoicePosrtal = ENumTypeEventEinvoicePosrtal.Print
            });
            if (!response.Succeeded)
            {
                _notify.Error(response.Message);
                _logger.LogError(GeneralMess.ConvertStatusToString(response.Message));
            }
            return Json(new { isValid = response.Succeeded, html = response.Data,mess= response.Message });
        }
         [EncryptedParameters("secret")]
        public async Task<IActionResult> PrintInvoice(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Vui lòng chọn hóa đơn");
                return Json(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            //var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            // getting all templateInvoice data  
            var response = await _mediator.Send(new ViewInvoiceQuery()
            {
                Carsher = currentUser.FullName,
                IdCarsher = currentUser.Id,
                ComId = currentUser.ComId,
                IdEInvoice = id,
                TypeEventEinvoicePosrtal = ENumTypeEventEinvoicePosrtal.Print
            });
            if (!response.Succeeded)
            {
                _logger.LogError(GeneralMess.ConvertStatusToString(response.Message));
            }
            return Json(new { isValid = response.Succeeded,html = response.Data});
        }
        [Authorize(Policy = "einvoice.delete")]
        public async Task<IActionResult> RemoveEInvoice(int[] lstid)
        {
            if (lstid.Count() == 0)
            {
                _notify.Error("Vui lòng chọn đơn");
                return new JsonResult(new { isValid = false });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            //var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            // getting all templateInvoice data  
            var response = await _mediator.Send(new RemoveEInvoiceCommand()
            {
              ComId= currentUser.ComId,
                lstId= lstid,
                Carsher = currentUser.FullName,
                IdCarsher = currentUser.Id,
            });
            if (!response.Succeeded)
            {
                _logger.LogError(GeneralMess.ConvertStatusToString(response.Message));
                return new JsonResult(new { isValid = false });
            }
            var html = await _viewRenderer.RenderViewToStringAsync("RemoveEInvoice", response.Data);
            return new JsonResult(new { isValid = true, html = html });
        }
        [HttpPost]
        [Authorize(Policy = "invoice.publishinvoice")]
        public async Task<IActionResult> PublishEInvoiceAsync(int[] lstid)
        {
            try
            {
                if (lstid.Count() == 0)
                {
                    _notify.Error("Vui lòng chọn đơn");
                    return new JsonResult(new { isValid = false });
                }
                var currentUser = User.Identity.GetUserClaimLogin();
                //var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                var send = await _mediator.Send(new PublishEInvoiceCommand()
                {
                    Carsher = currentUser.FullName,
                    IdCarsher = currentUser.Id,
                    lstId = lstid,
                    ComId = currentUser.ComId,
                    TypeSupplierEInvoice = ENumSupplierEInvoice.VNPT,
                    TypeEventInvoice = EnumTypeEventInvoice.PublishEInvoice
                });
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
                _logger.LogError("PublishEInvoiceAsync");
                _logger.LogError(e.ToString());
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false });
            }

        }
        public async Task<IActionResult> LoadAll(InvoiceModel model)
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
                int currentPage = skip>=0? skip / pageSize:0;
                model.Currentpage = currentPage+1;
               // var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                var currentUser = User.Identity.GetUserClaimLogin();
                // getting all templateInvoice data  
                var response = await _mediator.Send(new GetAllEInvoiceQuery(currentUser.ComId)
                {
                    EInvoiceModel = model,
                    TypeProduct = currentUser.IdDichVu,
                    sortColumn = sortColumn,
                    sortColumnDirection = sortColumnDirection,
                    currentPage = currentPage,
                    pageSize = pageSize,
                    skip = skip
                });
                if (response.Succeeded)
                {
                    return Json(new { draw = draw, recordsFiltered = response.Data.TotalItemCount, recordsTotal = response.Data.TotalItemCount, data = response.Data.Items });
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
    }
}
