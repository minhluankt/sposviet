using Application.Constants;
using Application.Enums;
using Application.Features.Brands.Query;
using Application.Features.Products.Commands;
using Application.Features.Products.Query;
using Application.Features.PromotionRuns.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Model;
using SystemVariable;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Extensions;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : BaseController<ProductController>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IRepositoryAsync<Product> _repository;
        private readonly IRepositoryAsync<UploadImgProduct> _repositoryUploadImgProduct;
        private readonly ITypeSpecificationRepository _typespec;
        private readonly ICategoryProductRepository<CategoryProduct> _repositoryCategory;
        private readonly ISpecificationRepository _specificationRepository;
        private readonly IRepositoryAsync<City> _repositoryCountry;
        private readonly IDistrictRepository _repositoryDirectory;
        private readonly IUnitOfWork _unitOfWork;
        private IStringLocalizer<SharedResource> _localizer;
        private int idType = (int)ProductEnumcs.Procuct;
        public ProductController(IRepositoryAsync<Product> repository, IFormFileHelperRepository fileHelper,
            ITypeSpecificationRepository typespec, IOptions<CryptoEngine.Secrets> config,
            IUnitOfWork unitOfWork, IDistrictRepository repositoryDirectory,
             IRepositoryAsync<UploadImgProduct> repositoryUploadImgProduct,
             IStringLocalizer<SharedResource> localizer,
        ISpecificationRepository specificationRepository, IHostingEnvironment hostingEnvironment,
        ICategoryProductRepository<CategoryProduct> repositoryCategory,
            IRepositoryAsync<City> repositoryCountry)
        {
            _config = config;
            _repositoryDirectory = repositoryDirectory;
            _localizer = localizer;
            _unitOfWork = unitOfWork;
            _repositoryUploadImgProduct = repositoryUploadImgProduct;
            _repositoryCategory = repositoryCategory;
            _hostingEnvironment = hostingEnvironment;
            _typespec = typespec;
            _fileHelper = fileHelper;
            _specificationRepository = specificationRepository;
            _repositoryCountry = repositoryCountry;
            _repository = repository;
        }
        public async Task<ActionResult> LoadFullDataTable(int idProduct = 0)
        {
            // var htmledit = await _viewRenderer.RenderViewToStringAsync("_AccessaryTable", _accessaryrepository.GetListByProduct(idProduct));
            return new JsonResult(new { isValid = true, html = "" });
            // return PartialView("_AccessaryTable",)
        }
        [Authorize(Policy = "product.list")]
        public async Task<IActionResult> IndexAsync()
        {
            // var brand = await _mediator.Send(new GetAllBrandCacheQuery());
            ProductSearch productModelView = new ProductSearch();
            productModelView.Citys = _repositoryCountry.GetAllQueryable().AsNoTracking();
            // productModelView.Districts = _repositoryDirectory.List();
            productModelView.CategoryProducts = await _repositoryCategory.GetAllAsync();
            var PriceIC = _typespec.GetByCode(ParametersProduct.MUCGIA);
            productModelView.PriceICs = _specificationRepository.GetByIdType(PriceIC != null ? PriceIC.Id : 0);
            _logger.LogInformation(User.Identity.Name + "--> Product index");
            return View(productModelView);
        }


        [Authorize(Policy = "product.create")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> CreateAsync()
        {
            _logger.LogInformation(User.Identity.Name + "-->product  CreateOrEdit");
            ProductModelView productModelView = new ProductModelView();
            // loai xe
            var dataBrand = await _mediator.Send(new GetAllBrandCacheQuery());
            var getevent = await _mediator.Send(new GetActivePromotionRunQuery());
            if (getevent.Succeeded)
            {
                productModelView.PromotionRuns = getevent.Data;
            }
            productModelView.Code = Common.RandomString(SystemVariableHelper.LengthCodeProduct);
            productModelView.Active = true;
            productModelView.CategoryProducts = await _repositoryCategory.GetAllAsync();
            if (dataBrand.Succeeded)
            {
                if (dataBrand.Data.Count() > 0)
                {
                    productModelView.Brands = dataBrand.Data;
                }
            }
            var PriceIC = _typespec.GetByCode(ParametersProduct.MUCGIA);
            productModelView.PriceICs = _specificationRepository.GetByIdType(PriceIC != null ? PriceIC.Id : 0);

            return View(productModelView);
        }




        [Authorize(Policy = "product.create")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> EditAsync(int id)
        {
            _logger.LogInformation(User.Identity.Name + "-->product  CreateOrEdit");
            ProductModelView productModelView = new ProductModelView();
            // loai xe
            var values = "id=" + id;
            var secret = CryptoEngine.Encrypt(values, _config.Value.Key);

            var dataBrand = await _mediator.Send(new GetAllBrandCacheQuery());
            var getevent = await _mediator.Send(new GetActivePromotionRunQuery());
            if (getevent.Succeeded)
            {
                productModelView.PromotionRuns = getevent.Data;
            }
            var data = await _mediator.Send(new GetByIdProductQuery() { Id = id });
            if (data.Succeeded)
            {
                productModelView = _mapper.Map<ProductModelView>(data.Data);
                if (getevent.Succeeded)
                {
                    productModelView.PromotionRuns = getevent.Data;
                }
                productModelView.UrlParameters = secret;
                productModelView.CategoryProducts = await _repositoryCategory.GetAllAsync();

                var PriceIC1 = _typespec.GetByCode(ParametersProduct.MUCGIA);
                productModelView.PriceICs = _specificationRepository.GetByIdType(PriceIC1 != null ? PriceIC1.Id : 0);


                // productModelView.idattachment = productModelView.Documents != null ? productModelView.Documents.Select(m => m.Name).ToArray() : null;

                if (!string.IsNullOrEmpty(productModelView.Img))
                {
                    string path = Path.Combine(_hostingEnvironment.WebRootPath, productModelView.Img);
                    if (System.IO.File.Exists(path))
                    {
                        using (var stream = System.IO.File.OpenRead(path))
                        {
                            productModelView.ImgUpload = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                            productModelView.Img = Convert.ToBase64String(productModelView.ImgUpload.OptimizeImageSize(150, 150));
                            stream.Close();
                        };
                    }
                }
                if (dataBrand.Succeeded)
                {
                    if (dataBrand.Data.Count() > 0)
                    {
                        productModelView.Brands = dataBrand.Data;
                    }
                }
                return View(productModelView);
            }
            else
            {
                return View(productModelView);
            }
        }


        [Authorize(Policy = "product.create")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> CreateOrEditAsync(int id = 0)
        {
            // var brand = await _mediator.Send(new GetAllBrandCacheQuery());
            try
            {
                _logger.LogInformation(User.Identity.Name + "-->product  CreateOrEdit");
                ProductModelView productModelView = new ProductModelView();
                // loai xe
                var dataBrand = await _mediator.Send(new GetAllBrandCacheQuery());
                var getevent = await _mediator.Send(new GetActivePromotionRunQuery());
                if (getevent.Succeeded)
                {
                    productModelView.PromotionRuns = getevent.Data;
                }
                if (id > 0)
                {
                    var values = "id=" + id;
                    var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                    productModelView.UrlParameters = secret;
                    var data = await _mediator.Send(new GetByIdProductQuery() { Id = id });
                    if (data.Succeeded)
                    {
                        productModelView = _mapper.Map<ProductModelView>(data.Data);
                        if (getevent.Succeeded)
                        {
                            productModelView.PromotionRuns = getevent.Data;
                        }
                        productModelView.CategoryProducts = await _repositoryCategory.GetAllAsync();

                        var PriceIC1 = _typespec.GetByCode(ParametersProduct.MUCGIA);
                        productModelView.PriceICs = _specificationRepository.GetByIdType(PriceIC1 != null ? PriceIC1.Id : 0);


                        // productModelView.idattachment = productModelView.Documents != null ? productModelView.Documents.Select(m => m.Name).ToArray() : null;

                        if (!string.IsNullOrEmpty(productModelView.Img))
                        {
                            string path = Path.Combine(_hostingEnvironment.WebRootPath, productModelView.Img);
                            using (var stream = System.IO.File.OpenRead(path))
                            {
                                productModelView.ImgUpload = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                                productModelView.Img = Convert.ToBase64String(productModelView.ImgUpload.OptimizeImageSize(150, 150));
                                stream.Close();
                            };

                        }
                        if (dataBrand.Succeeded)
                        {
                            if (dataBrand.Data.Count() > 0)
                            {
                                productModelView.Brands = dataBrand.Data;
                            }
                        }

                        var htmledit = await _viewRenderer.RenderViewToStringAsync("CreateOrEdit", productModelView);
                        return new JsonResult(new { isValid = true, html = htmledit });
                    }
                    else
                    {
                        _notify.Error(_localizer.GetString("NotData").Value);
                        return new JsonResult(new { isValid = false, html = "" });
                    }

                }
                productModelView.Code = Common.RandomString(SystemVariableHelper.LengthCodeProduct);
                productModelView.Active = true;
                productModelView.CategoryProducts = await _repositoryCategory.GetAllAsync();
                if (dataBrand.Succeeded)
                {
                    if (dataBrand.Data.Count() > 0)
                    {
                        productModelView.Brands = dataBrand.Data;
                    }
                }
                var PriceIC = _typespec.GetByCode(ParametersProduct.MUCGIA);
                productModelView.PriceICs = _specificationRepository.GetByIdType(PriceIC != null ? PriceIC.Id : 0);

                var html = await _viewRenderer.RenderViewToStringAsync("CreateOrEdit", productModelView);
                return new JsonResult(new { isValid = true, html = html });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false, html = "" });
            }
            // return View(productModelView);
        }

        [HttpGet]
        [EncryptedParameters("secret")]
        public async Task<JsonResult> GetStyleProduct(int id)
        {
            try
            {
                var get = await _mediator.Send(new GetStyleProductQuery() { IdProduct = id });
                if (get.Succeeded)
                {
                    return new JsonResult(new { isValid = true, data = get.Data });
                }
                return new JsonResult(new { isValid = false });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.ToString());
                return new JsonResult(new { isValid = false });
            }

        }



        public ActionResult RandomStringCode()
        {
            string code = Common.RandomString(SystemVariableHelper.LengthCodeProduct);
            return new JsonResult(new { isValid = true, data = code });
        }
        [HttpPost]
        [AllowAnonymous]
        [System.Web.Mvc.ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(ProductModelView collection)
        {
            
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (collection.Id == 0)
                        {


                            if (string.IsNullOrEmpty(collection.Img))
                            {
                                _notify.Error("Vui lòng chọn hình ảnh đại diện");
                                // return View(collection);
                                return new JsonResult(new { isValid = false, html = string.Empty });
                            }
                            var createProductCommand = _mapper.Map<CreateProductCommand>(collection);

                            var result = await _mediator.Send(createProductCommand);
                            if (result.Succeeded)
                            {
                                collection.Id = result.Data;
                                _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS008));
                                //return RedirectToAction("Index");
                            }
                            else
                            {
                            _notify.Error(GeneralMess.ConvertStatusToString(result.Message));
                            return new JsonResult(new { isValid = false, html = string.Empty });
                                //return View(collection);
                            }
                        }
                        else
                        {
                            var updateProductCommand = _mapper.Map<UpdateProductCommand>(collection);

                            var result = await _mediator.Send(updateProductCommand);
                            if (result.Succeeded)
                            {
                                _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                            }
                            else
                            {
                                _notify.Error(GeneralMess.ConvertStatusToString(result.Message));
                                return new JsonResult(new { isValid = false, html = string.Empty });
                                // return View(collection);
                            }

                        }
                        //return RedirectToAction("Index");

                        return new JsonResult(new { isValid = true, url = Url.Action("Index") });
                    }
                    catch (Exception e)
                    {

                        _logger.LogError(e, e.ToString());
                        _notify.Error(e.ToString());
                        //return View(collection);
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }
                }

                else
                {
                    foreach (var modelStateKey in ViewData.ModelState.Keys)
                    {
                        var modelStateVal = ViewData.ModelState[modelStateKey];
                        foreach (var error in modelStateVal.Errors)
                        {
                            var key = modelStateKey;
                            var errorMessage = error.ErrorMessage;
                            var exception = error.Exception;
                            _notify.Error(key + errorMessage);
                            // You may log the errors if you want
                        }
                    }

                    // return View(collection);
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
            

        }
        // chay lệnh update
        public async Task<IActionResult> UpdateImgAsync()
        {
            try
            {
                var list = _repository.GetAllQueryable().AsNoTracking().ToList();
                foreach (var item in list)
                {
                    if (!string.IsNullOrEmpty(item.AlbumImg))
                    {
                        if (item.AlbumImg.IndexOf("$") != -1)
                        {
                            string[] arr = item.AlbumImg.Split("$");
                            List<UploadImgProduct> UploadImgProducts = new List<UploadImgProduct>();
                            foreach (var img in arr)
                            {
                                string fileName = Path.Combine(this._hostingEnvironment.WebRootPath, "Upload/" + FolderUploadConstants.Product + "/" + img);
                                var fi1 = new FileInfo(fileName);
                                UploadImgProduct forms = new()
                                {
                                    FileName = img,
                                    IdProduct = item.Id,
                                    Size = fi1.Length
                                };
                                UploadImgProducts.Add(forms);

                            }
                            await _repositoryUploadImgProduct.AddRangeAsync(UploadImgProducts);
                        }
                        else
                        {
                            string fileName = Path.Combine(this._hostingEnvironment.WebRootPath, "Upload/" + FolderUploadConstants.Product + "/" + item.AlbumImg);
                            var fi1 = new FileInfo(fileName);
                            UploadImgProduct forms = new()
                            {
                                FileName = item.AlbumImg,
                                IdProduct = item.Id,
                                Size = fi1.Length
                            };
                            await _repositoryUploadImgProduct.AddAsync(forms);
                        }
                        await _unitOfWork.SaveChangesAsync(new CancellationToken());
                    }
                }
                return Content("OK");
            }
            catch (Exception e)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> LoadAll(ProductSearch model)
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
                var response = await _mediator.Send(new GetAllProductCacheQuery()
                {
                    Product = model,
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
        [Authorize(Policy = "product.details")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> DetailsAsync(int id)
        {
            try
            {
                _logger.LogInformation(User.Identity.Name + "--> product detailt");
                var data = await _mediator.Send(new GetByIdProductQuery() { Id = id });
                if (data.Succeeded)
                {
                    var datamodel = _mapper.Map<ProductModelView>(data.Data);
                    // var datamodel = data.Data;
                    if (!string.IsNullOrEmpty(datamodel.Img))
                    {
                        string path = Path.Combine(_hostingEnvironment.WebRootPath, datamodel.Img);
                        using (var stream = System.IO.File.OpenRead(path))
                        {
                            datamodel.ImgUpload = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                            datamodel.Img = Convert.ToBase64String(datamodel.ImgUpload.OptimizeImageSize(150, 150));
                            stream.Close();
                        };

                    }
                    var html = await _viewRenderer.RenderViewToStringAsync("_Detail", datamodel);
                    return new JsonResult(new { isValid = true, html = html, loadDataTable = true, modelFooter = true });
                }
                _notify.Error(_localizer.GetString("NotData").Value);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

        }

        [Authorize(Policy = "product.delete")]
        [HttpPost]
        [EncryptedParameters("secret")]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var deleteCommand = await _mediator.Send(new DeleteProductCommand { Id = id });
                if (deleteCommand.Succeeded)
                {
                    _notify.Success(_localizer.GetString("DeleteOk").Value);
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
