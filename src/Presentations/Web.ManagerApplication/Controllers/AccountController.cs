using Application.Constants;
using Application.Enums;
using Application.Features.Citys.Query;
using Application.Features.Customers.Commands;
using Application.Features.Customers.Query;
using Application.Features.Orders.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using Domain.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Model;
using System.Net.Mail;
using System.Security.Claims;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Controllers
{
    public class AccountController : BaseController<AccountController>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly INotifyUserRepository<NotifiUser> _notifiUser;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICustomerRepository _CustomerRepository;
        private readonly IUserManager _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepositoryAsync<City> _repositoryCountry;
        private readonly IUserRepository _userRepository;
        private readonly IFormFileHelperRepository _fileHelper;
        public AccountController(IUserManager userManager, IFormFileHelperRepository fileHelper,
             IOptions<CryptoEngine.Secrets> config, INotifyUserRepository<NotifiUser> notifiUser,
            IRepositoryAsync<City> repositoryCountry, SignInManager<ApplicationUser> signInManager,

            ICustomerRepository CustomerRepository, IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository)
        {
            _notifiUser = notifiUser;
            _config = config;
            _signInManager = signInManager;
            _repositoryCountry = repositoryCountry;
            _CustomerRepository = CustomerRepository;
            _fileHelper = fileHelper;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        public async Task<IActionResult> NotifmyAsync()
        {
            var usercom = await _userRepository.GetUserAsync(User);
            if (usercom == null)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR033));
                return RedirectToAction("Login");
            }


            return View();
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        public async Task<IActionResult> LoadDataNotifmyAsync(NotifyUserModel model)
        {
            var usercom = await _userRepository.GetUserAsync(User);
            if (usercom == null)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR033));
                return RedirectToAction("Login");
            }
            var ajaxRequest = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (ajaxRequest)
            {
                model.IdUser = usercom.Id;
                return PartialView("_ViewListNotify", await _notifiUser.GetAllPaginatedListAsync(model));
            }

            return View(_notifiUser.GetAllPaginatedListAsync(model));
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        public async Task<IActionResult> CountNotifmyUserAsync()
        {
            var usercom = await _userRepository.GetUserAsync(User);
            if (usercom == null)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR033));
                return new JsonResult(new { isValid = false });
            }
            return new JsonResult(new { isValid = true, count = _notifiUser.CountNotifyNoReviewAsync(usercom.Id) });
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        public async Task<IActionResult> DeleteDataNotifmyAsync(int id)
        {
            var usercom = await _userRepository.GetUserAsync(User);
            if (usercom == null)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR033));
                return RedirectToAction("Login");
            }
            var ajaxRequest = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (ajaxRequest)
            {
                _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS007));
                return new JsonResult(new { isValid = await _notifiUser.DeleteByIdAsync(id, usercom.Id) });
            }
            _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.ERR007));
            return new JsonResult(new { isValid = false });
        }



        public async Task<IActionResult> CheckLoginAsync()
        {
            var usercom = await _userRepository.GetUserAsync(User);
            if (usercom == null)
            {
                return new JsonResult(new { isValid = false });
            }
            return new JsonResult(new { isValid = true });

        }


        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        public async Task<IActionResult> DeliveryAddressAsync(int payment = 0)
        {
            try
            {
                var usercom = await _userRepository.GetUserAsync(User);
                if (usercom == null)
                {
                    _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR033));
                    return RedirectToAction("Login");
                }
                var mapdata = _mapper.Map<CustomerModelView>(usercom);
                mapdata.payment = payment;
                var data = await _mediator.Send(new GetAllCityCacheQuery());
                if (data.Succeeded)
                {
                    mapdata.Citys = data.Data.OrderBy(x => x.Name);
                }
                if (mapdata.IdDistrict == null)
                {
                    mapdata.IdDistrict = 0;
                }
                if (mapdata.IdWard == null)
                {
                    mapdata.IdWard = 0;
                }
                return View(mapdata);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                _notify.Error(e.Message);
                return RedirectToAction("Login");
            }

        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        public async Task<IActionResult> DeliveryAddressAsync(CustomerModelView model)
        {
            try
            {
                var usercom = await _userRepository.GetUserAsync(User);
                if (usercom == null)
                {
                    _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR033));
                    return RedirectToAction("Login");
                }
                var mapdata = _mapper.Map<UpdateInfoDeliveryCustomerCommand>(model);
                mapdata.Id = usercom.Id;
                var data = await _mediator.Send(mapdata);
                if (data.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(data.Message));
                    if (model.payment == 1)
                    {
                        return LocalRedirect("/Cart/payment");
                    }
                    return LocalRedirect("/Cart/Mycart");
                }
                _logger.LogError(GeneralMess.ConvertStatusToString(data.Message));
                _notify.Error(GeneralMess.ConvertStatusToString(data.Message));
                var datacity = await _mediator.Send(new GetAllCityCacheQuery());
                if (datacity.Succeeded)
                {
                    model.Citys = datacity.Data.OrderBy(x => x.Name);
                }
                if (model.IdDistrict == null)
                {
                    model.IdDistrict = 0;
                }
                if (model.IdWard == null)
                {
                    model.IdWard = 0;
                }
                return View(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                _notify.Error(e.Message);
                return LocalRedirect("/Cart/Mycart");
            }

        }


        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        public async Task<IActionResult> OrderMyAsync()
        {
            var usercom = await _userRepository.GetUserAsync(User);
            if (usercom == null)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR033));
                return RedirectToAction("Login");
            }
            //var data = await _mediator.Send(new GetByOrderCustomerQuery(usercom.Id));
            //if (data.Succeeded)
            //{
            //    var mapdata = _mapper.Map<CustomerModelView>(data.Data);
            //    return View(mapdata);
            //}
            return View();
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        public async Task<IActionResult> LoadAllOrder(OrderViewModel model)
        {
            var usercom = await _userRepository.GetUserAsync(User);
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

                //Paging Size (10, 20, 50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;

                int skip = start != null ? Convert.ToInt32(start) : 0;

                int recordsTotal = 0;
                DateTime? FromDate = null;
                DateTime? ToDate = null;

                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
                {
                    FromDate = Common.ConvertStringToDateTime(model.FromDate);
                    ToDate = Common.ConvertStringToDateTime(model.ToDate);
                }
                // getting all Customer data  
                var response = await _mediator.Send(new GetOrderQueryQuery()
                {
                    IdCustomer = usercom.Id,
                    FromDate = FromDate,
                    ToDate = ToDate,
                    codeOrder = model.OrderCode,
                    sortColumn = sortColumn,
                    sortColumnDirection = sortColumnDirection,
                    pageSize = pageSize,
                    skip = skip
                });
                if (response.Succeeded)
                {
                    return Json(new { draw = draw, recordsFiltered = response.Data.TotalRow, recordsTotal = response.Data.TotalRow, data = response.Data.Orders });
                }

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = "" });

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<IActionResult> LoginAsync(bool popup)
        {
            var ExternalLogins = (await _userManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (popup)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("Login", new LoginCustomerViewModel() { ExternalLogins = ExternalLogins, LoginPopup = popup });
                return new JsonResult(new { isValid = true, html = html });
            }

            return View(new LoginCustomerViewModel() { ExternalLogins = ExternalLogins, LoginPopup = popup });
        }
        public async Task<IActionResult> GetUserAsync()
        {
            var user = await _userRepository.GetUserAsync(User);
            if (user == null)
            {
                return new JsonResult(new { data = "", isLogin = false });
            }
            return new JsonResult(new { data = user, isLogin = true });
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassAsync()
        {
            TempData["info"] = TempData["info"];
            TempData["mailexit"] = TempData["mailexit"];
            var user = await _userRepository.GetUserAsync(User);
            var mapdata = _mapper.Map<CustomerModelView>(user);
            var values = $"Id={mapdata.Id}";
            mapdata.secretId = CryptoEngine.Encrypt(values, _config.Value.Key);
            return View(mapdata);
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]

        public async Task<IActionResult> UpdateAvatarAsync(string base64)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(User);
                if (user == null)
                {
                    _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR033));
                    return new JsonResult(new { isValid = false });
                }
                string filename = string.Empty;
                var addUser = _CustomerRepository.UpdateAvatar(base64, user.Id, out filename);
                if (addUser)
                {
                    var modellogin = new CookieCustomerUser
                    {
                        UserId = user.IdCodeGuid,
                        EmailAddress = user.Email,
                        Username = user.UserName,
                        Name = user.Name,
                        Image = user.Image,
                        Created = user.CreatedOn
                    };
                    await _userManager.RefreshSignInAsync(modellogin, this.HttpContext);

                    return new JsonResult(new { isValid = true, src = base64, data = GeneralMess.ConvertStatusToString(HeperConstantss.SUS006) });
                }
                return new JsonResult(new { isValid = false });

            }
            catch (Exception e)
            {

                return new JsonResult(new { isValid = false, data = e.Message });
            }

        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> ChangePassAsync(CustomerModelView model)
        {
            ModelState.Remove("UserName");
            var user = await _userRepository.GetUserAsync(User);
            var mapdata = _mapper.Map<CustomerModelView>(user);

            if (!ModelState.IsValid)
            {
                foreach (var modelStateKey in ViewData.ModelState.Keys)
                {
                    var modelStateVal = ViewData.ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        var key = modelStateKey;
                        var errorMessage = error.ErrorMessage;
                        var exception = error.Exception;
                        _notify.Error(errorMessage);
                        // You may log the errors if you want
                    }
                }
                return View(mapdata);
            }


            var checkpass = await _userRepository.CheckPasswordCustomerAsync(model.PasswordOld, model.Id);
            if (checkpass)
            {
                bool update = await _userRepository.UpdatePasswordAsync(model.Password, model.Id);
                if (update)
                {
                    ViewData["UpdatePass"] = "OK";
                    return View(mapdata);
                }
            }
            ViewData["UpdatePass"] = "ERR";
            return View(mapdata);

        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]

        public async Task<IActionResult> ProfileAsync()
        {
            Customer model = new Customer();
            //var ass = _httpContextAccessor.HttpContext.User;
            //var a = this.User.Claims.ToDictionary(x => x.Type, x => x.Value);
            ClaimsIdentity user = new ClaimsIdentity();
            var usercom = await _userRepository.GetUserAsync(User);
            if (usercom == null)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR033));
                return LocalRedirect("~/Home/Index");
            }
            var data = await _mediator.Send(new GetByIdCustomerQuery() { IdCode = usercom.IdCodeGuid.ToString() });
            if (data.Succeeded)
            {
                model = data.Data;
                var mapdata = _mapper.Map<CustomerModelView>(model);
                if (!string.IsNullOrEmpty(mapdata.Image))
                {
                    mapdata.Image = _fileHelper.ImagetoBase64(mapdata.Image, FolderUploadConstants.Customer);
                }
                var values = $"Id={mapdata.Id}";
                mapdata.secretId = CryptoEngine.Encrypt(values, _config.Value.Key);
                return View(mapdata);
            }
            _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR001));
            return LocalRedirect("~/Home/Index");

        }
        [EncryptedParameters("secret")]
        [HttpPost]
        public async Task<IActionResult> ComplementaryEmailAsync(int Id, string email, bool modal = false)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            //modal là  dạng xác nhận modal k phải ở đổi pass
            if (string.IsNullOrEmpty(email) && Id == 0)
            {
                return LocalRedirect("/Error");
            }
            var conf = await _CustomerRepository.ComplementaryEmailAsync(email, currentUser.ComId, Id);
            if (conf == HeperConstantss.SUS008)
            {
                string text = $"Chúng tôi đã gửi yêu cầu vào email <b>{email}</b> của bạn, vui lòng truy cập vào email để xác nhận";
                if (modal)
                {
                    return new JsonResult(new { isValid = true, html = text });
                }
                TempData["info"] = text;
                return RedirectToAction("ChangePass");
            }
            else if (conf == HeperConstantss.ERR005)
            {
                if (modal)
                {
                    return new JsonResult(new { isValid = false, html = GeneralMess.ConvertStatusToString(HeperConstantss.ERR005) });
                }
                TempData["mailexit"] = GeneralMess.ConvertStatusToString(HeperConstantss.ERR005);
                return RedirectToAction("ChangePass");
            }
            else
            {
                if (modal)
                {
                    return new JsonResult(new { isValid = false, html = GeneralMess.ConvertStatusToString(conf) });
                }
                TempData["Emailmatch"] = GeneralMess.ConvertStatusToString(HeperConstantss.ERR004);
                return RedirectToAction("ChangePass");
            }
        }
        [EncryptedParameters("secret")]
        public async Task<IActionResult> ConfirmEmail(int id, string email)
        {
            if (string.IsNullOrEmpty(email) && id == 0)
            {
                return LocalRedirect("/Error");
            }
            var conf = await _CustomerRepository.ConfirmEmailAccount(email, id);
            if (conf != null)
            {
                var modellogin = new CookieCustomerUser
                {
                    UserId = conf.IdCodeGuid,
                    EmailAddress = conf.Email,
                    Username = conf.UserName,
                    Name = conf.Name,
                    Image = conf.Image,
                    Created = conf.CreatedOn
                };
                await _userManager.RefreshSignInAsync(modellogin, this.HttpContext);
                //  _notify.Success(_localizer.GetString("RegisterOk").Value);
                return View(conf);
            }
            return LocalRedirect("/Error");
        }
        public async Task<IActionResult> UpdatePhone(int id, string phone)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            try
            {
                if (string.IsNullOrEmpty(phone) && id == 0)
                {
                    return LocalRedirect("/Error");
                }
                var conf = await _CustomerRepository.UpdatePhoneNumber(phone, id);
                if (conf)
                {
                    return new JsonResult(new { isValid = true, html = GeneralMess.ConvertStatusToString(HeperConstantss.SUS006) });
                }
                return new JsonResult(new { isValid = false, html = GeneralMess.ConvertStatusToString(HeperConstantss.ERR011) });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new JsonResult(new { isValid = false, html = e.Message });
            }

        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirecUrl = Url.Action("ExternalLoginCallback", new
            { ReturnUrl = returnUrl });


            var properties = _userManager.ConfigureExternalAuthenticationProperties(provider, redirecUrl);
            return new ChallengeResult(provider, properties);
        }
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            try
            {

                // thực hiện đăng nhập lại check nếu user chưa có thì có tạo k, và check facebook đã cho lấy email chưa.
                returnUrl = returnUrl ?? Url.Content("~/");
                var lstExternaLogin = await _userManager.GetExternalAuthenticationSchemesAsync();
                LoginCustomerViewModel loginCustomerViewModel = new LoginCustomerViewModel
                {
                    ReturnUrl = returnUrl,
                    ExternalLogins = lstExternaLogin.ToList(),
                };
                if (remoteError != null)
                {
                    _notify.Error(remoteError);
                    return LocalRedirect("~/");
                }
                var info = await _userManager.GetExternalLoginInfoAsync();
                if (info != null)
                {
                    string LoginProvider = info.LoginProvider;
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                    var dob = info.Principal.FindFirstValue(ClaimTypes.DateOfBirth);
                    var gender = info.Principal.FindFirstValue(ClaimTypes.Gender);
                    var identifier = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                    //string email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    //nếu user đã có trong hệ thống
                    var checkAccount = _userRepository.LoginProvider(LoginProvider, info.ProviderKey, email);
                    if (checkAccount.isSuccess)
                    {
                        await _userManager.SignIn(this.HttpContext, checkAccount.Data, true);
                        _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS003));
                        return LocalRedirect(returnUrl);
                    }
                    //chưa có user trong hệ thống thì tạo
                    Customer customer = new Customer();
                    customer.LoginProvider = LoginProvider;
                    customer.ProviderKey = info.ProviderKey;
                    customer.LoginLast = DateTime.Now;
                    customer.Password = CommonConstants.PassDefault;
                    if (!string.IsNullOrEmpty(email))
                    {

                        customer.Email = email;
                        customer.EmailConfirm = email;
                        customer.isEmailConfirm = true;
                        customer.Name = name;
                        customer.UserName = email;
                        customer.Status = (int)CustomerAccountStatus.Confirm;
                        var addUser = await _userRepository.RegisterAsync(customer);
                        await _userManager.SignIn(this.HttpContext, addUser, true);
                        _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS003));

                    }
                    else
                    {
                        // customer.Email = $"{identifier}@gmail.com";
                        customer.Name = name;
                        customer.UserName = identifier;
                        var addUser = await _userRepository.RegisterAsync(customer);
                        await _userManager.SignIn(this.HttpContext, addUser, true);
                        _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS003));
                    }
                    return LocalRedirect(returnUrl);
                }
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR028));
                return LocalRedirect(returnUrl);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                _notify.Error(e.Message);
                return LocalRedirect(returnUrl);
            }
        }


        [Authorize(AuthenticationSchemes = CookieAuthenticationCustomer.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> ProfileAsync(CustomerModelView model)
        {
            bool validte = _CustomerRepository.ValidateEmail(model.Email, model.Id);
            if (!validte)
            {
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR005));
                return View(model);
            }
            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                bool validtephone = _CustomerRepository.ValidatePhoneNumber(model.PhoneNumber, model.Id);
                if (!validtephone)
                {
                    _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR006));
                    return View(model);
                }
            }
            //var addr = new MailAddress(model.Email);
            //model.UserName = addr.User;
            /// TryUpdateModel(model);

            ModelState.Remove("UserName");
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            if (!ModelState.IsValid)
            {
                foreach (var modelStateKey in ViewData.ModelState.Keys)
                {
                    var modelStateVal = ViewData.ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        var key = modelStateKey;
                        var errorMessage = error.ErrorMessage;
                        var exception = error.Exception;
                        _notify.Error(errorMessage);
                        // You may log the errors if you want
                    }
                }
                return View(model);
            }

            try
            {

                var mapdata = _mapper.Map<UpdateCustomerCommand>(model);
                var result = await _mediator.Send(mapdata);
                if (result.Succeeded)
                {
                    _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS006));
                    return RedirectToAction("Profile");
                }
                else
                {
                    _notify.Error(result.Message);
                    return View(model);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                _notify.Error(e.Message);
                return View(model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginCustomerViewModel model)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                await _userManager.SignOut(this.HttpContext);
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                await _signInManager.SignOutAsync();

            }

            if (!ModelState.IsValid)
            {
                foreach (var modelStateKey in ViewData.ModelState.Keys)
                {
                    var modelStateVal = ViewData.ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        var key = modelStateKey;
                        var errorMessage = error.ErrorMessage;
                        var exception = error.Exception;
                        _logger.LogError(exception, errorMessage);
                        // You may log the errors if you want
                        _notify.Error(errorMessage);
                    }
                }
                if (model.LoginPopup)
                {
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
                model.ExternalLogins = (await _userManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return View(model);
            }


            try
            {
                var user = await _userRepository.Validate(model);
                if (!user.isSuccess)
                {
                    _notify.Error(GeneralMess.ConvertStatusToString(user.Message));
                    if (model.LoginPopup)
                    {
                        return new JsonResult(new { isValid = false, html = string.Empty });
                    }
                    model.ExternalLogins = (await _userManager.GetExternalAuthenticationSchemesAsync()).ToList();
                    return View(model);

                }
                //var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                await _userManager.SignIn(this.HttpContext, user.Data, true);
                _notify.Success($"{GeneralMess.ConvertStatusToString(HeperConstantss.SUS003)}  {user.Data.Name}");

                if (model.LoginPopup)
                {
                    return new JsonResult(new { isValid = true });
                }


                if (Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);
                //if (!string.IsNullOrEmpty(model.ReturnUrl))
                //{
                //    return LocalRedirect(model.ReturnUrl);
                //}
                return LocalRedirect("~/Home/Index");
            }
            catch (Exception e)
            {
                _notify.Error(e.Message);
                if (model.LoginPopup)
                {
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
                model.ExternalLogins = (await _userManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return View(model);
            }
        }

        public async Task<IActionResult> RegisterAsync(bool popup)
        {
            CustomerModelView CustomerModelView = new CustomerModelView();
            CustomerModelView.Citys = _repositoryCountry.GetAllQueryable().AsNoTracking().OrderByDescending(m => m.Name);
            CustomerModelView.RegisterPopup = popup;
            if (popup)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("Register", CustomerModelView);
                return new JsonResult(new { isValid = true, html = html });
            }

            return View(CustomerModelView);
        }
        public IActionResult ValidateEmail(string Email, int? Id = null)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            try
            {
                var addr = new MailAddress(Email);
                bool validte = _CustomerRepository.ValidateEmail(Email, currentUser.ComId, Id);
                if (validte)
                {
                    return new JsonResult(new { isValid = true, Name = addr.User });
                }
                _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR005));
                return new JsonResult(new { isValid = false });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false });
            }

        }
        public IActionResult ValidatePhoneNumber(string Phone, int? Id = null)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            try
            {
                bool validte = _CustomerRepository.ValidatePhoneNumber(Phone, currentUser.ComId, Id);
                if (validte)
                {
                    return new JsonResult(new { isValid = true });
                }
                _notify.Error(_localizer.GetString("ERR005").Value);
                return new JsonResult(new { isValid = false });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false });
            }

        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(int id, CustomerModelView model)
        {

            if (string.IsNullOrEmpty(model.Name))
            {
                _notify.Error(_localizer.GetString("NameCompanyNotnull").Value);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }
            var currentUser = User.Identity.GetUserClaimLogin();
            bool validte = _CustomerRepository.ValidateEmail(model.Email, currentUser.ComId);
            if (!validte)
            {
                _notify.Error(_localizer.GetString("ERR005").Value);
                return new JsonResult(new { isValid = false });
            }

            if (!ModelState.IsValid)
            {
                foreach (var modelStateKey in ViewData.ModelState.Keys)
                {
                    var modelStateVal = ViewData.ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        var key = modelStateKey;
                        var errorMessage = error.ErrorMessage;
                        var exception = error.Exception;
                        // You may log the errors if you want
                        _notify.Error(errorMessage);
                    }
                }
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

            try
            {
                if (model.Id == 0)
                {
                    if (model.ImageUpload != null)
                    {
                        model.Image = _fileHelper.UploadedFile(model.ImageUpload, model.PhoneNumber, FolderUploadConstants.Customer);

                    }
                    if (model.LogoUpload != null)
                    {
                        model.Logo = _fileHelper.UploadedFile(model.LogoUpload, model.PhoneNumber, FolderUploadConstants.Customer);
                    }
                    var mapdata = _mapper.Map<Customer>(model);
                    mapdata.Status = (int)CustomerAccountStatus.NoConfirm;
                    var user = await _userRepository.RegisterAsync(mapdata);
                    //await _userManager.SignIn(this.HttpContext, user, true);
                    //_notify.Success(_localizer.GetString("RegisterOk").Value);
                    return new JsonResult(new { isValid = true, html = string.Empty });
                    //return LocalRedirect("~/Home/Index");
                }
                return new JsonResult(new { isValid = true, html = string.Empty });
                // return LocalRedirect("~/Home/Index");
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

        }
        public async Task<IActionResult> LogoutAsync()
        {
            await _userManager.SignOut(this.HttpContext);
            await _signInManager.SignOutAsync();
            _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS002));
            return RedirectPermanent("~/Home/Index");
        }
    }
}
