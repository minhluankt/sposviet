using Application.Constants;
using Application.Enums;
using Application.Features.Customers.Commands;
using Application.Features.Customers.Query;
using Application.Features.Kitchens.Commands;
using Application.Features.OrderTablePos.Commands;
using Application.Features.OrderTablePos.Querys;
using Application.Features.OrderTables.Commands;
using Application.Features.PaymentMethods.Commands;
using Application.Features.PaymentMethods.Query;
using Application.Features.TemplateInvoices.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using HelperLibrary;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Model;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Globalization;
using System.Web;
using SystemVariable;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Areas.Selling.Models;
using Web.ManagerApplication.Extensions;
using static OfficeOpenXml.ExcelErrorValue;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class PaymentmethodController : BaseController<PosController>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IFormFileHelperRepository _fileHelper;
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        public PaymentmethodController(UserManager<ApplicationUser> userManager,
            IOptions<CryptoEngine.Secrets> config,
            IFormFileHelperRepository fileHelper,
            IHostingEnvironment hostingEnvironment)
        {
            _config = config;
            _fileHelper = fileHelper;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }
        public async Task<IActionResult> LoadAll(PaymentMethodModel model)
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
                // getting all templateInvoice data  
                var response = await _mediator.Send(new GetAllPaymentMethodQuery()
                {
                    Keyword = model.keyword,
                    Comid = currentUser.ComId,
                    sortColumn = sortColumn,
                    sortColumnDirection = sortColumnDirection,
                    
                });
               
                if (response.Succeeded)
                {
                    recordsTotal = response.Data.Count();
                    var json = response.Data.Select(x=> new PaymentMethod()
                    {
                        Id = x.Id,
                        secret = CryptoEngine.Encrypt("id="+x.Id, _config.Value.Key),
                        Name = x.Name,
                        Code = x.Code,
                        CreatedOn= x.CreatedOn,
                        CreatedBy = x.CreatedBy,
                        Active = x.Active,
                    }).Skip(skip).Take(pageSize);

                  
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = json });
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
        public async Task<IActionResult> IndexAsync(PaymentMethodModel model)
        {
            return View();
        }
        [Authorize(Policy = "paymentmethod.create")]
        public async Task<ActionResult> CreateAsync()
        {
            _logger.LogInformation(User.Identity.Name + "--> paymentmethod create");
            var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", new PaymentMethod() { });
            return new JsonResult(new { isValid = true, html = html });
             //return View("_CreateOrEdit", new PaymentMethod());
        }
        [Authorize(Policy = "paymentmethod.delete")]
        [HttpPost]
        [EncryptedParameters("secret")]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var deleteCommand = await _mediator.Send(new DeletePaymentMethodCommand { Id = id });
                if (deleteCommand.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS007));
                    return new JsonResult(new { isValid = true, html = string.Empty, loadTable = true });
                }
                else
                {
                    _notify.Error(deleteCommand.Message);
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
        }
        [Authorize(Policy = "paymentmethod.edit")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> EditAsync(int id)
        {
            _logger.LogInformation(User.Identity.Name + "--> paymentmethod edit");
            var data = await _mediator.Send(new GetByIdPaymentMethodQuery() { Id = id });
            if (data.Succeeded)
            {
                if (!string.IsNullOrEmpty(data.Data.Avatar))
                {
                    string path = Path.Combine(_hostingEnvironment.WebRootPath, $"{SystemVariableHelper.FolderUpload}{FolderUploadConstants.PaymentMethod}" + data.Data.Avatar);
                    using (var stream = System.IO.File.OpenRead(path))
                    {
                        var ImageUpload = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                        data.Data.Avatar = Convert.ToBase64String(ImageUpload.OptimizeImageSize(150, 150));
                        stream.Close();
                    };

                }
            }
            var htmlData = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", data.Data);
            return new JsonResult(new { isValid = true, html = htmlData });
            // return View("_Create");
        }
        [Authorize(Policy = "paymentmethod.create")]
        [HttpPost]
        public async Task<ActionResult> OnPostCreateOrEditAsync(PaymentMethod model)
        {
            string oldImg = string.Empty;
            string oldLogo = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id == 0)
                    {
                        if (string.IsNullOrEmpty(model.Code))
                        {
                            _notify.Error("Vui lòng nhập mã hình thức thanh toán");
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }

                        var currentUser = User.Identity.GetUserClaimLogin();
                        model.ComId = currentUser.ComId;
                        
                        var createProductCommand = _mapper.Map<CreatePaymentMethodCommand>(model);
                        if (model.ImageUpload != null)
                        {
                            createProductCommand.Avatar = _fileHelper.UploadedFile(model.ImageUpload, string.Empty, FolderUploadConstants.PaymentMethod,true);
                        }
                        var result = await _mediator.Send(createProductCommand);
                        if (result.Succeeded)
                        {
                            model.Id = result.Data;
                            _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS008));
                        }
                        else
                        {
                            _notify.Error(result.Message);
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                    }
                    else
                    {
                        var createProductCommand = _mapper.Map<UpdatePaymentMethodCommand>(model);
                        if (model.ImageUpload != null)
                        {
                            createProductCommand.Avatar = _fileHelper.UploadedFile(model.ImageUpload, string.Empty, FolderUploadConstants.PaymentMethod, true);
                            oldImg = createProductCommand.Avatar;
                        }
                        
                        var result = await _mediator.Send(createProductCommand);
                        if (result.Succeeded)
                        {
                            model.Id = result.Data;
                            _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                        }
                        else
                        {
                            _notify.Error(result.Message);
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                    }
                    return new JsonResult(new { isValid = true, loadTable = true, closeSwal = true });
                }
                catch (Exception ex)
                {
                    _notify.Error(ex.Message);
                    _fileHelper.DeleteFile(oldImg, FolderUploadConstants.PaymentMethod);
                    return View();
                }
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));
                _notify.Error(message);
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", model);
                return View("_CreateOrEdit", model);
            }

        }
        //public async Task<ActionResult> DetailsAsync(string IdCodeGuid)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    var response = await _mediator.Send(new GetByIdCustomerQuery() { IdCode = IdCodeGuid, ComId = user.ComId });
        //    if (response.Succeeded)
        //    {
        //        var html = await _viewRenderer.RenderViewToStringAsync("Details", response.Data);
        //        return new JsonResult(new { isValid = true, html = html });
        //    }
        //    return new JsonResult(new { isValid = false });
        //}
    }
}
