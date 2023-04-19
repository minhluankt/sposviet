using Application.Enums;
using Domain.Entities;
using Domain.ViewModel;

namespace Web.ManagerApplication.Areas.Selling.Models
{
    public class OrderTableViewModel
    {
        public OrderTableViewModel()
        {
            this.OrderTables = new List<OrderTable>();
            this.OrderTable = new OrderTable();
            this.CustomerModelViewPos = new List<CustomerModelViewPos>();

        }
        //  public List<ListNoteOrderModelViewPos> ListNoteOrderModelViewPos { get; set; }
        public List<CustomerModelViewPos> CustomerModelViewPos { get; set; }
        public string OrderTableCode { get; set; }
        public List<OrderTable> OrderTables { get; set; }
        public OrderTable OrderTable { get; set; }
    }
    public class PaymentModelView
    {
        public PaymentModelView()
        {
            this.PaymentMethods = new List<PaymentMethod>();
        }
        public bool VatMTT { get; set; }
        public OrderTable OrderTable { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public SupplierEInvoiceModel SupplierEInvoiceModel { get; set; }
    }
    public class SplitOrderModelView
    {
        public SplitOrderModelView()
        {
            this.RoomAndTables = new List<RoomAndTable>();
        }
        public EnumTypeSpitOrder TypeSpitOrder { get; set; }
        public OrderTable OrderTable { get; set; }
        public List<RoomAndTable> RoomAndTables { get; set; }
        public List<OrderTable> OrderTables { get; set; }
    }

    public class CustomerModelViewPos
    {
        public Guid IdOrder { get; set; }
        public int IdCustomer { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Note { get; set; }
    }
    public class ListNoteOrderModelViewPos
    {
        public Guid IdOrder { get; set; }
        public string Note { get; set; }
    }
    public class ChangeTableInOrderModel
    {
        public ChangeTableInOrderModel()
        {
            this.RoomAndTables = new List<RoomAndTable>();
        }
        public Guid? IdOrder { get; set; }
        public Guid? IdTable { get; set; }
        public Guid? OldIdTable { get; set; }
        public int TypeSelectTable { get; set; }//0 là mang về 1 là có chọn bàn
        public EnumTypeUpdatePos TypeUpdate { get; set; } // loại update
        public OrderTable OrderTable { get; set; }
        public IEnumerable<RoomAndTable> RoomAndTables { get; set; }
    }
}
