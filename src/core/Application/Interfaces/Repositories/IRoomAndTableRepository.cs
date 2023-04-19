using Application.Enums;
using Domain.Entities;
using Domain.ViewModel;
using System.Collections.Generic;

namespace Application.Interfaces.Repositories
{
    public interface IRoomAndTableRepository<T> where T : class
    {
        List<RoomAndTable> GetAll(int ComId);
        List<RoomAndTableModel> GetAllInOrderStatus(EnumStatusOrderTable enumStatusOrder, int ComId, EnumTypeProduct enumTypeProduct);
    }
}
