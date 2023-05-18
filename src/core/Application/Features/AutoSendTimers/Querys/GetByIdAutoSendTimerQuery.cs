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


namespace Application.Features.AutoSendTimers.Query
{

    public class GetByIdAutoSendTimerQuery : IRequest<Result<AutoSendTimer>>
    {
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
        public int Id { get; set; }
        public int? ComId { get; set; }
        public string IdCode { get; set; }

        public class GetAutoSendTimerByIdQueryHandler : IRequestHandler<GetByIdAutoSendTimerQuery, Result<AutoSendTimer>>
        {
            private readonly IRepositoryAsync<AutoSendTimer> _repository;

            public GetAutoSendTimerByIdQueryHandler(IRepositoryAsync<AutoSendTimer> repository)
            {
                _repository = repository;
            }
            public async Task<Result<AutoSendTimer>> Handle(GetByIdAutoSendTimerQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.Entities.Where(x => x.Id == query.Id && x.TypeSupplierEInvoice== query.TypeSupplierEInvoice && x.ComId == query.ComId).AsNoTracking().SingleOrDefaultAsync();
                if (product == null)
                {
                    return Result<AutoSendTimer>.Fail(HeperConstantss.ERR012);
                }
                return Result<AutoSendTimer>.Success(product);
            }
        }
    }
}
