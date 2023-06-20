using Application.Constants;
using Application.Enums;
using Application.Features.Customers.Commands;
using Application.Features.Customers.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using HelperLibrary;
using Domain.Identity;
using Infrastructure.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Model;
using Newtonsoft.Json;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Extensions;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class CustomerController : BaseController<PosController>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private IOptions<CryptoEngine.Secrets> _config;
        public CustomerController(UserManager<ApplicationUser> userManager,
            IHostingEnvironment hostingEnvironment, IFormFileHelperRepository fileHelper,
            IOptions<CryptoEngine.Secrets> config)
        {
            _fileHelper = fileHelper;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _config = config;
        }
        [Authorize(Policy = "customer.list")]
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> LoadAll(CustomerModelView model)
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
                var currentUser = User.Identity.GetUserClaimLogin();
               // var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                // getting all Customer data  
                var response = await _mediator.Send(new GetAllCustomerQuery()
                {
                    Name = model.Name,
                    Comid = currentUser.ComId,
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
                _logger.LogError(ex.ToString());
                throw;
            }

        }
        [Authorize(Policy = "customer.create")]
        public async Task<ActionResult> CreateAsync(bool IsPos = false)
        {
            _logger.LogInformation(User.Identity.Name + "--> Customer create");
            var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", new CustomerModel() { IsPos = IsPos });
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
        [Authorize(Policy = "customer.create")]
        [HttpPost]
        public async Task<ActionResult> OnPostCreateOrEditAsync(int id, CustomerModel model)
        {
            string oldImg = string.Empty;
            string oldLogo = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {

                    //FormFileHelper _FormFileHelper = new FormFileHelper(_hostingEnvironment);
                    //vả _FormFileHelper = new FormFileHelper();
                    if (model.Id == 0)
                    {
                        if (!string.IsNullOrEmpty(model.Code) && model.Code.Contains(" "))
                        {
                            _notify.Error("Mã khách hàng không được có khoản trắng");
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                        var currentUser = User.Identity.GetUserClaimLogin();
                       // var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                        model.Comid = currentUser.ComId;
                        if (!string.IsNullOrEmpty(model.CCCD))
                        {
                            model.UserName = model.CCCD.Replace(" ", "");
                        }
                        else if(!string.IsNullOrEmpty(model.PhoneNumber))
                        {
                            model.UserName = model.PhoneNumber.Trim().Replace(" ","");
                        }
                        else
                        {
                            model.UserName = LibraryCommon.RandomString(8);
                        }

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
                            oldImg = createProductCommand.Image;
                        }
                        if (model.LogoUpload != null)
                        {
                            createProductCommand.Logo = _fileHelper.UploadedFile(model.LogoUpload, model.PhoneNumber, FolderUploadConstants.Customer);
                            oldImg = createProductCommand.Logo;
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
                    return new JsonResult(new { isValid = true, loadTable = !model.IsPos, closeSwal = true });
                }
                catch (Exception ex)
                {
                    _notify.Error(ex.Message);
                    _fileHelper.DeleteFile(oldImg, FolderUploadConstants.Customer);
                    _fileHelper.DeleteFile(oldLogo, FolderUploadConstants.Customer);
                    return new JsonResult(new { isValid = false });
                }
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));

                _notify.Error(message);
                return new JsonResult(new { isValid = false });
            }

        }
        public async Task<ActionResult> DetailsAsync(string IdCodeGuid)
        {
            var user = User.Identity.GetUserClaimLogin();
           // var user = await _userManager.GetUserAsync(User);
            var response = await _mediator.Send(new GetByIdCustomerQuery() { IdCode = IdCodeGuid, ComId = user.ComId });
            if (response.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("Details", response.Data);
                return new JsonResult(new { isValid = true, html = html });
            }
            return new JsonResult(new { isValid = false });
        }

        public async Task<IActionResult> SearchCustomerPos(string text, AutocompleteTypeCustomer type = AutocompleteTypeCustomer.NONE )
        {
            var user = User.Identity.GetUserClaimLogin();
           // var user = await _userManager.GetUserAsync(User);
           
            var response = await _mediator.Send(new SearchCustomerQuery()
            {
                type=type,
                Name = text,
                Comid = user.ComId,
            });
            var listkq = response.Data.Select(x => new AutocompleteCustomerModel
            {
                Id = x.Id,
                Name = x.Name,
                Buyer = x.Buyer,
                Code = x.Code,
                Taxcode = x.Taxcode,
                Address = x.Address,
                Cccd = x.CCCD,
                Email = x.Email,
                CusBankName = x.CusBankName,
                CusBankNo = x.CusBankNo,
                PhoneNumber = x.PhoneNumber != null ? x.PhoneNumber : string.Empty,
                Img = SystemVariable.SystemVariableHelper.UrlImgDefault_user
            }).ToList();
            return Json(listkq);
        }
        public async Task<IActionResult> GetJsonDataCustomer()
        {
            var user = User.Identity.GetUserClaimLogin();
          //  var user = await _userManager.GetUserAsync(User);
            var response = await _mediator.Send(new SearchCustomerQuery()
            {
                Comid = user.ComId,
            });
            if (response.Succeeded)
            {
                var listkq = response.Data.OrderBy(x => x.Name).Select(x => new
                {
                    id = x.Id,
                    text = !string.IsNullOrEmpty(x.Name) ? x.Name : x.Buyer
                }).ToList();
                if (listkq.Count()>0)
                {
                    var data = JsonConvert.SerializeObject(listkq);
                    return Content(data);
                }
            }
            return Content("[]");
        }
        public async Task<IActionResult> GetAllDataCustomerAsync(int? idselectd,string text="")
        {
            var user = User.Identity.GetUserClaimLogin();
           // var allUsersExceptCurrentUser = from d in _suppliersRepository.GetAll(user.ComId) select new { id = d.Id, text = d.Name, selected = d.Id == idselectd };
            var response = await _mediator.Send(new SearchCustomerQuery()
            {
                Name= text,
                Comid = user.ComId,
            });
            if (response.Succeeded)
            {
                response.Data.Add(new Domain.Entities.Customer { Id=-1,Name="Khách lẻ" });
                var listkq = response.Data.OrderBy(x => x.Name).Select(x => new
                {
                    id = x.Id,
                    selected = x.Id== idselectd,
                    text = !string.IsNullOrEmpty(x.Name)? x.Name: x.Buyer
                }).ToList();
                if (listkq.Count() > 0)
                {
                    return Content(JsonConvert.SerializeObject(listkq));
                }
            }
            return Content("[]");
        }
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
    }
}
