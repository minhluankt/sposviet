using Application.Constants;
using Application.Enums;
using Application.Features.CategorysPost.Query;
using Application.Features.CategorysProduct.Query;
using Application.Features.Comments.Commands;
using Application.Features.CompanyInfo.Query;
using Application.Features.PaymentMethods.Query;
using Application.Features.Products.Query;
using Application.Features.Specification.Query;
using Application.Features.SupplierEInvoices.Query;
using Application.Features.Units.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using DeviceDetectorNET;
using DeviceDetectorNET.Cache;
using Domain.Entities;
using Hangfire.MemoryStorage.Database;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Model;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Area("API")]
    public class HandlingController : BaseController<HandlingController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IWardRepository _repositoryWard;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ISuppliersRepository _suppliersRepository;
        private readonly IDistrictRepository _repositoryDistrict;
        private readonly IHandlingRepository _repositoryUploadImgProduct;
        public HandlingController(IHandlingRepository repositoryUploadImgProduct,
            ISuppliersRepository suppliersRepository, IPaymentMethodRepository paymentMethodRepository,
            UserManager<ApplicationUser> userManager,
           IWardRepository repositoryWard, IUserRepository userRepository
            , IDistrictRepository repositoryDistrict)
        {
            _suppliersRepository = suppliersRepository;
            _userManager = userManager;
            _paymentMethodRepository = paymentMethodRepository;
            _userRepository = userRepository;
            _repositoryWard = repositoryWard;
            _repositoryDistrict = repositoryDistrict;
            _repositoryUploadImgProduct = repositoryUploadImgProduct;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CheckExpiredAsync()
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            if (currentUser==null)
            {
                return Content("[]");

            }
            var data = await _mediator.Send(new GetByIdCompanyInfoQuery() { Id = currentUser.ComId });
            if (data.Succeeded)
            {
               
                if (data.Data.DateExpiration <= DateTime.Now.Date)
                {
                    var jsondata = new
                    {
                        isExpired = true,
                        numdate = 0,
                        todate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                        datesExpired = data.Data.DateExpiration.Value.ToString("dd/MM/yyyy"),
                    };
                    var json = JsonConvert.SerializeObject(jsondata);
                    return Content(json);
                }
                else
                {
                    TimeSpan span = data.Data.DateExpiration.Value.Subtract(DateTime.Now.Date);
                    int datenumber = span.Days;
                    if (datenumber<=7)
                    {
                        var jsondata = new
                        {
                            todate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            isExpired = true,
                            numdate = datenumber,
                            datesExpired = data.Data.DateExpiration.Value.ToString("dd/MM/yyyy"),
                        };
                        var json = JsonConvert.SerializeObject(jsondata);
                        return Content(json);
                    }

                }
            }
            var jsondatas = new
            {
                numdate = 0,
                todate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                isExpired = false,
                datesExpired = data.Data.DateExpiration.Value.ToString("dd/MM/yyyy"),
            };
            var jsonkq = JsonConvert.SerializeObject(jsondatas);
            return Content(jsonkq);
        }

        public async Task<IActionResult> GetAllUnitAsync(int? idselectd)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var getPrices = await _mediator.Send(new GetAllUnitQuery(currentUser.ComId));
            if (getPrices.Succeeded)
            {
                  getPrices.Data.Insert(0, new Unit() { Id = 0, Name = "", Code = "" });
                var allUsersExceptCurrentUser = from d in getPrices.Data select new { Code = d.Code, id = d.Id != 0 ? d.Id.ToString() : "", text = d.Name,selected = idselectd == d.Id };
                var json = allUsersExceptCurrentUser.ToArray();
                var data = JsonConvert.SerializeObject(allUsersExceptCurrentUser);
                return Content(data);
            }
            return Content("[]");
        }
        public async Task<IActionResult> GetAllPaymentMethodAsync(int? idselectd)
        {
            var currentUser = User.Identity.GetUserClaimLogin();
            var getpayment = await _mediator.Send(new GetAllPaymentMethodQuery() { Comid= currentUser.ComId });
        
            if (getpayment.Succeeded)
            {
                var lstpayment = getpayment.Data.ToList();
                lstpayment.Insert(0, new PaymentMethod() { Id = 0, Name = "", Code = "" });
                var allUsersExceptCurrentUser = from d in lstpayment select new { Code = d.Code, id = d.Id != 0 ? d.Id.ToString() : "", text = d.Name, selected = idselectd == d.Id };
                var json = allUsersExceptCurrentUser.ToArray();
                var data = JsonConvert.SerializeObject(allUsersExceptCurrentUser);
                return Content(data);
            }
            return Content("[]");
        }
        public async Task<IActionResult> GetAllPriceAsync(int? idselectd)
        {
            var getPrices = await _mediator.Send(new GetByCodeSpecificationsQuery() { Code = ParametersProduct.MUCGIA });
            if (getPrices.Succeeded)
            {
                //  getPrices.Data.Insert(0, new Specifications() { Id = 0, Name = "box", Code = "box" });
                var allUsersExceptCurrentUser = from d in getPrices.Data select new { Code = d.Code, id = d.Id != 0 ? d.Id.ToString() : "", text = d.Name, slug = d.Slug, selected = idselectd == d.Id };
                var json = allUsersExceptCurrentUser.ToArray();
                var data = JsonConvert.SerializeObject(allUsersExceptCurrentUser);
                return new JsonResult(new { isValid = true, data = data });
            }
            return new JsonResult(new { isValid = false, html = string.Empty });
        }
        public async Task<IActionResult> GetAllProductInCategoryAsync(int idCategory)
        {
            List<int> listidcategory = new List<int>();
            listidcategory.Add(idCategory);
            var listproduct = await _mediator.Send(new GetProductByCategoryQuery() { lstIdcategory = listidcategory.ToArray() });
            if (listproduct.Succeeded)
            {
                if (listproduct.Data == null || listproduct.Data.Count() == 0)
                {
                    return new JsonResult(new { isValid = false, data = "[]" });
                }
                var allUsersExceptCurrentUser = from d in listproduct.Data
                                                select new
                                                {
                                                    Code = d.Code,
                                                    Id = d.Id,
                                                    Name = d.Name,
                                                    Img = d.Img,
                                                    Slug = d.Slug,
                                                    Price = d.Price.ToString("N0"),
                                                    isPromotion = d.isPromotion,
                                                    isRunPromotion = d.isRunPromotion,
                                                    Discount = d.Discount,
                                                    DiscountRun = d.DiscountRun,
                                                    PriceDiscount = (d.PriceDiscount != 0 ? d.PriceDiscount.ToString("N0") : (d.Price - ((decimal)d.Discount / 100 * d.Price)).ToString("N0")),
                                                    PriceDiscountRun = (d.PriceDiscountRun != 0 ? d.PriceDiscountRun.ToString("N0") : (d.Price - ((decimal)d.DiscountRun / 100 * d.Price)).ToString("N0")),
                                                };
                var json = allUsersExceptCurrentUser.ToArray();
                var data = JsonConvert.SerializeObject(allUsersExceptCurrentUser);
                return new JsonResult(new { isValid = true, data = data });
            }
            return new JsonResult(new { isValid = false, html = string.Empty });
        }

        public IActionResult GetAllSupller(int? idselectd)
        {
            var user = User.Identity.GetUserClaimLogin();
            var allUsersExceptCurrentUser = from d in _suppliersRepository.GetAll(user.ComId) select new { id = d.Id, text = d.Name, selected = d.Id == idselectd };
            var json = allUsersExceptCurrentUser.ToArray();
            var data = JsonConvert.SerializeObject(allUsersExceptCurrentUser);
            return Content(data);
        }
        public async Task<IActionResult> GetAllSupplierEInvoiceAsync(int? idselectd)
        {
            var user = User.Identity.GetUserClaimLogin();

            var response = await _mediator.Send(new GetAllSupplierEInvoiceQuery() { Comid= user.ComId });
            if (response.Succeeded)
            {
                //response.Data.Insert(0, new CategoryProduct() { Id = 0, Name = "" });
                var allUsersExceptCurrentUser = from d in response.Data select new { id = d.Id, text = d.CompanyName,type = d.TypeSeri, selected = (idselectd!=null?d.Id == idselectd:d.Selected) };
                var json = allUsersExceptCurrentUser.ToArray();
                var data = JsonConvert.SerializeObject(allUsersExceptCurrentUser);
                return Content(data);
            }
            return Content("[]");
        }

        public async Task<IActionResult> GetAllCategoryProductAsync(int? idselectd, int? iddata, bool iscreateCategory = false,bool IsPos = false)
        {
            
            int? comid = null;
            if (IsPos)
            {
               // var user = await _userManager.GetUserAsync(User);
                // var currentUser = await _userManager.GetUserAsync(Context.User);
                var user = User.Identity.GetUserClaimLogin();
                comid = user.ComId;
            }
            var getid = await _mediator.Send(new GetAllCategoryProductQuery() { ComId= comid });
            if (getid.Succeeded)
            {
                getid.Data.Insert(0, new CategoryProduct() { Id = 0, Name = "" });
                List<CategoryProduct> lst = new List<CategoryProduct>();
                int x = 0;
                var litslevel0 = getid.Data.Where(m => m.IdLevel == x).OrderByDescending(m => m.Sort);
                foreach (var item in litslevel0)
                {
                    lst.Add(item);
                    var lst1 = getid.Data.Where(m => m.IdPattern == item.Id).OrderByDescending(m => m.Sort);
                    foreach (var item1 in lst1)
                    {
                        lst.Add(item1);
                        var lst2 = getid.Data.Where(m => m.IdPattern == item1.Id).OrderByDescending(m => m.Sort);
                        foreach (var item2 in lst2)
                        {
                            lst.Add(item2);
                            // var lst2 = getid.Data.Where(m => m.IdPattern == item1.Id);
                        }
                    }
                }


                if (iscreateCategory)
                {
                    lst = lst.Where(x => x.IdLevel < 2).ToList();
                }
                List<CategoryProduct> lstremove = new List<CategoryProduct>();
                if (iddata != null) // nếu có iddata thì tìm và loại trừ
                {
                    var getiddata = getid.Data.Where(m => m.Id == iddata).SingleOrDefault();
                    if (getiddata != null)
                    {
                        lstremove.Add(getiddata);
                        if (getiddata.IdLevel == 0)
                        {
                            var lst1 = getid.Data.Where(m => m.IdPattern == iddata).ToList();
                            lstremove.AddRange(lst1);
                            foreach (var item in lst1)
                            {
                                var lst2 = getid.Data.Where(m => m.IdPattern == item.Id).ToList();
                                lstremove.AddRange(lst2);
                            }
                        }
                        else if (getiddata.IdLevel == 1)
                        {
                            var lst1 = getid.Data.Where(m => m.IdPattern == iddata).ToList();
                            lstremove.AddRange(lst1);
                        }
                        else if (getiddata.IdLevel == 2)
                        {
                            var lst1 = getid.Data.Where(m => m.IdLevel == getiddata.IdLevel).ToList();
                            lstremove.AddRange(lst1);
                        }
                    }

                }



                var allUsersExceptCurrentUser = (from d in lst where !lstremove.Select(x => x.Id).ToArray().Contains(d.Id) select d).ToList();
                allUsersExceptCurrentUser.Insert(0, new CategoryProduct() { Id = 0, Name = "", Code = "" });
                var getall = allUsersExceptCurrentUser.Select(d => new { id = d.Id != 0 ? d.Id.ToString() : "", idPattern = d.IdPattern, text = d.Name, slug = d.Slug, Level = d.IdLevel, isParent = d.IdLevel == 0, selected = d.Id == idselectd });
              
                var json = getall.ToArray();
                var data = JsonConvert.SerializeObject(json);
                return Content(data);
            }
            return Content("[]");
        }

        public async Task<IActionResult> GetAllCategoryPostAsync(int? idselectd)
        {
            var getid = await _mediator.Send(new GetAllCategoryPostQuery());
            if (getid.Succeeded)
            {
                getid.Data.Insert(0, new CategoryPost() { Id = 0, Name = "" });
                List<CategoryPost> lstnew = new List<CategoryPost>();//lst new
                List<CategoryPost> lst = getid.Data.Where(m => m.IdLevel == 0).OrderByDescending(x => x.Sort).ToList();//list move
                if (lst.Count() > 0)
                {
                    do
                    {
                        CategoryPost itemFirst = lst.LastOrDefault();
                        lstnew.Add(itemFirst);
                        lst.Remove(itemFirst);
                        var getchillitemFirst = getid.Data.Where(m => m.IdPattern == itemFirst.Id).OrderByDescending(x => x.Sort);
                        if (getchillitemFirst.Count() > 0)
                        {
                            lst.AddRange(getchillitemFirst);
                        }

                    } while (lst.Count() > 0);
                }


                //foreach (var item in getid.Data.Where(m => m.IdLevel == 0))
                //{
                //    lst.Add(item);
                //    lst.AddRange(getid.Data.Where(m => m.IdPattern == item.Id));
                //}

                var allUsersExceptCurrentUser = from d in lstnew select new { id = d.Id != 0 ? d.Id.ToString() : "", idPattern = d.IdPattern, text = d.Name, slug = d.Slug, isParent = d.IdLevel == 0, Level = d.IdLevel, selected = d.Id == idselectd };
                var json = allUsersExceptCurrentUser.ToArray();
                var data = JsonConvert.SerializeObject(allUsersExceptCurrentUser);
                return Content(data);
            }
            return Content("[]");
        }


        public IActionResult GetDataDistrictByIdCity(int? idcity, int? idselectd)
        {
            if (idcity != null && idcity > 0)
            {
                var lst = _repositoryDistrict.GetListDistrict(idcity.Value).OrderBy(x => x.Name);
                var allUsersExceptCurrentUser = from d in lst select new { id = d.Id, text = d.Name, selected = d.Id == idselectd };
                var json = allUsersExceptCurrentUser.ToArray();
                var data = JsonConvert.SerializeObject(allUsersExceptCurrentUser);
                return Content(data);
            }

            return Content("[]");
        }
        public async Task<IActionResult> GetDataJsonStypeProductAsync(StyleProductEnum code = StyleProductEnum.STYLESANPHAM, int? idselectd = null)
        {

            // ParametersProduct.STYLESANPHAM
            //var getcode = LibraryCommon.GetEnumMemberValue<StyleProductEnum>(code);
            var getPrices = await _mediator.Send(new GetByCodeSpecificationsQuery() { Code = ParametersProduct.STYLESANPHAM });
            if (getPrices.Succeeded)
            {
                //  getPrices.Data.Insert(0, new Specifications() { Id = 0, Name = "box", Code = "box" });
                var allUsersExceptCurrentUser = from d in getPrices.Data select new { Code = d.Code, id = d.Id != 0 ? d.Id.ToString() : "", text = d.Name, slug = d.Slug, selected = idselectd == d.Id };

                var json = allUsersExceptCurrentUser.ToArray();
                var data = JsonConvert.SerializeObject(allUsersExceptCurrentUser);
                return Content(data);
            }
            return Content("[]");
        }


        public IActionResult GetDataWartByIdDistrict(int? idDistrict, int? idselectd)
        {
            if (idDistrict != null && idDistrict > 0)
            {
                var lst = _repositoryWard.GetListWardByDistrict(idDistrict.Value).OrderBy(x => x.Name);
                var allUsersExceptCurrentUser = from d in lst select new { id = d.Id, text = d.Name, selected = d.Id == idselectd };
                var json = allUsersExceptCurrentUser.ToArray();
                var data = JsonConvert.SerializeObject(allUsersExceptCurrentUser);
                return Content(data);
            }

            return Content("[]");
        }



        [HttpPost]
        public async Task<IActionResult> DeleteImgAsync(int id, int idProduct)
        {
            var check = await _repositoryUploadImgProduct.DeleteImgProductAsync(id, idProduct);
            if (check)
            {
                _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS007));
                return new JsonResult(new { isValid = true, html = string.Empty });

            }
            _notify.Error(GeneralMess.ConvertStatusToString(HeperConstantss.ERR007));
            return new JsonResult(new { isValid = false, html = string.Empty });
        }
        private string GetDeviceBrowser(out int type, out string Browser, out string OS)
        {
            var detector = new DeviceDetector(Request.Headers["User-Agent"].ToString());
            detector.SetCache(new DictionaryCache());
            detector.Parse();
            string name = detector.GetDeviceName();
            if (detector.IsMobile())
            {
                type = (int)DeviceType.IsMobile;
            }
            else if (detector.IsTablet())
            {
                type = (int)DeviceType.IsTablet;
            }
            else if (detector.IsDesktop())
            {
                type = (int)DeviceType.IsDesktop;
            }
            else
            {
                // type = DeviceDetectorNET.Parser.Device.DeviceType.
                type = 0;
            }
            Browser = $"Name:{detector.GetBrowserClient().Match.Name}/Version:{detector.GetBrowserClient().Match.Version}";
            OS = $"Name: {detector.GetOs().Match.Name}/Version:{detector.GetOs().Match.Version}/Platform:{detector.GetOs().Match.Platform}";
            return name;

        }
        // command
        [HttpPost]
        public async Task<IActionResult> AddCommentProductAsync(CommentModel model)
        {
            try
            {
                var usercom = await _userRepository.GetUserAsync(User);
                if (usercom != null)
                {
                    model.IdCustomer = usercom.Id;
                    model.CusName = usercom.Name;
                    model.CusEmail = usercom.Email;
                    model.CusPhone = usercom.PhoneNumber;
                }
                if (string.IsNullOrEmpty(model.CusName) || string.IsNullOrEmpty(model.Comment))
                {
                    _logger.LogError("Chưa nhập comment hoặc chưa nhập họ tên commnet");
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
                int typeDevice = 0;
                string Browser = string.Empty;
                string OS = string.Empty;
                model.DeviceName = GetDeviceBrowser(out typeDevice, out Browser, out OS);
                model.DeviceType = typeDevice;
                model.Browser = Browser;
                model.OS = OS;
                var createProductCommand = _mapper.Map<CreateCommentProductCommand>(model);
                var result = await _mediator.Send(createProductCommand);
                if (result.Succeeded)
                {
                    // _notify.Success(GeneralMess.ConvertStatusToString(HeperConstantss.SUS014));
                    return new JsonResult(new { isValid = true, html = string.Empty });
                }
                else
                {
                    _notify.Error(result.Message);
                    return new JsonResult(new { isValid = false, html = string.Empty });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.ToString());
                _notify.Error(e.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });

            }


        }
    }
}
