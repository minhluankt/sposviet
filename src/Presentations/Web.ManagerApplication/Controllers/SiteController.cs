using Application.Constants;
using Application.Enums;
using Application.Features.CategorysPost.Query;
using Application.Features.CategorysProduct.Query;
using Application.Features.CompanyInfo.Query;
using Application.Features.ConfigSystems.Query;
using Application.Features.Posts.Querys;
using Application.Features.Products.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ViewModel;
using HelperLibrary;
using Microsoft.AspNetCore.Mvc;
using Model;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Models;
using X.PagedList;

namespace Web.ManagerApplication.Controllers
{
    public class SiteController : BaseController<HomeController>
    {
        private readonly IRepositoryAsync<ConfigSystem> _configRepository;
        private readonly IProductPepository<Product> _productepository;
        private readonly ITableLinkRepository _tableLinkRepository;
        private readonly ICategoryProductRepository<CategoryProduct> _Repositorycate;
        public SiteController(
            ITableLinkRepository tableLinkRepository, IRepositoryAsync<ConfigSystem> configRepository,
            IProductPepository<Product> productepository,
            ICategoryProductRepository<CategoryProduct> Repositorycate)
        {
            _configRepository = configRepository;
            _productepository = productepository;
            _Repositorycate = Repositorycate;
            _tableLinkRepository = tableLinkRepository;
        }
        public async Task<IActionResult> SearchAsync(ProductSearch productViewModel, int? page)
        {
            var company = await _mediator.Send(new GetByIdCompanyInfoQuery());
            if (company.Succeeded)
            {
                ViewBag.Website = company.Data.Website;
            }
            //int pageNumber = page ?? 1;
            //int pageSite = await GetpageSiteAsync();

            if (productViewModel.TypeSerach == (int)CategorySerach.SanPham)
            {

                //var listproduct = await _mediator.Send(new GetAllReSearchCacheQuery() { ProductSearch = productViewModel
                //    , searchCustome = true });
                //if (listproduct.Succeeded)
                //{
                //    productViewModel.ProductPagedList = await _productepository.ToPagedListAsync(listproduct.Data.Products, pageNumber, pageSite);

                //    ViewBag.Title = productViewModel.keyword;
                //    ViewBag.description = productViewModel.keyword;
                //    ViewBag.image = $"{FolderUploadConstants.ImgSeo}";
                //}
                ViewBag.Title = productViewModel.keyword;
                ViewBag.description = productViewModel.keyword;
                ViewBag.image = $"{FolderUploadConstants.ImgSeo}";

            }
            return View("CategoryProduct", productViewModel);
        }
        public IActionResult Home()
        {
            return LocalRedirect("/");
        }
        public async Task<IActionResult> IndexAsync(String slug = "")
        {

            slug = slug.Replace(".html", "");
            string _page = HttpContext.Request.Query["page"].ToString();
            string keyword = HttpContext.Request.Query["keyword"].ToString();

            int page = 1;
            if (!string.IsNullOrEmpty(_page))
            {
                page = int.Parse(_page);
            }


            if (slug == "")
            {
                return this.Home();
            }
            else
            {
                TableLink tableLink = await _tableLinkRepository.GetBySlug(slug);

                if (!string.IsNullOrEmpty(keyword))
                {
                    string sortby = HttpContext.Request.Query["sortby"].ToString();
                    string idPrice = HttpContext.Request.Query["idPrice"].ToString();
                    ProductSearch productViewModel = new ProductSearch();
                    productViewModel.Slug = slug;
                    productViewModel.keyword = keyword;

                    if (!string.IsNullOrEmpty(sortby))
                    {
                        productViewModel.sortby = sortby;
                    }
                    if (!string.IsNullOrEmpty(idPrice))
                    {
                        productViewModel.idPrice = int.Parse(idPrice);
                    }
                    productViewModel.TypeSerach = (int)CategorySerach.SanPham;
                    return await SearchAsync(productViewModel, page);
                }


                if (tableLink != null)
                {
                    if (tableLink.type == TypeLinkConstants.TypeProduct && tableLink.tableId == TypeLinkConstants.IdTypeProduct)
                    {
                        return await this.DetailProductAsync((int)tableLink.parentId);
                    }
                    else if (tableLink.type == TypeLinkConstants.TypePost && tableLink.tableId == TypeLinkConstants.IdTypePost)
                    {
                        return await this.DetailPostAsync((int)tableLink.parentId);
                    }
                    else if (tableLink.type == TypeLinkConstants.TypeCategoryProduct && tableLink.tableId == TypeLinkConstants.IdTypeCategoryProduct)
                    {
                        return await this.CategoryProduct(slug, (int)tableLink.parentId, page);
                    }
                    else if (tableLink.type == TypeLinkConstants.TypeCategoryPost && tableLink.tableId == TypeLinkConstants.IdTypeCategoryPost)
                    {
                        return await this.CategoryPost((int)tableLink.parentId, page);
                    }
                    else if (tableLink.type == TypeLinkConstants.TypeConfigSell && tableLink.tableId == TypeLinkConstants.IdTypeConfigSell)
                    {
                        return await this.CategorySellAsync(slug, (int)tableLink.parentId, page);
                    }
                }
                else
                {
                    //slug k co tring ban link
                    return this.Home();
                }

            }
            return View();
        }

