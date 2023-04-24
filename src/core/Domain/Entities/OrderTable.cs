using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using Domain.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class OrderTable : AuditableEntity // là phiếu của đặt bàn, chưa thanh toán nên sẽ lưu ở đây giống giỏ hàng
    {
        public OrderTable()
        {
            this.OrderTableItems = new List<OrderTableItem>();
            this.HistoryOrders = new List<HistoryOrder>();
            this.IdGuid = Guid.NewGuid();
        }
        [DefaultValue(EnumTypeInvoice.INVOICE)]
        public EnumTypeInvoice TypeInvoice { get; set; } // hóa đơn hay đơn đặt hàng
        public EnumTypeProduct TypeProduct { get; set; } // loại hóa đơn của dịch vụ nào
        public Guid IdGuid { get; set; }
        public int ComId { get; set; }
        public int OrderSort { get; set; }// số thứ tự tawg dần
        public string OrderTableCode { get; set; }// mã đặt bàn 1-1, 1-2
        public bool IsBringBack { get; set; }// mag về
        public string Buyer { get; set; }// khách lẻ
        public string CusCode { get; set; }// mã khách
        //public int? IdInvoice { get; set; }// hóa đơn
        public int? IdCustomer { get; set; }// khách hàng
        public Guid? IdRoomAndTableGuid { get; set; }
        public int? IdRoomAndTable { get; set; } // bàn/phòng
        public bool IsRetailCustomer { get; set; } // là khách lẻ
        public string Note { get; set; } // thu ngân
        public string IdCasher { get; set; } // 
        public string CasherName { get; set; } //  thu ngân
        public string StaffName { get; set; } //  nhân viên phục vụ
        public string IdStaff { get; set; } // id nhân viên phục vụ
        public int? IdPaymentMethod { get; set; } // hình thưc thanh toán
        public float Discount { get; set; } // % ck
        public double DiscountAmount { get; set; } // ck
        public decimal Amonut { get; set; } // tổng tiền
        public decimal Quantity { get; set; } // tổng số lượng
        public DateTime? PurchaseDate { get; set; }// ngày thanh toán
        public EnumStatusOrderTable Status { get; set; } // bàn mới, đã thahnh toán
        [ForeignKey("IdCustomer")]
        public virtual Customer Customer { get; set; } // trạng thái

        [ForeignKey("IdRoomAndTable")]
        public virtual RoomAndTable RoomAndTable { get; set; } // RoomAndTable
        [ForeignKey("IdPaymentMethod")]
        public virtual PaymentMethod PaymentMethod { get; set; } // trạng thái
        public virtual Invoice Invoice { get; set; } // trạng thái
        public virtual ICollection<OrderTableItem> OrderTableItems { get; set; } // trạng thái
        public virtual ICollection<HistoryOrder> HistoryOrders { get; set; } // trạng thái
        [NotMapped]
        public virtual List<NotifyOrderNewModel> NotifyOrderNewModels { get; set; } // để lưu in thông báo bếp

    }
    public class OrderTableItem
    {
        public OrderTableItem()
        {
            this.IdGuid = Guid.NewGuid();
        }
        public Guid IdGuid { get; set; }
        public int Id { get; set; }
        [ForeignKey("IdOrderTable")]
        public int IdOrderTable { get; set; }
        public int? IdProduct { get; set; }
        public string Name { get; set; }
        public bool IsServiceDate { get; set; } // là sản phẩm tính tiền giờ
        public DateTime? DateCreateService { get; set; } // bắt đầu tính tiền giờ dịch vụ nhà nghỉ khách sạn, cho thuê
        public string Code { get; set; }
        public decimal QuantityNotifyKitchen { get; set; }// số lượng món đã thông báo
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal EntryPrice { get; set; }//giá nhập
        public decimal Total { get; set; }
        public float Discount { get; set; } // % ck
        public decimal DiscountAmount { get; set; } // ck
        public EnumTypeProductCategory TypeProductCategory { get; set; }
        [NotMapped]
        public EnumStatusKitchenOrder Status { get; set; } = EnumStatusKitchenOrder.MOI;
        [JsonIgnore]
        public virtual OrderTable OrderTable { get; set; }
    }
    public class HistoryOrder
    {
        [ForeignKey("IdOrderTable")]
        public int IdOrderTable { get; set; } // bàn/phòng

        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public string Code { get; set; }// các thức là khi thông báo thì update các bản chưa notif chauw IsNotif thì randome code này để groupby
        public string ProductName { get; set; }// tên sản phẩm
        public string Name { get; set; }// +1 hướng dương
        public string NewTableName { get; set; }//  đến bàn nào cho lúc chuyển món
        public string OrderCode { get; set; }//  mã đơn của bàn mới cho lúc chuyển món
        public string Carsher { get; set; }// người thêm
        public bool IsNotif { get; set; }// khi bấm thông báo
        public DateTime CreateDate { get; set; }
        [DefaultValue(EnumTypeKitchenOrder.HUY)]
        public EnumTypeKitchenOrder TypeKitchenOrder { get; set; }
        [NotMapped]
        public int? IdProduct { get; set; }
        public virtual OrderTable OrderTable { get; set; }
    }

}
