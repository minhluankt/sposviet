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

namespace Application.Features.PurchaseOrders.Commands
{
    public partial class UpdatePurchaseOrderCommand : PurchaseOrder, IRequest<Result<bool>>
    {
        public int ComId { get; set; }
        public UpdatePurchaseOrderCommand()
        {
          
        }
    }
    public class UpdatePurchaseOrderHandler : IRequestHandler<UpdatePurchaseOrderCommand, Result<bool>>
    {
        private readonly IPurchaseOrderRepository<PurchaseOrder> _Repository;
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdatePurchaseOrderHandler(IPurchaseOrderRepository<PurchaseOrder> Repository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;

            _Repository = Repository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<bool>> Handle(UpdatePurchaseOrderCommand command, CancellationToken cancellationToken)
        {
            return await _Repository.UpdateAsync(command);
        }
    }
}
