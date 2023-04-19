using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Specification.Query
{

    public class GetByIdSpecificationQuery : IRequest<Result<Specifications>>
    {
        public int Id { get; set; }

        public class GetSpecificationByIdQueryHandler : IRequestHandler<GetByIdSpecificationQuery, Result<Specifications>>
        {
            private readonly IRepositoryAsync<Specifications> _repository;
            public GetSpecificationByIdQueryHandler(IRepositoryAsync<Specifications> repository)
            {
                _repository = repository;
            }
            public async Task<Result<Specifications>> Handle(GetByIdSpecificationQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetMulti(m => m.Id == query.Id).SingleOrDefaultAsync();
                if (product == null)
                {
                    return await Result<Specifications>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<Specifications>.SuccessAsync(product);
            }
        }
    }
}
