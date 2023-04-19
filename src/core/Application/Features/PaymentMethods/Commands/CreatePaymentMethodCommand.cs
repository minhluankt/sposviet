using Application.CacheKeys;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.Features.PaymentMethods.Commands
{

    public partial class CreatePaymentMethodCommand : PaymentMethod, IRequest<Result<int>>
    {

    }
    public class CreatePaymentMethodHandler : IRequestHandler<CreatePaymentMethodCommand, Result<int>>
    {
        private readonly ILogger<CreatePaymentMethodCommand> _log;
        private readonly IRepositoryAsync<PaymentMethod> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreatePaymentMethodHandler(IRepositoryAsync<PaymentMethod> brandRepository, 
            ILogger<CreatePaymentMethodCommand> log,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
            _log = log;
        }

        public async Task<Result<int>> Handle(CreatePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _log.LogInformation("CreatePaymentMethodCommand  start " + request.Name);
                //string Vfkey = $"{request.ComId}{request.Code}";
                string slug = Common.ConvertToSlug(request.Name);
                if (!string.IsNullOrEmpty(slug))
                {
                    var fidn = _Repository.Entities.AsNoTracking().Where(m => m.Slug == slug && m.ComId == request.ComId).SingleOrDefault();
                    if (fidn != null)
                    {
                        _log.LogError("CreatePaymentMethodCommand Create: Đã tồn tại tên " + request.Name);
                        return await Result<int>.FailAsync("Đã tồn tại hình thức thanh toán !"+ request.Name);
                    }
                }
                //if (!string.IsNullOrEmpty(request.Code))
                //{
                //    var fidn = _Repository.Entities.AsNoTracking().Where(m => m.ComId == request.ComId && m.Code == request.Code).SingleOrDefault();
                //    if (fidn != null)
                //    {
                //        _log.LogError("CreatePaymentMethodCommand Create: Đã tồn tại mã " + request.Code);
                //        return await Result<int>.FailAsync("Đã tồn tại hình thức thanh toán !" + request.Code);
                //    }
                //}
                var product = _mapper.Map<PaymentMethod>(request);
                
                // product.IdCodeGuid = Guid.NewGuid();
                await _Repository.AddAsync(product);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
               

                _log.LogInformation("CreatePaymentMethodCommand  end " + request.Name);
                return await Result<int>.SuccessAsync(product.Id);
            }
            catch (Exception ex)
            {
              
                _log.LogError("CreatePaymentMethodCommand Create " + request.Name + "\n" + ex.ToString());
                return await Result<int>.FailAsync(ex.Message);
            }

        }
    }
}
