using Application.Constants;
using Application.Enums;
using Application.Features.CategoryCevenues.Query;
using Application.Features.PaymentMethods.Query;
using Application.Features.RevenueExpenditures.Commands;
using Application.Features.RevenueExpenditures.Query;
using Application.Hepers;
using Application.Providers;
using Domain.ViewModel;
using Hangfire.MemoryStorage.Database;
using HelperLibrary;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Areas.Selling.Models;

namespace Web.ManagerApplication.RevenueExpenditure.Selling.Controllers
{
    [Area("Selling")]
    public class RevenueExpenditureController : BaseController<RevenueExpenditureController>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly UserManager<ApplicationUser> _userManager;
        public RevenueExpenditureController(UserManager<ApplicationUser> userManager, IOptions<CryptoEngine.Secrets> config)
        {
            _config = config;
            _userManager = userManager;
        }
        [Authorize(Policy = "revenueexpenditure.index")]
        public IActionResult Index()
        {
            return View();
        }
       
        private string GetDisplayName(object value)
        {
            var type = value.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException(string.Format("Type {0} is not an enum", type));
            }

            // Get the enum field.
            var field = type.GetField(value.ToString());
            if (field == null)
            {
                return value.ToString();
            }

            // Gets the value of the Name property on the DisplayAttribute, this can be null.
            var attributes = field.GetCustomAttribute<DisplayAttribute>();
            return attributes != null ? attributes.Name : value.ToString();
        }
        [HttpPost]
        public async Task<IActionResult> LoadAll(CategoryCevenueModel model)
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
                if (!string.IsNullOrEmpty(searchValue))
                {
                    model.Name = searchValue;
                }
                //Paging Size (10, 20, 50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 10;

                int skip = start != null ? Convert.ToInt32(start) : 0;

                int recordsTotal = 0;
                //var currentUser = User.Identity.GetUserClaimLogin();
                var currentUser = User.Identity.GetUserClaimLogin();
                // getting all Customer data  
                var response = await _mediator.Send(new GetAllRevenueExpenditureQuery(currentUser.ComId, model.Type)
                {
                    RangesDate = model.RangesDate,
                    Code = model.Name,
                    Comid = currentUser.ComId,
                    sortColumn = sortColumn,
                    sortColumnDirection = sortColumnDirection,
                    pageSize = pageSize,
                    skip = skip
                });
                if (response.Succeeded)
                {
                    if (recordsTotal == 0)
                    {
                        recordsTotal = int.Parse(response.Message);
                    }
                    var lst = response.Data;
                    lst.ForEach(async x =>
                    {
                        x.IdString = CryptoEngine.Encrypt("id=" + x.Id, _config.Value.Key);
                       // x.CreatedBy = (await _userManager.FindByIdAsync(x.CreatedBy))?.FullName;
                        x.CategoryCevenueName = x.Typecategory != EnumTypeCategoryThuChi.None ? x.Title : x.Type == EnumTypeRevenueExpenditure.THU ? $"Thu {x.CategoryCevenueName}" : x.Type == EnumTypeRevenueExpenditure.CHI ? $"Chi {x.CategoryCevenueName}" : x.CategoryCevenueName;
                    });
                    var json = lst.Select(x => new
                    {
                        id = x.Id,
                        code = x.Code,
                        idString = x.IdString,
                        categoryCevenueName = x.CategoryCevenueName,
                        date = x.Date.ToString("dd/MM/yyyy"),
                        createdOn = x.CreatedOn.ToString("dd/MM/yyyy"),
                        createdBy = x.CreatedBy,
                        paymentName = x.PaymentName,
                        customerName = x.CustomerName,
                        status = x.Status,
                        type = x.Type,
                        amount = x.Amount,
                        codeOriginaldocument = x.CodeOriginaldocument,
                        objectRevenueExpenditure = x.ObjectRevenueExpenditure,
                     
                    });

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

        [Authorize(Policy = "revenueexpenditure.create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(Domain.Entities.RevenueExpenditure collection)
        {
            var getusser = await _userManager.GetUserAsync(User);
            collection.ComId = getusser.ComId;
            try
            {
                if (collection.Id == 0)
                {
                    if (collection.ObjectRevenueExpenditure==EnumTypeObjectRevenueExpenditure.KHACHHANG || collection.ObjectRevenueExpenditure == EnumTypeObjectRevenueExpenditure.DOITAC)
                    {
                        collection.IdCustomer = int.Parse(collection.CustomerName);
                        collection.CustomerName = string.Empty;
                    }
                    var createProductCommand = _mapper.Map<CreateRevenueExpenditureCommand>(collection);
                    var result = await _mediator.Send(createProductCommand);
                    if (result.Succeeded)
                    {
                        collection.Id = result.Data;
                        _notify.Success(HeperConstantss.SUS008);

                    }
                    else
                    {
                        _notify.Error(result.Message);
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }
                }
                else
                {
                    var updateProductCommand = _mapper.Map<UpdateRevenueExpenditureCommand>(collection);
                    var result = await _mediator.Send(updateProductCommand);
                    if (result.Succeeded) _notify.Success(HeperConstantss.SUS006);

                }
                return new JsonResult(new { isValid = true, loadTable = true, closeSwal = true });
            }
            catch (Exception e)
            {
                _notify.Error(e.ToString());
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
        }

        private List<SelectListItem>  GetEnumTypeObjectRevenueExpenditure(EnumTypeObjectRevenueExpenditure type=EnumTypeObjectRevenueExpenditure.KHACHHANG)
        {
            var select = Enum.GetValues(typeof(EnumTypeObjectRevenueExpenditure)).Cast<EnumTypeObjectRevenueExpenditure>().OrderBy(x => (Convert.ToInt32(x))).Where(x => (Convert.ToInt32(x)) >= 0).Select(x => new SelectListItem
            {
                Text = GetDisplayName(x),
                Value = Convert.ToInt32(x).ToString(),
                Selected = x== type
            }).ToList();
            select.Insert(0, new SelectListItem()
            {
                Text = "",
                Value = ""
            });
            return select;
        }
        // GET: RevenueExpenditureController/Edit/5
        [Authorize(Policy = "revenueexpenditure.create")]
        public async Task<ActionResult> CreateReceipts()//phiếu thu
        {
            var getusser = await _userManager.GetUserAsync(User);
            var getpayment = await _mediator.Send(new GetAllPaymentMethodQuery() { Comid= getusser.ComId });
            var getCategoryCevenueQuery = await _mediator.Send(new GetAllCategoryCevenueQuery(getusser.ComId,type: EnumTypeRevenueExpenditure.THU));
            
            var html = await _viewRenderer.RenderViewToStringAsync("_Create", new RevenueExpenditureModel() { 
                Type= EnumTypeRevenueExpenditure.THU,
                ObjectRevenueExpenditures= GetEnumTypeObjectRevenueExpenditure(),
                PaymentMethods= getpayment.Data.ToList(),
                CategoryCevenues = getCategoryCevenueQuery.Data.ToList(),
                TypeName ="thu"
            });
            return new JsonResult(new { isValid = true, typeNme = "thu", html = html, title = "Thêm mới phiếu thu" });
        }
       
        [Authorize(Policy = "revenueexpenditure.create")]
        public async Task<ActionResult> CreatePayment()
        {
            var getusser = await _userManager.GetUserAsync(User);
            var getpayment = await _mediator.Send(new GetAllPaymentMethodQuery() { Comid = getusser.ComId });
            var getCategoryCevenueQuery = await _mediator.Send(new GetAllCategoryCevenueQuery(getusser.ComId, type: EnumTypeRevenueExpenditure.CHI));

            var html = await _viewRenderer.RenderViewToStringAsync("_Create", new RevenueExpenditureModel()
            {
                Type = EnumTypeRevenueExpenditure.CHI,
                ObjectRevenueExpenditures = GetEnumTypeObjectRevenueExpenditure(),
                PaymentMethods = getpayment.Data.ToList(),
                CategoryCevenues = getCategoryCevenueQuery.Data.ToList(),
                TypeName = "chi"
            });
            return new JsonResult(new { isValid = true, html = html, typeNme = "chi", title = "Thêm mới phiếu chi" });
        }

        [Authorize(Policy = "revenueexpenditure.edit")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Edit(int id)
        {
            var getusser = await _userManager.GetUserAsync(User);
            var data = await _mediator.Send(new GetByIdRevenueExpenditureQuery(getusser.ComId) { Id = id });
            if (data.Succeeded)
            {
                var getpayment = await _mediator.Send(new GetAllPaymentMethodQuery() { Comid = getusser.ComId });
                var getCategoryCevenueQuery = await _mediator.Send(new GetAllCategoryCevenueQuery(getusser.ComId, type: data.Data.Type));
                if (data.Data.Type==Application.Enums.EnumTypeRevenueExpenditure.THU)
                {
                    data.Data.TypeName = "thu";
                }
                else
                {
                    data.Data.TypeName = "chi";
                }
                var model = _mapper.Map<RevenueExpenditureModel>(data.Data);
                model.PaymentMethods = getpayment.Data.ToList();
                model.CategoryCevenues = getCategoryCevenueQuery.Data.ToList();
                model.ObjectRevenueExpenditures = GetEnumTypeObjectRevenueExpenditure(model.ObjectRevenueExpenditure);
                var html = await _viewRenderer.RenderViewToStringAsync("_Edit", model);
                return new JsonResult(new { isValid = true, html = html, isEditRevenueExpenditure=true, typeNme = data.Data.TypeName, title = "Chỉnh sửa" });
            }
            return new JsonResult(new { isValid = false, html = string.Empty });
        }

        [HttpGet]
        public async Task<ActionResult> GetDashboard(CategoryCevenueModel model) 
        {
            var getusser = await _userManager.GetUserAsync(User);
            var deleteCommand = await _mediator.Send(new GetReportRevenueExpenditureQuery(getusser.ComId) { RangesDate=model.RangesDate});
            if (deleteCommand.Succeeded)
            {
                return new JsonResult(new { isValid = true, data = deleteCommand.Data });
            }
            else
            {
                _notify.Error(deleteCommand.Message);
                return new JsonResult(new { isValid = false });
            }
        }

        // POST: RevenueExpenditureController/Delete/5
        [Authorize(Policy = "revenueexpenditure.delete")]
        [HttpPost]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var getusser = await _userManager.GetUserAsync(User);

                var deleteCommand = await _mediator.Send(new DeleteRevenueExpenditureCommand(getusser.ComId, id));
                if (deleteCommand.Succeeded)
                {
                    return new JsonResult(new { isValid = true, loadTable = true });
                }
                else
                {
                    _notify.Error(deleteCommand.Message);
                    return new JsonResult(new { isValid = false });
                }
            }
            catch (Exception ex)
            {
                _notify.Error(ex.Message);
                return new JsonResult(new { isValid = false });
            }
        }
    }
}
