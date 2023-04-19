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

namespace Application.Features.TypeSpecification.Query
{

    public class GetByIdTypeSpecificationsQuery : IRequest<Result<TypeSpecifications>>
    {
        public int Id { get; set; }

        public class GetTypeSpecificationsByIdQueryHandler : IRequestHandler<GetByIdTypeSpecificationsQuery, Result<TypeSpecifications>>
        {
            private readonly IRepositoryAsync<TypeSpecifications> _repository;
            public GetTypeSpecificationsByIdQueryHandler(IRepositoryAsync<TypeSpecifications> repository)
            {
                _repository = repository;
            }
            public async Task<Result<TypeSpecifications>> Handle(GetByIdTypeSpecificationsQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<TypeSpecifications>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<TypeSpecifications>.SuccessAsync(product);
            }
        }
    }
}
