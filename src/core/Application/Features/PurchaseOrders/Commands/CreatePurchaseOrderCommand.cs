using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PurchaseOrders.Commands
{
    public partial class CreatePurchaseOrderCommand : PurchaseOrder, IRequest<Result<int>>
    {
       
        public int ComId { get; set; }
        public int? IdPurchaseOrder { get; set; }
        public ENumTypeManagerInv TypeManagerInv { get; set; }
        public CreatePurchaseOrderCommand()
        {
        }
    }
    public class CreatePurchaseOrderHandler : IRequestHandler<CreatePurchaseOrderCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
       
        private readonly IPurchaseOrderRepository<PurchaseOrder> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private readonly IManagerInvNoRepository _managerInvNoRepository;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreatePurchaseOrderHandler(IPurchaseOrderRepository<PurchaseOrder> brandRepository,
             IFormFileHelperRepository fileHelper, IManagerInvNoRepository managerInvNoRepository,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _managerInvNoRepository = managerInvNoRepository;
            _fileHelper = fileHelper;
          
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
          
            try
            {
                if (request.Comid==0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR019);
                }
                var map = _mapper.Map<PurchaseOrder>(request);
                string code = string.Empty;
                if (request.IdPurchaseOrder!=null)
                {
                    var chck = await _Repository.GetByIdAsync(request.IdPurchaseOrder.Value);
                    if (chck!=null)
                    {
                        map.PurchaseOrderCode= chck.PurchaseNo;
                    }
                }
               
                await _Repository.AddAsync(map);
              
                return Result<int>.Success();
            }
            catch (System.Exception e)
            {
              
                return await Result<int>.FailAsync(e.Message);
            }
           
        }
    }
}
