using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.BankAccounts.Query
{
 
    public class GetPageListQuery : EntitySearchModel, IRequest<Result<PaginatedList<BankAccount>>>
    {
        public GetPageListQuery(int _comId)
        {
            Comid = _comId;
        }

        public class GetAreaByIdQueryHandler : IRequestHandler<GetPageListQuery, Result<PaginatedList<BankAccount>>>
        {
            private readonly IBankAccountRepository _repository;
            public GetAreaByIdQueryHandler(IBankAccountRepository repository)
            {
                _repository = repository;
            }
            public async Task<Result<PaginatedList<BankAccount>>> Handle(GetPageListQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetAllAsync(query);
                if (product == null)
                {
                    return await Result<PaginatedList<BankAccount>>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<PaginatedList<BankAccount>>.SuccessAsync(product);
            }
        }
    }
}
