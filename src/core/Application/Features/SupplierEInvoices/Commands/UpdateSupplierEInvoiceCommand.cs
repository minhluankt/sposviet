using Application.Hepers;
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


namespace Application.Features.SupplierEInvoices.Commands
{

    public partial class UpdateSupplierEInvoiceCommand : SupplierEInvoice, IRequest<Result<int>>
    {
    }
    public class UpdateSupplierEInvoiceHandler : IRequestHandler<UpdateSupplierEInvoiceCommand, Result<int>>
    {
        private readonly ILogger<UpdateSupplierEInvoiceHandler> _log;
        private readonly ISupplierEInvoiceRepository<SupplierEInvoice> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateSupplierEInvoiceHandler(ISupplierEInvoiceRepository<SupplierEInvoice> brandRepository,
            ILogger<UpdateSupplierEInvoiceHandler> log,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
            _log = log;
        }


        public async Task<Result<int>> Handle(UpdateSupplierEInvoiceCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var product = _mapper.Map<SupplierEInvoice>(command);
                var up=  await _Repository.UpdateAsync(product, command.ComId);
                if (!up.Succeeded)
                {
                    _log.LogError("UpdateSupplierEInvoiceCommand Không tìm thấy Đơn vị  " + command.TypeSupplierEInvoice);
                    return await Result<int>.FailAsync("Không tìm thấy đơn vị" + command.TypeSupplierEInvoice);
                }
                await _unitOfWork.SaveChangesAsync();
                return await Result<int>.SuccessAsync(product.Id);
            }
            catch (Exception ex)
            {
                _log.LogError("UpdateSupplierEInvoiceCommand Create " + command.TypeSupplierEInvoice + "\n" + ex.ToString());
                return await Result<int>.FailAsync(ex.Message);
            }

        }
    }
}
