using Application.Constants;
using Application.Enums;
using Application.Features.CategorysProduct.Query;
using Application.Features.CompanyInfo.Query;
using Application.Features.ConfigSystems.Query;
using Application.Features.Posts.Querys;
using Application.Features.Products.Query;
using Application.Features.ReSearchs.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Model;
using System.Web;
using SystemVariable;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Areas.Admin.Models;
using Web.ManagerApplication.Models;
using X.PagedList;


namespace Web.ManagerApplication.Controllers
{
    public class SearchController : BaseController<SearchController>
    {
        private readonly IReSearchRepository _reSearchCacheRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductPepository<Product> _productepository;
        private readonly ITableLinkRepository _tableLinkRepository;
        private readonly ICategoryProductRepository<CategoryProduct> _Repositorycate;
        private readonly ICategoryPostRepository<CategoryPost> _categoryPostRepository;


        public SearchController(ITableLinkRepository tableLinkRepository,
            IUserRepository userRepository,
                IReSearchRepository reSearchCacheRepository, ICategoryPostRepository<CategoryPost> categoryPostRepository,
            IProductPepository<Product> productepository,
            ICategoryProductRepository<CategoryProduct> Repositorycate)
        {
            _reSearchCacheRepository = reSearchCacheRepository;
            _userRepository = userRepository;
            _categoryPostRepository = categoryPostRepository;
            _productepository = productepository;
            _Repositorycate = Repositorycate;
            _tableLinkRepository = tableLinkRepository;
        }
        public IActionResult Index(ProductSearch model)
        {
            try
            {
                // var user = await _userRepository.GetUserAsync(User);
                model.TypeProduct = (int)CategorySerach.SanPham;

                // model.TypeCategory = _typeCategory.GetByCode(string.Empty, model.ProductType); //
                // model.Categorys = await _Repositorycate.GetListByIdTypeCategory(productViewModel.TypeCategory.Id);
                //if (!string.IsNullOrEmpty(model.keyword))
                //{
                //    var task = Task.Run(() =>
                //    {
                //        _reSearchCacheRepository.Add(new ReSearch() { ProductType = model.ProductType, Name = model.keyword, IdCustomer = user != null ? user.Id : null });

                //    });
                //}

                return View("~/Views/Site/CategoryProduct.cshtml", model);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return View("~/Views/Site/CategoryProduct.cshtml", new SearchViewModel());

            }
        }
        private async Task<int> GetpageSiteAsync()
        {
            var getid = await _mediator.Send(new GetAllConfigQuery());
            if (getid.Succeeded)
            {
                var get = getid.Data.Where(m => m.Key == ParametersConfigSystem.pageSizeProductInCategory).SingleOrDefault();
                if (get != null)
                {
                    if (!string.IsNullOrEmpty(get.Value))
                    {
                        return int.Parse(get.Value);
                    }
                }
            }
            return 15;
        }
        public async Task<IActionResult> GetListPostAsync(int page, int idcategory, string text)
        {
            try
            {
                int[] lstcategory = null;
                if (idcategory > 0)
                {
                    lstcategory = await _categoryPostRepository.GetListArrayChillAllByIdAsync(idcategory);
                }
                PostViewModel postView = new PostViewModel();
                var data = await _mediator.Send(new GetAllIQueryablePostQuery() { IncludeCategory = false, lstIdcategory = lstcategory, text = text });
                if (data.Succeeded)
                {
                    postView.PostsIPagedList = await data.Data.ToPagedListAsync(page, await this.GetpageSiteAsync());
                }
                var company = await _mediator.Send(new GetByIdCompanyInfoQuery());
                if (company.Succeeded)
                {
                    ViewBag.Img = company.Data.Logo;
                }
                var html = await _viewRenderer.RenderViewToStringAsync("_PartialListPost", postView);
                return new JsonResult(new { isValid = true, html = html });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false, html = "" });
            }

        }
        public async Task<IActionResult> GetProductAsync(SearchViewModel model)
        {
            var user = await _userRepository.GetUserAsync(User);
            try
            {

                ProductSearch ProductModelView = new ProductSearch();
                ProductModelView.idCategory = model.idcategory;
                ProductModelView.keyword = HttpUtility.UrlDecode(model.keyword);
                ProductModelView.sortby = model.sortby;
                ProductModelView.idPrice = model.idPrice;
                ProductModelView.TypeSerach = model.TypeSerach;
                ProductModelView.isPromotion = model.isPromotion;

                if (model.idcategory > 0)
                {
                    var data = await _mediator.Send(new GetByIdCategoryProductQuery() { Id = model.idcategory });
                    if (data.Succeeded)
                    {
                        ProductModelView.CategoryProduct = data.Data;
                        //if (string.IsNullOrEmpty(model.keyword))
                        {
                            var getarrCategory = (await _Repositorycate.GetListArrayChillAllByIdAsync(ProductModelView.idCategory));
                            ProductModelView.lstidCategory = getarrCategory;
                        }
                    }
                }
                var listproduct = await _mediator.Send(new GetIQueryableProductQuery()
                {
                    ProductSearch = ProductModelView,
                    searchCustome = true
                });

                if (listproduct.Succeeded)
                {
                    if (!string.IsNullOrEmpty(ProductModelView.keyword) && listproduct.Data.Count() > 0)
                    {
                        var task = Task.Run(() =>
                        {
                            _reSearchCacheRepository.Add(new ReSearch() { ProductType = model.ProductType, Name = model.keyword, IdCustomer = user != null ? user.Id : null });

                        });
                    }

                    if (model.pagesite == 0)
                    {
                        model.pagesite = await GetpageSiteAsync();
                    }
                    ProductModelView.ProductPagedList = await _productepository.ToPagedListAsync(listproduct.Data, model.pagenumber, model.pagesite);
                }
                var company = await _mediator.Send(new GetByIdCompanyInfoQuery());
                if (company.Succeeded)
                {
                    ProductModelView.Img = company.Data.Logo;
                }
                var html = await _viewRenderer.RenderViewToStringAsync("_PartialListProduct", ProductModelView);

                return new JsonResult(new { isValid = true, html = html, countData = listproduct.Data.Count() });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new JsonResult(new { isValid = false });
            }

            //    Products = await _productepository.ToPagedListAsync(listproduct.Data.Products, model.pagenumber, model.pagesite);
            //    var html = await _viewRenderer.RenderViewToStringAsync("_PartialListProduct", Products);
            //    return new JsonResult(new { isValid = true, html = html, countData = listproduct.Data.Products.Count() });
            //}

            // return new JsonResult(new { isValid = false });

            // thêm điều kiện tìm kiếm và onchange select2 nhé,


        }

        public async Task<IActionResult> GetProductByCategoryAsync(SearchIndexModel searchIndex)
        {
            try
            {
                int idcategory = searchIndex.idcate;
                if (idcategory == 0 && !string.IsNullOrEmpty(searchIndex.txtcategory))
                {
                    idcategory = int.Parse(EncryptionHelper.Decrypt(System.Web.HttpUtility.UrlDecode(searchIndex.txtcategory), SystemVariableHelper.publicKey));

                }
                List<ProductSellModel> productSellModels = new List<ProductSellModel>();
                ProductSellModel modeldata = new ProductSellModel();

                if (idcategory > 0)
                {
                    var data = await _mediator.Send(new GetByIdCategoryProductQuery() { Id = idcategory });
                    if (data.Succeeded)
                    {

                        // modeldata.Id = data.Data.Id;
                        modeldata.IdCode = EncryptionHelper.Encrypt(data.Data.Id.ToString(), SystemVariableHelper.publicKey);
                        modeldata.IdParent = data.Data.IdPattern;
                        modeldata.Name = data.Data.Name;
                        modeldata.Slug = data.Data.Slug;
                        modeldata.SlugCateParent = searchIndex.slugcate;
                        modeldata.Sort = data.Data.Sort;
                        modeldata.sortby = searchIndex.sortby;
                        var company = await _mediator.Send(new GetByIdCompanyInfoQuery());
                        if (company.Succeeded)
                        {
                            modeldata.Img = company.Data.Logo;
                        }
                    }
                    var getarrCategory = await _Repositorycate.GetListArrayChillAllByIdAsync(idcategory);
                    ProductSearch model = new ProductSearch();
                    model.lstidCategory = getarrCategory;
                    model.isPromotion = searchIndex.isPromotion;
                    model.pageNumber = searchIndex.page;
                    model.idPrice = searchIndex.idPrice;
                    model.sortby = searchIndex.sortby;
                    model.pagesite = await this.GetpageSiteAsync();
                    var listproduct = await _mediator.Send(new GetIQueryableProductQuery()
                    {
                        ProductSearch = model
                    });

                    if (listproduct.Succeeded)
                    {
                        modeldata.products = await _productepository.ToPagedListAsync(listproduct.Data, model.pageNumber, model.pagesite);
                    }

                    productSellModels.Add(modeldata);
                }
                if (searchIndex.loadmore)
                {
                    return View("_ListProductSellItem", productSellModels.FirstOrDefault());
                }
                return View("GetProductSell", productSellModels);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

        }
        public async Task<IActionResult> GetProductSellAsync(string slug, int idcate = 0)

        {
            try
            {
                var data = await _mediator.Send(new SearchProductQuery() { isPromotion = true, IncludeCategory = true });
                if (data.Succeeded)
                {
                    bool fag = false;
                    List<ProductSellModel> productSellModels = new List<ProductSellModel>();
                    var geta = data.Data.ToList().GroupBy(x => x.IdCategory);
                    foreach (var items in geta)
                    {

                        var firstpro = items.FirstOrDefault();
                        if (firstpro.CategoryProduct.IdPattern > 0)
                        {
                            fag = true;
                        }
                        var productSellModel = new ProductSellModel() { Id = items.Key };

                        productSellModel.IdParent = firstpro.CategoryProduct.IdPattern;
                        productSellModel.Name = firstpro.CategoryProduct.Name;
                        productSellModel.Slug = firstpro.CategoryProduct.Slug;
                        productSellModel.SlugCateParent = slug;
                        productSellModel.Sort = firstpro.CategoryProduct.Sort;
                        productSellModel.productslist = items.ToList();
                        productSellModels.Add(productSellModel);

                    }
                    if (fag)
                    {

                        var updatelist = productSellModels.GroupBy(x => x.IdParent);
                        productSellModels = new List<ProductSellModel>();
                        foreach (var item in updatelist)
                        {
                            //var u = item.ToList();
                            var getcate = await _mediator.Send(new GetByIdCategoryProductQuery() { Id = item.Key.Value, IncludeProduct = false });
                            if (getcate.Succeeded)
                            {
                                var productSellModel = new ProductSellModel();
                                productSellModel.IdCode = EncryptionHelper.Encrypt(getcate.Data.Id.ToString(), SystemVariableHelper.publicKey);
                                productSellModel.IdParent = getcate.Data.IdPattern;
                                productSellModel.Name = getcate.Data.Name;
                                productSellModel.Slug = getcate.Data.Slug;
                                productSellModel.SlugCateParent = slug;
                                productSellModel.Sort = getcate.Data.Sort;
                                List<Product> products = new List<Product>();
                                foreach (var pro in item)
                                {
                                    products.AddRange(pro.productslist);
                                }
                                productSellModel.products = products.OrderByDescending(x => x.Id).ToPagedList(1, await GetpageSiteAsync());
                                productSellModels.Add(productSellModel);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in productSellModels)
                        {
                            item.products = item.productslist.OrderByDescending(x => x.Id).ToPagedList(1, await GetpageSiteAsync());
                        }
                    }
                    var company = await _mediator.Send(new GetByIdCompanyInfoQuery());
                    if (company.Succeeded)
                    {
                        ViewBag.ImgNotData = company.Data.Logo;
                    }
                    return View("GetProductSell", productSellModels);
                }
                return View("GetProductSell", new List<Product>());
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.ToString());
                throw;
            }

        }
        public async Task<IActionResult> autoSearchAsync(SearchViewModel model)
        {
            try
            {
                var data = await _mediator.Send(new GetAllReSearchCacheQuery() { keyword = model.keyword, history = model.history });
                if (data.Succeeded)
                {
                    if (data.Data != null && data.Data.Count() > 0)
                    {
                        var listkq = data.Data.Select(x => new AutocompleteViewModel
                        {
                            Name = x.Name,
                            HistoryLoca = false,
                            Slug = x.Slug,
                            Length = x.Name.Length,
                        }).ToList();
                        if (!string.IsNullOrEmpty(model.keyword))
                        {
                            if (listkq.Count() > 0)
                            {
                                var itemnew = listkq.Where(m => m.Name.ToLower() == model.keyword.Trim().ToLower()).FirstOrDefault();
                                if (itemnew == null)
                                {
                                    listkq.Insert(0, new AutocompleteViewModel { Name = model.keyword, Slug = Common.ConvertToSlug(model.keyword) });
                                }
                                else
                                {
                                    listkq.Remove(itemnew);
                                    listkq.Insert(0, new AutocompleteViewModel { Name = model.keyword, Slug = Common.ConvertToSlug(model.keyword) });
                                }
                            }
                            else
                            {

                                listkq.Insert(0, new AutocompleteViewModel { Name = model.keyword, Slug = Common.ConvertToSlug(model.keyword) });

                            }

                        }
                        if (model.Task > 0 && listkq.Count() > 0)
                        {
                            listkq = listkq.Take(model.Task).ToList();
                        }
                        else
                        {
                            listkq = listkq.Take(10).OrderBy(x => x.Length).ToList();
                        }
                        //string json = Common.ConverModelToJson<List<AutocompleteViewModel>>(listkq);

                        return Json(listkq);
                    }
                    var rt = new List<AutocompleteViewModel>();
                    rt.Add(new AutocompleteViewModel { Name = model.keyword, Slug = model.keyword });
                    // string jsons = Common.ConverModelToJson<List<AutocompleteViewModel>>(rt);


                    return Json(rt);
                    // return new JsonResult(new { data = json }, System.Web.Mvc.JsonRequestBehavior.AllowGet);
                }
                return new JsonResult(null);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new JsonResult(null);
            }


            //ProductViewModel productViewModel = new ProductViewModel();
            //productViewModel.text = model.name;
            //productViewModel.TypeProduct = (ProductEnumcs)model.ProductType;
            //productViewModel.TypeCategory = _typeCategory.GetByCode(string.Empty, model.ProductType);
            //productViewModel.Categorys = await _Repositorycate.GetListByIdTypeCategory(productViewModel.TypeCategory.Id);
            //return View("~/Views/Site/CategoryProduct.cshtml", productViewModel);
        }
    }
}
