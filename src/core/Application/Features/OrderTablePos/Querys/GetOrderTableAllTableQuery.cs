using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.OrderTablePos.Querys
{

    public class GetOrderTableAllTableQuery : IRequest<Result<List<RoomAndTableModel>>>
    {
        public int Comid { get; set; }
        public EnumTypeProduct enumTypeProduct { get; set; }

        public class GetOrderTableAllTableQueryHandler : IRequestHandler<GetOrderTableAllTableQuery, Result<List<RoomAndTableModel>>>
        {
            private readonly IOrderTableRepository _repository;
            private readonly IRoomAndTableRepository<RoomAndTable> _RoomAndTablerepository;
            private readonly IManagerInvNoRepository _managerInvNoRepository;

            public GetOrderTableAllTableQueryHandler(IOrderTableRepository repository, IRoomAndTableRepository<RoomAndTable> RoomAndTablerepository,
                IManagerInvNoRepository managerInvNoRepository)
            {
                _RoomAndTablerepository = RoomAndTablerepository;
                _managerInvNoRepository = managerInvNoRepository;
                _repository = repository;
            }
            public async Task<Result<List<RoomAndTableModel>>> Handle(GetOrderTableAllTableQuery query, CancellationToken cancellationToken)
            {
                List<RoomAndTableModel> roomAndTableModel = new List<RoomAndTableModel>();
                var getMangve = _repository.GetOrderByBringback(query.Comid, EnumStatusOrderTable.DANG_DAT, query.enumTypeProduct).ToList();
                var getAll = _RoomAndTablerepository.GetAllInOrderStatus(EnumStatusOrderTable.DANG_DAT, query.Comid, query.enumTypeProduct);

                //foreach (var item in getAll)
                //{
                //    roomAndTableModel.Add(new RoomAndTableModel() { Idtable = item.IdGuid, IsOrder = item.OrderTables.Count() > 0 });
                //}
                roomAndTableModel = getAll;
                if (getMangve.Count() > 0)
                {
                    roomAndTableModel.Add(new RoomAndTableModel() { Idtable = Guid.NewGuid(), IsBringBack = true, IsOrder = true });
                }

                return await Result<List<RoomAndTableModel>>.SuccessAsync(roomAndTableModel);
            }
        }
    }
}
