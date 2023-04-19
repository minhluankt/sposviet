using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

using System.Threading;
using Application.CacheKeys;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Application.DTOs.Mail;
using HelperLibrary;
using SystemVariable;
using Application.Interfaces.Shared;
using Application.Interfaces.CacheRepositories;
using Microsoft.Extensions.Options;
using Application.Enums;
using Application.Constants;
using Application.Hepers;
using Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;

namespace Infrastructure.Infrastructure.Repositories
{
  

    public class UserRepository : IUserRepository
    {
        private bool _disposed;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private IOptions<CryptoEngine.Secrets> _config;
        //private readonly IRepositoryCacheAsync<CompanyAdminInfo> _infocompanyrepositorycache;
        //private readonly IMailService _mailservice;
        //private IFormFileHelperRepository _fileform;
        private IUserManager _userManager;
        private ICustomerRepository _customerrepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IRepositoryAsync<Customer> _Repository;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public UserRepository(IRepositoryAsync<Customer> Repository,
            IUserManager userManager,
            IHttpContextAccessor httpContextAccessor,
            IMailService mailservice, ICustomerRepository customerrepository,
            //IRepositoryCacheAsync<CompanyAdminInfo> infocompanyrepositorycache, 
            //IOptions<CryptoEngine.Secrets> config,
            // IServiceScopeFactory serviceScopeFactory,
            // IFormFileHelperRepository fileform,
            IDistributedCache distributedCache, IUnitOfWork unitOfWork)
        {
            // _infocompanyrepositorycache = infocompanyrepositorycache;
            //_fileform = fileform;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _customerrepository = customerrepository;
            //  _mailservice = mailservice;
            _unitOfWork = unitOfWork;
            //_config = config;
            _distributedCache = distributedCache;
            _Repository = Repository;
        }
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public async Task<Customer> GetUserByIdCodeAsync(string code)
        {
            var data = await _Repository.SingleByExpressionAsync(m => m.IdCodeGuid.ToString() == code);
            return data;
        } 
        public async Task<Customer> GetUserByUserNameAsync(string code)
        {
            var data = await _Repository.SingleByExpressionAsync(m => m.UserName == code);
            return data;
        }
        public async Task<Customer> GetUserAsync(ClaimsPrincipal User)
        {
            Customer model = new Customer();
            ClaimsIdentity user = new ClaimsIdentity();
            if (User.Identities.Count() > 1)
            {
                user = User.Identities.Where(m => m.AuthenticationType == Application.Constants.CookieAuthenticationCustomer.AuthenticationScheme).SingleOrDefault();
            }
            else
            {
                user = new ClaimsIdentity(User.Identity);
            }
            if (user.AuthenticationType == Application.Constants.CookieAuthenticationCustomer.AuthenticationScheme)
            {
                string iduser = user.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).SingleOrDefault().Value;
                var data = await _Repository.SingleByExpressionAsync(m => m.IdCodeGuid.ToString() == iduser);
                if (data == null)
                {
                    await _userManager.SignOut();
                }
                return data;
            }
            await _userManager.SignOut();
            return null;
        }
        public async Task<Customer> GetFullUserAsync(ClaimsPrincipal User)
        {
            Customer model = new Customer();
            ClaimsIdentity user = new ClaimsIdentity();
            if (User.Identities.Count() > 1)
            {
                user = User.Identities.Where(m => m.AuthenticationType == Application.Constants.CookieAuthenticationCustomer.AuthenticationScheme).SingleOrDefault();
            }
            else
            {
                user = new ClaimsIdentity(User.Identity);
            }
            if (user.AuthenticationType == Application.Constants.CookieAuthenticationCustomer.AuthenticationScheme)
            {
                string iduser = user.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).SingleOrDefault().Value;
                var data = await _Repository.GetByIdAsync(m => m.IdCodeGuid.ToString() == iduser,x=>x.Include(x=>x.City).Include(x=>x.District).Include(x=>x.Ward));
                if (data == null)
                {
                    await _userManager.SignOut();
                }
                return data;
            }
            await _userManager.SignOut();
            return null;
        }

        public async Task<bool> UpdatePasswordAsync(string pass, int Id)
        {
            var data = await _Repository.GetByIdAsync(Id);
            if (data != null)
            {
                var salt = Hasher.GenerateSalt();
                var hashedPassword = Hasher.GenerateHash(pass, salt);
                data.Password = hashedPassword;
                data.Salt = salt;
                await _Repository.UpdateAsync(data);
                await _unitOfWork.SaveChangesAsync(new CancellationToken());
                return true;
            }
            return false;
        }
        public async Task<bool> CheckPasswordCustomerAsync(string pass, int Id)
        {
            var data = await _Repository.GetByIdAsync(Id);
            if (data != null)
            {
                if (data.Password == Hasher.GenerateHash(pass, data.Salt))
                {
                    return true;
                }
            }
            return false;
        }
        public ResponseModel<CookieCustomerUser> LoginProvider(string LoginProvider, string ProviderKey, string email = "")
        {
            ResponseModel<CookieCustomerUser> responseModel = new ResponseModel<CookieCustomerUser>();
            var getcheck = _Repository.Entities.Where(m => m.LoginProvider == LoginProvider && m.ProviderKey == ProviderKey).SingleOrDefault();
            if (getcheck != null)
            {
                responseModel.isSuccess = true;
                getcheck.LoginLast = DateTime.Now;
                _Repository.Update(getcheck);
                _unitOfWork.SaveChangesAsync();
                var customer = new CookieCustomerUser
                {
                    UserId = getcheck.IdCodeGuid,
                    EmailAddress = getcheck.Email,
                    Name = getcheck.Name,
                    Username = getcheck.UserName,
                    Created = getcheck.CreatedOn,
                    Image = getcheck.Image
                };
                responseModel.Data = customer;
                return responseModel;
            }
            // nếu  có email rồi thì update vào đó luôn
            if (!string.IsNullOrEmpty(email))
            {
                var checkmail = _Repository.Entities.Where(m => m.Email.ToLower() == email.ToLower()).SingleOrDefault();
                if (checkmail != null)
                {
                    responseModel.isSuccess = true;
                    checkmail.LoginLast = DateTime.Now;
                    if (checkmail.LoginProvider!=null&& !checkmail.LoginProvider.Contains(LoginProvider))
                    {
                        if (string.IsNullOrEmpty(checkmail.LoginProvider))
                        {
                            checkmail.LoginProvider = LoginProvider;
                        }
                        else
                        {
                            checkmail.LoginProvider = checkmail.LoginProvider + ";" + LoginProvider;
                        }
                    }

                    if (checkmail.ProviderKey != null &&  !checkmail.ProviderKey.Contains(ProviderKey))
                    {
                        if (string.IsNullOrEmpty(checkmail.ProviderKey))
                        {
                            checkmail.ProviderKey = ProviderKey;
                        }
                        else
                        {
                            checkmail.ProviderKey = checkmail.ProviderKey + ";" + ProviderKey;
                        }
                    }

                    _Repository.Update(checkmail);
                    _unitOfWork.SaveChangesAsync();

                    var customer = new CookieCustomerUser
                    {
                        UserId = checkmail.IdCodeGuid,
                        EmailAddress = checkmail.Email,
                        Name = checkmail.Name,
                        Username = checkmail.UserName,
                        Created = checkmail.CreatedOn,
                        Image = checkmail.Image
                    };
                    responseModel.Data = customer;
                    return responseModel;
                }
            }


            responseModel.isSuccess = false;
            responseModel.Message = HeperConstantss.ERR026;
            return responseModel;

        }
        public async Task<ResponseModel<CookieCustomerUser>> Validate(LoginCustomerViewModel model)
        {
            ResponseModel<CookieCustomerUser> responseModel = new ResponseModel<CookieCustomerUser>();
            Customer emailRecords = null;
            if (IsValidEmail(model.UserName))
            {

                emailRecords = await _Repository.Entities.Where(x => x.Email == model.UserName || x.UserName == model.UserName).SingleOrDefaultAsync();//k check Emailconf do chưa chắc khách xác nhận
                if (emailRecords == null)
                {
                    responseModel.isSuccess = false;
                    responseModel.Message = HeperConstantss.ERR027;
                    return responseModel;
                }
                if (emailRecords.EmailConfirm == model.UserName && !emailRecords.isEmailConfirm)
                {
                    responseModel.isSuccess = false;
                    responseModel.Message = HeperConstantss.ERR024;
                    return responseModel;
                }
            }
            else
            {
                emailRecords = await  _Repository.Entities.Where(x => x.UserName == model.UserName).SingleOrDefaultAsync();
            }


            if (emailRecords == null)
            {
                responseModel.isSuccess = false;
                responseModel.Message = HeperConstantss.ERR027;
                return responseModel;
            }
            if (emailRecords.Status == (int)CustomerAccountStatus.NoConfirm)
            {
                responseModel.isSuccess = false;
                responseModel.Message = HeperConstantss.ERR024;
            }
            else if (emailRecords.Status == (int)CustomerAccountStatus.Lock)
            {
                responseModel.isSuccess = false;
                responseModel.Message = HeperConstantss.ERR025;
            }
            else if (string.IsNullOrEmpty(emailRecords.Salt))
            {
                responseModel.isSuccess = false;
                responseModel.Message = HeperConstantss.ERR026;
                return responseModel;
            }

            if (emailRecords.Password == Hasher.GenerateHash(model.Password, emailRecords.Salt))
            {
                responseModel.isSuccess = true;
                emailRecords.LoginLast = DateTime.Now;
                await _Repository.UpdateAsync(emailRecords);
                await _unitOfWork.SaveChangesAsync();
                var customer = new CookieCustomerUser
                {
                    UserId = emailRecords.IdCodeGuid,
                    EmailAddress = emailRecords.Email,
                    Name = emailRecords.Name,
                    Image = emailRecords.Image,
                    Username = emailRecords.UserName,
                    Created = emailRecords.CreatedOn
                };
                responseModel.Data = customer;
                return responseModel;
            }
            else
            {
                responseModel.isSuccess = false;
                responseModel.Message = HeperConstantss.ERR026;
                return responseModel;
            }
        }

        public async Task<CookieCustomerUser> RegisterAsync(Customer model)
        {

            var salt = Hasher.GenerateSalt();
            if (!string.IsNullOrEmpty(model.Password))
            {
                var hashedPassword = Hasher.GenerateHash(model.Password, salt);
                model.Salt = salt;
                model.Password = hashedPassword;
            }

            //model.Status = (int)CustomerAccountStatus.NoConfirm;
            model.IdCodeGuid = Guid.NewGuid();
            model.EmailConfirm = model.Email;

            await _Repository.AddAsync(model);
            await _unitOfWork.SaveChangesAsync(new CancellationToken());
            //sendmail
            if (string.IsNullOrEmpty(model.LoginProvider) && !string.IsNullOrEmpty(model.Email))
            {
                await _customerrepository.SendMailConfirmAsync(model);
                // await SendMailConfirmAsync(model);
            }
            return new CookieCustomerUser
            {
                UserId = model.IdCodeGuid,
                EmailAddress = model.Email,
                Username = model.UserName,
                Name = model.Name,
                Created = model.CreatedOn
            };
        }


        private async Task SendMailConfirmAsync(Customer model)
        {
            // send mail
            await _customerrepository.SendMailConfirmAsync(model);
        }
  

        public async Task<IList<Claim>> GetClaimsAsync(IKey userId)
        {
            throw new InvalidOperationException();
        }
    }

}
