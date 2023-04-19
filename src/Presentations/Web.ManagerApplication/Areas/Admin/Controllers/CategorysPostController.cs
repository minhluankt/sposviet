using Application.Constants;
using Application.Features.CategorysPost.Commands;
using Application.Features.CategorysPost.Query;
using Application.Features.TypeCategorys.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Areas.Admin.Models.Categorys;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class CategorysPostController : BaseController<CategorysPostController>
    {
        private IStringLocalizer<SharedResource> _localizer;
        private readonly IRepositoryAsync<CategoryPost> _repository;
        public CategorysPostController(IRepositoryAsync<CategoryPost> repository, IStringLocalizer<SharedResource> localize)
        {
            _localizer = localize;
            _repository = repository;
        }
        // GET: CategorysController
        public IActionResult Index()
        {


            return View();
        }
        public async Task<IActionResult> Getjsontreeview()
        {
            var response = await _mediator.Send(new GetAllCategoryPostCacheQuery());
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
                    jsonModelView.isDirectory = item.IdPattern == null || item.IdPattern == 0 ? true : false;
                    jsonModelView.expanded = true;
                    jsonModelViews.Add(jsonModelView);
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
        public async Task<ActionResult> CreateOrEdit(int id = 0, bool createItem = false, int idPattern = 0)
        {
            var response = await _mediator.Send(new GetAllCategoryPostCacheQuery());
            //List<TypeCategory> typeCategories = new List<TypeCategory>();
            var datatype = await _mediator.Send(new GetAllTypeCategoryCacheQuery());
            if (datatype.Succeeded)
            {
                // typeCategories = datatype.Data;
            }
            if (response.Succeeded)
            {
                if (id == 0)
                {

                    var model = new CategoryViewModel
                    {
                        createItem = createItem,
                        //Itemategory = datatype.Data.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).AsEnumerable(),
                        Itemategory = datatype.Data,
                        //SelectListItemsTypeCategory = new SelectList(typeCategories, "Id", "Name"),
                        SelectListItemCate = response.Data.Where(m => m.IdPattern == null || m.IdPattern == 0).Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name, Selected = t.Id == idPattern })
                    };
                    if (createItem)
                    {
                        var getid = await _mediator.Send(new GetByIdCategoryPostQuery() { Id = idPattern });
                        if (getid.Succeeded)
                        {
                            model.IdTypeCategory = getid.Data.IdTypeCategory;
                        }
                        else
                        {
                            _notify.Error(getid.Message);
                            return new JsonResult(new { isValid = false, html = string.Empty });
                        }
                    }
                    if (idPattern > 0)
                    {
                        model.IdPattern = idPattern;
                    }
                    return View("_Create", model);
                }
                else
                {

                    var getid = await _mediator.Send(new GetByIdCategoryPostQuery() { Id = id });
                    if (getid.Succeeded)
                    {
                        var model = new CategoryViewModel
                        {
                            fullName = getid.Data.Name,
                            id = getid.Data.Id,
                            Sort = getid.Data.Sort,
                            Url = getid.Data.Url,
                            IdLevel = getid.Data.IdLevel,
                            IdPattern = getid.Data.IdPattern ?? 0,
                            Itemategory = datatype.Data,
                            IdTypeCategory = getid.Data.IdTypeCategory,
                            // SelectListItemsTypeCategory = new SelectList(typeCategories, "Id", "Name", getid.Data.IdTypeCategory),
                            SelectListItemCate = response.Data.Where(m => m.IdPattern == null || m.IdPattern == 0).Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name, Selected = getid.Data.IdPattern == t.Id })
                        };

                        return View("_Edit", model);
                    }
                    _notify.Error(getid.Message);
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }

            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(CategoryViewModel model)
        {
            var collection = new CategoryPost { Name = model.fullName, Url = model.Url, Id = model.id, IdTypeCategory = model.IdTypeCategory, IdPattern = model.IdPattern, Active = model.active = true };
            try
            {
                if (model.IdTypeCategory == 0)
                {
                    _notify.Error("Chưa chọn loại chuyên mục");
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
                if (collection.Id == 0)
                {
                    if (collection.IdPattern > 0 && collection.IdPattern != null)
                    {
                        collection.IdLevel = 1;
                    }
                    var createPostCommand = _mapper.Map<CreateCategorysPostCommand>(collection);
                    var result = await _mediator.Send(createPostCommand);
                    if (result.Succeeded)
                    {
                        collection.Id = result.Data;
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
                    var updatePostCommand = _mapper.Map<UpdateCategorysPostCommand>(collection);
                    var result = await _mediator.Send(updatePostCommand);
                    if (result.Succeeded)
                    {
                        _notify.Success(_localizer.GetString("EditOk").Value);
                        return new JsonResult(new { isValid = true, html = string.Empty });
                    }
                    _notify.Error(result.Message);
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

        // POST: CategorysController/Create
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

        // POST: CategorysController/Edit/5
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
        [HttpPost]
        public async Task<ActionResult> Delete(int Id)
        {
            var delete = await _mediator.Send(new DeleteCategorysPostCommand() { Id = Id });
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

        // POST: CategorysController/Delete/5
        //[HttpPost]
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
