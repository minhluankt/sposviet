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
using Infrastructure.Infrastructure.Identity.Models;
using Joker.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Model;
using NuGet.Packaging;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Drawing2D;
using System.Reflection;
using SystemVariable;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Extensions;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class ProductController : BaseController<ProductController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ITypeSpecificationRepository _typespec;
        private readonly ICategoryProductRepository<CategoryProduct> _repositoryCategory;
        private readonly ISpecificationRepository _specificationRepository;
        private int idType = (int)ProductEnumcs.Procuct;
        public ProductController(IRepositoryAsync<Product> repository, IFormFileHelperRepository fileHelper,
            ITypeSpecificationRepository typespec, IOptions<CryptoEngine.Secrets> config, UserManager<ApplicationUser> userManager,
        ISpecificationRepository specificationRepository, IHostingEnvironment hostingEnvironment,
        ICategoryProductRepository<CategoryProduct> repositoryCategory,
            IRepositoryAsync<City> repositoryCountry)
        {
            _userManager = userManager;
            _config = config;
            _repositoryCategory = repositoryCategory;
            _hostingEnvironment = hostingEnvironment;
            _typespec = typespec;
            _specificationRepository = specificationRepository;
        }
        [Authorize(Policy = "product.list")]
        public IActionResult Index()
        {
            // var brand = await _mediator.Send(new GetAllBrandCacheQuery());
            ProductSearch productModelView = new ProductSearch();
            _logger.LogInformation(User.Identity.Name + "--> Product index");
            return View(productModelView);
        }
        [Authorize(Policy = "product.priceBook")]
        public IActionResult PriceBook()
        {
            // var brand = await _mediator.Send(new GetAllBrandCacheQuery());
            ProductSearch productModelView = new ProductSearch();
            _logger.LogInformation(User.Identity.Name + "--> PriceBook index");
            return View(productModelView);
        }
        private string GetDisplayName(object value)
        {
            var type = value.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException(string.Format("Type {0} is not an enum", type));
            }
            var field = type.GetField(value.ToString());
            if (field == null)
            {
                return value.ToString();
            }

            var attributes = field.GetCustomAttribute<DisplayAttribute>();
            return attributes != null ? attributes.Name : value.ToString();
        }
        private List<SelectListItem> GetSelectListItem(LISTVAT type = LISTVAT.NOVAT)
        {
            var select = Enum.GetValues(typeof(LISTVAT)).Cast<LISTVAT>()
                .OrderBy(x => (Convert.ToInt32(x)))
                .Where(x => (Convert.ToInt32(x)) > 0)
                .Select(x => new SelectListItem
                {
                    Text = GetDisplayName(x),
                    Value = Convert.ToInt32(x).ToString(),
                    Selected = x == type
                }).ToList();
            select.Insert(0, new SelectListItem()
            {
                Text = "",
                Value = "",
            });
            return select;
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
                if (string.IsNullOrEmpty(model.Name))
                {
                    model.Name = searchValue;
                }
                //Paging Size (10, 20, 50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;

                int skip = start != null ? Convert.ToInt32(start) : 0;

                int recordsTotal = 0;

                var currentUser = User.Identity.GetUserClaimLogin();;
                model.ComId = currentUser.ComId;
                // getting all Customer data  
                var response = await _mediator.Send(new GetAllProductCacheQuery()
                {
                    TypeProduct = currentUser.IdDichVu,
                    Product = model,
                    sortColumn = sortColumn,
                    sortColumnDirection = sortColumnDirection,
                    pageSize = pageSize,
                    skip = skip
                });
                if (response.Succeeded)
                {
                    recordsTotal = int.Parse(response.Message);
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

        [Authorize(Policy = "product.create")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> CreateOrEditAsync(EnumTypeProductCategory TypeProductCategory,int id = 0)
        {
            // var brand = await _mediator.Send(new GetAllBrandCacheQuery());
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();
                _logger.LogInformation(User.Identity.Name + "-->product  CreateOrEdit");
                ProductModelView productModelView = new ProductModelView();
                productModelView.TypeProductCategory = TypeProductCategory;
             
                if (id > 0)
                {
                    var values = "id=" + id;
                    var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                    productModelView.UrlParameters = secret;
                   
                    var data = await _mediator.Send(new GetByIdProductQuery() { Id = id ,Comid= currentUser.ComId,IncludeComboProduct=true});
                    if (data.Succeeded)
                    {
                        productModelView = _mapper.Map<ProductModelView>(data.Data);
                        if (data.Data.TypeProductCategory==EnumTypeProductCategory.COMBO)
                        {
                            var getcombo = await _mediator.Send(new SearchProductQuery() { 
                                ComId = currentUser.ComId,
                                LstIdProduct = data.Data?.ComponentProducts?.Select(x=>x.IdPro).ToArray()
                            });
                            if (getcombo.Succeeded)
                            {
                                var listdata = await getcombo.Data.ToListAsync();//chuyển thành list
                                if (data.Data.ComponentProducts.Count() != listdata.Count())
                                {
                                    _notify.Error("Thành phần combo đã bị xóa đi một ít sản phẩm, vui lòng kiểm tra lại");
                                }
                                productModelView.JsonListComboProductModel = data.Data.ComponentProducts.Select(x => new ComboProductModel()
                                {
                                    Id = x.Id,
                                    IdPro = x.IdPro,
                                    Quantity = x.Quantity
                                }).ToList();
                                productModelView.JsonListComboProductModel.ForEach( x =>
                                {
                                    var getfirst =  listdata.FirstOrDefault(z => z.Id == x.IdPro);
                                    x.Code = getfirst.Code;
                                    x.Name = getfirst.Name;
                                    x.RetailPrice = getfirst.RetailPrice;
                                    x.IdPro = getfirst.Id;
                                    x.Price = getfirst.Price;
                                }
                                );
                            }
                            else
                            {
                                _notify.Error("Không tìm thấy các sản phẩm trong thành phần!");
                            }
                            
                        }
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

                        var htmledit = await _viewRenderer.RenderViewToStringAsync("CreateOrEdit", productModelView);
                        return new JsonResult(new { 
                            isValid = true,
                            html = htmledit,
                            typeProductCategory = (int)productModelView.TypeProductCategory,
                            idcategory = productModelView.idCategory,
                            idUnit = productModelView.IdUnit,
                            title = "Cập nhật sản phẩm" });
                    }
                    else
                    {
                        _notify.Error(_localizer.GetString("NotData").Value);
                        return new JsonResult(new { isValid = false, html = "" });
                    }
                }
                // productModelView.CategoryProducts = await _repositoryCategory.GetAllAsync();
                productModelView.DirectSales = true;
                productModelView.VATRate = (int)NOVAT.NOVAT;
                var html = await _viewRenderer.RenderViewToStringAsync("CreateOrEdit", productModelView);
                return new JsonResult(new { isValid = true, html = html, idcategory =0, idUnit = 0, typeProductCategory = (int)TypeProductCategory });
            }
            catch (Exception e) 
            {
                _logger.LogError(e, e.Message);
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false, html = "",title="Thêm mới sản phẩm" });
            }
        }
        [Authorize(Policy = "product.create")]

        public async Task<ActionResult> CreateAsync()
        {
            _logger.LogInformation(User.Identity.Name + "-->product  CreateOrEdit");
            ProductModelView productModelView = new ProductModelView();
            // loai xe

            var getevent = await _mediator.Send(new GetActivePromotionRunQuery());
            if (getevent.Succeeded)
            {
                productModelView.PromotionRuns = getevent.Data;
            }
            //productModelView.Code = Common.RandomString(SystemVariableHelper.LengthCodeProduct);
            productModelView.VATRate = (int)NOVAT.NOVAT;
            productModelView.Active = true;
            productModelView.CategoryProducts = await _repositoryCategory.GetAllAsync();

            return View(productModelView);
        }

        public async Task<JsonResult> GetProductJson()
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var data = await _mediator.Send(new SearchProductQuery() { ComId= currentUser.ComId,IncludeCategory=true, IsTolist = true, CheckIsStopBusiness = true, typeProduct = currentUser.IdDichVu});
            if (data.Succeeded)
            {
                //var getdata = data.Data.ToList();
                var jspm = data.Data.OrderByDescending(x=>x.IdCategory).Select(x => new ProductPosModel()
                {
                    Code = x.Code,
                    Img = x.Img,
                    Name = x.Name,
                    Id = x.Id,
                    IsVAT = x.IsVAT,
                    RetailPrice = x.Price,
                    idString = CryptoEngine.Encrypt("id=" + x.Id, _config.Value.Key),
                    Quantity = x.Quantity,
                    IdCategory = (x.CategoryProduct != null ? x.CategoryProduct.Id : 0),
                    NameCategory = (x.CategoryProduct != null ? x.CategoryProduct.Name : "Chung"),
                }).ToList();
                var GroupBycate = jspm.GroupBy(x => x.IdCategory).Select(x => new
                {
                    name = x.First().NameCategory,
                    id = x.First().IdCategory,
                    countpro = x.Count()
                });
                return new JsonResult(new { isValid = true, jsonPro = jspm, jsoncate = GroupBycate });
            }
            return new JsonResult(new { isValid = false});
        }
        [HttpPost]
        [Authorize(Policy = "product.updatePrice")]
        public async Task<ActionResult> UpdatePriceAsync(int id,decimal price,decimal PriceNoVAT,decimal VATRate)
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();;
                var get = await _mediator.Send(new UpdatePriceCommand() { Id = id,Price= price,PriceNoVAT= PriceNoVAT,VATRate=VATRate, ComId = currentUser.ComId});
                if (get.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                    return new JsonResult(new { isValid = true });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(get.Message));
                return new JsonResult(new { isValid = false });
            }
            catch (Exception e)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(e.Message));
                _logger.LogError(e, e.ToString());
                return new JsonResult(new { isValid = false });
            }
        }
        [HttpPost]
        [Authorize(Policy = "product.updatePrice")]
        public async Task<ActionResult> UpdateVATPriceAsync(int id,decimal VatRate,decimal PriceNoVat)
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();;
                var get = await _mediator.Send(new UpdatePriceCommand() { Id = id,PriceNoVAT= PriceNoVat,VATRate= VatRate, ComId = currentUser.ComId,TypeUpdatePriceProduct=EnumTypeUpdatePriceProduct.VATPRICE});
                if (get.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                    return new JsonResult(new { isValid = true });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(get.Message));
                return new JsonResult(new { isValid = false });
            }
            catch (Exception e)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(e.Message));
                _logger.LogError(e, e.ToString());
                return new JsonResult(new { isValid = false });
            }
        }

        [HttpPost]
        [Authorize(Policy = "product.updatePrice")]
        public async Task<ActionResult> UpdatePriceNoVATAsync(int id,decimal VatRate,decimal PriceNoVat)
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();;
                var get = await _mediator.Send(new UpdatePriceCommand() { Id = id,PriceNoVAT= PriceNoVat,VATRate= VatRate, ComId = currentUser.ComId,TypeUpdatePriceProduct=EnumTypeUpdatePriceProduct.PRICENOVAT});
                if (get.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                    return new JsonResult(new { isValid = true });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(get.Message));
                return new JsonResult(new { isValid = false });
            }
            catch (Exception e)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(e.Message));
                _logger.LogError(e, e.ToString());
                return new JsonResult(new { isValid = false });
            }
        }

        [Authorize(Policy = "product.edit")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> EditAsync(int id)
        {
            _logger.LogInformation(User.Identity.Name + "-->product  CreateOrEdit");
            ProductModelView productModelView = new ProductModelView();

            var values = "id=" + id;
            var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
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

                return View(productModelView);
            }
            else
            {
                return View(productModelView);
            }
        }


        [HttpPost]
        [System.Web.Mvc.ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(ProductModelView collection)
        {
            if (collection.TypeProductCategory==EnumTypeProductCategory.COMBO)
            {
                if (collection.JsonListComboProducts.Count()==0)
                {
                    _notify.Error("Bạn chưa chọn hàng hóa thành phần cho commbo");
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
            }
            var currentUser = User.Identity.GetUserClaimLogin();;
            collection.TypeProduct = currentUser.IdDichVu;
            collection.Comid = currentUser.ComId;
            if (ModelState.IsValid)
            {
                try
                {
                    if (collection.Id == 0)
                    {
                        if (currentUser.IdDichVu == EnumTypeProduct.BAN_LE || currentUser.IdDichVu == EnumTypeProduct.TAPHOA_SIEUTHI || currentUser.IdDichVu == EnumTypeProduct.AMTHUC)
                        {

                        }
                        else if (string.IsNullOrEmpty(collection.Img))
                        {
                            _notify.Error("Vui lòng chọn hình ảnh đại diện");
                            // return View(collection);
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }

                        var createProductCommand = _mapper.Map<CreateProductCommand>(collection);
                        collection.ComponentProducts = new List<ComponentProduct>();

                        if (collection.TypeProductCategory==EnumTypeProductCategory.COMBO)
                        {
                            if (collection.JsonListComboProducts.Count() > 0)
                            {
                                createProductCommand.RetailPrice = collection.JsonListComboProducts.Sum(x => x.RetailPrice * x.Quantity);
                                createProductCommand.ComponentProducts = collection.JsonListComboProducts.Select(x => new ComponentProduct()
                                {
                                    Id = x.Id,
                                    IdPro = x.IdPro,
                                    TypeComponentProduct = EnumtypeComponentProduct.COMPONENT,
                                    Quantity = x.Quantity
                                }).ToList();
                            }
                            if (collection.JsonListExtraToppingProductModel.Count() > 0)
                            {
                                var getlist = collection.JsonListExtraToppingProductModel.Select(x => new ComponentProduct()
                                {
                                    Id = x.Id,
                                    IdPro = x.IdPro,
                                    TypeComponentProduct = EnumtypeComponentProduct.EXTRA_TOPPING,
                                    //Quantity = x.Quantity
                                }).ToList();
                                createProductCommand.ComponentProducts.AddRange(getlist);
                            }
                        }
                        createProductCommand.VATRate = createProductCommand.IsVAT ? createProductCommand.VATRate : (int)NOVAT.NOVAT;
                        var result = await _mediator.Send(createProductCommand);
                        if (result.Succeeded)
                        {
                            collection.Id = result.Data;
                            _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS008));
                            //return RedirectToAction("Index");
                        }
                        else
                        {
                            _notify.Error(result.Message);
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                    }
                    else
                    {
                        if (collection.TypeProductCategory==EnumTypeProductCategory.NONE && !collection.IsServiceDate)
                        {
                            collection.TypeProductCategory = EnumTypeProductCategory.PRODUCT;
                        }
                        if (collection.TypeProductCategory == EnumTypeProductCategory.NONE)
                        {
                            _notify.Error("Dữ liệu không hợp lệ, không xác định được TypeProductCategory");
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                        var updateProductCommand = _mapper.Map<UpdateProductCommand>(collection);
                        if (collection.TypeProductCategory == EnumTypeProductCategory.COMBO)
                        {
                            if (collection.JsonListComboProducts.Count() > 0)//dành cho thafh phần
                            {
                                updateProductCommand.RetailPrice = collection.JsonListComboProducts.Sum(x => x.RetailPrice * x.Quantity);
                                updateProductCommand.ComponentProducts = collection.JsonListComboProducts.Select(x => new ComponentProduct()
                                {
                                    Id = x.Id,
                                    IdPro = x.IdPro,
                                    TypeComponentProduct = EnumtypeComponentProduct.COMPONENT,
                                    Quantity = x.Quantity
                                }).ToList();
                            }
                            if (collection.JsonListExtraToppingProductModel.Count() > 0)//dành cho món thêm
                            {
                                updateProductCommand.ExtraToppings = collection.JsonListExtraToppingProductModel.Select(x => new ComponentProduct()
                                {
                                    Id = x.Id,
                                    IdPro = x.IdPro,
                                    TypeComponentProduct = EnumtypeComponentProduct.EXTRA_TOPPING
                                }).ToList();
                            }

                        }


                        var result = await _mediator.Send(updateProductCommand);
                        if (result.Succeeded)
                        {
                            _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                        }
                        else
                        {
                            _notify.Error(result.Message);
                            return new JsonResult(new { isValid = false, html = string.Empty });
                            // return View(collection);
                        }
                    }
                    //return RedirectToAction("Index");

                    return new JsonResult(new { isValid = true, loadTable = true, closeSwal = true, url = Url.Action("Index") });
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
        [Authorize(Policy = "product.delete")]
        [HttpPost]
        public async Task<JsonResult> Delete(int[] lstid)
        {
            var currentUser = User.Identity.GetUserClaimLogin(); ;
            try
            {
                if (lstid==null)
                {
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
                var deleteCommand = await _mediator.Send(new DeleteProductCommand { lstid = lstid,isDeleteMuti=true,ComId= currentUser.ComId });
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
        [Authorize(Policy = "product.details")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> DetailsAsync(int id)
        {
            try
            {
                var currentUser = User.Identity.GetUserClaimLogin();
                _logger.LogInformation(User.Identity.Name + "--> product detailt");
                var data = await _mediator.Send(new GetByIdProductQuery() { Id = id , IncludeComboProduct =true});
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
                    if (data.Data.TypeProductCategory == EnumTypeProductCategory.COMBO)
                    {
                        var getcombo = await _mediator.Send(new SearchProductQuery()
                        {
                            ComId = currentUser.ComId,
                            LstIdProduct = data.Data?.ComponentProducts?.Select(x => x.IdPro).ToArray()
                        });
                        if (getcombo.Succeeded)
                        {
                            var listdata = await getcombo.Data.ToListAsync();//chuyển thành list
                            if (data.Data.ComponentProducts.Count() != listdata.Count())
                            {
                                _notify.Error("Thành phần combo đã bị xóa đi một ít sản phẩm, vui lòng kiểm tra lại");
                            }
                            datamodel.JsonListComboProductModel = data.Data.ComponentProducts.Select(x => new ComboProductModel()
                            {
                                Id = x.Id,
                                IdPro = x.IdPro,
                                Quantity = x.Quantity
                            }).ToList();
                            datamodel.JsonListComboProductModel.ForEach(x =>
                            {
                                var getfirst = listdata.FirstOrDefault(z => z.Id == x.IdPro);
                                x.Code = getfirst.Code;
                                x.Name = getfirst.Name;
                                x.RetailPrice = getfirst.RetailPrice;
                                x.IdPro = getfirst.Id;
                                x.Price = getfirst.Price;
                            }
                            );
                        }
                        else
                        {
                            _notify.Error("Không tìm thấy các sản phẩm trong thành phần!");
                        }

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
        public async Task<JsonResult> PrintBarCode(int[] lstid)
        {
            var currentUser = User.Identity.GetUserClaimLogin(); 
            try
            {
                if (lstid==null)
                {
                    _notify.Error("Vui lòng chọn sản phẩm");
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
                var deleteCommand = await _mediator.Send(new PrintBarCodeQuery { Comid = currentUser.ComId, lstidPro = lstid });
                if (deleteCommand.Succeeded)
                {
                    var html = await _viewRenderer.RenderViewToStringAsync("PrintBarCode", deleteCommand.Data);
                    //_notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS007));
                    return new JsonResult(new { isValid = true, html = html });
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
        [Authorize(Policy = "product.stopbusiness")]
        [HttpPost]
        [EncryptedParameters("secret")]
        public async Task<JsonResult> StopBusiness(int[] lstid)
        {
            var currentUser = User.Identity.GetUserClaimLogin(); ;
            try
            {
                if (lstid==null)
                {
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
                var deleteCommand = await _mediator.Send(new UpdateProductCommonCommand { lstid = lstid,ComId= currentUser.ComId,TypeUpdateProduct=ENumTypeUpdateProduct.STOPBUSINESS });
                if (deleteCommand.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                    return new JsonResult(new { isValid = true, html = string.Empty,data= deleteCommand.Data});
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
        [Authorize(Policy = "product.stopbusiness")]
        [HttpPost]
        [EncryptedParameters("secret")]
        public async Task<JsonResult> UnStopBusiness(int[] lstid)
        {
            var currentUser = User.Identity.GetUserClaimLogin(); ;
            try
            {
                if (lstid == null)
                {
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
                var deleteCommand = await _mediator.Send(new UpdateProductCommonCommand { lstid = lstid, ComId = currentUser.ComId, TypeUpdateProduct = ENumTypeUpdateProduct.UNSTOPBUSINESS });
                if (deleteCommand.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                    return new JsonResult(new { isValid = true,  data = deleteCommand.Data });
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
