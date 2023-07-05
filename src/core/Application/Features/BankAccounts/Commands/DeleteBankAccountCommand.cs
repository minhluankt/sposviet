using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;

using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.BankAccounts.Commands
{

    public class DeleteBankAccountCommand : IRequest<Result>
    {
        public DeleteBankAccountCommand(int _comId, int _id)
        {
            ComId = _comId;
            Id = _id;
        }
        public int ComId { get; set; }
        public int Id { get; set; }
        public class DeleteBankAccountHandler : IRequestHandler<DeleteBankAccountCommand, Result>
        {

            private readonly IBankAccountRepository _Repository;

            public DeleteBankAccountHandler(IBankAccountRepository repository)
            {
                _Repository = repository;
            }
            public async Task<Result> Handle(DeleteBankAccountCommand command, CancellationToken cancellationToken)
            {
                try
                {
                    return await _Repository.DeleteAsync(command.ComId,command.Id);
                }
                catch (System.Exception e)
                {
                    return Result<int>.Fail(e.Message);
                }
                
            }
        }
    }
}
