using Domain.Entities;
using System.Collections.Generic;

namespace Domain.ViewModel
{
    public class PosModel
    {
        public PosModel()
        {
            this.RoomAndTables = new List<RoomAndTable>();
            this.Areas = new List<Area>();
            this.OrderTables = new List<OrderTable>();
            this.Products = new List<ProductPosModel>();
            this.PaymentMethods = new List<PaymentMethod>();
        }
        public List<OrderTable> OrderTables { get; set; }
        public List<RoomAndTable> RoomAndTables { get; set; }
        public List<Area> Areas { get; set; }
        public List<ProductPosModel> Products { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public Casher Casher { get; set; }
    }
    public class ProductPosModel
    {
        public string Name { get; set; }
        public string Img { get; set; } //mã sản phẩm
        public string Code { get; set; } //mã sản phẩm
        public string idString { get; set; } //mã sản phẩm
        public int Id { get; set; }
        public decimal RetailPrice { get; set; } // giá bán lẻ
        public decimal Quantity { get; set; } // tồn kho
        public int? IdCategory { get; set; }  //danh mục nào
        public string NameCategory { get; set; } //danh mục nào

    }
    public class Casher // người bán
    {
        public string FullName { get; set; }
        public string Id { get; set; }

    }
}
