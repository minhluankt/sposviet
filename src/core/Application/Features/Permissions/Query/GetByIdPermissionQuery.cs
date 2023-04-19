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

namespace Application.Features.Permissions.Query
{

    public class GetByIdPermissionQuery : IRequest<Result<Permission>>
    {
        public int Id { get; set; }

        public class GetPermissionByIdQueryHandler : IRequestHandler<GetByIdPermissionQuery, Result<Permission>>
        {
            private readonly IRepositoryAsync<Permission> _repository;

            public GetPermissionByIdQueryHandler(IRepositoryAsync<Permission> repository)
            {
                _repository = repository;
            }
            public async Task<Result<Permission>> Handle(GetByIdPermissionQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                return Result<Permission>.Success(product);
            }
        }
    }
}
