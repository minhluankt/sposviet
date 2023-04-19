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

namespace Application.Features.Citys.Query
{

    public class GetByIdCityQuery : IRequest<Result<City>>
    {
        public int Id { get; set; }

        public class GetCityByIdQueryHandler : IRequestHandler<GetByIdCityQuery, Result<City>>
        {
            private readonly IRepositoryAsync<City> _repository;
            public GetCityByIdQueryHandler(IRepositoryAsync<City> repository)
            {
                _repository = repository;
            }
            public async Task<Result<City>> Handle(GetByIdCityQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<City>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<City>.SuccessAsync(product);
            }
        }
    }
}
