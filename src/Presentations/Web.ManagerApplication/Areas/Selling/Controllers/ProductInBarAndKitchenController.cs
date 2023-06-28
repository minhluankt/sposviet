using Application.Enums;
using Application.Features.BarAndKitchens.Query;
using Application.Features.DefaultFoodOrders.Commands;
using Application.Features.DefaultFoodOrders.Query;
using Application.Features.ProductInBarAndKitchens.Query;
using Application.Hepers;
using Application.Providers;
using Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class ProductInBarAndKitchenController : BaseController<AreasController>
    {
        [Authorize(Policy = "productInBarAndKitchen.list")]
        [EncryptedParameters("secret")]
        public async Task<IActionResult> IndexAsync(int id)
        {
            if (id == 0)
            {
                _notify.Error("Không tìm thấy dữ liệu");
                return RedirectToAction("Index", "BarAndKitchen");
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            var get = await _mediator.Send(new GetByIdBarAndKitchenQuery() { Comid = currentUser.ComId,Id=id });
            if (get.Failed)
            {
                _notify.Error("Không tìm thấy dữ liệu");
                return RedirectToAction("Index", "BarAndKitchen");
            }
            return View(get.Data);
        }
        public async Task<IActionResult> GetListJsonId(int id)
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();
                var get = await _mediator.Send(new GetAllProductInBarAndKitchenQuery() { IdBar = id });
                if (get.Succeeded)
                {
                    return Json(new { isValid = true, data = ConvertSupport.ConverModelToJson(get.Data.Select(x => x.IdProduct).ToArray()) });
                }
                return new JsonResult(new { isValid = false });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new JsonResult(new { isValid = false });
            }
        }
        [HttpPost]
        public async Task<IActionResult> LoadAll(string Name, int IdBarAndKitchen, int? IdCategory)
        {
            thêm tính năng xoas1 cái và nhiều tại hàm FoodInBarAndKitchen
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
                currentPage = currentPage + 1;
                // getting all Customer data
                var response = await _mediator.Send(new GetPaginatedListProductInBarAndKitchenQuery(currentPage, pageSize)
                {
                    sortColumn = sortColumn,
                    sortColumnDirection = sortColumnDirection,
                    pageSize = pageSize,
                    skip = skip,
                    ComId = currentUser.ComId,
                    IdCategory = IdCategory,
                    IdBarAndKitchen = IdBarAndKitchen,
                    Name = Name,
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
                _notify.Error(ex.ToString());
                return Json(new { draw = draw, recordsFiltered = 0, recordsTotal = 0, data = "" });
            }

        }
        [Authorize(Policy = "productInBarAndKitchen.update")]
        [HttpPost]
        public async Task<IActionResult> UpdateAsync(int[] lstid, int IdBarAndKitchen)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var _send = await _mediator.Send(new UpdateProductInBarAndKitchenCommand()
            {
                ComId = currentUser.ComId,
                IdBarAndKitchen = IdBarAndKitchen,
                ListId = lstid
            });
            if (_send.Succeeded)
            {
                _notify.Success(GeneralMess.ConvertStatusToString(_send.Message));
                return new JsonResult(new { isValid = true });
            }
            _notify.Error(GeneralMess.ConvertStatusToString(_send.Message));
            return new JsonResult(new { isValid = true });
        }
    }
}
