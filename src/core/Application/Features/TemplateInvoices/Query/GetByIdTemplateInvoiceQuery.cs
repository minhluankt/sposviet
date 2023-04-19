using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.TemplateInvoices.Query
{

    public class GetByIdTemplateInvoiceQuery : IRequest<Result<TemplateInvoice>>
    {
        public int Id { get; set; }
        public int? ComId { get; set; }
        public string IdCode { get; set; }

        public class GetTemplateInvoiceByIdQueryHandler : IRequestHandler<GetByIdTemplateInvoiceQuery, Result<TemplateInvoice>>
        {
            private readonly IRepositoryAsync<TemplateInvoice> _repository;

            public GetTemplateInvoiceByIdQueryHandler(IRepositoryAsync<TemplateInvoice> repository)
            {
                _repository = repository;
            }
            public async Task<Result<TemplateInvoice>> Handle(GetByIdTemplateInvoiceQuery query, CancellationToken cancellationToken)
            {

                var product = await _repository.Entities.Where(x => x.Id == query.Id && x.ComId == query.ComId).SingleOrDefaultAsync();
                if (product == null)
                {
                    return Result<TemplateInvoice>.Fail(HeperConstantss.ERR012);
                }
                return Result<TemplateInvoice>.Success(product);
            }
        }
    }
}
