using Application.Enums;
using System;
using System.Collections.Generic;

namespace Domain.ViewModel
{
    public class SplitOrderModel
    {
        public List<DetailtSpitModel> lstOrder { get; set; }
        public Guid? IdOrderOld { get; set; }// đơn hiện tại
        public Guid? IdOrderNew { get; set; }//tách đến đơn khác
        public Guid? IdTable { get; set; }
        public int ComId { get; set; }
        public string json { get; set; }
        public string CasherName { get; set; }
        public string IdCasher { get; set; }
        public bool IsNewOrder { get; set; }//tạo đơn mới
        public bool IsBringBack { get; set; }//bàn mang về
        public EnumTypeSpitOrder TypeUpdate { get; set; }
    }
    public class DetailtSpitModel
    {
        public Guid? idOrder { get; set; }
        public string Code { get; set; }
        public string Note { get; set; }
        public Guid? idOrderItem { get; set; } //id này là id kiểu Guid 
        public int? idOrderItemInt { get; set; }//id này là id kiểu int 
        public int? IdProduct { get; set; }
        public decimal? Quantity { get; set; }
        public decimal QuantityNotifyKitchen { get; set; }
    }
}
