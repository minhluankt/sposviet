using Application.Constants;
using Application.Features.CategorysPost.Query;
using Application.Features.CategorysProduct.Query;
using Application.Features.CompanyInfo.Query;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.ManagerApplication.Models;

namespace Web.ManagerApplication.Views.Shared.Components.Header
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryProductRepository<CategoryProduct> _repositoryCategory;
        private readonly IMediator _mediator;

        public HeaderViewComponent(ICategoryProductRepository<CategoryProduct> repositoryCategory, IMediator mediator,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _repositoryCategory = repositoryCategory;
            _userRepository = userRepository; _mediator = mediator;

        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            HomeViewModel model = new HomeViewModel();
            var getid = await _mediator.Send(new GetAllCategoryProductQuery());
            if (getid.Succeeded)
            {
                model.CategoryProducts = getid.Data;
                var getinfo = await _mediator.Send(new GetByIdCompanyInfoQuery());
                if (getinfo.Succeeded)
                {
                    if (getinfo.Data != null)
                    {
                        if (!string.IsNullOrEmpty(getinfo.Data.Logo))
                        {
                            ViewData["logo"] = $"{SystemVariable.SystemVariableHelper.FolderUpload}{FolderUploadConstants.ComPany}/{getinfo.Data.Logo}";
                        }
                        else
                        {
                            ViewData["logo"] = "images/logobsd.jpg";
                        }
                    }
                    else
                    {
                        ViewData["logo"] = "images/logobsd.jpg";
                    }

                }
                var getcategorypost = await _mediator.Send(new GetAllCategoryPostQuery());
                if (getcategorypost.Succeeded)
                {
                    model.CategoryPosts = getcategorypost.Data;
                }
                return View(model);
            }
            return View(model);
        }
    }

    public class DanhMucDetailtModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}