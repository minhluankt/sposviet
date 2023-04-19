using Application.Enums;
using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using Infrastructure.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Infrastructure.Repositories
{
    public class RoomAndTableRepository : IRoomAndTableRepository<RoomAndTable>
    {
        private readonly IRepositoryAsync<RoomAndTable> _repositoryRoomAndTable;
        private readonly IRepositoryAsync<OrderTable> _OrderTablerepository;
        private UserManager<ApplicationUser> _userManager;
        private IOptions<CryptoEngine.Secrets> _config;

        public RoomAndTableRepository(
            IRepositoryAsync<RoomAndTable> repositoryRoomAndTable, IRepositoryAsync<OrderTable> OrderTablerepository,
            UserManager<ApplicationUser> userManager,
            IOptions<CryptoEngine.Secrets> config)
        {
            _OrderTablerepository = OrderTablerepository;

            _repositoryRoomAndTable = repositoryRoomAndTable;
            _userManager = userManager;
            _config = config;
        }
        public List<RoomAndTable> GetAll(int ComId)
        {
            return _repositoryRoomAndTable.GetAllQueryable().Where(x => x.ComId == ComId).Include(x=>x.Area).ToList();
        }

        public List<RoomAndTableModel> GetAllInOrderStatus(EnumStatusOrderTable enumStatusOrder, int ComId, EnumTypeProduct enumTypeProduct)
        {
            var s = from tb in _repositoryRoomAndTable.Entities
                    join od in _OrderTablerepository.Entities on tb.IdGuid equals od.IdRoomAndTableGuid
                    where tb.ComId == ComId && od.Status == enumStatusOrder && od.TypeProduct == enumTypeProduct
                    select new RoomAndTableModel()
                    {
                        Idtable = tb.IdGuid,
                        IsOrder = true,
                    };

            return s.ToList();
            // return await _repositoryRoomAndTable.GetAllQueryable().Where(x => x.ComId == ComId && x.Active).Include(x => x.OrderTables.Where(x => x.Status == enumStatusOrder)).ToListAsync();
        }
    }
}
