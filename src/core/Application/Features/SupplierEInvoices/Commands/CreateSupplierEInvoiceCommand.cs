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

namespace Application.Features.SupplierEInvoices.Commands
{

    public partial class CreateSupplierEInvoiceCommand : SupplierEInvoice, IRequest<Result<SupplierEInvoice>>
    {

    }
    public class CreateSupplierEInvoiceHandler : IRequestHandler<CreateSupplierEInvoiceCommand, Result<SupplierEInvoice>>
    {
        private readonly ILogger<CreateSupplierEInvoiceCommand> _log;
        private readonly ISupplierEInvoiceRepository<SupplierEInvoice> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateSupplierEInvoiceHandler(ISupplierEInvoiceRepository<SupplierEInvoice> brandRepository,
            ILogger<CreateSupplierEInvoiceCommand> log,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
            _log = log;
        }

        public async Task<Result<SupplierEInvoice>> Handle(CreateSupplierEInvoiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = _mapper.Map<SupplierEInvoice>(request);
                await _Repository.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();
                return await Result<SupplierEInvoice>.SuccessAsync(product);
            }
            catch (Exception ex)
            {
                _log.LogError("CreateSupplierEInvoiceCommand Create " + request.TypeSupplierEInvoice + "\n" + ex.ToString());
                return await Result<SupplierEInvoice>.FailAsync(ex.Message);
            }

        }
    }
}
