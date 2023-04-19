using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.OrderTablePos.Querys
{

    public class GetOrderOneQuery : IRequest<Result<OrderTable>>
    {
        public int Comid { get; set; }
        public Guid? IdRoomAndTable { get; set; }
        public Guid? IdOrder { get; set; }
        public EnumStatusOrderTable Status { get; set; } = EnumStatusOrderTable.DANG_DAT;
        public EnumTypeProduct TypeProduct { get; set; } = EnumTypeProduct.AMTHUC;
        public bool IsBringback { get; set; }
        public bool IncludeItem { get; set; } = true;
        public bool OutRoom { get; set; } = true;

        public class GetOrderOneQueryHandler : IRequestHandler<GetOrderOneQuery, Result<OrderTable>>
        {
            private readonly IRepositoryAsync<OrderTable> _repository;

            public GetOrderOneQueryHandler(IRepositoryAsync<OrderTable> repository)
            {
                _repository = repository;
            }
            public async Task<Result<OrderTable>> Handle(GetOrderOneQuery query, CancellationToken cancellationToken)
            {
                var product = _repository.GetAllQueryable().AsNoTracking().Where(x => x.ComId == query.Comid && x.Status == query.Status && x.TypeProduct == query.TypeProduct);
                
                if (query.IncludeItem)
                {
                    product = product.Include(x => x.OrderTableItems);
                }
                if (query.OutRoom)
                {
                    product = product.Include(x => x.RoomAndTable);
                }
                if (query.IdRoomAndTable != null)
                {
                    product = product.Where(x => x.IdRoomAndTableGuid == query.IdRoomAndTable);
                }
                if (query.IsBringback)
                {
                    product = product.Where(x => x.IsBringBack);
                }
                if (query.IdOrder != null)
                {
                    product = product.Where(x => x.IdGuid == query.IdOrder);
                }
                
                return await Result<OrderTable>.SuccessAsync(await product.SingleOrDefaultAsync());
            }
        }
    }
}
