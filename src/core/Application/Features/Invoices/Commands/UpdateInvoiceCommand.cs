using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Commands
{

    public partial class UpdateInvoiceCommand : IRequest<Result<PublishInvoiceModelView>>
    {
        public Guid? Id { get; set; }
        public int? IdCustomer { get; set; }
        public bool IsDeletePT { get; set; }//có xóa phiếu thu liên quan không
        public bool IsDelete { get; set; }
        public int ComId { get; set; }
        public int[] lstid { get; set; }
        public string CasherName { get; set; }
        public string IdCasherName { get; set; }
        public string Note { get; set; }
        public EnumTypeEventInvoice TypeEventInvoice { get; set; }
    }
    public class UpdateInvoiceHandler : IRequestHandler<UpdateInvoiceCommand, Result<PublishInvoiceModelView>>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IInvoicePepository<Invoice> _Repository;
        private readonly IRepositoryAsync<HistoryInvoice> _historyInvoiceRepository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateInvoiceHandler(IInvoicePepository<Invoice> InvoiceRepository, IRepositoryAsync<HistoryInvoice> historyInvoiceRepository,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _historyInvoiceRepository = historyInvoiceRepository;
            _Repository = InvoiceRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<PublishInvoiceModelView>> Handle(UpdateInvoiceCommand command, CancellationToken cancellationToken)
        {
            if (command.TypeEventInvoice==EnumTypeEventInvoice.DeleteIsMerge)
            {
                if (command.Id == null)
                {
                    return await Result<PublishInvoiceModelView>.FailAsync(HeperConstantss.ERR000);
                }
                return await _Repository.DeleteIsMergeInvoice(command.Id.Value, command.ComId, command.CasherName);
            }
            else if (command.TypeEventInvoice==EnumTypeEventInvoice.UpdateCustomer)
            {
                return await _Repository.UpdateCustomerInvoice(command.Id.Value, command.ComId, command.IdCustomer.Value, command.CasherName);
            }
            else if(command.lstid!=null && command.lstid.Count()>0)
            {
                return await _Repository.CancelInvoice(command.lstid, command.ComId, command.CasherName, command.Note, command.TypeEventInvoice, command.IsDelete);

            }
            else
            {
                if (command.Id==null)
                {
                    return await Result<PublishInvoiceModelView>.FailAsync(HeperConstantss.ERR000);
                }
                return await _Repository.CancelInvoice(command.Id.Value, command.ComId, command.CasherName, command.Note, command.TypeEventInvoice);
                
            }
          
        }
    }
}
