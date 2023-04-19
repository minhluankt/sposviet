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
   
    public partial class SendCQTCommand : IRequest<IResult<PublishInvoiceModelView>>
    {
        public string IdCarsher { get; set; }
        public string dataSign { get; set; }
        public string Carsher { get; set; }
        public int[] lstId { get; set; }
        public int ComId { get; set; }
        public int? IdEInvoice { get; set; }
        public ENumTypeSeri TypeSeri { get; set; }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
    }
    public class SendCQTHandler : IRequestHandler<SendCQTCommand, IResult<PublishInvoiceModelView>>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IEInvoiceRepository<EInvoice> _Repository;
        private readonly ISupplierEInvoiceRepository<SupplierEInvoice> _supplierEInvoiceRepository;
        private readonly IRepositoryAsync<HistoryInvoice> _historyInvoiceRepository;
        private IUnitOfWork _unitOfWork { get; set; }

        public SendCQTHandler(IEInvoiceRepository<EInvoice> InvoiceRepository,
            ISupplierEInvoiceRepository<SupplierEInvoice> supplierEInvoiceRepository,
            IRepositoryAsync<HistoryInvoice> historyInvoiceRepository,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _supplierEInvoiceRepository = supplierEInvoiceRepository;
            _historyInvoiceRepository = historyInvoiceRepository;
            _Repository = InvoiceRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<IResult<PublishInvoiceModelView>> Handle(SendCQTCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (command.TypeSeri ==ENumTypeSeri.TOKEN)
                {
                    return await _Repository.SendCQTTokenAsync(command.lstId, command.dataSign, command.ComId, command.Carsher, command.IdCarsher);
                }
                else
                {
                    return await _Repository.SendCQTAsync(command.lstId, command.ComId, command.Carsher, command.IdCarsher);
                }
                
            }
            catch (Exception e)
            {
                return await Result<PublishInvoiceModelView>.FailAsync(e.Message);
            }
           
        }
    }
}
