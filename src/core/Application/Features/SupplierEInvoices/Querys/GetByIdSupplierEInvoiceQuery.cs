using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.SupplierEInvoices.Query
{

    public class GetByIdSupplierEInvoiceQuery : IRequest<Result<SupplierEInvoice>>
    {
        public int Id { get; set; }
        public int? ComId { get; set; }
        public string IdCode { get; set; }

        public class GetSupplierEInvoiceByIdQueryHandler : IRequestHandler<GetByIdSupplierEInvoiceQuery, Result<SupplierEInvoice>>
        {
            private readonly IRepositoryAsync<SupplierEInvoice> _repository;

            public GetSupplierEInvoiceByIdQueryHandler(IRepositoryAsync<SupplierEInvoice> repository)
            {
                _repository = repository;
            }
            public async Task<Result<SupplierEInvoice>> Handle(GetByIdSupplierEInvoiceQuery query, CancellationToken cancellationToken)
            {

                var product = await _repository.Entities.Where(x => x.Id == query.Id && x.ComId == query.ComId).SingleOrDefaultAsync();
                if (product == null)
                {
                    return Result<SupplierEInvoice>.Fail(HeperConstantss.ERR012);
                }
                return Result<SupplierEInvoice>.Success(product);
            }
        }
    }
}
