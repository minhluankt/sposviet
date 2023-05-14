using Application.Enums;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface INotifyChitkenRepository
    {
        Task UpdateNotifyAllByRoomTable(int Comid, OrderTable order, EnumTypeProduct IdDichVu = EnumTypeProduct.AMTHUC);// đổi bàn
        Task<Result<List<NotifyOrderNewModel>>> NotifyOrder(int Comid, Guid Idorder,string Cashername, EnumTypeProduct IdDichVu = EnumTypeProduct.AMTHUC);
        Task NotifyOrderByItem(List<OrderTableItem> entity, OrderTable order, string CasherName, string IdCasher);// thông báo mới theo item chỉ định 
        Task UpdateNotifyKitchenSpitOrderGraftAsync(int ComId, List<Guid> lstOrderOld, OrderTable ordernew, List<OrderTableItem> itemlistbyordernew, EnumTypeProduct enumTypeProduct = EnumTypeProduct.AMTHUC);
        Task<Result<int>> UpdateNotifyAllStatusOrder(int Comid, Guid[] IdKitchen, EnumTypeNotifyKitchenOrder typeupdate = EnumTypeNotifyKitchenOrder.Orocessed, EnumStatusKitchenOrder Status = EnumStatusKitchenOrder.MOI, EnumTypeProduct IdDichVu = EnumTypeProduct.AMTHUC);
        Task<Result<int>> UpdateNotifyOrder(int Comid, Guid? IdOrder, Guid? IdKitchen, int? IdProduct, bool UpdateOne, EnumTypeNotifyKitchenOrder typeupdate, EnumStatusKitchenOrder Status, EnumTypeProduct IdDichVu = EnumTypeProduct.AMTHUC);
        IQueryable<Kitchen> GetAllNotifyOrder(int Comid, EnumStatusKitchenOrder status = EnumStatusKitchenOrder.MOI);
        Task<KitChenTableModel> GetAllNotifyOrderByTable(int Comid);
        Task<Kitchen> UpdateNotifyProcessingFood(int Comid,int IdChitKen,string StaffName, EnumStatusKitchenOrder status=EnumStatusKitchenOrder.Processing);
        Task<IResult<List<Kitchen>>> UpdateNotifyDoneByProduct(int Comid, int IdProduct, int Quantity);//Quantity để xác định là người nhấn với số lượng món dg hiển thị khi họ xem check đúng số lượng mới update, vì khi lúc nhân viên chọn báo bếp món mới, trùng với lúc bếp nhấn done thì dễ bị update all sản phẩm đó
        Task UpdateNotifyKitchenCancelAsync(int Comid, Guid IdOrder, int IdProduct, decimal Quantity, string CasherName, string IdCasher, bool removeAllfood = false);
        Task<List<NotifyOrderNewModel>> UpdateNotifyKitchenCancelListAsync(List<Kitchen> entity, int ComId,string CasherName, string IdCashername);
        Task UpdateNotifyKitchenDoneListAsync(List<OrderTableItem> entity, OrderTable newOrder, Guid idorderOld, int ComId);
        Task UpdateNotifyKitchenSpitOrderAsync(OrderTable OrderOld, List<OrderTableItem> lstorderold, int ComId, OrderTable newOrder, List<OrderTableItem> lstordernew = null, bool isCreatNewOrder = false,DateTime? _createDateHis = null);//hủy khi tách đơn
        Task UpdateNotifyKitchenTachdonVaoDonDacoAsync(OrderTable OrderOld, List<OrderTableItem> lstorderold, int ComId, OrderTable newOrder, List<OrderTableItem> lstordernew, List<OrderTableItem> lstorderoldremove, string CasherName, string IdCasher);//dùng cho tách đơn dạng đươn đã có
    }
}
