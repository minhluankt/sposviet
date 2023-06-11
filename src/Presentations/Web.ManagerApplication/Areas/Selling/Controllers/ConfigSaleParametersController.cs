using Application.Constants;
using Application.Enums;
using Application.Features.ConfigSystems.Commands;
using Application.Features.ConfigSystems.Query;
using Application.Hepers;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web.Helpers;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class ConfigSaleParametersController : BaseController<ConfigSaleParametersController>
    {
        
        [Authorize(Policy = "configsell.index")]
        public async Task<IActionResult> IndexAsync()
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var getid = await _mediator.Send(new GetAllConfigQuery() { ComId= currentUser.ComId});
            if (getid.Succeeded)
            {
                return View(getid.Data);
            }
            return View(new ConfigSystem());
        }
        [HttpGet]
        public async Task<IActionResult> GetConfigSellAsync(string key)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var _send = await _mediator.Send(new GetByKeyConfigSystemQuery(key) { ComId = currentUser.ComId });
            if (_send.Succeeded)
            {
                return new JsonResult(new { isValid = true, data = _send.Data });
            }
            return new JsonResult(new { isValid = false });
        }

        [Authorize(Policy = "configsystem.index")]
        [HttpPost]
        public async Task<IActionResult> Update(ConfigSaleParametersModel model)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            try
            {
                var newDic = new List<ConfigSaleParametersItem>();
                foreach (var item in model.ConfigSaleParametersItems)
                {
                    newDic.Add(item);
                }

                var createProductCommand = _mapper.Map<UpdateConfigSystemCommand>(model); // map dữ liệu để gọi update
                createProductCommand.lstKey = newDic;
                createProductCommand.ComId = currentUser.ComId;
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
        public async Task<IActionResult> GetConfig()// cái này là của bên báo bếp k dùng hàm này cho các yêu cầu khác
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var _send = await _mediator.Send(new GetByKeyConfigSystemQuery(EnumConfigParameters.PRINT_BAO_BEP.ToString()) { ComId = currentUser.ComId });
            if (_send.Succeeded)
            {
                bool getvalue = Convert.ToBoolean(_send.Data.Value);
                if (getvalue)
                {
                    var checkconfig = _send.Data.ConfigSystems.SingleOrDefault(x => x.Key == EnumConfigParameters.PRINT_KET_NOI.ToString());
                    if (checkconfig != null)
                    {
                        if (Convert.ToBoolean(checkconfig.Value))
                        {
                            return Json(new { isValid = false });
                        }
                    }
                    return Json(new { isValid = true});
                }
                else
                {
                    return Json(new { isValid = false });
                }
            }
            return Json(new { isValid = false });
        }
    }
}
