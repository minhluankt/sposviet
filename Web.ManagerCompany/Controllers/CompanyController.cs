using Application.Constants;
using Application.Enums;
using Application.Features.CompanyInfo.Commands;
using Application.Features.CompanyInfo.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using Infrastructure.Infrastructure.DbContexts;
using Infrastructure.Infrastructure.Identity.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Drawing.Drawing2D;
using System.Reflection;
using Web.ManagerCompany.Abstractions;

namespace Web.ManagerCompany.Controllers
{
    public class CompanyController : BaseController<HomeController>
    {
        private IdentityContext _identityContext;
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICompanyAdminInfoRepository _company;
        public CompanyController(UserManager<ApplicationUser> userManager,
            IdentityContext identityContext,


            IOptions<CryptoEngine.Secrets> config, ICompanyAdminInfoRepository company)
        {
            _identityContext = identityContext;
            _company = company;
            _config = config;
            _userManager = userManager;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var getid = await _mediator.Send(new GetAllCompanyQuery());
            if (getid.Succeeded)
            {
                return View(getid.Data.Select(x => new CompanyAdminInfoViewModel()
                {

                    Name = x.Name,
                    Active = x.Active,
                    Address = x.Address,
                    CusTaxCode = x.CusTaxCode,
                    DateExpiration = x.DateExpiration,
                    PhoneNumber = x.PhoneNumber,
                    NumberDateExpiration = x.NumberDateExpiration,
                    AccountName = x.AccountName,
                    CreatedOn = x.CreatedOn,
                    StartDate = x.StartDate,
                    Id = x.Id,
                    IdType = x.IdType,
                    IdDichVu = x.IdDichVu,
                    TypeCompany = x.TypeCompany,
                    secret = CryptoEngine.Encrypt("id=" + x.Id, _config.Value.Key)
                }));
            }
            return View(new List<CompanyAdminInfo>());
        }
        [Authorize(Policy = "company.create")]
        public ActionResult Create()
        {
            LoadViewbag();
            LoadViewbagDemoThat();
            return View(new CompanyAdminInfoViewModel() { Active = true });
        }
        private void LoadViewbag(EnumTypeProduct type = EnumTypeProduct.NONE)
        {
            ViewBag.SelectList = Enum.GetValues(typeof(EnumTypeProduct)).Cast<EnumTypeProduct>().OrderBy(x => (Convert.ToInt32(x))).Select(x => new SelectListItem
            {
                Text = GetDisplayName(x),
                Value = Convert.ToInt32(x).ToString(),
                Selected = x == type
            });
        }
        private void LoadViewbagDemoThat(EnumTypeCompany type = EnumTypeCompany.DEMO)
        {
            ViewBag.SelectListTypeCompany = Enum.GetValues(typeof(EnumTypeCompany)).Cast<EnumTypeCompany>().OrderBy(x => (Convert.ToInt32(x))).Select(x => new SelectListItem
            {
                Text = GetDisplayName(x),
                Value = Convert.ToInt32(x).ToString(),
                Selected = x == type
            });
        }
        private string GetDisplayName(object value)
        {
            var type = value.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException(string.Format("Type {0} is not an enum", type));
            }

            // Get the enum field.
            var field = type.GetField(value.ToString());
            if (field == null)
            {
                return value.ToString();
            }

