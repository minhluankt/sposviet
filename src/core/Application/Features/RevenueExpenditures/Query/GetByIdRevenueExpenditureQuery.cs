using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.RevenueExpenditures.Query
{

    public class GetByIdRevenueExpenditureQuery : IRequest<Result<RevenueExpenditure>>
    {
        public int ComId { get; set; }
        public int Id { get; set; }


        public GetByIdRevenueExpenditureQuery(int _comId)
        {
            ComId = _comId;
        }

        public class GetRevenueExpenditureByIdQueryHandler : IRequestHandler<GetByIdRevenueExpenditureQuery, Result<RevenueExpenditure>>
        {
            private readonly IRepositoryAsync<RevenueExpenditure> _repository;
            public GetRevenueExpenditureByIdQueryHandler(IRepositoryAsync<RevenueExpenditure> repository)
            {
                _repository = repository;
            }
            public async Task<Result<RevenueExpenditure>> Handle(GetByIdRevenueExpenditureQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<RevenueExpenditure>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<RevenueExpenditure>.SuccessAsync(product);
            }
        }
    }
}
