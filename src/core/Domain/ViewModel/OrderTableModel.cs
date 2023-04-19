using Application.Enums;
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
            // this.IdGuid = Guid.NewGuid();
        }
        public Guid? IdGuid { get; set; }
        public int IdOrder { get; set; }// 
        public string OrderCode { get; set; }// ví dụ OD-13
        public Guid? IdOrderItem { get; set; }// 
        public int ComId { get; set; }// 
        public int IdProduct { get; set; }// id của sản phẩm
        public string ProductCode { get; set; }// id của sản phẩm
        public string TableName { get; set; }// tên bàn/phòng
        public bool IsBringBack { get; set; }// mag về
        public bool IsRetailCustomer { get; set; } // là khách lẻ
        public bool IsCancel { get; set; } // là hủy món
        public bool IsRemoveCustomer { get; set; } // là xóa khách hàng
        public string CusCode { get; set; }// khách hàng
        public int? IdRoomAndTable { get; set; } // bàn/phòng
        public Guid? IdRoomAndTableGuid { get; set; } // bàn/phòng
        public string Buyer { get; set; }// khách hàng
        public int? IdCustomer { get; set; }// khách hàng
        public string Note { get; set; }
        public string IdCasher { get; set; } // thu ngân
        public string CasherName { get; set; } //  thu ngân
        public decimal Amount { get; set; } //  thu ngân
        public decimal Quantity { get; set; } //  thu ngân
        public string QuantityFloat { get; set; } //  thu ngân
        public DateTime? DateCreateService { get; set; } //  thu ngân
        public EnumTypeUpdatePos TypeUpdate { get; set; } //  thu ngân
        public EnumTypeProduct TypeProduct { get; set; } // loại dịch vụ
        public EnumTypeInvoice TypeInvoice { get; set; } // loại hóa đơn, đươn đặt
        public List<OrderTableItemModel> OrderTableItems { get; set; }

    }
    public class OrderTableItemModel
    {
        public Guid IdGuid { get; set; }
        public int Id { get; set; }
        public int IdOrderTable { get; set; }
        public int? IdProduct { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal QuantityNotifyKitchen { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public float Discount { get; set; } // % ck
        public double DiscountAmount { get; set; } // ck
    }
}
