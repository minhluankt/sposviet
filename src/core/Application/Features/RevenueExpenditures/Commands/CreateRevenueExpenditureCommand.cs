using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.RevenueExpenditures.Commands
{
    public partial class CreateRevenueExpenditureCommand : RevenueExpenditure, IRequest<Result<int>>
    {
        public CreateRevenueExpenditureCommand(int _comId)
        {
            ComId = _comId;
        }
    }
    public class CreateRevenueExpenditureHandler : IRequestHandler<CreateRevenueExpenditureCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IManagerInvNoRepository _managerInvNorepository;
        private readonly IRepositoryAsync<RevenueExpenditure> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateRevenueExpenditureHandler(IRepositoryAsync<RevenueExpenditure> brandRepository, IManagerInvNoRepository managerInvNorepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _managerInvNorepository = managerInvNorepository;
            _fileHelper = fileHelper;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateRevenueExpenditureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var code = await _managerInvNorepository.UpdateInvNo(request.ComId,(request.Type==EnumTypeRevenueExpenditure.THU? ENumTypeManagerInv.Receipts: ENumTypeManagerInv.Payment) ,false);

                var map = _mapper.Map<RevenueExpenditure>(request);
                if (request.Type==EnumTypeRevenueExpenditure.CHI)
                {
                    map.Code = $"PC{request.ComId}{(code.ToString("00000000"))}";
                }
                else if (request.Type == EnumTypeRevenueExpenditure.THU)
                {
                    map.Code = $"PT{request.ComId}{(code.ToString("00000000"))}";
                }
                else
                {
                    return Result<int>.Fail("Không đúng loại phiếu");
                }
              

                await _Repository.AddAsync(map);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success();
            }
            catch (System.Exception e)
            {
                return Result<int>.Fail(e.ToString());
            }
           
        }
    }
}
