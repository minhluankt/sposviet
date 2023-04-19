﻿using Application.Enums;
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
  
    public partial class SycnInvoiceCommand : IRequest<IResult<PublishInvoiceModelView>>
    {
        public string IdCarsher { get; set; }
        public string Carsher { get; set; }
        public int[] lstId { get; set; }
        public int ComId { get; set; }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
        public EnumTypeSyncEInvoice TypeSyncEInvoice { get; set; }//trạng thái loại đồng bộ
    }
    public class SycnInvoiceHandler : IRequestHandler<SycnInvoiceCommand, IResult<PublishInvoiceModelView>>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IEInvoiceRepository<EInvoice> _Repository;
        private readonly IRepositoryAsync<HistoryInvoice> _historyInvoiceRepository;
        private IUnitOfWork _unitOfWork { get; set; }

        public SycnInvoiceHandler(IEInvoiceRepository<EInvoice> InvoiceRepository, IRepositoryAsync<HistoryInvoice> historyInvoiceRepository,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _historyInvoiceRepository = historyInvoiceRepository;
            _Repository = InvoiceRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<IResult<PublishInvoiceModelView>> Handle(SycnInvoiceCommand command, CancellationToken cancellationToken)
        {
            return await _Repository.SycnInvoiceAsync(command.lstId, command.ComId, command.Carsher, command.IdCarsher, command.TypeSyncEInvoice);
        }
    }
}
