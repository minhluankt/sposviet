using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.BankAccounts.Commands
{
    public partial class UpdateBankAccountCommand : BankAccount, IRequest<Result<int>>
    {
        public UpdateBankAccountCommand(int _comId)
        {
            ComId = _comId;
        }
        public IFormFile Img { get; set; }
    }
    public class UpdateBankAccountHandler : IRequestHandler<UpdateBankAccountCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<BankAccount> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateBankAccountHandler(IRepositoryAsync<BankAccount> brandRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;

            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateBankAccountCommand command, CancellationToken cancellationToken)
        {
            var brand = await _Repository.GetByIdAsync(command.Id);
            if (brand == null)
            {
                return await Result<int>.FailAsync(HeperConstantss.ERR012);
            }
            else
            {
                brand.BankNumber = command.BankNumber.Trim();
                brand.BankAddress = command.BankAddress;
                brand.BankName = command.BankName;
                brand.AccountName = command.AccountName;
                brand.BinVietQR = command.BinVietQR;
                brand.Active = command.Active;
               

                var checkcode = await _Repository.Entities.CountAsync(predicate: m => m.BankNumber == brand.BankNumber && m.Id != brand.Id && m.ComId==brand.ComId);
                if (checkcode > 0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR014);
                }
                if (command.IsSetDefault != brand.IsSetDefault && command.IsSetDefault)
                {
                    await _Repository.Entities.Where(x => x.ComId == command.ComId && x.Id!= brand.Id).ForEachAsync(x => x.IsSetDefault = false);
                }
                brand.IsSetDefault = command.IsSetDefault;
                await _Repository.UpdateAsync(brand);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(brand.Id);
            }
        }
    }
}
