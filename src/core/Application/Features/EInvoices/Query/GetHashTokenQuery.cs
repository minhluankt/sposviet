using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.EInvoices.Query
{

    public class GetHashTokenQuery : IRequest<IResult<string>>
    {

        public int[] lstid { get; set; }
        public int ComId { get; set; }

        public class GetHashTokenQueryHandler : IRequestHandler<GetHashTokenQuery, IResult<string>>
        {
            private readonly IRepositoryAsync<Domain.Entities.EInvoice> _repository;
            private readonly IEInvoiceRepository<Domain.Entities.EInvoice> _EInvoicerepository;
            public GetHashTokenQueryHandler(IRepositoryAsync<Domain.Entities.EInvoice> repository, IEInvoiceRepository<Domain.Entities.EInvoice> EInvoicerepository)
            {
                _EInvoicerepository = EInvoicerepository;
                _repository = repository;
            }
            public async Task<IResult<string>> Handle(GetHashTokenQuery query, CancellationToken cancellationToken)
            {
                return await _EInvoicerepository.GetHashTokenVNPTAsync(query.lstid, query.ComId);
            }
        }
    }
}
