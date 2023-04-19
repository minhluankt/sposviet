using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.EInvoices.Commands
{

    public partial class CancelEInvoiceCommand : IRequest<IResult<PublishInvoiceModelView>>
    {
        public int[] lstId { get; set; }
        public int ComId { get; set; }
        public string IdCarsher { get; set; }
        public string Carsher { get; set; }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
    }

    public class CancelEInvoiceHandler : IRequestHandler<CancelEInvoiceCommand, IResult<PublishInvoiceModelView>>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IEInvoiceRepository<EInvoice> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public CancelEInvoiceHandler(IEInvoiceRepository<EInvoice> InvoiceRepository, 
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _Repository = InvoiceRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<IResult<PublishInvoiceModelView>> Handle(CancelEInvoiceCommand command, CancellationToken cancellationToken)
        {
            return await _Repository.CancelEInvoiceAsync(command.lstId, command.ComId, command.Carsher, command.IdCarsher);
        }
    }
}