        public async Task<IActionResult> CategorySellAsync(string slug, int parentId, int page)
        {
            ViewBag.Slug = slug;
            int idcategory = 0;
            string getcate = HttpContext.Request.Query["cate"].ToString();
            if (!string.IsNullOrEmpty(getcate))
            {
                TableLink tableLink = await _tableLinkRepository.GetBySlug(getcate);
                if (tableLink != null)
                {
                    idcategory = tableLink.parentId.Value;
                    var data = await _mediator.Send(new GetByIdCategoryProductQuery() { Id = idcategory });
                    if (data.Succeeded)
                    {
                        string sortby = HttpContext.Request.Query["sortby"].ToString();
                        string idPrice = HttpContext.Request.Query["idPrice"].ToString();
                        CategoryProductSellIndexModel sellIndexModel = new CategoryProductSellIndexModel();
                        sellIndexModel.CategoryProduct = data.Data;
                        sellIndexModel.sortby = sortby;
                        sellIndexModel.idPrice = !string.IsNullOrEmpty(idPrice) ? int.Parse(idPrice) : 0;
                        var getarrCategory = await _Repositorycate.GetListArrayChillAllByIdAsync(data.Data.Id);
                        ProductSearch model = new ProductSearch();
                        model.lstidCategory = getarrCategory;
                        model.isPromotion = true;
                        model.pageNumber = page;
                        model.idPrice = sellIndexModel.idPrice;
                        model.sortby = sortby;
                        model.pagesite = await this.GetpageSiteAsync();
                        var listproduct = await _mediator.Send(new GetIQueryableProductQuery()
                        {
                            ProductSearch = model
                        });

                        if (listproduct.Succeeded)
                        {
                            sellIndexModel.Products = await listproduct.Data.ToPagedListAsync(model.pageNumber, model.pagesite);
                        }
                        return View("CategorySellLoadmore", sellIndexModel);
                    }

                }


            }
            var getconfigsell = _configRepository.GetAll(m => m.Key.ToLower() == ParametersConfigSystem.SellSettingInHome.ToLower()).SingleOrDefault();
            if (getconfigsell != null)
            {
                var modelsell = Common.ConverJsonToModel<SellModelSetting>(getconfigsell.Value);
                if (modelsell != null)
                {
                    ViewBag.ImgSell = modelsell.ImgSell;
                }
            }
            return View("CategorySell");
        }
        public async Task<IActionResult> DetailProductAsync(int idProduct)
        {
            ProductModelView ProductModelView = new ProductModelView();
            var data = await _mediator.Send(new GetByIdProductQuery() { Id = idProduct, IsView = true, IncludeCommnet = true });
            if (data.Succeeded)
            {
                ProductModelView.Product = data.Data;
                //var listproduct = await _mediator.Send(new GetListProductCustomQuery(await GetpageSiteAsync()) {
                //    idCategory = data.Data.IdCategory});
                //if (listproduct.Succeeded)
                //{
                //    ProductModelView.Products = listproduct.Data;
                //}
                var company = await _mediator.Send(new GetByIdCompanyInfoQuery());
                if (company.Succeeded)
                {
                    if (company.Data != null)
                    {
                        ViewBag.Website = company.Data.Website;
                        ProductModelView.CompanyInfoAdmin = company.Data;
                    }

                }
                string name = data.Data.Name;
                if (name.Length > 60)
                {
                    name = $"{name.Substring(0, 55)}...";
                }
                ViewBag.Title = !string.IsNullOrEmpty(data.Data.seotitle) ? data.Data.seotitle : name;
                string dec = LibraryCommon.StripHTML(data.Data.Description);
                if (dec.Length > 130)
                {
                    dec = dec.Substring(0, 120);
                }
                ViewBag.description = !string.IsNullOrEmpty(data.Data.seoDescription) ? data.Data.seoDescription : dec;
                ViewBag.Keyword = !string.IsNullOrEmpty(data.Data.seokeyword) ? data.Data.seokeyword : data.Data.Name;
                ViewBag.image = $"{SystemVariable.SystemVariableHelper.FolderUpload}{FolderUploadConstants.Product}/{data.Data.Img}";

                if (ProductModelView.Product.UploadImgProducts.Count() == 0)
                {
                    ProductModelView.Product.UploadImgProducts.Add(new UploadImgProduct() { FileName = ProductModelView.Product.Img });
                }
                return View("DetailProduct", ProductModelView);
            }
            return this.Home();
        }

