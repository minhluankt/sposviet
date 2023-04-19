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

namespace Application.Features.TypeCategorys.Query
{

    public class GetByIdTypeCategoryQuery : IRequest<Result<TypeCategory>>
    {
        public int Id { get; set; }

        public class GetTypeCategoryByIdQueryHandler : IRequestHandler<GetByIdTypeCategoryQuery, Result<TypeCategory>>
        {
            private readonly IRepositoryAsync<TypeCategory> _repository;
            public GetTypeCategoryByIdQueryHandler(IRepositoryAsync<TypeCategory> repository)
            {
                _repository = repository;
            }
            public async Task<Result<TypeCategory>> Handle(GetByIdTypeCategoryQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<TypeCategory>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<TypeCategory>.SuccessAsync(product);
            }
        }
    }
}