            // Gets the value of the Name property on the DisplayAttribute, this can be null.
            var attributes = field.GetCustomAttribute<DisplayAttribute>();
            return attributes != null ? attributes.Name : value.ToString();
        }
        [Authorize(Policy = "permissions.edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateOrEdit(CompanyAdminInfoViewModel model)
        {
           
            //làm cái bán lẻ giao diện mới
            model.PhoneNumber = model.PhoneNumber?.Replace(" ", "").Trim();
            model.CusTaxCode = model.CusTaxCode?.Replace(" ", "").Trim();
            model.Email = model.Email?.Trim();
            model.AccountName = model.AccountName?.Trim();
            //model.TypeCompany = EnumTypeCompany.DEMO;
            model.Status = EnumStatusCompany.Active;
            _logger.LogInformation(User.Identity.Name + "--> CompanyInfo OnPostCreateOrEdit");
            try
            {
                if (model.DateExpiration!=null)
                {
                    if (model.DateExpiration<DateTime.Now.Date)
                    {
                        LoadViewbag(model.IdDichVu);
                        LoadViewbagDemoThat(model.TypeCompany);
                        _notify.Error("Ngày hết hạn không được nhỏ hơn ngày hôm nay");
                        return View("Create", model);
                    }
                }
                if (model.Id == 0)
                {
                    if (ModelState.IsValid)
                    {
                        if (string.IsNullOrEmpty(model.AccountName))
                        {
                            LoadViewbag(model.IdDichVu);
                            LoadViewbagDemoThat(model.TypeCompany);
                            _notify.Error("Chưa điền tài khoản");
                            return View("Create", model);
                        }
                        var createProductCommand = _mapper.Map<CreateCompanyInfoCommand>(model);
                        var result = await _mediator.Send(createProductCommand);
                        if (result.Succeeded)
                        {
                            //tạo tài khoản
                            try
                            {
                                var user = new ApplicationUser
                                {
                                    Level = 2,
                                    ComId = result.Data,
                                    IdDichVu = model.IdDichVu,
                                    UserName = model.AccountName,
                                    Email = model.Email,
                                    FullName = model.Name,
                                    PhoneNumber = model.PhoneNumber,
                                    IsActive = true,
                                    EmailConfirmed = true,
                                    IsStoreOwner = true,
                                };
                                var create = await _userManager.CreateAsync(user, user.UserName);
                                if (result.Succeeded)
                                {
                                    await _userManager.AddToRoleAsync(user, "SuperAdmin");
                                    await _company.UpdateUserNameCompany(result.Data, user.UserName);
                                }
                                else
                                {
                                    _notify.Error("Tạo tài khoản không thành công");
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.LogInformation($"Tạo user của công ty{model.Name} {model.PhoneNumber} lỗi");
                                _logger.LogError(e.ToString());
                                _notify.Error(e.Message);
                            }
                            _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS008));
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            LoadViewbagDemoThat(model.TypeCompany);
                            LoadViewbag(model.IdDichVu);
                            _notify.Error(GeneralMess.ConvertStatusToString(result.Message));
                            return View("Create", model);
                        }
                    }
                    //var errors = ModelState.Select(x => x.Value.Errors)
                    //       .Where(y => y.Count > 0)
                    //       .ToList();
                    LoadViewbagDemoThat(model.TypeCompany);
                    LoadViewbag(model.IdDichVu);
                    return View("Create", model);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var createProductCommand = _mapper.Map<UpdateCompanyInfoCommand>(model);

                        var result = await _mediator.Send(createProductCommand);
                        if (result.Succeeded)
                        {
                            model.Id = result.Data;
                            _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            LoadViewbagDemoThat(model.TypeCompany);
                            LoadViewbag(model.IdDichVu);
                            _notify.Error(GeneralMess.ConvertStatusToString(result.Message));
                            return View("Edit", model);
                        }
                    }
                    LoadViewbagDemoThat(model.TypeCompany);
                    LoadViewbag(model.IdDichVu);
                    return View("Edit", model);
                }

            }
            catch (Exception ex)
            {
                _notify.Error(ex.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

        }

        [Authorize(Policy = "company.edit")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Edit(int id)
        {
            var data = await _mediator.Send(new GetByIdCompanyInfoQuery() { Id = id });
            if (data.Succeeded)
            {
                LoadViewbag(data.Data.IdDichVu);
                LoadViewbagDemoThat(data.Data.TypeCompany);
                var _ndt = _mapper.Map<CompanyAdminInfoViewModel>(data.Data);
                return View(_ndt);
            }
            _notify.Error(data.Message);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Policy = "company.delete")]
        [EncryptedParameters("secret")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var data = await _mediator.Send(new DeleteCompanyCommand() { Id = id });
                if (data.Succeeded)
                {

                    using (var transaction = _identityContext.Database.BeginTransaction())
                    {
                        try
                        {
                            var getall = _identityContext.Users.Where(x => x.ComId == id && x.IdDichVu == (EnumTypeProduct)data.Data).AsNoTracking();

                            foreach (var user in getall)
                            {
                                var getuser = await _userManager.FindByIdAsync(user.Id);
                                var rolesForUser = await _userManager.GetRolesAsync(getuser);


                                await _userManager.RemoveLoginAsync(user, string.Empty, string.Empty);

                                if (rolesForUser.Count() > 0)
                                {
                                    foreach (var item in rolesForUser.ToList())
                                    {
                                        if (item.Trim().ToUpper()!=Roles.SuperAdmin.ToString().ToUpper())
                                        {
                                            var result = await _userManager.RemoveFromRoleAsync(user, item);
                                        }
                                    }
                                }
                                _identityContext.Users.Remove(user);
                                _identityContext.SaveChanges();
                            }
                            transaction.Commit();
                            transaction.Dispose();
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();

                        }
                        //xóa các tài khoản liên quan

                        // await _userManager.DeleteAsync(user);

                    }
                    _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS007));
                    return Json(new { isValid = true });
                }
                _notify.Error(data.Message);
                return Json(new { isValid = false });
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                return Json(new { isValid = true });
            }

        }

    }
}
