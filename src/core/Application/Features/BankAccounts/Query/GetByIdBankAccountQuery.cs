using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.BankAccounts.Query
{

    public class GetByIdBankAccountQuery : IRequest<Result<BankAccount>>
    {
        public int ComId { get; set; }
        public int Id { get; set; }


        public GetByIdBankAccountQuery(int _comId)
        {
            ComId = _comId;
        }

        public class GetBankAccountByIdQueryHandler : IRequestHandler<GetByIdBankAccountQuery, Result<BankAccount>>
        {
            private readonly IRepositoryAsync<BankAccount> _repository;
            public GetBankAccountByIdQueryHandler(IRepositoryAsync<BankAccount> repository)
            {
                _repository = repository;
            }
            public async Task<Result<BankAccount>> Handle(GetByIdBankAccountQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<BankAccount>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<BankAccount>.SuccessAsync(product);
            }
        }
    }
}
