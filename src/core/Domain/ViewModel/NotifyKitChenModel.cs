using Application.Enums;
using System;
using System.Collections.Generic;

namespace Domain.ViewModel
{
    public class NotifyKitChenModel
    {
        public string Cashername { get; set; }
        public string IdStaff { get; set; }
        public Guid? IdOrder { get; set; }
        public Guid? idChitken { get; set; }
        public int? Id { get; set; }//id int chitken
        public string ProName { get; set; }
        public string orderCode { get; set; }
        public int? IdProduct { get; set; }
        public EnumTypeNotifyKitchenOrder TypeNotifyKitchenOrder { get; set; } = EnumTypeNotifyKitchenOrder.NOTIFYCHITKEN;
        public EnumStatusKitchenOrder Status { get; set; } = EnumStatusKitchenOrder.READY;
        public EnumTypeNotifyKitChen TypeNotifyKitChen { get; set; } = EnumTypeNotifyKitChen.NHA_BEP_1;//LOẠI MÀN HÌNH NHÀ BẾP
        public int ComId { get; set; }
        public int? IdOrderItem { get; set; }
        public int Quantity { get; set; }
        public bool UpdateOne { get; set; }
        public bool UpdateFull { get; set; }
        public bool IsProgress { get; set; }//dg nhận món
        public bool IsCancel { get; set; }//
        public int[] lstIdChiken { get; set; }
        public Guid[] ListIdChitken { get; set; }//list này để thông báo
    }
    public class KitChenModel
    {
        public KitChenModel()
        {
            this.OrderByPrioritiesModels = new List<OrderByPrioritiesModel>();
            this.OrderByRoomModels = new List<OrderByRoomModel>();
            this.OrderByFoodModels = new List<OrderByFoodModel>();
        }
        public List<OrderByPrioritiesModel> OrderByPrioritiesModels { get; set; }
        public List<OrderByRoomModel> OrderByRoomModels { get; set; }
        public List<OrderByFoodModel> OrderByFoodModels { get; set; }


    }
    public class KitChenTableListModel//dùng cho gộp theo bàn 
    {
        public KitChenTableListModel()
        {
            this.OrderDetailByOrders = new List<OrderDetailByOrder>();
        }
        public string createDateTable { get; set; }
        public double TimeSpan { get; set; }//giây đã trôi qua so với thời gian tạo đươn và thời gian heinej tại
        public decimal quantity { get; set; }
        public string tableName { get; set; }
        public Guid? idRoomTable { get; set; }
        public List<OrderDetailByOrder> OrderDetailByOrders { get; set; }
        
    }
    public class KitChenTableModel
    {
        public List<KitChenTableListModel> KitChenTableListModels { get; set; }
        public List<OrderByFoodModel> OrderByFoodModels { get; set; }
    }

    public class OrderByPrioritiesModel//chi tiết
    {
        public OrderByPrioritiesModel()
        {
            this.detailtKitchenModels = new List<DetailtKitchenModel>();
        }
        public string proName { get; set; }
        public DateTime? updateDate { get; set; }
        public DateTime? dateReady { get; set; }
        public DateTime timeAgo { get; set; }
        public string createDate { get; set; }
        public string orderStaff { get; set; }
        public string orderCode { get; set; }
        public string Note { get; set; }
        public Guid idKitchen { get; set; }
        public decimal quantity { get; set; }
        public string tableName { get; set; }
        public List<DetailtKitchenModel> detailtKitchenModels { get; set; }

    }

    public class DetailtKitchenModel//dùng để hủy
    {
        public DetailtKitchenModel()
        {
            this.Status = EnumStatusKitchenOrder.CANCEL;
        }
        public int Id { get; set; }
        public int IdKitchen { get; set; }
        public decimal Quantity { get; set; }
        public string IdCashername { get; set; }
        public bool IsRemove { get; set; }
        public string Cashername { get; set; }
        public string Note { get; set; }
        public DateTime? DateCancel { get; set; }
        public EnumTypeKitchenOrder TypeKitchenOrder { get; set; }
        public EnumStatusKitchenOrder Status { get; set; }
    }
    public class OrderDetailByOrder//chi tiết của bảng order và chitken join bảng
    {
        public EnumStatusKitchenOrder Status { get; set; }
        public string Note { get; set; }
        public string proCode { get; set; }
        public string proName { get; set; }
        public DateTime createDate { get; set; }
        public string createDateTable { get; set; }
        public string orderStaff { get; set; }
        public string idStaff { get; set; }
        public int? idProduct { get; set; }
        public Guid? idRoomTable { get; set; }
        public string tableName { get; set; }
        public string orderCode { get; set; }
        public string createDateFood { get; set; }
        public Guid? idOrder { get; set; }
        public Guid? IdChitKen { get; set; }
        public int? IdIntChitKen { get; set; }
        public int IdItemOrder { get; set; }
        public decimal quantity { get; set; }
    }
    public class OrderByRoomModel//theo phòng
    {
        public int? idProduct { get; set; }
        public Guid? idRoomTable { get; set; }
        public string tableName { get; set; }
        public string orderCode { get; set; }
        public string createDateFood { get; set; }
        public Guid? idOrder { get; set; }
        public int IdItemOrder { get; set; }
        public decimal quantity { get; set; }
        public List<OrderByRoomDetailtModel> OrderByRoomDetailtModels { get; set; }
    }

    public class OrderByRoomDetailtModel//theo phòng
    {
        public Guid idKitchen { get; set; }
        public DateTime? updateDate { get; set; }
        public DateTime? dateReady { get; set; }
        public string proName { get; set; }
        public string createDate { get; set; }
        public string orderStaff { get; set; }
        public decimal quantity { get; set; }
        public string tableName { get; set; }
        public string Note { get; set; }
        public List<DetailtKitchenModel> detailtKitchenModels { get; set; }
    }
    public class OrderByFoodModel//theo món ăn
    {
        public OrderByFoodModel()
        {
            this.OrderDetailByOrders = new List<OrderDetailByOrder>();
        }
        public string proName { get; set; }
        public string proCode { get; set; }
        public string note { get; set; }
        public int? idProduct { get; set; }
        public decimal quantity { get; set; }
        public List<OrderDetailByOrder> OrderDetailByOrders { get; set; }//trường hợp cho loại nhà bếp 2
    }
}
