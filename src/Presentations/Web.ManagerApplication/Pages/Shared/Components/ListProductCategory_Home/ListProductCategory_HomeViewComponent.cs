
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Features.Products.Query;
using Application.Features.CategorysProduct.Query;
using Application.Features.ConfigSystems.Query;
using Application.Constants;
using Application.Hepers;

namespace Web.ManagerApplication.Pages.Shared.Components.ListProductCategory_Home
{
    public class ListProductCategory_HomeViewComponent : ViewComponent
    {
        private IMediator _mediator;
        public ListProductCategory_HomeViewComponent(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int?[] listid = new int?[1000];
            int pageSize = 15;
            var getlstIdAndNameCategoryShowInHome = await _mediator.Send(new GetByKeyConfigSystemQuery(ParametersConfigSystem.lstIdAndNameCategoryShowInHome));
            var getpageSizeProductInCategory = await _mediator.Send(new GetByKeyConfigSystemQuery(ParametersConfigSystem.pageSizeProductInCategory));
            if (getlstIdAndNameCategoryShowInHome.Succeeded)
            {
                if (!string.IsNullOrEmpty(getlstIdAndNameCategoryShowInHome.Data.Value))
                {
                    listid = Common.ConverJsonToArrInt(getlstIdAndNameCategoryShowInHome.Data.Value);
                }
            }
            if (getpageSizeProductInCategory.Succeeded)
            {
                if (!string.IsNullOrEmpty(getpageSizeProductInCategory.Data.Value))
                {
                    pageSize = int.Parse(getpageSizeProductInCategory.Data.Value);
                }
            }

            var getid = await _mediator.Send(new GetAllCategoryIncludeProductQuery() { lstIdCategory= listid,taskProduct= pageSize, checkActiveProduct=true });
            if (getid.Succeeded)
            {
                return View(getid.Data);
            }
            return View(new List<CategoryProduct>());
        }
    }
}
