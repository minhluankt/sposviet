using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PromotionRuns.Query
{

    public class GetByIdPromotionRunQuery : IRequest<Result<PromotionRun>>
    {
        public int Id { get; set; }

        public class GetPromotionRunByIdQueryHandler : IRequestHandler<GetByIdPromotionRunQuery, Result<PromotionRun>>
        {
            private readonly IRepositoryAsync<PromotionRun> _repository;
            public GetPromotionRunByIdQueryHandler(IRepositoryAsync<PromotionRun> repository)
            {
                _repository = repository;
            }
            public async Task<Result<PromotionRun>> Handle(GetByIdPromotionRunQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<PromotionRun>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<PromotionRun>.SuccessAsync(product);
            }
        }
    }
}
