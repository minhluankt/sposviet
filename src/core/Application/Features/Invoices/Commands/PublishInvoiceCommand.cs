﻿
using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Commands
{

    public partial class PublishInvoiceCommand : IRequest<IResult<PublishInvoiceModelView>>
    {
        public string CasherName { get; set; }//thuế s
        public string IdCarsher { get; set; }//thuế s
        public string serial { get; set; }//
        public string pattern { get; set; }//
        public string dataxmlhash { get; set; }//
        public string serialCert { get; set; }//
        public int[] lstId { get; set; }
        public int IdManagerPatternEInvoice { get; set; }
        public float VATRate { get; set; }
        public DateTime? ArisingDate { get; set; }
        public int ComId { get; set; }
        public Guid? IdInvoice { get; set; }
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
        public EnumTypeEventInvoice TypeEventInvoice { get; set; }
    }
    public class PublishInvoiceHandler : IRequestHandler<PublishInvoiceCommand, IResult<PublishInvoiceModelView>>
    {
        private readonly ILogger<PublishInvoiceHandler> _log;
        private readonly IDistributedCache _distributedCache;
        private readonly IInvoicePepository<Invoice> _Repository;
        private readonly IRepositoryAsync<HistoryInvoice> _historyInvoiceRepository;
        private IUnitOfWork _unitOfWork { get; set; }

        public PublishInvoiceHandler(IInvoicePepository<Invoice> InvoiceRepository,
            ILogger<PublishInvoiceHandler> log,
            IRepositoryAsync<HistoryInvoice> historyInvoiceRepository,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _historyInvoiceRepository = historyInvoiceRepository;
            _Repository = InvoiceRepository;
            _log = log;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<IResult<PublishInvoiceModelView>> Handle(PublishInvoiceCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (command.TypeEventInvoice == EnumTypeEventInvoice.PublishEInvoiceTokenByHash)
                {
                    return await _Repository.PublishInvoiceByToKen(command.ComId, command.TypeSupplierEInvoice, command.serial, command.pattern, command.dataxmlhash, command.IdCarsher, command.CasherName);
                }
                else if (command.TypeEventInvoice == EnumTypeEventInvoice.PublishEInvoieDraft)
                {
                    PublishInvoiceModel publishInvoiceModel = new PublishInvoiceModel();
                    publishInvoiceModel.TypeSupplierEInvoice = command.TypeSupplierEInvoice;
                    publishInvoiceModel.VATRate = command.VATRate;
                    publishInvoiceModel.ArisingDate = command.ArisingDate;
                    publishInvoiceModel.IdManagerPatternEInvoice = command.IdManagerPatternEInvoice;
                    return await _Repository.PublishEInvoieDraft(command.ComId, command.IdInvoice.Value, publishInvoiceModel);
                }
                else if (command.TypeEventInvoice == EnumTypeEventInvoice.DeleteEInvoieDraft)
                {
                    PublishInvoiceModel publishInvoiceModel = new PublishInvoiceModel();
                    publishInvoiceModel.TypeSupplierEInvoice = command.TypeSupplierEInvoice;
                    publishInvoiceModel.VATRate = command.VATRate;
                    publishInvoiceModel.IdManagerPatternEInvoice = command.IdManagerPatternEInvoice;
                    var publish = await _Repository.DeleteEInvoieDraft(command.ComId, command.IdInvoice.Value);
                    if (publish.Succeeded)
                    {
                        return await Result<PublishInvoiceModelView>.SuccessAsync();
                    }
                    else
                    {
                        return await Result<PublishInvoiceModelView>.FailAsync(publish.Message);
                    }

                }

                var model = new PublishInvoiceModel()
                {
                    ArisingDate = command.ArisingDate,
                    lstid = command.lstId,
                    ComId = command.ComId,
                    IdManagerPatternEInvoice = command.IdManagerPatternEInvoice,
                    VATRate = command.VATRate,
                    TypeEventInvoice = command.TypeEventInvoice,
                    TypeSupplierEInvoice = command.TypeSupplierEInvoice,
                };
                return await _Repository.PublishInvoice(model);
            }
            catch (Exception e)
            {
                _log.LogError("Có lỗi khi phát hành:" + e.ToString());
                return await Result<PublishInvoiceModelView>.FailAsync(e.Message);
            }
           
        }
    }
}
