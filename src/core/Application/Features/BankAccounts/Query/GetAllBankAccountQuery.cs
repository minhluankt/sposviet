using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.BankAccounts.Query
{
    public class GetAllBankAccountQuery : DatatableModel, IRequest<Result<List<BankAccount>>>
    {
        public string Name { get; set; }
        public int IdBankAccount { get; set; }
        public int ComId { get; set; }
        public bool Paging { get; set; } = true;
        public GetAllBankAccountQuery(int _comId)
        {
            ComId = _comId;
        }
    }

    public class GetAllBankAccountCachedQueryHandler : IRequestHandler<GetAllBankAccountQuery, Result<List<BankAccount>>>
    {

        private readonly IRepositoryAsync<BankAccount> _repository;
        private readonly IMapper _mapper;

        public GetAllBankAccountCachedQueryHandler(
            IRepositoryAsync<BankAccount> repository,


            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<BankAccount>>> Handle(GetAllBankAccountQuery request, CancellationToken cancellationToken)
        {
            IQueryable<BankAccount> lst = _repository.GetAllQueryable().Where(x => x.ComId == request.ComId).AsNoTracking();
            if (!string.IsNullOrEmpty(request.Name))
            {
                lst = lst.Where(x => x.BankName.ToLower().Contains(request.Name.ToLower()));
            }
            if (request.IdBankAccount > 0)
            {
                lst = lst.Where(x => x.Id == request.IdBankAccount);
            }
           
            if (!request.Paging)
            {
                return Result<List<BankAccount>>.Success(await lst.OrderByDescending(m => m.Id).ToListAsync(), lst.Count().ToString());
            }
            return Result<List<BankAccount>>.Success(await lst.OrderByDescending(m => m.Id).Skip(request.skip).Take(request.pageSize).ToListAsync(), lst.Count().ToString());
        }
    }
}
