using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.ManagerPatternEInvoices.Query
{

    public class GetByIdManagerPatternEInvoiceQuery : IRequest<Result<ManagerPatternEInvoice>>
    {
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
        public int Id { get; set; }
        public int? ComId { get; set; }
        public string IdCode { get; set; }

        public class GetManagerPatternEInvoiceByIdQueryHandler : IRequestHandler<GetByIdManagerPatternEInvoiceQuery, Result<ManagerPatternEInvoice>>
        {
            private readonly IRepositoryAsync<ManagerPatternEInvoice> _repository;

            public GetManagerPatternEInvoiceByIdQueryHandler(IRepositoryAsync<ManagerPatternEInvoice> repository)
            {
                _repository = repository;
            }
            public async Task<Result<ManagerPatternEInvoice>> Handle(GetByIdManagerPatternEInvoiceQuery query, CancellationToken cancellationToken)
            {

                var product = await _repository.Entities.Where(x => x.Id == query.Id && x.TypeSupplierEInvoice== query.TypeSupplierEInvoice && x.ComId == query.ComId).AsNoTracking().SingleOrDefaultAsync();
                if (product == null)
                {
                    return Result<ManagerPatternEInvoice>.Fail(HeperConstantss.ERR012);
                }
                return Result<ManagerPatternEInvoice>.Success(product);
            }
        }
    }
}
