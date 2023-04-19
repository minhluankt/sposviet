using Application.CacheKeys;
using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.PaymentMethods.Commands
{

    public partial class UpdatePaymentMethodCommand : PaymentMethod, IRequest<Result<int>>
    {
    }
    public class UpdatePaymentMethodHandler : IRequestHandler<UpdatePaymentMethodCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<UpdatePaymentMethodCommand> _log;
        private readonly IRepositoryAsync<PaymentMethod> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdatePaymentMethodHandler(IRepositoryAsync<PaymentMethod> brandRepository, ILogger<UpdatePaymentMethodCommand> log,
            IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;
            _Repository = brandRepository;

            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;

            _log = log;
        }

        public async Task<Result<int>> Handle(UpdatePaymentMethodCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // FormFileHelper _formFileHelper = new FormFileHelper(_hostingEnvironment);

                _log.LogInformation("UpdatePaymentMethodCommand update start: " + command.Name);
                PaymentMethod brand = await _Repository.GetByIdAsync(command.Id);
                string imgold = string.Empty;
                string logoold = string.Empty;
                if (brand == null)
                {
                    _log.LogError(HeperConstantss.ERR012 + "___" + command.Name);
                    return Result<int>.Fail(HeperConstantss.ERR012);
                }
                else
                {
                    string slug = Common.ConvertToSlug(command.Name);
                    if (!string.IsNullOrEmpty(slug))
                    {
                        var fidn = _Repository.Entities.AsNoTracking().Where(m => m.ComId == command.ComId && m.Slug == slug && m.Id!=brand.Id).SingleOrDefault();
                        if (fidn != null)
                        {
                            _log.LogError("CreatePaymentMethodCommand Create: Đã tồn tại tên " + command.Name);
                            return await Result<int>.FailAsync("Đã tồn tại hình thức thanh toán !" + command.Name);
                        }
                    }
                    //if (!string.IsNullOrEmpty(command.Code))
                    //{
                    //    var fidn = _Repository.Entities.AsNoTracking().Where(m => m.ComId == command.ComId && m.Code == command.Code && m.Id != brand.Id).SingleOrDefault();
                    //    if (fidn != null)
                    //    {
                    //        _log.LogError("CreatePaymentMethodCommand Create: Đã tồn tại mã " + command.Code);
                    //        return await Result<int>.FailAsync("Đã tồn tại hình thức thanh toán !" + command.Code);
                    //    }
                    //}
                    brand.Slug = slug;
                    brand.Name = command.Name;
                    //brand.Code = command.Code;
                    brand.Content = command.Content;
                    if (!string.IsNullOrEmpty(command.Avatar))
                    {
                        imgold = brand.Avatar;
                        brand.Avatar = command.Avatar;
                    }
                    brand.Active = command.Active;
                    await _Repository.UpdateAsync(brand);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    _log.LogInformation("UpdatePaymentMethodCommand update end: " + command.Code);
                    try
                    {
                        _log.LogInformation("UpdatePaymentMethodCommand update Image start:" + command.Code);
                        if (!string.IsNullOrEmpty(imgold))
                        {
                            _fileHelper.DeleteFile(imgold, FolderUploadConstants.PaymentMethod);
                        }
                      
                    }
                    catch (Exception e)
                    {
                        _log.LogError("UpdatePaymentMethodCommand update Image:" + command.Name + "\n" + e.ToString());
                    }

                    return Result<int>.Success(brand.Id);
                }
            }
            catch (Exception e)
            {
                _log.LogError("UpdatePaymentMethodCommand update" + command.Name + "\n" + e.ToString());
                return Result<int>.Fail(e.Message);
            }
        }
    }
}
