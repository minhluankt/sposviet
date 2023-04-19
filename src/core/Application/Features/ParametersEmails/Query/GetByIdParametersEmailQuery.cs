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

namespace Application.Features.ParametersEmails.Query
{

    public class GetByIdParametersEmailQuery : IRequest<Result<ParametersEmail>>
    {
        public int Id { get; set; }

        public class GetParametersEmailByIdQueryHandler : IRequestHandler<GetByIdParametersEmailQuery, Result<ParametersEmail>>
        {
            private readonly IRepositoryAsync<ParametersEmail> _repository;

            public GetParametersEmailByIdQueryHandler(IRepositoryAsync<ParametersEmail> repository)
            {
                _repository = repository;
            }
            public async Task<Result<ParametersEmail>> Handle(GetByIdParametersEmailQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                return Result<ParametersEmail>.Success(product);
            }
        }
    }
}
