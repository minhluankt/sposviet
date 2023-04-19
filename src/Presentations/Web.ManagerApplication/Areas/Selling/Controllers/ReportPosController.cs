using Application.Constants;
using Application.Enums;
using Application.Features.Customers.Commands;
using Application.Features.Customers.Query;
using Application.Features.ReportPoss.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.ViewModel;
using HelperLibrary;
using Infrastructure.Infrastructure.Identity.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Model;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Drawing2D;
using System.Reflection;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Extensions;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class ReportPosController : BaseController<PosController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ReportPosController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        private void LoadViewbag(EnumTypeReportPos type = EnumTypeReportPos.DOANHTHU)
        {
            var select = Enum.GetValues(typeof(EnumTypeReportPos)).Cast<EnumTypeReportPos>().OrderBy(x => (Convert.ToInt32(x))).Where(x=> (Convert.ToInt32(x)) >=0).Select(x => new SelectListItem
            {
                Text = GetDisplayName(x),
                Value = Convert.ToInt32(x).ToString(),
                Selected = x == type
            }).ToList();
            select.Insert(0, new SelectListItem()
            {
                Text = "",
                Value = "",
            });
            ViewBag.SelectList = select;
        } 
        private void LoadViewbagProduct(EnumTypeReportProduct type = EnumTypeReportProduct.NONE)
        {
            var select = Enum.GetValues(typeof(EnumTypeReportProduct)).Cast<EnumTypeReportProduct>().OrderBy(x => (Convert.ToInt32(x))).Where(x=> (Convert.ToInt32(x)) >=0).Select(x => new SelectListItem
            {
                Text = GetDisplayName(x),
                Value = Convert.ToInt32(x).ToString(),
                Selected = x == type
            }).ToList();
            select.Insert(0, new SelectListItem()
            {
                Text = "",
                Value = ""
            });
            ViewBag.SelectList = select;
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
        [Authorize(Policy = "reportPos.Revenue")]
        public IActionResult Revenue()// báo cáo doanh thu
        {
            LoadViewbag();
            return View();
        }
        public async Task<IActionResult> GetReportDashBoard(SearchReportPosModel model)// báo cáo doanh thu
        {
            try
            {
                if (string.IsNullOrEmpty(model.rangesDate))
                {
                    _notify.Error("Chưa chọn ngày báo cáo");
                    return Json(new { isValid = false });
                }
                var currentUser = User.Identity.GetUserClaimLogin();
                model.Comid = currentUser.ComId;
                var _map = _mapper.Map<GetReportDashboardQuery>(model);
                var _send = await _mediator.Send(_map);
                if (_send.Succeeded)
                {
                    switch (model.TypeReportDashboard)
                    {
                     
                        case EnumTypeReportDashboard.DOANHTHU:
                            return Json(new { isValid = true, data = _send.Data.ReportPosModel, typeReportPos = model.TypeReportDashboard });
                     
                        default:
                            _notify.Error("Loại báo cáo không hợp lệ");
                            break;
                    }

                }
                return Json(new { isValid = false });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                _notify.Error(e.Message);
                return Json(new { isValid = false });
            }

        }
        [Authorize(Policy = "reportPos.Onhand")]
        public IActionResult Onhand(ProductSearch model)// báo cáo tồn kho
        {
            return View(model);
        }
        [Authorize(Policy = "reportPos.ExportImportOnhand")]
        public IActionResult ExportImportOnhand(ProductSearch model)// báo cáo xuất nhập tồn kho
        {
            return View(model);
        }  
        public async Task<IActionResult> GetReportExportImportOnhandAsync(SearchReportPosModel model)// báo cáo tồn kho
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
				// var currentUser = await _userManager.GetUserAsync(HttpContext.User);
				// getting all Customer data  
				model.Comid = currentUser.ComId;
				var _map = _mapper.Map<ExportImportOnhandQuery>(model);
				var response = await _mediator.Send(_map);
				if (response.Succeeded)
				{
					recordsTotal = response.Data.ReportXuatNhapTonKhos.Count();
                    return Json(new
                    {
                        draw = draw,
                        recordsFiltered = recordsTotal,
                        recordsTotal = recordsTotal,
                        data = response.Data.ReportXuatNhapTonKhos
      //.Select(x => new
      //{
      //	x.Id,
      //	x.Name,
      //	x.Unit,
      //	x.CategoryProductName,
      //	x.Code,
      //	x.so,
      //}
           //         );
					});
				}

				//Returning Json Data  
				return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = "" });

			}
			catch (Exception ex)
			{
				_notify.Error(ex.Message);
				_logger.LogError(ex.ToString());
				throw;
			}
		}
        public async Task<IActionResult> GetReportOnhandAsync(SearchReportPosModel model)// báo cáo tồn kho
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
                // var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                // getting all Customer data  
                model.Comid = currentUser.ComId;
                var _map = _mapper.Map<GetReportOnhandQuery>(model);
                var response = await _mediator.Send(_map);
                if (response.Succeeded)
                {
                    recordsTotal = response.Data.Products.Count();
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = response.Data.Products.Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.Unit,
                        x.Quantity,
                        nameCategory = x.CategoryProduct?.Name,
                        x.Code,
                        x.RetailPrice,
                        x.Price,
                        giatritk = (x.RetailPrice* x.Quantity),
                    } 
                    ) });
                }

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = "" });

            }
            catch (Exception ex)
            {
                _notify.Error(ex.Message);
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        [Authorize(Policy = "reportPos.Products")]
        public IActionResult Products()// báo cáo mặt hàng
        {
            LoadViewbagProduct();
            return View();
        }
        public async Task<IActionResult> GetReportRevenueAsync(SearchReportPosModel model)// báo cáo doanh thu
        {
            try
            {
                if (string.IsNullOrEmpty(model.rangesDate))
                {
                    _notify.Error("Chưa chọn ngày báo cáo");
                    return Json(new { isValid = false });
                }
                var currentUser = User.Identity.GetUserClaimLogin();
                model.Comid = currentUser.ComId;
                var _map = _mapper.Map<GetReportPosQuery>(model);
                var _send = await _mediator.Send(_map);
                if (_send.Succeeded)
                {
                    switch (model.typeReportPos)
                    {
                        case EnumTypeReportPos.NONE:
                            break;
                        case EnumTypeReportPos.DOANHTHU:
                            return Json(new { isValid = true, data = _send.Data.ReportPosModel, typeReportPos = model.typeReportPos });
                        case EnumTypeReportPos.HUYDON:
                            return Json(new { isValid = true, isShowChart = _send.Data.isShowChart, data = _send.Data.ReportHuyDon, typeReportPos = model.typeReportPos });
                        case EnumTypeReportPos.HINHTHUCPHUVU:
                            return Json(new { isValid = true, data = _send.Data.ReportHinhThucPhucVu, typeReportPos = model.typeReportPos });
                        //case EnumTypeReportPos.HOADONCHUATHANHTOAN:
                        //    break;
                        default:
                            _notify.Error("Loại báo cáo không hợp lệ");
                            break;
                    }

                }
                return Json(new { isValid = false });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                _notify.Error(e.Message);
                return Json(new { isValid = false });
            }
        }
        
        public async Task<IActionResult> GetReportProductsAsync(SearchReportPosModel model)//bsao cáo mặt hàng
        {
            try
            {
                if (string.IsNullOrEmpty(model.rangesDate))
                {
                    _notify.Error("Chưa chọn ngày báo cáo");
                    return Json(new { isValid = false });
                }
                var currentUser = User.Identity.GetUserClaimLogin();
                model.Comid = currentUser.ComId;
                var _map = _mapper.Map<GetReportsProductsQuery>(model);
                var _send = await _mediator.Send(_map);
                if (_send.Succeeded)
                {
                    switch (model.typeReportProduct)
                    {
                        case EnumTypeReportProduct.NONE:
                            break;
                        case EnumTypeReportProduct.DANHMUCMATHANG:
                            return Json(new { isValid = true, data = _send.Data, typeReportPos = model.typeReportProduct });
                           
                        case EnumTypeReportProduct.MATHANGBANCHAY:
                            return Json(new { isValid = true, data = _send.Data, typeReportPos = model.typeReportProduct });
                          
                        default:
                            break;
                    }
                    
                }
                return Json(new { isValid = false });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                _notify.Error(e.Message);
                return Json(new { isValid = false });
            }
            
        }
    }
}
