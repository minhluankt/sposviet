using Application.Constants;
using Application.Enums;
using Application.Features.ReportPoss.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ViewModel;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class DashboardController : BaseController<DashboardController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepositoryAsync<Invoice> _repository;
        private readonly IRepositoryAsync<OrderTable> _repositoryOrderTable;
        public DashboardController(IRepositoryAsync<Invoice> repository, UserManager<ApplicationUser> userManager, IRepositoryAsync<OrderTable> repositoryOrderTable)
        {
            _repositoryOrderTable = repositoryOrderTable;
            _userManager = userManager;
            _repository = repository;
        }
        [Authorize(Policy = PermissionUser.quanlyketoan)]
        public IActionResult IndexAsync()
        {
            return View();
        }
        public async Task<IActionResult> GetDashBoardIndex()// báo cáo doanh thu
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();
                // var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                DashboardModel model = new DashboardModel();
                var getall = _repository.Entities.Where(x => x.ComId == currentUser.ComId);
                var getallorder = _repositoryOrderTable.Entities.Where(x => x.ComId == currentUser.ComId);
                model.DOANHSO = await getall.Where(x => x.Status == Application.Enums.EnumStatusInvoice.DA_THANH_TOAN && x.CreatedOn.Date == DateTime.Now.Date).SumAsync(X => X.Amonut);
                model.DOANHSOHOMQUA = getall.Where(x => x.Status == Application.Enums.EnumStatusInvoice.DA_THANH_TOAN && x.CreatedOn.Date == DateTime.Now.AddDays(-1).Date).Sum(X => X.Amonut);
                model.DONDAXONG = await getall.Where(x => x.Status == Application.Enums.EnumStatusInvoice.DA_THANH_TOAN && x.CreatedOn.Date == DateTime.Now.Date).CountAsync();
                model.DONDAXONGHOMQUA = getallorder.Where(x => x.Status == Application.Enums.EnumStatusOrderTable.DA_THANH_TOAN && x.CreatedOn.Date == DateTime.Now.AddDays(-1).Date).Count();
                
                var dondangpv = await getallorder.Where(x => x.Status == Application.Enums.EnumStatusOrderTable.DANG_DAT && x.CreatedOn.Date == DateTime.Now.Date).Select(x=>x.Amonut).ToListAsync();
                model.DONDANGPHUCVU = dondangpv.Count();
                model.Customer = await getall.Where(x => !x.IsRetailCustomer && x.CreatedOn.Date == DateTime.Now.Date).CountAsync();
                model.CustomerHomQua = await getall.Where(x => !x.IsRetailCustomer && x.CreatedOn.Date == DateTime.Now.AddDays(-1).Date).CountAsync();
                _logger.LogInformation(User.Identity.Name + "--> Product index");
                return Json(new { 
                    isValid = true,
                    customer = model.Customer,
                    customerHomQua = model.CustomerHomQua,
                    doanhso = model.DOANHSO,
                    doanhsohomqua = model.DOANHSOHOMQUA,
                    dondaxong = model.DONDAXONG,
                    donxonghomqua = model.DONDAXONGHOMQUA,
                    doanhthudondangphucvu = dondangpv.Sum(),
                    dondangphucvu = model.DONDANGPHUCVU });
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
