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


namespace Application.Features.Mail.Querys
{

    public class GetAllEmailHistoryQuery : IRequest<Result<IQueryable<Mailhistory>>>
    {
        public int Id { get; set; }

        public class GetAllEmailHistoryQueryHandler : IRequestHandler<GetAllEmailHistoryQuery, Result<IQueryable<Mailhistory>>>
        {
            private readonly IRepositoryAsync<Mailhistory> _repository;

            public GetAllEmailHistoryQueryHandler(IRepositoryAsync<Mailhistory> repository)
            {
                _repository = repository;
            }
            public async Task<Result<IQueryable<Mailhistory>>> Handle(GetAllEmailHistoryQuery query, CancellationToken cancellationToken)
            {
                var product = _repository.GetAllQueryable().OrderByDescending(m => m.Id);
                return await Result<IQueryable<Mailhistory>>.SuccessAsync(product);
            }
        }
    }
}
