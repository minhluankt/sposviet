
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

namespace Application.Features.Invoices.Commands
{

    public partial class PublishInvoiceCommand : IRequest<Result<PublishInvoiceModelView>>
    {
        public string CasherName { get; set; }//thuế s
        public string IdCarsher { get; set; }//thuế s
        public int[] lstId { get; set; }
        public int IdManagerPatternEInvoice { get; set; }
        public float VATRate { get; set; }
        public int ComId { get; set; }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
        public EnumTypeEventInvoice TypeEventInvoice { get; set; }
    }
    public class PublishInvoiceHandler : IRequestHandler<PublishInvoiceCommand, Result<PublishInvoiceModelView>>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IInvoicePepository<Invoice> _Repository;
        private readonly IRepositoryAsync<HistoryInvoice> _historyInvoiceRepository;
        private IUnitOfWork _unitOfWork { get; set; }

        public PublishInvoiceHandler(IInvoicePepository<Invoice> InvoiceRepository, IRepositoryAsync<HistoryInvoice> historyInvoiceRepository,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _historyInvoiceRepository = historyInvoiceRepository;
            _Repository = InvoiceRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<PublishInvoiceModelView>> Handle(PublishInvoiceCommand command, CancellationToken cancellationToken)
        {
            var model = new PublishInvoiceModel()
            {
                lstid = command.lstId,
                ComId = command.ComId,
                IdManagerPatternEInvoice = command.IdManagerPatternEInvoice,
                VATRate = command.VATRate,
                TypeEventInvoice = command.TypeEventInvoice,
                TypeSupplierEInvoice = command.TypeSupplierEInvoice,
            };
            return await _Repository.PublishInvoice(model);
        }
    }
}
