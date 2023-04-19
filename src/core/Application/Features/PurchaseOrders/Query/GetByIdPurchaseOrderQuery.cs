using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PurchaseOrders.Query
{

    public class GetByIdPurchaseOrderQuery : IRequest<Result<PurchaseOrder>>
    {
        public EnumTypePurchaseOrder TypePurchaseOrder { get; set; }
        public int ComId { get; set; }
        public int Id { get; set; }


        public GetByIdPurchaseOrderQuery(int _comId)
        {
            ComId = _comId;
        }

        public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetByIdPurchaseOrderQuery, Result<PurchaseOrder>>
        {
            private readonly IRepositoryAsync<PurchaseOrder> _repository;
            public GetPurchaseOrderByIdQueryHandler(IRepositoryAsync<PurchaseOrder> repository)
            {
                _repository = repository;
            }
            public async Task<Result<PurchaseOrder>> Handle(GetByIdPurchaseOrderQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(x=>x.Id== query.Id && x.Comid== query.ComId&& x.Type== query.TypePurchaseOrder, x=>x.Include(x=>x.ItemPurchaseOrders).Include(x=>x.Suppliers));
                if (product == null)
                {
                    return await Result<PurchaseOrder>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<PurchaseOrder>.SuccessAsync(product);
            }
        }
    }
}
