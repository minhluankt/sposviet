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
   
    public class ViewEInvoiceQuery : IRequest<Result<string>>
    {
        public Guid IdInvoice { get; set; }
        public int ComId { get; set; }

        public class ViewEInvoiceQueryHandler : IRequestHandler<ViewEInvoiceQuery, Result<string>>
        {
            private readonly IRepositoryAsync<Invoice> _repository;
            private readonly IInvoicePepository<Invoice> _Invoicerepository;
            public ViewEInvoiceQueryHandler(IRepositoryAsync<Invoice> repository, IInvoicePepository<Invoice> Invoicerepository)
            {
                _Invoicerepository = Invoicerepository;
                _repository = repository;
            }
            public async Task<Result<string>> Handle(ViewEInvoiceQuery query, CancellationToken cancellationToken)
            {
                return await _Invoicerepository.ViewEInvoieDraft(query.ComId, query.IdInvoice);
            }
        }
    }
}
