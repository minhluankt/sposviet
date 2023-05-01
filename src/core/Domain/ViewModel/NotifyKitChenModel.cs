using Application.Enums;
using System;
using System.Collections.Generic;

namespace Domain.ViewModel
{
    public class NotifyKitChenModel
    {
        public string Cashername { get; set; }
        public Guid? IdOrder { get; set; }
        public Guid? idChitken { get; set; }
        public string orderCode { get; set; }
        public int? IdProduct { get; set; }
        public EnumTypeNotifyKitchenOrder TypeNotifyKitchenOrder { get; set; } = EnumTypeNotifyKitchenOrder.NOTIFYCHITKEN;
        public EnumStatusKitchenOrder Status { get; set; } = EnumStatusKitchenOrder.READY;
        public int ComId { get; set; }
        public bool UpdateOne { get; set; }
        public bool UpdateFull { get; set; }
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
    public class OrderByPrioritiesModel
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

    public class OrderByRoomModel//theo phòng
    {
        public Guid? idRoomTable { get; set; }
        public string tableName { get; set; }
        public string orderCode { get; set; }
        public Guid? idOrder { get; set; }
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
        public List<DetailtKitchenModel> detailtKitchenModels { get; set; }
    }
    public class OrderByFoodModel//theo món ăn
    {
        public string proName { get; set; }
        public string proCode { get; set; }
        public int? idProduct { get; set; }
        public decimal quantity { get; set; }
    }
}