        public async Task<IActionResult> DetailPostAsync(int id)
        {
            PostViewModel ProductModelView = new PostViewModel();
            var data = await _mediator.Send(new GetByIdPostQuery() { Id = id, IsView = true });

            if (data.Succeeded)
            {
                ProductModelView.Post = data.Data;
                var getid = await _mediator.Send(new GetAllIQueryablePostQuery() { IncludeCategory = false });
                if (getid.Succeeded)
                {
                    // bafi vieets cung chuyên mục
                    ProductModelView.PostsQuery = getid.Data.Where(x => x.IdCategory == data.Data.IdCategory).OrderByDescending(m => m.ViewNumber).Take(6).ToList();
                    // bài viết xem nhiều nhất
                    ProductModelView.ListPost = getid.Data.OrderByDescending(m => m.ViewNumber).Take(20).ToList();

                }
                var company = await _mediator.Send(new GetByIdCompanyInfoQuery());
                if (company.Succeeded)
                {
                    ViewBag.Website = company.Data.Website;
                }
                string name = data.Data.Name;
                if (name.Length > 60)
                {
                    name = $"{name.Substring(0, 55)}...";
                }
                ViewBag.Title = !string.IsNullOrEmpty(data.Data.seotitle) ? data.Data.seotitle : name;
                string dec = LibraryCommon.StripHTML(data.Data.Decription).Substring(0, 150);
                ViewBag.description = !string.IsNullOrEmpty(data.Data.seoDescription) ? data.Data.seoDescription : dec;
                ViewBag.Keyword = !string.IsNullOrEmpty(data.Data.seokeyword) ? data.Data.seokeyword : data.Data.Name;
                ViewBag.image = $"{SystemVariable.SystemVariableHelper.FolderUpload}{FolderUploadConstants.Post}/{data.Data.Img}";
                return View("DetailPost", ProductModelView);
            }

            return this.Home();
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

        public async Task<IActionResult> CategoryProduct(string slug, int idcategory, int page)
        {
            string sortby = HttpContext.Request.Query["sortby"].ToString();
            string idPrice = HttpContext.Request.Query["idPrice"].ToString();

            ProductSearch ProductModelView = new ProductSearch();
            if (!string.IsNullOrEmpty(sortby))
            {
                ProductModelView.sortby = sortby;
            }
            if (!string.IsNullOrEmpty(idPrice))
            {
                ProductModelView.idPrice = int.Parse(idPrice);
            }
            ProductModelView.Slug = slug;
            var data = await _mediator.Send(new GetByIdCategoryProductQuery() { Id = idcategory });
            if (data.Succeeded)
            {
                ViewBag.Title = data.Data.Name;
                ViewBag.description = data.Data.Name;
                ViewBag.image = $"{FolderUploadConstants.ImgSeo}";


                ProductModelView.CategoryProduct = data.Data;
                ProductModelView.TypeSerach = (int)TypeSerach.Category;
                ProductModelView.CategorySerach = (int)CategorySerach.SanPham;
                ProductModelView.idCategory = idcategory;
                //var getarrCategory = (await _Repositorycate.GetListArrayChillAllByIdAsync(idcategory));
                //var listproduct = await _mediator.Send(new GetProductByCategoryQuery() { lstIdcategory= getarrCategory });
                //if (listproduct.Succeeded)
                //{
                //    ProductModelView.ProductPagedList = await listproduct.Data.ToPagedListAsync(page, await GetpageSiteAsync());
                //}
                return View("CategoryProduct", ProductModelView);

            }
            _notify.Error(data.Message);
            return RedirectToAction("index", "Home");
        }
        public async Task<IActionResult> CategoryPost(int idcategory, int page)
        {
            PostViewModel ProductModelView = new PostViewModel();
            var data = await _mediator.Send(new GetByIdCategoryPostQuery() { Id = idcategory });
            if (data.Succeeded)
            {

                ViewBag.Title = data.Data.Name;
                ViewBag.description = data.Data.Name;
                ViewBag.image = $"{FolderUploadConstants.ImgSeo}";
                var company = await _mediator.Send(new GetByIdCompanyInfoQuery());
                if (company.Succeeded)
                {
                    ViewBag.Website = company.Data.Website;
                    // ProductModelView.Company = company.Data;
                }
                int[] lstIdcategory = null;
                var GetListChildCategoryPostByIdCacheQuery = await _mediator.Send(new GetListChildCategoryPostByIdCacheQuery() { IdCategory = idcategory });
                if (GetListChildCategoryPostByIdCacheQuery.Succeeded)
                {
                    var tmp = GetListChildCategoryPostByIdCacheQuery.Data.Select(x => x.Id);
                    lstIdcategory = tmp.Any() ? tmp.ToArray() : null;
                }
                ProductModelView.Category = data.Data;
                //ProductModelView.idCategory = data.Data.Id;

                var listproduct = await _mediator.Send(new GetAllIQueryablePostQuery() { });
                if (listproduct.Succeeded)
                {

                    //  ProductModelView.PostsIPagedList = await PaginatedList<Post>.ToPagedListAsync(listproduct.Data, page, await GetpageSiteAsync());
                    ProductModelView.PostsIPagedList = await listproduct.Data.Where(x => lstIdcategory.Contains(x.IdCategory)).ToPagedListAsync(page, await GetpageSiteAsync());
                    ProductModelView.ListPost = listproduct.Data.OrderByDescending(m => m.ViewNumber).Take(20).ToList();
                }
                return View("CategoryPost", ProductModelView);
            }
            _notify.Error(data.Message);
            return LocalRedirect("/home/index");
        }

    }
}
