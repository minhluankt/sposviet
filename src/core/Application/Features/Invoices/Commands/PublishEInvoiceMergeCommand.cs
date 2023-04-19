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
    public partial class PublishEInvoiceMergeCommand : PublishInvoiceMergeModel, IRequest<Result<PublishInvoiceModelView>>
    {
        public int ComId { get; set; }
        public EnumTypeEventInvoice TypeEventInvoice { get; set; }
    }
    public class PublishEInvoiceMergeHandler : IRequestHandler<PublishEInvoiceMergeCommand, Result<PublishInvoiceModelView>>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IInvoicePepository<Invoice> _Repository;
        private readonly IRepositoryAsync<HistoryInvoice> _historyInvoiceRepository;
        private IUnitOfWork _unitOfWork { get; set; }

        public PublishEInvoiceMergeHandler(IInvoicePepository<Invoice> InvoiceRepository, IRepositoryAsync<HistoryInvoice> historyInvoiceRepository,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _historyInvoiceRepository = historyInvoiceRepository;
            _Repository = InvoiceRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<PublishInvoiceModelView>> Handle(PublishEInvoiceMergeCommand command, CancellationToken cancellationToken)
        {
            if (command.ComId==0)
            {
                return await Result<PublishInvoiceModelView>.FailAsync("Không tìm thấy COMID");
            }
            return await _Repository.PublishEInvoiceMerge(command, command.ComId);
        }
    }

}
