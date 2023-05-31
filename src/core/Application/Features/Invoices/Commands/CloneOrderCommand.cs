using Application.Constants;
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
 
    public partial class CloneOrderCommand : IRequest<Result<OrderTable>>
    {
        public Guid? Id { get; set; }
        public int ComId { get; set; }
        public int? IdCustomer { get; set; }
        public string IdCasherName { get; set; }
        public string CasherName { get; set; }
    }
    public class CloneOrderHandler : IRequestHandler<CloneOrderCommand, Result<OrderTable>>
    {
        private readonly IInvoicePepository<Invoice> _Repository;

        public CloneOrderHandler(IInvoicePepository<Invoice> InvoiceRepository)
        {
            _Repository = InvoiceRepository;
        }

        public async Task<Result<OrderTable>> Handle(CloneOrderCommand command, CancellationToken cancellationToken)
        {
            return await _Repository.CloneOrder(command.Id.Value, command.ComId, command.IdCasherName, command.CasherName);

        }
    }
}
