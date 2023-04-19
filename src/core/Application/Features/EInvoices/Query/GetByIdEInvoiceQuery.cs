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

namespace Application.Features.EInvoices.Query
{

    public class GetByIdEInvoiceQuery : IRequest<Result<Domain.Entities.EInvoice>>
    {

        public Guid Id { get; set; }
        public int ComId { get; set; }
        public bool IncludeOrderTable { get; set; } = true;
        public bool IncludeCustomer { get; set; } = true;

        public class GetEInvoiceByIdQueryHandler : IRequestHandler<GetByIdEInvoiceQuery, Result<Domain.Entities.EInvoice>>
        {
            private readonly IRepositoryAsync<Domain.Entities.EInvoice> _repository;
            private readonly IEInvoiceRepository<Domain.Entities.EInvoice> _EInvoicerepository;
            public GetEInvoiceByIdQueryHandler(IRepositoryAsync<Domain.Entities.EInvoice> repository, IEInvoiceRepository<Domain.Entities.EInvoice> EInvoicerepository)
            {
                _EInvoicerepository = EInvoicerepository;
                _repository = repository;
            }
            public async Task<Result<Domain.Entities.EInvoice>> Handle(GetByIdEInvoiceQuery query, CancellationToken cancellationToken)
            {
                var EInvoice = _repository.Entities.Where(m => m.Fkey == query.Id && m.ComId == query.ComId);
                EInvoice = EInvoice.Include(x => x.EInvoiceItems);
                var EInvoiceData = await EInvoice.SingleOrDefaultAsync();
                if (EInvoiceData == null)
                {
                    return await Result<Domain.Entities.EInvoice>.FailAsync(HeperConstantss.ERR012);
                }

                return await Result<Domain.Entities.EInvoice>.SuccessAsync(EInvoiceData);
            }
        }
    }
}
