using Application.Constants;
using Application.Enums;
using Application.Features.BankAccounts.Commands;
using Application.Features.BankAccounts.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using BankService.Model;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SystemVariable;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.BankAccounts.Selling.Controllers
{
    [Area("Selling")]
    public class BankAccountController : BaseController<BankAccountController>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private IFormFileHelperRepository _iFormFileHelperRepository;
        public BankAccountController(IOptions<CryptoEngine.Secrets> _config, IFormFileHelperRepository _iFormFileHelperRepository)
        {
            this._config = _config;
            this._iFormFileHelperRepository = _iFormFileHelperRepository;
        }
        [Authorize(Policy = "bankAccount.index")]
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult GetJsonBank(int? bin)
        {
            var getjson = _iFormFileHelperRepository.GetContentFile(FileConstants.BankAccountVietQR, FolderUploadConstants.BankAccountFolder);
            var listbank = Common.ConverJsonToModel<List<BankAccountModel>>(getjson);
            var jsonselect2 = listbank.Select(x => new
            {
                id=x.bin,
                selected=x.bin== bin,
                text=$"{x.name} - {x.shortName}",
                logo=x.logo,
                code = x.code,
                shortName = x.shortName,
            });
            return Content(Common.ConverObjectToJsonString(jsonselect2));
        }
        public async Task<ActionResult> GetJsonAccountBankAsync()
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var send = await _mediator.Send(new GetAllBankAccountQuery(currentUser.ComId) { Paging = false });
            if (send.Succeeded)
            {
                var jsonselect2 = send.Data.Select(x => new
                {
                    id = x.Id,
                    text = $"{x.AccountName} - {x.BankNumber}",
                    binVietQR = x.BinVietQR,
                    accountName = x.AccountName,
                    bankNumber = x.BankNumber,
                    bankName = x.BankName,
                });
                return Content(Common.ConverObjectToJsonString(jsonselect2));
            }
            return Content("[]");
        }

        [HttpPost]
        public async Task<IActionResult> LoadAll(BankAccount model)
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
                if (!string.IsNullOrEmpty(searchValue))
                {
                    model.BankNumber = searchValue;
                }
                //Paging Size (10, 20, 50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 10;

                int skip = start != null ? Convert.ToInt32(start) : 0;

                int recordsTotal = 0;
                var currentUser = User.Identity.GetUserClaimLogin();
                int currentPage = skip >= 0 ? skip / pageSize : 0;
                var response = await _mediator.Send(new GetPageListQuery(currentUser.ComId)
                {
                    Name = model.BankNumber,
                    Comid = currentUser.ComId,
                    sortDirection = sortColumn,
                    sortOn = sortColumnDirection,
                    PageSize = pageSize,
                    PageNumber = currentPage
                });
                if (response.Succeeded)
                {
                    response.Data.Items.ForEach(x =>
                    {
                       x.secret= CryptoEngine.Encrypt("id=" + x.Id, _config.Value.Key);
                    });

                    return Json(new { draw = draw, recordsFiltered = response.Data.TotalItemCount, recordsTotal = response.Data.TotalItemCount, data = response.Data.Items });
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

        [Authorize(Policy = "bankAccount.edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(BankAccount collection, int? codebank)
        {
            var getusser = User.Identity.GetUserClaimLogin();
            //var getusser = await _userManager.GetUserAsync(User);
            collection.ComId = getusser.ComId;

            if (codebank==null)
            {
                _notify.Error("Vui lòng chọn ngân hàng");
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            collection.BinVietQR = codebank;
            var getjson = _iFormFileHelperRepository.GetContentFile(FileConstants.BankAccountVietQR, FolderUploadConstants.BankAccountFolder);
            var listbank = Common.ConverJsonToModel<List<BankAccountModel>>(getjson);
            var getbank = listbank.SingleOrDefault(x => x.bin == codebank);
            if (getbank == null)
            {
                _notify.Error("Ngân hàng không tồn tại!");
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            collection.BankName = $"{getbank.name} - {getbank.shortName}";
            try
            {
                if (collection.Id == 0)
                {
                    var createProductCommand = _mapper.Map<CreateBankAccountCommand>(collection);
                    var result = await _mediator.Send(createProductCommand);
                    if (result.Succeeded)
                    {
                        collection.Id = result.Data;
                        _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS008));

                    }
                    else
                    {
                        _notify.Error(GeneralMess.ConvertStatusToString(result.Message));
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }
                }
                else
                {
                    var updateProductCommand = _mapper.Map<UpdateBankAccountCommand>(collection);
                    var result = await _mediator.Send(updateProductCommand);
                    if (result.Succeeded) _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));

                }
                return new JsonResult(new { isValid = true, loadTable = true, closeSwal = true });
            }
            catch (Exception e)
            {
                _notify.Error(e.ToString());
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
        }

        // GET: BankAccountsController/Edit/5
        [Authorize(Policy = "bankAccount.create")]
        public async Task<ActionResult> Create()
        {
            var html = await _viewRenderer.RenderViewToStringAsync("_Create", new Domain.Entities.BankAccount() { Active = true });
            return new JsonResult(new { isValid = true, html = html, title = "Thêm mới tài khoản" });
        }
        [Authorize(Policy = "bankAccount.edit")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Edit(int id)
        {
            var getusser = User.Identity.GetUserClaimLogin();
            //  var getusser = await _userManager.GetUserAsync(User);
            var data = await _mediator.Send(new GetByIdBankAccountQuery(getusser.ComId) { Id = id });
            if (data.Succeeded)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_Edit", data.Data);
                return new JsonResult(new { isValid = true, html = html, title = "Chỉnh sửa tài khoản" });
            }
            return new JsonResult(new { isValid = false, html = string.Empty });
        }



        // POST: BankAccountsController/Delete/5
        [Authorize(Policy = "bankAccount.delete")]
        [HttpPost]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                //var getusser = await _userManager.GetUserAsync(User);
                var getusser = User.Identity.GetUserClaimLogin();
                var deleteCommand = await _mediator.Send(new DeleteBankAccountCommand(getusser.ComId, id));
                if (deleteCommand.Succeeded)
                {
                    return new JsonResult(new { isValid = true, loadTable = true });
                }
                else
                {
                    _notify.Error(deleteCommand.Message);
                    return new JsonResult(new { isValid = false });
                }
            }
            catch (Exception ex)
            {
                _notify.Error(ex.Message);
                return new JsonResult(new { isValid = false });
            }
        }
    }
}
