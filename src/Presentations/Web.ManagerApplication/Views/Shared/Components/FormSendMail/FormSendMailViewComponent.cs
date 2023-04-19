
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.ManagerApplication.Models;

namespace Web.ManagerApplication.Views.Shared.Components.FormSendMail
{
    public class FormSendMailViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //SearchViewModel model = new SearchViewModel();
            //var getid = await _mediator.Send(new GetAllCategoryQuery() { GetPattern = true, IncludebyCategory = true, GetByBDS = true });
            //if (getid.Succeeded)
            //{
            //    return View(getid.Data);
            //}
            return View();
        }
    }
}
