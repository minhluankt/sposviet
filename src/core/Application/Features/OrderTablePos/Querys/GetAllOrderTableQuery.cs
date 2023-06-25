using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using Domain.XmlDataModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.OrderTablePos.Querys
{

    public class GetAllOrderTableQuery : IRequest<Result<List<OrderTableInPos>>>
    {
        public int Comid { get; set; }
        public class GetAllOrderTableQueryHandler : IRequestHandler<GetAllOrderTableQuery, Result<List<OrderTableInPos>>>
        {
            private readonly IRepositoryAsync<OrderTable> _repository;
            private readonly IRepositoryAsync<RoomAndTable> _RoomAndTablerepository;
            private readonly IManagerInvNoRepository _managerInvNoRepository;

            public GetAllOrderTableQueryHandler(IRepositoryAsync<OrderTable> repository, IRepositoryAsync<RoomAndTable> RoomAndTablerepository,
                IManagerInvNoRepository managerInvNoRepository)
            {
                _RoomAndTablerepository = RoomAndTablerepository;
                _managerInvNoRepository = managerInvNoRepository;
                _repository = repository;
            }
            public async Task<Result<List<OrderTableInPos>>> Handle(GetAllOrderTableQuery query, CancellationToken cancellationToken)
            {
                try
                {
                    var product = await _repository.Entities.Where(x=>x.ComId==query.Comid&&x.Status==Enums.EnumStatusOrderTable.DANG_DAT&&!x.IsBringBack).Include(x => x.OrderTableItems.Where(x=>x.IsServiceDate)).Include(x => x.RoomAndTable).ThenInclude(x=>x.Area).AsNoTracking().Select(x => new OrderTableInPos()
                    {
                        IsServiceDate = x.OrderTableItems.Where(x => x.IsServiceDate).Count()>0, //hiển thị thêm ngoài view để xác định và thanth oán  Getpayment
                        IdRoomAndTable = x.IdRoomAndTableGuid,
                        IdGuid = x.IdGuid,
                        Amount = x.Amonut,
                        TableName = x.RoomAndTable.Name,
                        AreaName = x.RoomAndTable.Area!=null? x.RoomAndTable.Area.Name:"",
                        Date = x.CreatedOn,
                        NumberTime = DateTime.Now.Subtract(x.CreatedOn).TotalSeconds
                    }).ToListAsync();
                    return await Result<List<OrderTableInPos>>.SuccessAsync(product);
                }
                catch (Exception e)
                {
                    return await Result<List<OrderTableInPos>>.FailAsync(e.Message);

                }
            }
        }
    }
}
