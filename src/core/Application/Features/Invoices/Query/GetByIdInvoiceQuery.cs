using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Query
{

    public class GetByIdInvoiceQuery : IRequest<Result<Invoice>>
    {

        public Guid Id { get; set; }
        public int ComId { get; set; }
        public bool IncludeOrderTable { get; set; } = true;
        public bool IncludeCustomer { get; set; } = true;

        public class GetInvoiceByIdQueryHandler : IRequestHandler<GetByIdInvoiceQuery, Result<Invoice>>
        {
            private readonly IRepositoryAsync<Invoice> _repository;
            private readonly IInvoicePepository<Invoice> _Invoicerepository;
            private readonly IEInvoiceRepository<EInvoice> _einvoicerepository;
            public GetInvoiceByIdQueryHandler(IRepositoryAsync<Invoice> repository, IEInvoiceRepository<EInvoice> einvoicerepository,
                IInvoicePepository<Invoice> Invoicerepository)
            {
                _einvoicerepository = einvoicerepository;
                _Invoicerepository = Invoicerepository;
                _repository = repository;
            }
            public async Task<Result<Invoice>> Handle(GetByIdInvoiceQuery query, CancellationToken cancellationToken)
            {
                var Invoice = _repository.Entities.AsNoTracking().Where(m => m.IdGuid == query.Id && m.ComId == query.ComId);
                Invoice = Invoice.Include(x => x.InvoiceItems).Include(x=>x.HistoryInvoices);
                if (query.IncludeOrderTable)
                {
                    Invoice = Invoice.Include(x => x.OrderTable);
                }
                if (query.IncludeCustomer)
                {
                    Invoice = Invoice.Include(x => x.Customer);
                }
                var InvoiceData = Invoice.Join;
               // var InvoiceData = await Invoice.SingleOrDefaultAsync();

                if (InvoiceData == null)
                {
                    return await Result<Invoice>.FailAsync(HeperConstantss.ERR012);
                }
                
                if (InvoiceData.IdEInvoice!=null)
                {
                    var einv = await _einvoicerepository.FindByIdAsync(InvoiceData.IdEInvoice.Value,true);
                    InvoiceData.EInvoice = einv;
                }

                return await Result<Invoice>.SuccessAsync(InvoiceData);
            }
        }
    }
}
