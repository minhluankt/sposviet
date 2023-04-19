
using Application.Constants;
using Application.Features.CategorysProduct.Query;
using Application.Features.ConfigSystems.Commands;
using Application.Features.ConfigSystems.Query;
using Application.Hepers;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.ManagerApplication.Abstractions;
using Web.ManagerApplication.Areas.Admin.Models;
using Web.ManagerApplication.Areas.Admin.Models.Categorys;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ConfigSystemController : BaseController<ConfigSystemController>
    {
        [Authorize(Policy = "configsystem.list")]
        public async Task<IActionResult> IndexAsync()
        {
            var datamodel = new ConfigSystemViewModel();

            _logger.LogInformation(User.Identity.Name + "--> CompanyInfo index");
            var getallcatergory = await _mediator.Send(new GetAllCategoryProductCacheQuery());
            var getid = await _mediator.Send(new GetAllConfigQuery());
            if (getid.Succeeded)
            {
                var get = getid.Data.Where(m => m.Key == ParametersConfigSystem.pageSizeProductInCategory).SingleOrDefault();
                if (get != null)
                {
                    if (!string.IsNullOrEmpty(get.Value))
                    {
                        datamodel.pageSizeProductInCategory = int.Parse(get.Value);
                    }
                }
                var getlayoutHeader = getid.Data.Where(m => m.Key == ParametersConfigSystem.layoutHeader).SingleOrDefault();
                if (getlayoutHeader != null)
                {
                    if (!string.IsNullOrEmpty(getlayoutHeader.Value))
                    {
                        datamodel.layoutHeader = int.Parse(getlayoutHeader.Value);
                    }
                }
                var getsettingsell = getid.Data.Where(m => m.Key == ParametersConfigSystem.SellSettingInHome).SingleOrDefault();
                if (getsettingsell != null)
                {
                    if (!string.IsNullOrEmpty(getsettingsell.Value))
                    {

                        var modelsell = Common.ConverJsonToModel<SellModelSetting>(getsettingsell.Value);
                        if (modelsell != null)
                        {
                            datamodel.SellModelSetting = modelsell;
                        }
                    }
                }
            }

            datamodel.listcategory = getallcatergory.Data == null ? new List<CategoryConfig>() : getallcatergory.Data.Select(x => new CategoryConfig { Id = x.Id, Name = x.Name, Selected = false }).ToList();
            //var datamodel = _mapper.Map<ConfigSystemViewModel>(getid.Data);
            //datamodel.lstIdAndNameCategoryShowInHomeModel = Common.ConverJsonToArrInt(datamodel.lstIdAndNameCategoryShowInHome);
            //datamodel.listcategory = getallcatergory.Data==null?new List<CategoryConfig>(): getallcatergory.Data.Select(x=> new CategoryConfig { Id=x.Id,Name=x.Name,Selected= datamodel.lstIdAndNameCategoryShowInHomeModel.Contains(x.Id)}).ToList();

            // return View();
            // }

            return View(datamodel);
        }
        public async Task<IActionResult> Getjsontreeview()
        {
            var response = await _mediator.Send(new GetAllCategoryProductCacheQuery());
            if (response.Succeeded)
            {

                var getKey = await _mediator.Send(new GetByKeyConfigSystemQuery(ParametersConfigSystem.lstIdAndNameCategoryShowInHome));
                var dataListId = Common.ConverJsonToArrInt(getKey.Data.Value);
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
                    // jsonModelView.isDirectory = item.IdPattern == null || item.IdPattern == 0 ? true : false;
                    jsonModelView.expanded = true;
                    jsonModelView.selected = dataListId == null ? false : dataListId.Contains(item.Id) ? true : false;
                    jsonModelViews.Add(jsonModelView);
                }
                string json = Common.ConverModelToJson(jsonModelViews);
                return new JsonResult(new { isValid = true, json = json });
            }
            _notify.Error("Không lấy được dữ liệu");
            return new JsonResult(new { isValid = false });
        }
        [Authorize(Policy = "configsystem.update")]
        public async Task<IActionResult> UpdateSettingCategoryInHome(string jsonData) // update cấu hình danh mục hiển thị tại trang chỉ, 
        {
            try
            {
                _logger.LogInformation("_update UpdateSettingCategoryInHome");
                List<JsonModelView> jsonModelViews = Common.ConverJsonToModel<List<JsonModelView>>(jsonData);
                var data = jsonModelViews.Select(x => x.id).ToArray();
                string cvertojson = Common.ConverObjectToJsonString(data); // conver dữ liệu sang string json
                var model = new ConfigSystem() { Key = ParametersConfigSystem.lstIdAndNameCategoryShowInHome, Value = cvertojson }; // tạo model đối tượng ConfigSystem

                var createProductCommand = _mapper.Map<UpdateConfigSystemCommand>(model); // map dữ liệu để gọi update
                var response = await _mediator.Send(createProductCommand);
                if (response.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                    return new JsonResult(new { isValid = true });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR011));
                return new JsonResult(new { isValid = false });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new JsonResult(new { isValid = false });
            }
        }

        [Authorize(Policy = "configsystem.update")]
        [HttpPost]
        public async Task<IActionResult> UpdateSettingSell(SellModelSetting model)
        {
            try
            {
                var json = Common.ConverModelToJson<SellModelSetting>(model);
                var newDic = new Dictionary<string, string>();
                newDic.Add(ParametersConfigSystem.SellSettingInHome, json);

                var createProductCommand = _mapper.Map<UpdateConfigSystemCommand>(model); // map dữ liệu để gọi update
                //createProductCommand.lstKey = newDic;
                var response = await _mediator.Send(createProductCommand);
                if (response.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                    return new JsonResult(new { isValid = true });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR011));
                return new JsonResult(new { isValid = false });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new JsonResult(new { isValid = false });
            }
        }

        [Authorize(Policy = "configsystem.update")]
        [HttpPost]
        public async Task<IActionResult> UpdateSettingGeneral(ConfigSystemModel model)
        {
            try
            {
                _logger.LogInformation("_update UpdateSettingGeneral");
                var newDic = new Dictionary<string, string>();

                if (model.pageSizeTable > 0)
                {
                    newDic.Add(ParametersConfigSystem.pageSizeTable, model.pageSizeTable.ToString());
                }
                if (model.pageSizeProductInCategory > 0)
                {
                    newDic.Add(ParametersConfigSystem.pageSizeProductInCategory, model.pageSizeProductInCategory.ToString());
                }
                if (model.layoutHeader > 0)
                {
                    newDic.Add(ParametersConfigSystem.layoutHeader, model.layoutHeader.ToString());
                }

                var createProductCommand = _mapper.Map<UpdateConfigSystemCommand>(model); // map dữ liệu để gọi update
                //createProductCommand.lstKey = newDic;
                var response = await _mediator.Send(createProductCommand);
                if (response.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                    return new JsonResult(new { isValid = true });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR011));
                return new JsonResult(new { isValid = false });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new JsonResult(new { isValid = false });
            }
        }

    }
}
