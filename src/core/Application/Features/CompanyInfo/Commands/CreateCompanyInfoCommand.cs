using Application.CacheKeys;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CompanyInfo.Commands
{

    public partial class CreateCompanyInfoCommand : CompanyAdminInfo, IRequest<Result<int>>
    {

    }
    public class CreateCompanyInfoHandler : IRequestHandler<CreateCompanyInfoCommand, Result<int>>
    {
        private readonly ILogger<CreateCompanyInfoCommand> _log;
        private readonly IRepositoryAsync<CompanyAdminInfo> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateCompanyInfoHandler(IRepositoryAsync<CompanyAdminInfo> brandRepository,
            ILogger<CreateCompanyInfoCommand> log,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
            _log = log;
        }

        public async Task<Result<int>> Handle(CreateCompanyInfoCommand request, CancellationToken cancellationToken)
        {
            try
            {

                _log.LogInformation("CreateCompanyInfoCommand  start " + request.CusTaxCode);
                string vfkeycus = string.Empty;
                //if (!string.IsNullOrEmpty(request.CusTaxCode))
                //{

                //}
                var fidn = _Repository.Entities.Where(m => m.VFkeyPhone == request.VFkeyPhone).FirstOrDefault();
                if (fidn != null)
                {
                    _log.LogError("CreateCompanyInfoCommand Create: Đã tồn tại công ty phone: " + request.VFkeyPhone);
                    return await Result<int>.FailAsync("Đã tồn tại công ty có cùng số điện thoại cùng dịch vụ đăng ký !");
                }
                if (!string.IsNullOrEmpty(request.CusTaxCode))
                {
                    fidn = _Repository.Entities.Where(x => x.VFkeyCusTaxCode == request.VFkeyCusTaxCode).FirstOrDefault();
                    if (fidn != null)
                    {
                        _log.LogError("CreateCompanyInfoCommand Create: Đã tồn tại công ty " + request.CusTaxCode + "phone: " + request.VFkeyPhone);
                        return await Result<int>.FailAsync("Đã tồn tại công ty!");
                    }
                }
                else
                {
                    request.VFkeyCusTaxCode = null;
                }

                var product = _mapper.Map<CompanyAdminInfo>(request);
                if (product.NumberDateExpiration > 0 && product.DateExpiration==null)
                {
                    product.DateExpiration = product.StartDate.Value.AddMonths(product.NumberDateExpiration);
                }
                await _Repository.AddAsync(product);
                await _distributedCache.RemoveAsync(CompanyAdminInfoCacheKeys.ListKey);
                await _unitOfWork.SaveChangesAsync(cancellationToken);



                _log.LogInformation("CreateCompanyInfoCommand  end " + request.CusTaxCode);
                return await Result<int>.SuccessAsync(product.Id);
            }
            catch (Exception ex)
            {
                _log.LogError("CreateCompanyInfoCommand Create " + request.CusTaxCode + "\n" + ex.ToString());
                return await Result<int>.FailAsync(ex.Message);
            }

        }
    }
}
