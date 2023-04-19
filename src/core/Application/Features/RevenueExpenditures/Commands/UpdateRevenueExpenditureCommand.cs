using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.RevenueExpenditures.Commands
{
    public partial class UpdateRevenueExpenditureCommand : RevenueExpenditure, IRequest<Result<int>>
    {
        public UpdateRevenueExpenditureCommand(int _comId)
        {
            ComId = _comId;
        }
        public IFormFile Img { get; set; }
    }
    public class UpdateRevenueExpenditureHandler : IRequestHandler<UpdateRevenueExpenditureCommand, Result<int>>
    {
        private readonly IManagerInvNoRepository _managerInvNorepository;
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<RevenueExpenditure> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateRevenueExpenditureHandler(IRepositoryAsync<RevenueExpenditure> brandRepository, IManagerInvNoRepository managerInvNorepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _managerInvNorepository = managerInvNorepository;
            _fileHelper = fileHelper;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateRevenueExpenditureCommand command, CancellationToken cancellationToken)
        {
            var brand = await _Repository.GetByIdAsync(command.Id);
            if (brand == null)
            {
                return await Result<int>.FailAsync(HeperConstantss.ERR012);
            }
            else
            {
                brand.Date = command.Date;
                brand.CodeOriginaldocument = command.CodeOriginaldocument;
                await _Repository.UpdateAsync(brand);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(brand.Id);
            }
        }
    }
}
