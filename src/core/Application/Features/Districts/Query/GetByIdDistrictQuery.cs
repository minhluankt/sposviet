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

namespace Application.Features.Districts.Query
{

    public class GetByIdDistrictQuery : IRequest<Result<District>>
    {
        public int Id { get; set; }

        public class GetDistrictByIdQueryHandler : IRequestHandler<GetByIdDistrictQuery, Result<District>>
        {
            private readonly IRepositoryAsync<District> _repository;
            public GetDistrictByIdQueryHandler(IRepositoryAsync<District> repository)
            {
                _repository = repository;
            }
            public async Task<Result<District>> Handle(GetByIdDistrictQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<District>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<District>.SuccessAsync(product);
            }
        }
    }
}
