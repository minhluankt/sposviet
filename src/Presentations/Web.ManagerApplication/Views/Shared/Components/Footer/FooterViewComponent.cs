
using Application.Features.CompanyInfo.Query;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace Web.ManagerApplication.Views.Shared.Components.Footer
{
    public class FooterViewComponent : ViewComponent
    {
        private IMediator _mediator;
        public FooterViewComponent(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var getid = await _mediator.Send(new GetByIdCompanyInfoQuery());
            if (getid.Succeeded)
            {
                return View(getid.Data);
            }
            return View(new CompanyAdminInfo());
        }
    }
}