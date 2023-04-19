using Application.CacheKeys;
using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CompanyInfo.Commands
{

    public partial class UpdateCompanyInfoCommand : CompanyAdminInfo, IRequest<Result<int>>
    {
    }
    public class UpdateCompanyInfoHandler : IRequestHandler<UpdateCompanyInfoCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<UpdateCompanyInfoCommand> _log;
        private readonly IRepositoryAsync<CompanyAdminInfo> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateCompanyInfoHandler(IRepositoryAsync<CompanyAdminInfo> brandRepository,
            ILogger<UpdateCompanyInfoCommand> log,
            IFormFileHelperRepository fileHelper,

            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {

            _Repository = brandRepository;
            _fileHelper = fileHelper;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;

            _log = log;
        }

        public async Task<Result<int>> Handle(UpdateCompanyInfoCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // FormFileHelper _formFileHelper = new FormFileHelper(_hostingEnvironment);
                command.PhoneNumber = command.PhoneNumber?.Replace(" ", "").Trim();
                command.CusTaxCode = command.CusTaxCode?.Replace(" ", "").Trim();
                command.Email = command.Email?.Trim();

                _log.LogInformation("UpdateCompanyInfoCommand update start: " + command.CusTaxCode);
                CompanyAdminInfo brand = await _Repository.GetAllQueryable().SingleOrDefaultAsync(x => x.Id == command.Id);
                string imgold = string.Empty;
                string logoold = string.Empty;
                if (brand == null)
                {
                    _log.LogError(HeperConstantss.ERR012 + "___" + command.CusTaxCode);
                    return Result<int>.Fail(HeperConstantss.ERR012);
                }
                else
                {

                    if (brand.CusTaxCode != command.CusTaxCode && !string.IsNullOrEmpty(command.CusTaxCode))
                    {
                        int checktaxcodeandType = _Repository.Entities.Count(m => m.VFkeyCusTaxCode == command.VFkeyCusTaxCode && m.Id != command.Id);
                        if (checktaxcodeandType > 0)
                        {
                            _log.LogInformation("UpdateCompanyInfoCommand update trùng mã số thuế: " + command.CusTaxCode);
                            return Result<int>.Fail(HeperConstantss.ERR029);
                        }

                    }
                    if (brand.PhoneNumber != command.PhoneNumber && !string.IsNullOrEmpty(command.PhoneNumber))
                    {
                        int checktaxcodeandType = _Repository.Entities.Count(m => m.VFkeyPhone == command.VFkeyPhone && m.Id != command.Id);
                        if (checktaxcodeandType > 0)
                        {
                            _log.LogInformation("UpdateCompanyInfoCommand update trùng số điện thoại: " + command.PhoneNumber);
                            return Result<int>.Fail(HeperConstantss.ERR006);
                        }
                        brand.PhoneNumber = command.PhoneNumber;
                    }

                    brand.CusTaxCode = command.CusTaxCode;
                    brand.Name = command.Name;
                    brand.Website = command.Website;
                    brand.Title = command.Title;
                    brand.Address = command.Address;
                    if (!string.IsNullOrEmpty(command.Image))
                    {
                        imgold = brand.Image;
                        brand.Image = command.Image;
                    }
                    if (!string.IsNullOrEmpty(command.Logo))
                    {
                        logoold = brand.Logo;
                        brand.Logo = command.Logo;
                    }
                    brand.Url = command.Url;
                    brand.Email = command.Email;

                    brand.FaxNumber = command.FaxNumber;

                    brand.IdDichVu = command.IdDichVu;
                    brand.IdWard = command.IdWard;
                    brand.IdCity = command.IdCity;
                    brand.IdDistrict = command.IdDistrict;
                    brand.Active = command.Active;
                    brand.IdType = command.IdType;

                    if (brand.NumberDateExpiration != command.NumberDateExpiration || brand.StartDate != command.StartDate)
                    {
                        brand.StartDate = command.StartDate;
                        brand.NumberDateExpiration = command.NumberDateExpiration;
                        brand.DateExpiration = brand.StartDate.Value.AddYears(command.NumberDateExpiration);

                    }


                    brand.Keyword = command.Keyword;
                    brand.Description = command.Description;

                    await _Repository.UpdateAsync(brand);
                    await _distributedCache.RemoveAsync(CompanyAdminInfoCacheKeys.ListKey);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    _log.LogInformation("UpdateCompanyInfoCommand update end: " + command.CusTaxCode);
                    try
                    {
                        _log.LogInformation($"UpdateCompanyInfoCommand delete Image start: {command.CusTaxCode}");
                        if (!string.IsNullOrEmpty(imgold))
                        {
                            _fileHelper.DeleteFile(imgold, FolderUploadConstants.ComPany);
                            _log.LogInformation($"UpdateCompanyInfoCommand delete Image done: {imgold} " + command.CusTaxCode);
                        }
                        if (!string.IsNullOrEmpty(logoold))
                        {
                            _fileHelper.DeleteFile(logoold, FolderUploadConstants.ComPany);
                            _log.LogInformation($"UpdateCompanyInfoCommand delete Image done: {logoold} " + command.CusTaxCode);
                        }
                    }
                    catch (Exception e)
                    {
                        _log.LogError("UpdateCompanyInfoCommand update Image:" + command.CusTaxCode + "\n" + e.ToString());
                    }

                    return Result<int>.Success(brand.Id);
                }
            }
            catch (Exception e)
            {
                _log.LogError("UpdateCompanyInfoCommand update" + command.CusTaxCode + "\n" + e.ToString());
                return Result<int>.Fail(e.Message);
            }
        }


    }
}
