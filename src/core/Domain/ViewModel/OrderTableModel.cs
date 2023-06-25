using Application.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Domain.ViewModel
{
    public class OrderTableModel
    {
        public OrderTableModel()
        {
            this.OrderTableItems = new List<OrderTableItemModel>();
            this.NotifyOrderNewModels = new List<NotifyOrderNewModel>();
            // this.IdGuid = Guid.NewGuid();
        }
        public Guid? IdGuid { get; set; }
        public int IdOrder { get; set; }// 
        public string OrderCode { get; set; }// ví dụ OD-13
        public Guid? IdOrderItem { get; set; }// 
        public int ComId { get; set; }// 
        public double TimeNumber { get; set; }// id của sản phẩm
        public int IdProduct { get; set; }// id của sản phẩm
        public string CreateDate { get; set; }// thời gian tạo
        public string ProductCode { get; set; }// id của sản phẩm
        public string TableName { get; set; }// tên bàn/phòng
        public string AreaName { get; set; }// tên khu vực
        public bool IsBringBack { get; set; }// mag về
        public bool IsServiceDate { get; set; } // là món dịch vụ tính giờ
        public bool IsCancel { get; set; } // là hủy món
        public bool IsStartDate { get; set; } // là dành cho update ngày giờ bắt đầu tính tiền cho sp có tính giờ
        public bool IsRemoveCustomer { get; set; } // là xóa khách hàng
        public string CusCode { get; set; }// khách hàng
        public int? IdRoomAndTable { get; set; } // bàn/phòng
        public Guid? IdRoomAndTableGuid { get; set; } // bàn/phòng
        public string Buyer { get; set; }// khách hàng
        public int? IdCustomer { get; set; }// khách hàng
        public string Note { get; set; }
        public string IdCasher { get; set; } // thu ngân
        public string CasherName { get; set; } //  thu ngân
        public decimal PriceOld { get; set; } // 
        public decimal? Price { get; set; } //  thu ngân
        public decimal Discount { get; set; } //  thu ngân
        public decimal DiscountAmount { get; set; } //  thu ngân
        public decimal Amount { get; set; } //  
        public decimal Quantity { get; set; } // 
        public string QuantityFloat { get; set; } //  thu ngân
        [JsonIgnore]
        public string HtmlPrint { get; set; } //  html báo bếp
        public DateTime? DateCreateService { get; set; } //  thu ngân
        public EnumTypeUpdatePos TypeUpdate { get; set; } // 
        public EnumTypeProduct TypeProduct { get; set; } // loại dịch vụ
        public EnumTypeInvoice TypeInvoice { get; set; } // loại hóa đơn, đươn đặt
        public List<OrderTableItemModel> OrderTableItems { get; set; }
        [JsonIgnore]
        public List<NotifyOrderNewModel> NotifyOrderNewModels { get; set; }//dùng để báo hủy bếp


    }
    public class OrderTableItemModel
    {
        public Guid IdGuid { get; set; }
        public int Id { get; set; }
        public bool IsVAT { get; set; }
        public int IdOrderTable { get; set; }
        public int? IdProduct { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string Code { get; set; }
        public decimal QuantityNotifyKitchen { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal? PriceOld { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public float Discount { get; set; } // % ck
        public double DiscountAmount { get; set; } // ck
        public bool IsServiceDate { get; set; } // là sản phẩm tính tiền giờ
        public DateTime? DateCreateService { get; set; } // bắt đầu tính tiền giờ dịch vụ nhà nghỉ khách sạn, cho thuê
        public DateTime? DateEndService { get; set; } // dừng tính tiền giờ dịch vụ nhà nghỉ khách sạn, cho thuê
    }
    public class OrderTableInPos
    {
        public string TableName { get; set; }
        public string AreaName { get; set; }
        public DateTime Date { get; set; }
        public double NumberTime { get; set; }
        public decimal Amount { get; set; }
        public bool IsServiceDate { get; set; }
        public Guid IdGuid { get; set; }
        public Guid? IdRoomAndTable { get; set; }
    }
}
