using Application.Constants;
using Application.Enums;
using Application.Features.Banners.Query;
using Application.Features.CompanyInfo.Query;
using Application.Features.ConfigSystems.Query;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Models;

namespace Web.ManagerApplication.Controllers
{
    [Authorize]
    public class OrderStaffController : BaseController<OrderStaffController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public OrderStaffController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [Authorize(Policy = PermissionUser.nhanvienphucvu)]
        public async Task<IActionResult> IndexAsync()
        {
        
            HomeViewModel homeViewModel = new HomeViewModel();
            var getid = await _mediator.Send(new GetByIdCompanyInfoQuery());
            if (getid.Succeeded)
            {
                if (getid.Data != null)
                {
                    ViewBag.Website = getid.Data.Website;
                    ViewBag.Title = getid.Data.Title;
                    ViewBag.description = getid.Data.Description;
                    ViewBag.Keyword = getid.Data.Keyword;
                    if (!string.IsNullOrWhiteSpace(getid.Data.Image))
                    {
                        ViewBag.image = $"{SystemVariable.SystemVariableHelper.FolderUpload}{FolderUploadConstants.ComPany}/{getid.Data.Image}";
                    }
                    else
                    {
                        ViewBag.image = $"{FolderUploadConstants.ImgSeo}";
                    }
                }
                else
                {
                    ViewBag.image = $"{FolderUploadConstants.ImgSeo}";
                }
            }
            var response = await _mediator.Send(new GetAllBannerCacheQuery());
            if (response.Succeeded)
            {
                homeViewModel.Banners = response.Data;
            }

            var getConfigQuer = await _mediator.Send(new GetAllConfigQuery());
            if (getConfigQuer.Succeeded)
            {
                var getlayoutHeader = getConfigQuer.Data.Where(m => m.Key == ParametersConfigSystem.layoutHeader).SingleOrDefault();
                if (getlayoutHeader != null)
                {
                    if (!string.IsNullOrEmpty(getlayoutHeader.Value))
                    {
                        homeViewModel.layoutHeader = int.Parse(getlayoutHeader.Value);
                    }
                }
            }

            _logger.LogInformation("Hi There!");

            // _logger.LogInformation("Hi There!");
            return View(homeViewModel);
        }
    }
}
