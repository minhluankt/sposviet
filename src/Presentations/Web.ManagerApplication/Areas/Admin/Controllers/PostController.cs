using Application.Constants;
using Application.Features.CategorysPost.Query;
using Application.Features.Posts.Commands;
using Application.Features.Posts.Querys;
using Application.Hepers;
using Application.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostController : BaseController<PostController>
    {
        [Authorize(Policy = "PagePost.list")]
        public IActionResult Index()
        {
            _logger.LogInformation(User.Identity.Name + "--> PagePost index");
            return View();
        }

        [Authorize(Policy = "PagePost.create")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> CreateOrEdit(int id = 0)
        {
            PostModel model = new PostModel();
            var getcate = await _mediator.Send(new GetAllCategoryPostCacheQuery());

            _logger.LogInformation(User.Identity.Name + "--> PagePost CreateOrEdit");
            if (id == 0)
            {
                if (getcate.Succeeded)
                {
                    model.CategoryPosts = getcate.Data;
                }
                model.Active = true;
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", model);
                return new JsonResult(new { isValid = true, html = html, title = "Thêm mới bài viết" });
            }
            else
            {
                var getid = await _mediator.Send(new GetByIdPostQuery() { Id = id });
                if (getid.Succeeded)
                {
                    // var post = _mapper.Map<PostModel>(getid.Data);
                    model = getid.Data;
                    if (getcate.Succeeded)
                    {
                        model.CategoryPosts = getcate.Data;
                    }
                    var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", model);
                    return new JsonResult(new { isValid = true, html = html, title = "Chỉnh sửa bài viết" });
                }
                _notify.Error(getid.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

        }
        //[Authorize(Policy = "PagePost.create")]
        //public ActionResult Create()
        //{
        //    _logger.LogInformation(User.Identity.Name + "--> PagePost Create");
        //    return View();

        //}
        //[Authorize(Policy = "PagePost.edit")]
        //public async Task<ActionResult> EditAsync(int id)
        //{
        //    _logger.LogInformation(User.Identity.Name + "--> PagePost EditAsync");
        //    var getid = await _mediator.Send(new GetByIdPostQuery() { Id = id });
        //    if (getid.Succeeded)
        //    {
        //        return View(getid.Data);
        //    }
        //    _notify.Error(getid.Message);
        //    return RedirectToAction("Index");

        //}
        [Authorize(Policy = "post.details")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> DetailsAsync(int id)
        {
            try
            {
                _logger.LogInformation(User.Identity.Name + "--> product post");
                var data = await _mediator.Send(new GetByIdPostQuery() { Id = id });
                if (data.Succeeded)
                {
                    //var datamodel = _mapper.Map<ProductModelView>(data.Data);
                    var html = await _viewRenderer.RenderViewToStringAsync("_Detail", data.Data);
                    return new JsonResult(new { isValid = true, html = html, modelFooter = true });
                }
                _notify.Error(data.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

        }
        public async Task<IActionResult> LoadAll()
        {
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            try
            {

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
                var response = await _mediator.Send(new GetAllPostQuery()
                {
                    sortColumn = sortColumn,
                    sortColumnDirection = sortColumnDirection,
                    pageSize = pageSize,
                    skip = skip
                });
                if (response.Succeeded)
                {
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = response.Data.Count, data = response.Data.Data });
                }

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = "" });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json(new { draw = draw, recordsFiltered = 0, recordsTotal = 0, data = "" });
            }
        }

        [Authorize(Policy = "PagePost.delete")]
        [HttpPost]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Delete(int Id)
        {
            _logger.LogInformation(User.Identity.Name + "--> PagePost Delete");
            var delete = await _mediator.Send(new DeletePostCommand() { Id = Id });
            if (delete.Succeeded)
            {

                _notify.Success($"Xóa thành công Id: {Id}");
                return new JsonResult(new { isValid = true, loadTable = true });
            }
            else
            {
                _notify.Error(delete.Message);
                return null;
            }
        }
        [Authorize(Policy = "PagePost.create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(PostModel model, IFormFile Img)
        {

            var collection = model;
            try
            {
                if (collection.Id == 0)
                {
                    //if (Img == null || Img.Length == 0)
                    //{
                    //    _notify.Error("Vui lòng chọn hình ảnh");
                    //    return new JsonResult(new { isValid = false, html = string.Empty });
                    //}
                    var createProductCommand = _mapper.Map<CreatePostCommand>(collection);
                    createProductCommand.ImgUpload = Img;
                    var result = await _mediator.Send(createProductCommand);
                    if (result.Succeeded)
                    {
                        collection.Id = result.Data;
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
                    var updateProductCommand = _mapper.Map<UpdatePostCommand>(collection);
                    updateProductCommand.ImgUpload = Img;
                    var result = await _mediator.Send(updateProductCommand);
                    if (result.Succeeded)
                    {
                        _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                    }
                    else
                    {
                        _notify.Error(result.Message);
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }

                }
                return new JsonResult(new { isValid = true, loadTable = true, closeSwal = true });
            }
            catch (Exception e)
            {
                _notify.Error(e.ToString());
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
        }
    }
}
