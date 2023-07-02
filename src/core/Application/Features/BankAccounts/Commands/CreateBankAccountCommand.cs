using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.BankAccounts.Commands
{
    public partial class CreateBankAccountCommand : BankAccount, IRequest<Result<int>>
    {
        public CreateBankAccountCommand(int _comId)
        {
            ComId = _comId;
        }
    }
    public class CreateBankAccountHandler : IRequestHandler<CreateBankAccountCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IRepositoryAsync<BankAccount> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateBankAccountHandler(IRepositoryAsync<BankAccount> brandRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateBankAccountCommand request, CancellationToken cancellationToken)
        {
            BankAccount roomAnds = new BankAccount();
            var map = _mapper.Map<BankAccount>(request);
            var fidn = await _Repository.Entities.Where(m => m.ComId == request.ComId && m.BankNumber == map.BankNumber).SingleOrDefaultAsync();
            if (fidn != null)
            {
                return Result<int>.Fail(HeperConstantss.ERR014);
            }
            await _Repository.AddAsync(map);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success();
        }
    }
}
