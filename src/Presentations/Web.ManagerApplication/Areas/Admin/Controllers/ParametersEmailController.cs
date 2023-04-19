using Application.Constants;
using Application.Features.ParametersEmails.Commands;
using Application.Features.ParametersEmails.Query;
using Application.Hepers;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ParametersEmailController : BaseController<ParametersEmailController>
    {
        [Authorize(Policy = "parametersemail.list")]
        public IActionResult Index()
        {
            _logger.LogInformation(User.Identity.Name + "--> Banner index");
            return View();
        }
        public async Task<IActionResult> LoadAll()
        {
            try
            {
                var data = await _mediator.Send(new GetAllParametersEmailsCacheQuery());
                if (data.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", data.Data);
                    return new JsonResult(new { isValid = true, html = html });
                    //return View("_ViewAll", data.Data);
                }
                ////var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", new Domain.Entities.TypeSpecification());
                return new JsonResult(new { isValid = false, html = "" });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new JsonResult(new { isValid = false, html = "" });
            }
        }
        public async Task<IActionResult> DetailtAsync(int id)
        {
            _logger.LogInformation(User.Identity.Name + "--> ParametersEmail DetailtAsync");
            var getid = await _mediator.Send(new GetByIdParametersEmailQuery() { Id = id });
            if (getid.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_Detailt", getid.Data);
                return new JsonResult(new { isValid = true, html = html });
            }
            return new JsonResult(new { isValid = false, html = "" });
        }

        [Authorize(Policy = "parametersemail.delete")]
        [HttpPost]
        public async Task<ActionResult> Delete(int Id)
        {
            _logger.LogInformation(User.Identity.Name + "--> Banner Delete");
            var delete = await _mediator.Send(new DeleteParametersEmailCommand() { Id = Id });
            if (delete.Succeeded)
            {

                _notify.Success($"Xóa thành công Id: {Id}");
                var response = await _mediator.Send(new GetAllParametersEmailsCacheQuery());
                if (response.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                    return new JsonResult(new { isValid = true, html = html, LoadDataTable = true });
                }
                else
                {
                    _notify.Error(response.Message);
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
            }
            else
            {
                _notify.Error(delete.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
        }
        [Authorize(Policy = "ParametersEmail.createoredit")]
        public async Task<ActionResult> CreateOrEdit(int id = 0)
        {
            _logger.LogInformation(User.Identity.Name + "--> ParametersEmail CreateOrEdit");
            if (id == 0)
            {
                //var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", new ParametersEmail());
                //return new JsonResult(new { isValid = true, html = html });
                return View(new ParametersEmail());
            }
            else
            {
                var getid = await _mediator.Send(new GetByIdParametersEmailQuery() { Id = id });
                if (getid.Succeeded)
                {

                   // var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", getid.Data);
                   // return new JsonResult(new { isValid = true, html = html });
                    return View(getid.Data);
                }
                _notify.Error(getid.Message);
                // return new JsonResult(new { isValid = false, html = string.Empty });
                return RedirectToAction("Index");
            }

        }

        // POST: ParametersEmailsController/Create
        [Authorize(Policy = "ParametersEmails.edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(Domain.Entities.ParametersEmail collection)
        {
            try
            {
                if (collection.Id == 0)
                {
                    var createProductCommand = _mapper.Map<CreateParametersEmailCommand>(collection);
                    var result = await _mediator.Send(createProductCommand);
                    if (result.Succeeded)
                    {
                        collection.Id = result.Data;
                        _notify.Success(HeperConstantss.SUS008);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _notify.Error(result.Message);
                       // return new JsonResult(new { isValid = false, html = string.Empty });
                        return View("CreateOrEdit",collection);
                    }
                }
                else
                {
                    var updateProductCommand = _mapper.Map<UpdateParametersEmailCommand>(collection);
                    var result = await _mediator.Send(updateProductCommand);
                    if (result.Succeeded)
                    {
                        _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006)); 
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _notify.Error(result.Message);
                        return View("CreateOrEdit", collection);
                    }

                }
              //  var response = await _mediator.Send(new GetAllParametersEmailsCacheQuery());
                //if (response.Succeeded)
                //{
                //    _notify.Success(HeperConstantss.SUS006);
                //    // var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                //    //return new JsonResult(new { isValid = true, html = html });
                //    return RedirectToAction("Index");
                //}
                //else
                //{
                //    _notify.Error(response.Message);
                //    // return null;
                //    return View("CreateOrEdit", collection);
                //}
            }
            catch (Exception e)
            {
                _notify.Error(e.ToString());
                return View("CreateOrEdit", collection);

            }
        }

        // GET: ParametersEmailsController/Edit/5
        [Authorize(Policy = "ParametersEmails.create")]
        public async Task<ActionResult> Create()
        {
            var html = await _viewRenderer.RenderViewToStringAsync("_Create", new Domain.Entities.ParametersEmail());
            return new JsonResult(new { isValid = true, html = html });
        }
        
    }
}
