using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Shared;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Model;
using Newtonsoft.Json;
using SmartBreadcrumbs.Attributes;
using System.Net.Mail;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    //  [Route("Users/[action]")]

    public class UsersController : BaseController<UsersController>
    {
        private IStringLocalizer<SharedResource> _localizer;
        private readonly IUserManagerRepository<ApplicationUser> _usermanegerRepository;
        private readonly ILogger<UsersController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAuthenticatedUserService _userService;
        public UsersController(UserManager<ApplicationUser> userManager
            , IAuthenticatedUserService userService, IUserManagerRepository<ApplicationUser> usermanegerRepository, IStringLocalizer<SharedResource> localizer,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, ILogger<UsersController> logger)
        {
            _localizer = localizer;
            _usermanegerRepository = usermanegerRepository;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        // GET: User
        [Authorize(Policy = "user.list")]
        [Breadcrumb("Danh sách nhân viên", AreaName = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> LoadAll()
        {
            try
            {
                //var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                //var allUsersExceptCurrentUser = await _userManager.Users.Where(x => x.ComId == currentUser.ComId).ToListAsync();
                var model = _mapper.Map<IEnumerable<UserViewModel>>(await _usermanegerRepository.GetAllUserAsync(HttpContext.User));
                return PartialView("_ViewAll", model);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return default;
            }
        }

        // GET: User/Details/5
        public IActionResult getSelect2(string idselectd)
        {
            var allUsersExceptCurrentUser = from d in _userManager.Users select new { id = d.Id, text = !string.IsNullOrEmpty(d.FullName) ? d.FullName : d.FirstName + " " + d.LastName, selected = d.Id == idselectd };
            var json = allUsersExceptCurrentUser.ToArray();
            var data = JsonConvert.SerializeObject(allUsersExceptCurrentUser);
            return Content(data);
        }

        // GET: User/Create
        [Authorize("user.create")]
        public async Task<ActionResult> CreateAsync()
        {
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Create", new UserViewModel()) });
        }

        // POST: User/Create
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
        [Authorize(Policy = "user.edit")]
        // GET: User/Edit/5
        public async Task<ActionResult> EditAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(EnumStatusString.Err1));
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            var users = _mapper.Map<UserViewModel>(user);
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Edit", users) });
        }
        [Authorize(Policy = "user.edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(UserViewModel model)
        {
            try
            {
                ModelState.Remove("Password");
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.Password) && (model.Password != model.ConfirmPassword))
                    {
                        _notify.Error(GeneralMess.ConvertStatusToString(EnumStatusString.PassSameConfirmPassword));
                        return new JsonResult(new { isvalid = false, html = string.Empty });
                    }
                    ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
                    string oldvalue = JsonConvert.SerializeObject(user);
                    if (user == null)
                    {
                        _notify.Error(GeneralMess.ConvertStatusToString(EnumStatusString.Err1));
                        return new JsonResult(new { isvalid = false, html = string.Empty });
                    }
                    if (!Common.IsValidEmail(model.Email))
                    {
                        _notify.Error(_localizer.GetString("EmailFormat").Value);
                        return new JsonResult(new { isvalid = false, html = string.Empty });
                    }
                    //MailAddress address = new MailAddress(model.Email);
                    //string userName = address.User;

                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        var newPassword = _userManager.PasswordHasher.HashPassword(user, model.Password);
                        user.PasswordHash = newPassword;
                    }

                    user.PhoneNumber = model.PhoneNumber;
                    user.FullName = model.FullName;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    //user.UserName = userName;
                    user.Email = model.Email;
                    user.EmailConfirmed = true;


                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        string newvalue = JsonConvert.SerializeObject(user);
                        var allUsersExceptCurrentUser = await _usermanegerRepository.GetAllUserAsync(HttpContext.User);
                        var userss = _mapper.Map<IEnumerable<UserViewModel>>(allUsersExceptCurrentUser);
                        var htmlData = await _viewRenderer.RenderViewToStringAsync("_ViewAll", userss);
                        //await _mediator.Send(new AddActivityLogCommand() { userId = _userService.UserId, Action = "Edit", NewValues = newvalue, OldValues = oldvalue });
                        //_notify.Success($"Chỉnh sửa {model.UserName} thành công!.");
                        _notify.Success(_localizer.GetString("EditOk").Value);
                        //return RedirectToAction("Index");
                        return new JsonResult(new { isValid = true, html = htmlData });
                    }
                    foreach (var error in result.Errors)
                    {
                        _notify.Error(error.Description);
                    }
                    //var html = await _viewRenderer.RenderViewToStringAsync("_Create", model);
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
                var err = ModelState.Values.SelectMany(v => v.Errors)
                              .Select(v => v.ErrorMessage + " " + v.Exception).ToList();
                _notify.Error(err.FirstOrDefault());
                return new JsonResult(new { isValid = false, html = "" });
            }
            catch (Exception error)
            {
                _logger.LogError(default(EventId), error, error.Message);
                _notify.Error(error.ToString());
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
        }
        [Authorize(Policy = "user.delete")]
        [HttpPost]
        public async Task<IActionResult> OnPostDelete(string Id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(Id);
            // ApplicationUser userlogin = await _userManager.GetUserAsync(User);
            ApplicationUser userlogin = await _usermanegerRepository.GetUserAsync(User);

            if (userlogin.UserName.ToLower() == user.UserName.ToLower())
            {
                _notify.Error(_localizer.GetString("DeleteUserLogin").Value);
                return new JsonResult(new { isvalid = false, html = string.Empty });
            }
            if (user == null)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(EnumStatusString.Err1));
                return new JsonResult(new { isvalid = false, html = string.Empty });
            }
            if (user.UserName.ToLower() == "superadmin")
            {
                _notify.Error("User SuperAdmin không được phép xóa");
                return new JsonResult(new { isvalid = false, html = string.Empty });
            }
            var delete = await _userManager.DeleteAsync(user);
            if (delete.Succeeded)
            {
                // await _mediator.Send(new AddActivityLogCommand() { userId = _userService.UserId, Action = "Delete", NewValues = $"Xóa User {user.UserName} thành công!." + JsonConvert.SerializeObject(user) });
                _notify.Success(_localizer.GetString("DeleteOk").Value);
                var allUsersExceptCurrentUser = await _usermanegerRepository.GetAllUserAsync(HttpContext.User);
                var users = _mapper.Map<IEnumerable<UserViewModel>>(allUsersExceptCurrentUser);
                var htmlData = await _viewRenderer.RenderViewToStringAsync("_ViewAll", users);
                return new JsonResult(new { isValid = true, html = htmlData });
            }
            _logger.LogInformation(_userService.UserId + " Xóa User thất bại " + user.UserName);
            foreach (var error in delete.Errors)
            {
                _notify.Error(error.Description);
            }
            return new JsonResult(new { isvalid = false, html = string.Empty });
        }
        // POST: User/Edit/5
        [Authorize(Policy = "user.create")]
        [HttpPost]
        public async Task<IActionResult> OnPostCreate(UserViewModel userModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!Common.IsValidEmail(userModel.Email))
                    {
                        _notify.Error(_localizer.GetString("EmailFormat").Value);
                        return new JsonResult(new { isValid = false, html = "" });
                    }

                    MailAddress address = new MailAddress(userModel.Email);
                    string userName = address.User;
                    if (!string.IsNullOrEmpty(userModel.UserName))
                    {
                        userName = userModel.UserName;
                    }
                    var currentUser = User.Identity.GetUserClaimLogin();
                    //string userName = userModel.UserName;
                    var user = new ApplicationUser
                    {
                        ComId = currentUser.ComId,
                        IdDichVu = currentUser.IdDichVu,
                        UserName = userName,
                        Email = userModel.Email,
                        FullName = userModel.FullName,
                        PhoneNumber = userModel.PhoneNumber,
                        FirstName = userModel.FirstName,
                        LastName = userModel.LastName,
                        EmailConfirmed = true,
                    };
                    var result = await _userManager.CreateAsync(user, userModel.Password);
                    if (result.Succeeded)
                    {
                        // await _mediator.Send(new AddActivityLogCommand() { userId = _userService.UserId, Action = "Create", NewValues = $"Thêm mới User {user.UserName} thành công!." + JsonConvert.SerializeObject(user) });
                        //await _userManager.AddToRoleAsync(user, Roles.Basic.ToString());
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var allUsersExceptCurrentUser = await _usermanegerRepository.GetAllUserAsync(HttpContext.User);
                        var users = _mapper.Map<IEnumerable<UserViewModel>>(allUsersExceptCurrentUser);
                        var htmlData = await _viewRenderer.RenderViewToStringAsync("_ViewAll", users);
                        _notify.Success($"{_localizer.GetString("AddOk").Value} user {user.UserName}");
                        return new JsonResult(new { isValid = true, html = htmlData });
                    }
                    _logger.LogInformation($"Thêm mới User {user.UserName} thất bại!." + JsonConvert.SerializeObject(user));
                    foreach (var error in result.Errors)
                    {
                        _notify.Error(error.Description);
                    }
                    var html = await _viewRenderer.RenderViewToStringAsync("_Create", userModel);
                    return new JsonResult(new { isValid = false, html = html });
                }
                var err = ModelState.Values.SelectMany(v => v.Errors)
                             .Select(v => v.ErrorMessage + " " + v.Exception).ToList();
                _notify.Error(err.FirstOrDefault());
                return new JsonResult(new { isValid = false, html = "" });
            }
            catch (Exception e)
            {
                _logger.LogError(default, e);
                _notify.Error(e.ToString());
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

        }

        // GET: User/Delete/5
        [Authorize(Policy = "user.lock")]
        [HttpPost]
        public async Task<IActionResult> LockAccountAsync(string userId, int lockacc)
        {

            var currentUser = await _userManager.FindByIdAsync(userId);
            if (lockacc == 1)
            {
                currentUser.LockoutForever = true;
                _notify.Success($"Khóa thành công User: {currentUser.FullName}");
            }
            else
            {
                _notify.Success($"Mở khóa thành công User: {currentUser.FullName}");
                currentUser.LockoutForever = false;
            }


            await _userManager.UpdateAsync(currentUser);

            // await _mediator.Send(new AddActivityLogCommand() { userId = userId, Action = "LockoutForever", NewValues = $"LockoutForever {currentUser.FullName}!." });
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public async Task<IActionResult> ActiveAccountAsync(string userId)
        {

            var currentUser = await _userManager.FindByIdAsync(userId);
            currentUser.IsActive = true;
            await _userManager.UpdateAsync(currentUser);
            // await _mediator.Send(new AddActivityLogCommand() { userId = userId, Action = "ActiveAccount", NewValues = $"ActiveAccount {currentUser.FullName}!." });
            _notify.Success($"Kích hoạt thành công User: {currentUser.FullName}");
            return RedirectToAction(nameof(Index));

        }
        // GET: User/Delete/5ActiveAccount


        // POST: User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
    }
}
