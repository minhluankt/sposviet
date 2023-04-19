using Application.Features.CompanyInfo.Commands;
using Application.Features.CompanyInfo.Query;
using Application.Hepers;
using Application.Providers;
using Domain.Entities;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class StoreController : BaseController<StoreController>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly UserManager<ApplicationUser> _userManager;
        public StoreController(UserManager<ApplicationUser> userManager, IOptions<CryptoEngine.Secrets> config)
        {
            _config = config;
            _userManager = userManager;
        }
        [Authorize(Policy = "Store.edit")]
        public async Task<IActionResult> IndexAsync()
        {
            var getusser = User.Identity.GetUserClaimLogin();
            var data = await _mediator.Send(new GetByIdCompanyInfoQuery() { Id = getusser.ComId });
            if (data.Succeeded)
            {
                return View(data.Data);
            }
            return View(new CompanyAdminInfo());
        }
        [HttpPost] 
        [Authorize(Policy = "Store.edit")]
        public async Task<IActionResult> IndexAsync(CompanyAdminInfo model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                _notify.Error("Vui lòng nhập tên của hàng");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.Address))
            {
                _notify.Error("Vui lòng nhập địa chỉ của hàng");
                return View(model);
            }
            //if (string.IsNullOrEmpty(model.Email))
            //{
            //    _notify.Error("Vui lòng nhập email của hàng");
            //    return View(model);
            //}
       
            var getusser = User.Identity.GetUserClaimLogin();
            var data = await _mediator.Send(new UpdateCompanyInStortCommand() { 
                Id = getusser.ComId,
                Name = model.Name,
                Address = model.Address,
                Title = model.Title,
                Email = model.Email,
            });
            if (data.Succeeded)
            {
                _notify.Success("Cập nhật thành công");
                return View(data.Data);
            }
            _notify.Error("Cập nhật thất bại");
            return View(model);
        }
    }
}
