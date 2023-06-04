using Application.Constants;
using Application.Enums;
using Application.Features.Customers.Commands;
using Application.Features.Customers.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Identity;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Model;
using SmartBreadcrumbs.Attributes;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Extensions;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class CustomerController : BaseController<CustomerController>
    {
        private IStringLocalizer<SharedResource> _localizer;
        private readonly IFormFileHelperRepository _fileHelper;
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUnitOfWork _IUnitOfWork;
        private readonly IRepositoryAsync<Customer> _repository;
        private TypeCustomerEnum idType = TypeCustomerEnum.Customer;

        [Obsolete]
        public CustomerController(IFormFileHelperRepository fileHelper,
            IUnitOfWork IUnitOfWork, IHostingEnvironment hostingEnvironment,
            RoleManager<ApplicationRole> roleManager, IStringLocalizer<SharedResource> localizer,
        IRepositoryAsync<Customer> repository)
        {
            _fileHelper = fileHelper;
            _roleManager = roleManager;
            _repository = repository;
            _hostingEnvironment = hostingEnvironment;
            _IUnitOfWork = IUnitOfWork;

        }
        // GET: CustomerController
        [Breadcrumb("Danh sách khách hàng")]
        [Authorize(Policy = "customer.list")]
        public ActionResult Index()
        {
            _logger.LogInformation(User.Identity.Name + "--> Customer index");
            return View();
        }

        public async Task<IActionResult> LoadAll(CustomerModel model)
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

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

                // getting all Customer data  
                var response = await _mediator.Send(new GetAllCustomerQuery()
                {
                    TextPhoneOrEmail = model.TextPhoneOrEmail,
                    Name = model.Name,
                    sortColumn = sortColumn,
                    sortColumnDirection = sortColumnDirection,
                    pageSize = pageSize,
                    skip = skip
                });
                if (response.Succeeded)
                {
                    if (recordsTotal == 0)
                    {
                        recordsTotal = int.Parse(response.Message);
                    }
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = response.Data });
                }

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = "" });

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: CustomerController/Details/5
        public async Task<ActionResult> DetailsAsync(int id)
        {
            _logger.LogInformation(User.Identity.Name + "--> Customer detailt");
            var data = await _mediator.Send(new GetByIdCustomerQuery() { Id = id });
            if (data.Succeeded)
            {
                var datamodel = _mapper.Map<CustomerModel>(data.Data);
                if (!string.IsNullOrEmpty(datamodel.Image))
                {
                    string path = Path.Combine(_hostingEnvironment.WebRootPath, "Upload/Customer/" + datamodel.Image);
                    using (var stream = System.IO.File.OpenRead(path))
                    {
                        datamodel.ImageUpload = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                        datamodel.Image = Convert.ToBase64String(datamodel.ImageUpload.OptimizeImageSize(150, 150));
                        stream.Close();
                    };

                }
                if (!string.IsNullOrEmpty(datamodel.Logo))
                {
                    string path = Path.Combine(_hostingEnvironment.WebRootPath, "Upload/Customer/" + datamodel.Logo);
                    using (var stream = System.IO.File.OpenRead(path))
                    {
                        datamodel.LogoUpload = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                        datamodel.Logo = Convert.ToBase64String(datamodel.LogoUpload.OptimizeImageSize(150, 150));
                        stream.Close();
                    };
                }
                var html = await _viewRenderer.RenderViewToStringAsync("_Detail", datamodel);
                return new JsonResult(new { isValid = true, html = html });
            }
            _notify.Error(_localizer.GetString("NotData").Value);
            return new JsonResult(new { isValid = false, html = string.Empty });
        }

        // GET: CustomerController/Create
        [Authorize(Policy = "customer.create")]
        public async Task<ActionResult> CreateAsync()
        {
            _logger.LogInformation(User.Identity.Name + "--> Customer create");
            var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", new CustomerModel() { });
            return new JsonResult(new { isValid = true, html = html });
            // return View("_Create");
        }
        [Authorize(Policy = "customer.edit")]
        public async Task<ActionResult> EditAsync(int id)
        {
            _logger.LogInformation(User.Identity.Name + "--> edit detailt");
            var data = await _mediator.Send(new GetByIdCustomerQuery() { Id = id });
            if (data.Succeeded)
            {
                var datamodel = _mapper.Map<CustomerModel>(data.Data);
                if (!string.IsNullOrEmpty(datamodel.Image))
                {
                    string path = Path.Combine(_hostingEnvironment.WebRootPath, "Upload/Customer/" + datamodel.Image);
                    using (var stream = System.IO.File.OpenRead(path))
                    {
                        datamodel.ImageUpload = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                        datamodel.Image = Convert.ToBase64String(datamodel.ImageUpload.OptimizeImageSize(150, 150));
                        stream.Close();
                    };

                }
                if (!string.IsNullOrEmpty(datamodel.Logo))
                {
                    string path = Path.Combine(_hostingEnvironment.WebRootPath, "Upload/Customer/" + datamodel.Logo);
                    using (var stream = System.IO.File.OpenRead(path))
                    {
                        datamodel.LogoUpload = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                        datamodel.Logo = Convert.ToBase64String(datamodel.LogoUpload.OptimizeImageSize(150, 150));
                        stream.Close();
                    };
                }

                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", datamodel);
                return new JsonResult(new { isValid = true, html = html });
            }
            return new JsonResult(new { isValid = false, html = string.Empty });
        }
        // POST: CustomerController/Create
        [Authorize(Policy = "customer.create")]
        [HttpPost]
        public async Task<ActionResult> OnPostCreateOrEditAsync(int id, CustomerModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    //FormFileHelper _FormFileHelper = new FormFileHelper(_hostingEnvironment);
                    //vả _FormFileHelper = new FormFileHelper();
                    if (model.Id == 0)
                    {
                        var createProductCommand = _mapper.Map<CreateCustomerCommand>(model);
                        if (model.ImageUpload != null)
                        {

                            createProductCommand.Image = _fileHelper.UploadedFile(model.ImageUpload, model.PhoneNumber, FolderUploadConstants.Customer);
                        }
                        if (model.LogoUpload != null)
                        {
                            createProductCommand.Logo = _fileHelper.UploadedFile(model.LogoUpload, model.PhoneNumber, FolderUploadConstants.Customer);
                        }

                        var result = await _mediator.Send(createProductCommand);
                        if (result.Succeeded)
                        {
                            model.Id = result.Data;
                            _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS008));
                        }
                        else
                        {
                            _notify.Error(result.Message);
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                    }
                    else
                    {
                        var createProductCommand = _mapper.Map<UpdateCustomerCommand>(model);
                        if (model.ImageUpload != null)
                        {

                            createProductCommand.Image = _fileHelper.UploadedFile(model.ImageUpload, model.PhoneNumber, FolderUploadConstants.Customer);
                        }
                        if (model.LogoUpload != null)
                        {
                            createProductCommand.Logo = _fileHelper.UploadedFile(model.LogoUpload, model.PhoneNumber, FolderUploadConstants.Customer);
                        }

                        var result = await _mediator.Send(createProductCommand);
                        if (result.Succeeded)
                        {
                            model.Id = result.Data;
                            _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                        }
                        else
                        {
                            _notify.Error(result.Message);
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                    }
                    return new JsonResult(new { isValid = true, html = string.Empty, loadTable = true }); ;
                }
                catch (Exception ex)
                {
                    _notify.Error(ex.Message);
                    return View();
                }
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", model);
                return new JsonResult(new { isValid = false, html = html });
            }

        }

        // GET: CustomerController/Edit/5

        // POST: CustomerController/Edit/5


        // GET: CustomerController/Delete/5
        [Authorize(Policy = "customer.delete")]
        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var deleteCommand = await _mediator.Send(new DeleteCustomerCommand { Id = id });
                if (deleteCommand.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS007));
                    return new JsonResult(new { isValid = true, html = string.Empty, loadTable = true });
                }
                else
                {
                    _notify.Error(deleteCommand.Message);
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
        }
        // POST: CustomerController/Delete/5

    }
}
