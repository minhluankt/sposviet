using Application.Constants;
using Application.Enums;
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

    public class GetActivePromotionRunQuery : IRequest<Result<IEnumerable<PromotionRun>>>
    {
        public bool IsProcessing { get; set; }

        public class GetPromotionRunByIdQueryHandler : IRequestHandler<GetActivePromotionRunQuery, Result<IEnumerable<PromotionRun>>>
        {
            private readonly IRepositoryAsync<PromotionRun> _repository;
            public GetPromotionRunByIdQueryHandler(IRepositoryAsync<PromotionRun> repository)
            {
                _repository = repository;
            }
            public async Task<Result<IEnumerable<PromotionRun>>> Handle(GetActivePromotionRunQuery query, CancellationToken cancellationToken)
            {
                var product =  _repository.GetAll(x=> (x.IsActive && x.Status==(int)StatusPromotionRun.Processing) || x.Status == (int)StatusPromotionRun.Upcoming);
               
                if (product == null)
                {
                    return await Result<IEnumerable<PromotionRun>>.FailAsync(HeperConstantss.ERR012);
                }
                if (query.IsProcessing)
                {
                    product = product.Where(x=>x.Status== (int)StatusPromotionRun.Processing);
                }
                return await Result<IEnumerable<PromotionRun>>.SuccessAsync(product);
            }
        }
    }
}
