using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.ManagerPatternEInvoices.Commands
{

    public partial class UpdateManagerPatternEInvoiceCommand : ManagerPatternEInvoice, IRequest<Result<int>>
    {
    }
    public class UpdateManagerPatternEInvoiceHandler : IRequestHandler<UpdateManagerPatternEInvoiceCommand, Result<int>>
    {
        private readonly ILogger<UpdateManagerPatternEInvoiceHandler> _log;
        private readonly IManagerPatternEInvoiceRepository<ManagerPatternEInvoice> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateManagerPatternEInvoiceHandler(IManagerPatternEInvoiceRepository<ManagerPatternEInvoice> brandRepository,
            ILogger<UpdateManagerPatternEInvoiceHandler> log,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
            _log = log;
        }


        public async Task<Result<int>> Handle(UpdateManagerPatternEInvoiceCommand command, CancellationToken cancellationToken)
        {
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                var product = _mapper.Map<ManagerPatternEInvoice>(command);

                var getkey = await _Repository.GetbykeyAsync(product.VFkey);
                if (getkey != null)
                {
                    if (getkey.Id!= product.Id)
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR014);
                    }
                    
                }
                var up=  await _Repository.UpdateAsync(product);
                if (!up.Succeeded)
                {
                    _log.LogError("UpdateManagerPatternEInvoiceCommand Không tìm thấy Đơn vị  " + command.TypeSupplierEInvoice);
                    return await Result<int>.FailAsync("Không tìm thấy đơn vị" + command.TypeSupplierEInvoice);
                }
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return await Result<int>.SuccessAsync(product.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _log.LogError("UpdateManagerPatternEInvoiceCommand Create " + command.TypeSupplierEInvoice + "\n" + ex.ToString());
                return await Result<int>.FailAsync(ex.Message);
            }

        }
    }
}
