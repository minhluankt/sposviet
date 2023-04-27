using Application.CacheKeys;
using Application.Constants;
using Application.DTOs.Mail;
using Application.Enums;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using Application.Interfaces.Shared;
using Application.Providers;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using Hangfire.Logging;
using HelperLibrary;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemVariable;

namespace Infrastructure.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IManagerIdCustomerRepository _managerIdCustomerRepository;
        private readonly IParametersEmailRepository _parametersEmailRepository;
        private readonly IFormFileHelperRepository _iFormFileHelperRepository;
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IRepositoryCacheAsync<CompanyAdminInfo> _infocompanyrepositorycache;
        private readonly IMailService _mailservice;
        private readonly ILogger<CustomerRepository> _logger;
        private IFormFileHelperRepository _fileform;
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IRepositoryAsync<Customer> _repository;

        public CustomerRepository(IRepositoryAsync<Customer> repository,
           IFormFileHelperRepository iFormFileHelperRepository, IManagerIdCustomerRepository managerIdCustomerRepository,
           ILogger<CustomerRepository> logger, IParametersEmailRepository parametersEmailRepository,
            IUnitOfWork unitOfWork, IRepositoryAsync<Customer> Repository, IMailService mailservice,
            IRepositoryCacheAsync<CompanyAdminInfo> infocompanyrepositorycache, IOptions<CryptoEngine.Secrets> config, 
        IFormFileHelperRepository fileform)
        {
            _managerIdCustomerRepository = managerIdCustomerRepository;
            _parametersEmailRepository = parametersEmailRepository;
            _iFormFileHelperRepository = iFormFileHelperRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _repository = repository;
            _mailservice = mailservice; 
            _config = config;   
            _fileform = fileform;
            _infocompanyrepositorycache = infocompanyrepositorycache;
        }

        public IEnumerable<Customer> GetAllIEnumerable(Customer model)
        {
            throw new NotImplementedException();
        }

        public List<Customer> GetAllIQueryableDatatable(Customer model, string sortColumn, string sortColumnDirection, int pageSize, int skip, out int recordsTotal)
        {
            var datalist = _repository.GetMulti(predicate: m => m.Id > 0);
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                datalist = datalist.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            if (!string.IsNullOrEmpty(model.Name))
            {
                datalist = datalist.Where(m => m.Name.ToLower().Contains(model.Name.ToLower()));
            }

            recordsTotal = datalist.Count();
            if (recordsTotal == 0)
            {
                return new List<Customer>();
            }
            var data = datalist.Skip(skip).Take(pageSize).ToList();

            return data;
        }

        public bool ValidateEmail(string Email, int ComId,int? Id)
        {
            try
            {
                if (Id != null && Id > 0)
                {
                    return !_repository.Entities.AsNoTracking().Where(m => m.Email == Email && m.Id != Id && m.Comid == ComId).Any();
                }
                return !_repository.Entities.AsNoTracking().Where(m => m.Email == Email && m.Comid == ComId).Any();
            }
            catch
            {
                return false;
            }
        } 
        public async Task<Customer> ConfirmEmailAccount(string Email, int Id)
        {
            try
            {
                var get = _repository.Entities.Where(m => m.EmailConfirm == Email && m.Id == Id).SingleOrDefault();
                if (get!=null)
                {
                    get.Status = (int)CustomerAccountStatus.Confirm;
                    get.Email = Email;
                    get.EmailConfirm = Email;
                    get.isEmailConfirm = true;
                    get.LoginLast = DateTime.Now;
                    if (!string.IsNullOrEmpty(get.LoginProvider))
                    {
                        if (get.LoginProvider.ToLower() == CommonTypeLoginExt.Facebook.ToLower() && !LibraryCommon.IsValidEmail(get.UserName) && get.UserName!= get.ProviderKey)
                        {
                            get.UserName = Email;
                        }
                    }
                    _repository.Update(get);
                    await _unitOfWork.SaveChangesAsync();
                    return get;
                }
                return null;
            }
            catch(Exception e)
            {
                return null;
            }
        } 
        public async Task<string> ComplementaryEmailAsync(string Email, int ComId, int Id)
        {
            try
            {
                var get = _repository.Entities.Where(m =>m.Id == Id).SingleOrDefault();
                if (get!=null)
                {
                    var checkemailserrver = ValidateEmail(Email, ComId,Id);
                    if (!checkemailserrver)
                    {
                        return HeperConstantss.ERR005;
                    }
                    if (Email== get.EmailConfirm && get.isEmailConfirm)
                    {
                        return HeperConstantss.ERR004;
                    }
                    get.EmailConfirm = Email;
                    get.isEmailConfirm = false;
                    _repository.Update(get);
                    await _unitOfWork.SaveChangesAsync();
                    //string title = FormatTitleMail.TitleConfirmEmailAccount;
                    await SendMailConfirmAsync(get);
                    return HeperConstantss.SUS008;
                }
                return HeperConstantss.ERR012;
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        public async Task SendMailConfirmAsync(Customer model,string titlemail = "")
        {
            if (string.IsNullOrEmpty(model.EmailConfirm))
            {
                _logger.LogError("SendMailConfirmAsync thất bại do không có địa chỉ email: "+JsonConvert.SerializeObject(model));
            }
            else
            {
                var getkey = _parametersEmailRepository.GetBykey(KeyTitleMail.xac_nhan_tai_khoan);
                // send mail
                var infocom = await _infocompanyrepositorycache.GetFirstAsync(CompanyAdminInfoCacheKeys.ListKey);

               
                //string title = "";
                var values = $"id={ model.Id}&email={(!string.IsNullOrEmpty(model.EmailConfirm) ? model.EmailConfirm : model.Email)}";
                var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                string url = $"{infocom.Website}/Account/ConfirmEmail?secret={secret}";
                EmailParametersModel _mailmodel = new EmailParametersModel();

                _mailmodel.website = infocom.Website;
                _mailmodel.hotline = infocom.Hotline;
                _mailmodel.Url = url;

                string content = _mailservice.GetTemplate(_mailmodel, getkey.Value);
                string titleEmail = _mailservice.GetTemplateTitle(_mailmodel, getkey.Title);

                //
                MailRequest mailRequest = new MailRequest();
                mailRequest.Title = titleEmail;
                mailRequest.EmailTo = (!string.IsNullOrEmpty(model.EmailConfirm) ? model.EmailConfirm : model.Email);
                mailRequest.Content = content;
                mailRequest.Subject = titleEmail;
                mailRequest.FullNameUserSend = "System";
                var task = Task.Run(() =>
                {
                    _mailservice.SendEmailOne(mailRequest, CreateScope: true);
                });
            }
           
        }
        public bool ValidatePhoneNumber(string PhoneNumber, int ComId, int? Id)
        {
            try
            {
                if (Id != null && Id > 0)
                {
                    return !_repository.Entities.AsNoTracking().Where(m => m.PhoneNumber == PhoneNumber && m.Id != Id && m.Comid == ComId).Any();
                }
                return !_repository.Entities.AsNoTracking().Where(m => m.PhoneNumber == PhoneNumber && m.Comid == ComId).Any();
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> ValidateCode(string Code, int? ComId)
        {
            return await _repository.Entities.AsNoTracking().Where(m => m.Code == Code && m.Comid == ComId).AnyAsync();
        }
        public async Task<bool> ValidatePhoneNumberAndEmail(string PhoneNumber,string Email,int ComId, int? Id)
        {
            try
            {
                var getdata = _repository.Entities.AsNoTracking().Where(m => m.Comid == ComId);
                if (!string.IsNullOrEmpty(PhoneNumber))
                {
                    getdata = getdata.Where(x => x.PhoneNumber == PhoneNumber);
                    if (await getdata.CountAsync()>0)
                    {
                        return true;
                    }
                }
                if (!string.IsNullOrEmpty(Email))
                {
                    getdata = getdata.Where(x => x.Email == Email);
                    if (await getdata.CountAsync() > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public string Extension(string image)
        {
            var extension = "";
            switch (image.ToUpper())
            {
                case "IVBOR":
                    return extension = ".png";
                case "/9J/4":
                    return extension = ".jpg";
            }
            return extension;
        }
        public bool  UpdateAvatar(string base64, int id, out string filename)
        {
            base64 = base64.Replace("data:image/png;base64,","");
            var getuser =  _repository.GetById(id);
            if (getuser!=null)
            {
                string imgold = getuser.Image;
                var extension = Extension(base64.Substring(0, 5));
                var imageName = string.Format(@"{0}", Guid.NewGuid()) + extension;
                string name = _iFormFileHelperRepository.UploadedFile(base64, imageName, FolderUploadConstants.Customer);
                getuser.Image = name;
                filename = name;
                _repository.Update(getuser);
                _unitOfWork.SaveChanges();
                if (!string.IsNullOrEmpty(imgold))
                {
                    _iFormFileHelperRepository.DeleteFile(imgold, FolderUploadConstants.Customer);
                }
                return true;
            }
            filename = string.Empty;
            return false;
        }

        public async Task<bool> UpdatePhoneNumber(string phone, int Id)
        {
            var get = await _repository.GetByIdAsync(Id);
            if (get==null || string.IsNullOrEmpty(phone))
            {
                return false;
            }
            get.PhoneNumber = phone;

            await _repository.UpdateAsync(get);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IResult<Customer>> Crreate(Customer model)
        {
            try
            {
                var idCus = await _managerIdCustomerRepository.UpdateIdAsync(model.Comid.Value);
                if (string.IsNullOrEmpty(model.Code))
                {
                    model.Code = $"KH{idCus.ToString("0000000")}";
                }
                if (!string.IsNullOrEmpty(model.Password))
                {
                    model.Password = model.Code;
                }
                var salt = Hasher.GenerateSalt();
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var hashedPassword = Hasher.GenerateHash(model.Password, salt);
                    model.Salt = salt;
                    model.Password = hashedPassword;
                }
                await _repository.AddAsync(model);
                await _unitOfWork.SaveChangesAsync(CancellationToken.None);
                return await Result<Customer>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateCustomerCommand Create " + model.PhoneNumber + "\n" + ex.ToString());
                return await Result<Customer>.FailAsync(ex.Message);
            }
        }

     
    }
}
