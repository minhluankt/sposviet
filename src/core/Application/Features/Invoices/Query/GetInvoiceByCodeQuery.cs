using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Query
{
    public class GetInvoiceByCodeQuery : IRequest<Result<List<Invoice>>>
    {
        public string InvoiceCodePatern { get; set; }
        public int ComId { get; set; }

        public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByCodeQuery, Result<List<Invoice>>>
        {
            private readonly IRepositoryAsync<Invoice> _repository;
            private readonly IInvoicePepository<Invoice> _Invoicerepository;
            public GetInvoiceByIdQueryHandler(IRepositoryAsync<Invoice> repository, IInvoicePepository<Invoice> Invoicerepository)
            {
                _Invoicerepository = Invoicerepository;
                _repository = repository;
            }
            public async Task<Result<List<Invoice>>> Handle(GetInvoiceByCodeQuery query, CancellationToken cancellationToken)
            {
                var Invoice = await _repository.Entities.AsNoTracking().Where(m => m.InvoiceCodePatern == query.InvoiceCodePatern && m.ComId == query.ComId).ToListAsync();
                return await Result<List<Invoice>>.SuccessAsync(Invoice);
            }
        }
    }
}
