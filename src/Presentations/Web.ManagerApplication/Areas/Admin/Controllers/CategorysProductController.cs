using Application.Constants;
using Application.Features.CategorysProduct.Commands;
using Application.Features.CategorysProduct.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Areas.Admin.Models.Categorys;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategorysProductController : BaseController<CategorysProductController>
    {
        private IStringLocalizer<SharedResource> _localizer;
        private readonly IRepositoryAsync<CategoryProduct> _repository;
        private readonly IProductPepository<Product> _productrepository;
        public CategorysProductController(IRepositoryAsync<CategoryProduct> repository,
            IProductPepository<Product> productrepository,
            IStringLocalizer<SharedResource> localize)
        {
            _localizer = localize;
            _productrepository = productrepository;
            _repository = repository;
        }
        // GET: CategorysController
        [Authorize(Policy = "CategorysProduct.list")]
        public IActionResult Index()
        {


            return View();
        }
        public async Task<IActionResult> Getjsontreeview()
        {
            var response = await _mediator.Send(new GetAllCategoryProductCacheQuery());
            if (response.Succeeded)
            {
                var getActivelist = response.Data;
                List<JsonModelView> jsonModelViews = new List<JsonModelView>();

                foreach (var item in getActivelist)
                {
                    // var checkisDirectory = response.Data.Where(m => m.IdPattern == item.Id).FirstOrDefault();
                    JsonModelView jsonModelView = new JsonModelView();
                    jsonModelView.id = item.Id;
                    jsonModelView.LastModifiedOn = item.LastModifiedOn != null ? item.LastModifiedOn.Value.ToString("dd/MM/yyyy") : "";
                    jsonModelView.CreatedOn = item.CreatedOn.ToString("dd/MM/yyyy");
                    jsonModelView.parentId = item.IdPattern;
                    jsonModelView.name = item.Name;
                    //jsonModelView.isDirectory = checkisDirectory != null ? true : false;
                    jsonModelView.isDirectory = item.IdLevel == 0 || item.IdLevel == 1 ? true : false;
                    jsonModelView.expanded = true;
                    jsonModelViews.Add(jsonModelView);
                }
                if (jsonModelViews != null)
                {
                    jsonModelViews.ForEach(x => x.countProduct = _productrepository.GetProductbyCategoryId(x.id).Count());
                }
                string json = Common.ConverModelToJson(jsonModelViews);
                return new JsonResult(new { isValid = true, json = json });
            }
            _notify.Error("Không lấy được dữ liệu");
            return new JsonResult(new { isValid = false });
        }
        //public async Task<IActionResult> LoadAll()
        //{
        //    try
        //    {
        //        var response = await _mediator.Send(new GetAllCategoryCacheQuery());
        //        if (response.Succeeded)
        //        {
        //            return PartialView("_ViewAll", response.Data);
        //        }
        //        return default;
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(default,e);
        //        _notify.Error(e.Message);
        //        return default;
        //    }
        //}
        // GET: CategorysController/Details/5
        public ActionResult Details(int id)
        {

            return View();
        }

        // GET: CategorysController/Create
        [Authorize(Policy = "CategorysProduct.CreateOrEdit")]
        public async Task<ActionResult> CreateOrEdit(int id = 0, bool createItem = false, int idPattern = 0)
        {
            var response = await _mediator.Send(new GetAllCategoryProductCacheQuery());
            //List<TypeCategory> typeCategories = new List<TypeCategory>();


            if (response.Succeeded)
            {
                if (id == 0)
                {

                    var model = new CategoryViewModel
                    {
                        createItem = createItem,
                        IdPattern = 0,
                        //SelectListItemsTypeCategory = new SelectList(typeCategories, "Id", "Name"),
                        //SelectListItemCate = response.Data.Where(m => m.IdPattern == null || m.IdPattern == 0).Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name, Selected = t.Id == idPattern })
                    };

                    //if (createItem)
                    //{
                    //    var getid = await _mediator.Send(new GetByIdCategoryProductQuery() { Id = idPattern });
                    //    if (getid.Succeeded)
                    //    {
                    //        model.IdTypeCategory = getid.Data.IdTypeCategory;
                    //    }
                    //    else
                    //    {
                    //        _notify.Error(getid.Message);
                    //        return new JsonResult(new { isValid = false, html = string.Empty });
                    //    }
                    //}
                    if (idPattern > 0)
                    {
                        model.IdPattern = idPattern;
                    }
                    return View("_Create", model);
                }
                else
                {

                    var getid = await _mediator.Send(new GetByIdCategoryProductQuery() { Id = id });
                    if (getid.Succeeded)
                    {
                        var model = new CategoryViewModel
                        {
                            fullName = getid.Data.Name,
                            Sort = getid.Data.Sort,
                            Icon = getid.Data.Icon,
                            id = getid.Data.Id,
                            IdLevel = getid.Data.IdLevel,
                            IdPattern = getid.Data.IdPattern ?? 0,
                            // SelectListItemsTypeCategory = new SelectList(typeCategories, "Id", "Name", getid.Data.IdTypeCategory),
                            // SelectListItemCate = response.Data.Where(m => m.IdPattern == null || m.IdPattern == 0).Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name, Selected = getid.Data.IdPattern == t.Id })
                        };

                        return View("_Edit", model);
                    }
                    _notify.Error(getid.Message);
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }

            }
            return View();
        }

        [Authorize(Policy = "CategorysProduct.CreateOrEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(CategoryViewModel model)
        {
            if (string.IsNullOrEmpty(model.fullName))
            {
                _notify.Error(GeneralMess.ConvertStatusToString("Vui lòng nhập tên danh mục"));
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

            var collection = new CategoryProduct { Icon = model.Icon, Name = model.fullName, Sort = model.Sort, Id = model.id, IdPattern = model.IdPattern, Active = model.active = true };
            try
            {
                if (collection.Id == 0)
                {
                    if (collection.IdPattern > 0 && collection.IdPattern != null)
                    {
                        var getIdPattern = await _mediator.Send(new GetByIdCategoryProductQuery() { Id = model.IdPattern.Value });

                        collection.IdLevel = getIdPattern.Data.IdLevel + 1;
                    }
                    if (collection.IdLevel > 2)
                    {
                        _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR008));
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }
                    var createProductCommand = _mapper.Map<CreateCategorysProductCommand>(collection);
                    var result = await _mediator.Send(createProductCommand);
                    if (result.Succeeded)
                    {
                        collection.Id = result.Data;
                        _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS008));
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
                    var updateProductCommand = _mapper.Map<UpdateCategorysProductCommand>(collection);
                    var result = await _mediator.Send(updateProductCommand);
                    if (result.Succeeded)
                    {
                        _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                        return new JsonResult(new { isValid = true, html = string.Empty });
                    }
                    _notify.Error(GeneralMess.ConvertStatusToString(result.Message));
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
                //var response = await _mediator.Send(new GetAllCategoryCacheQuery());
                //if (response.Succeeded)
                //{
                //    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                //    return new JsonResult(new { isValid = true, html = html });
                //}
                //else
                //{
                //    _notify.Error(response.Message);
                //    return null;
                //}
            }
            catch (Exception e)
            {
                _notify.Error(e.ToString());
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
        }

        // Product: CategorysController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CategorysController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // Product: CategorysController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CategorysController/Delete/5
        [Authorize(Policy = "CategorysProduct.delete")]
        [HttpPost]
        public async Task<ActionResult> Delete(int Id)
        {
            var delete = await _mediator.Send(new DeleteCategorysProductCommand() { Id = Id });
            if (delete.Succeeded)
            {

                _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS007));
                return new JsonResult(new { isValid = true, html = string.Empty, loadTreeview = true });
            }
            else
            {
                _notify.Error(GeneralMess.ConvertStatusToString(delete.Message));
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
        }

        // Product: CategorysController/Delete/5
        //[HttpProduct]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
