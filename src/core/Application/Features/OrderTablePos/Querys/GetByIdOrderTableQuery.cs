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

    public class GetByIdOrderTableQuery : IRequest<Result<List<OrderTable>>>
    {
        public Guid? IdRoomAndTable { get; set; }
        public Guid? idinvoice { get; set; }
        public Guid? IdOrder { get; set; }
        public Guid? IdOrderNotIn { get; set; }// udngf để where trừ nó ra
        public int Comid { get; set; }
        public bool IsBringBack { get; set; }
        public bool IncludeItem { get; set; } = true;
        public bool IncludeCustomer { get; set; } = true;
        public bool OutInvNo { get; set; } = true;
        public bool OutRoom { get; set; }
        public bool IsStopService { get; set; }
        public EnumStatusOrderTable Status { get; set; } = EnumStatusOrderTable.DANG_DAT;
        public EnumTypeProduct TypeProduct { get; set; } = EnumTypeProduct.AMTHUC;


        public class GetOrderTableByIdQueryHandler : IRequestHandler<GetByIdOrderTableQuery, Result<List<OrderTable>>>
        {
            private readonly IRepositoryAsync<OrderTable> _repository;
            private readonly IRepositoryAsync<RoomAndTable> _RoomAndTablerepository;
            private readonly IManagerInvNoRepository _managerInvNoRepository;

            public GetOrderTableByIdQueryHandler(IRepositoryAsync<OrderTable> repository, IRepositoryAsync<RoomAndTable> RoomAndTablerepository,
                IManagerInvNoRepository managerInvNoRepository)
            {
                _RoomAndTablerepository = RoomAndTablerepository;
                _managerInvNoRepository = managerInvNoRepository;
                _repository = repository;
            }
            public async Task<Result<List<OrderTable>>> Handle(GetByIdOrderTableQuery query, CancellationToken cancellationToken)
            {

                int OutInvNo = 0;
                if (query.IsBringBack)
                {
                    if (query.OutInvNo)
                    {
                        var getInvno = await _managerInvNoRepository.GetInvNoAsync(query.Comid, ENumTypeManagerInv.OrderTable);
                        OutInvNo = getInvno + 1;
                    }
                    var product = _repository.GetMulti(x => x.IsBringBack && x.ComId == query.Comid && x.TypeProduct == query.TypeProduct && x.Status == query.Status, x => x.Include(x => x.OrderTableItems).Include(x => x.Customer)).AsNoTracking();

                    return await Result<List<OrderTable>>.SuccessAsync(await product.OrderByDescending(x => x.Id).ToListAsync(), OutInvNo.ToString());
                }
                else
                {
                    //var table = await _RoomAndTablerepository.Entities.SingleOrDefaultAsync(x => x.IdGuid == query.IdRoomAndTable && x.ComId == query.Comid);
                    //if (table == null)
                    //{
                    //    return await Result<List<OrderTable>>.SuccessAsync(new List<OrderTable>(), (getInvno + 1).ToString());
                    //}
                    //  var product = _repository.GetMulti(x => x.ComId == query.Comid && x.Status == EnumStatusOrderTable.DANG_DAT, x => x.Include(x => x.OrderTableItems).Include(x => x.Customer));
                    var product = _repository.GetAllQueryable().IgnoreQueryFilters().AsNoTracking().Where(x => x.ComId == query.Comid && x.Status == query.Status && x.TypeProduct == query.TypeProduct);
                    if (query.TypeProduct == EnumTypeProduct.BAN_LE || query.TypeProduct == EnumTypeProduct.TAPHOA_SIEUTHI)
                    {
                        if (query.idinvoice != null)
                        {
                            product = product.Where(x => x.IdGuid == query.idinvoice);
                        }
                    }
                    if (query.IncludeItem)
                    {
                        product = product.Include(x => x.OrderTableItems);
                    }
                    if (query.IncludeCustomer)
                    {
                        product = product.Include(x => x.Customer);
                    }
                    if (query.OutRoom)
                    {
                        product = product.Include(x => x.RoomAndTable);
                    }
                    if (query.IdRoomAndTable != null)
                    {
                        product = product.Where(x => x.IdRoomAndTableGuid == query.IdRoomAndTable);
                    }
                    if (query.IdOrder != null)
                    {
                        product = product.Where(x => x.IdGuid == query.IdOrder);
                    }
                    if (query.IdOrderNotIn != null)
                    {
                        product = product.Where(x => x.IdGuid != query.IdOrderNotIn);
                    }
                    if (query.OutInvNo)
                    {
                        var getInvno = await _managerInvNoRepository.GetInvNoAsync(query.Comid, ENumTypeManagerInv.OrderTable);
                        OutInvNo = getInvno + 1;
                    }
                    return await Result<List<OrderTable>>.SuccessAsync(await product.OrderByDescending(x => x.Id).ToListAsync(), OutInvNo.ToString());
                }
            }
        }
    }
}
