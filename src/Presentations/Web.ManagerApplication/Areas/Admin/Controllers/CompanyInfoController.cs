using Application.Constants;
using Application.Features.CompanyInfo.Commands;
using Application.Features.CompanyInfo.Query;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SmartBreadcrumbs.Attributes;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Extensions;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyInfoController : BaseController<CompanyInfoController>
    {
        private IStringLocalizer<SharedResource> _localizer;
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IHostingEnvironment _hostingEnvironment;
        public CompanyInfoController(IFormFileHelperRepository fileHelper, IStringLocalizer<SharedResource> localizer, IHostingEnvironment hostingEnvironment)
        {
            _localizer = localizer;
            _fileHelper = fileHelper;
            _hostingEnvironment = hostingEnvironment;
        }
        [Breadcrumb("Thông tin công ty")]
        [Authorize(Policy = "companinfo.edit")]
        public async Task<IActionResult> Index(int id = 0)
        {
            try
            {
                _logger.LogInformation(User.Identity.Name + "--> CompanyInfo index");
                var getid = await _mediator.Send(new GetByIdCompanyInfoQuery() { Id = id });
                if (getid.Succeeded)
                {
                    if (getid.Data == null)
                    {
                        return View("_CreateOrEdit", new CompanyAdminInfoViewModel());
                    }
                    var datamodel = _mapper.Map<CompanyAdminInfoViewModel>(getid.Data);
                    if (!string.IsNullOrEmpty(datamodel.Image))
                    {
                        string path = Path.Combine(_hostingEnvironment.WebRootPath, "Upload/Company/" + datamodel.Image);
                        if (System.IO.File.Exists(path))
                        {
                            using (var stream = System.IO.File.OpenRead(path))
                            {
                                datamodel.ImageUpload = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                                datamodel.Image = Convert.ToBase64String(datamodel.ImageUpload.OptimizeImageSize(150, 150));
                                stream.Close();
                            };

                        }
                    }
                    if (!string.IsNullOrEmpty(datamodel.Logo))
                    {
                        string path = Path.Combine(_hostingEnvironment.WebRootPath, "Upload/Company/" + datamodel.Logo);

                        if (System.IO.File.Exists(path))
                        {
                            using (var stream = System.IO.File.OpenRead(path))
                            {
                                datamodel.LogoUpload = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                                datamodel.Logo = Convert.ToBase64String(datamodel.LogoUpload.OptimizeImageSize(150, 150));
                                stream.Close();
                            };

                        }

                    }

                    return View("_CreateOrEdit", datamodel);
                }
                _notify.Error(_localizer.GetString("NotData").Value);
                return RedirectToAction("/admin");
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                throw;
            }
        }
        [Authorize(Policy = "companinfo.edit")]
        public async Task<ActionResult> OnPostCreateOrEditAsync(int id, CompanyAdminInfoViewModel model)
        {
            _logger.LogInformation(User.Identity.Name + "--> CompanyInfo CreateOrEdit");
            //if (ModelState.IsValid)
            // {
            try
            {
                if (model.Id == 0)
                {
                    var createProductCommand = _mapper.Map<CreateCompanyInfoCommand>(model);
                    if (model.ImageUpload != null)
                    {

                        createProductCommand.Image = _fileHelper.UploadedFile(model.ImageUpload, model.CusTaxCode, FolderUploadConstants.ComPany);
                    }
                    if (model.LogoUpload != null)
                    {
                        createProductCommand.Logo = _fileHelper.UploadedFile(model.LogoUpload, model.CusTaxCode, FolderUploadConstants.ComPany);
                    }

                    var result = await _mediator.Send(createProductCommand);
                    if (result.Succeeded)
                    {
                        model.Id = result.Data;
                        _notify.Success(_localizer.GetString("AddOk").Value);

                        return new JsonResult(new { isValid = true, html = string.Empty });
                    }
                    else
                    {
                        _notify.Error(result.Message);
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }
                }
                else
                {
                    var createProductCommand = _mapper.Map<UpdateCompanyInfoCommand>(model);
                    if (model.ImageUpload != null)
                    {
                        createProductCommand.Image = _fileHelper.UploadedFile(model.ImageUpload, model.CusTaxCode, FolderUploadConstants.ComPany);
                        _logger.LogInformation($"CompanyInfo CreateOrEdit Image done: {model.ImageUpload}");
                    }
                    if (model.LogoUpload != null)
                    {
                        createProductCommand.Logo = _fileHelper.UploadedFile(model.LogoUpload, model.CusTaxCode, FolderUploadConstants.ComPany);
                        _logger.LogInformation($"CompanyInfo CreateOrEdit Image done: {model.LogoUpload}");
                    }
                    var result = await _mediator.Send(createProductCommand);
                    if (result.Succeeded)
                    {
                        model.Id = result.Data;
                        _notify.Success(_localizer.GetString("EditOk").Value);

                        return new JsonResult(new { isValid = true, html = string.Empty }); ;
                    }
                    else
                    {
                        _notify.Error(result.Message);
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }
                }
                return new JsonResult(new { isValid = true, html = string.Empty });
            }
            catch (Exception ex)
            {
                _notify.Error(ex.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            // }
            // else
            // {

            //var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", model);
            //   return new JsonResult(new { isValid = false, html = string.Empty });
            //}

        }
    }
}
