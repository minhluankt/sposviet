using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.Mail.Querys
{

    public class GetByIdMailHistoryQuery : IRequest<Result<Mailhistory>>
    {
        public int Id { get; set; }

        public class GetMailHistoryByIdQueryHandler : IRequestHandler<GetByIdMailHistoryQuery, Result<Mailhistory>>
        {
            private readonly IRepositoryAsync<Mailhistory> _repository;

            public GetMailHistoryByIdQueryHandler(IRepositoryAsync<Mailhistory> repository)
            {
                _repository = repository;
            }
            public async Task<Result<Mailhistory>> Handle(GetByIdMailHistoryQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return Result<Mailhistory>.Fail(HeperConstantss.ERR012);
                }
                return Result<Mailhistory>.Success(product);
            }
        }
    }
}
