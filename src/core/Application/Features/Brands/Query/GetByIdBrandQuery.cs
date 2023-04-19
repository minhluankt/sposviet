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

namespace Application.Features.Brands.Query
{

    public class GetByIdBrandQuery : IRequest<Result<Brand>>
    {
        public int Id { get; set; }

        public class GetBrandByIdQueryHandler : IRequestHandler<GetByIdBrandQuery, Result<Brand>>
        {
            private readonly IRepositoryAsync<Brand> _repository;
            public GetBrandByIdQueryHandler(IRepositoryAsync<Brand> repository)
            {
                _repository = repository;
            }
            public async Task<Result<Brand>> Handle(GetByIdBrandQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<Brand>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<Brand>.SuccessAsync(product);
            }
        }
    }
}
