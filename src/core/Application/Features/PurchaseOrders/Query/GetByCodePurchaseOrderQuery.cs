using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;

using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PurchaseOrders.Query
{
    
    public class GetByCodePurchaseOrderQuery : IRequest<Result<List<PurchaseOrder>>>
    {
        public EnumTypePurchaseOrder TypePurchaseOrder { get; set; }
        public int ComId { get; set; }
        public string Code { get; set; }


        public GetByCodePurchaseOrderQuery(int _comId)
        {
            ComId = _comId;
        }

        public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetByCodePurchaseOrderQuery, Result<List<PurchaseOrder>>>
        {
            private readonly IRepositoryAsync<PurchaseOrder> _repository;
            public GetPurchaseOrderByIdQueryHandler(IRepositoryAsync<PurchaseOrder> repository)
            {
                _repository = repository;
            }
            public async Task<Result<List<PurchaseOrder>>> Handle(GetByCodePurchaseOrderQuery query, CancellationToken cancellationToken)
            {
                var product =  _repository.GetMulti(x => x.PurchaseOrderCode == query.Code && x.Comid == query.ComId && x.Type == query.TypePurchaseOrder, x => x.Include(x => x.ItemPurchaseOrders));
                if (product == null)
                {
                    return await Result<List<PurchaseOrder>>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<List<PurchaseOrder>>.SuccessAsync(await product.ToListAsync());
            }
        }
    }
}
