using Domain.Entities;
using Model;
using System.Collections.Generic;

namespace Domain.ViewModel
{
    public class OrderViewModel : ParametersPageModel
    {
        public string CusName { get; set; }
        public string CusCode { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string OrderCode { get; set; }
        public decimal Amount { get; set; }
        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public int Quantity { get; set; }
        public string AmountInWord { get; set; }
        public string Note { get; set; }
        public string CodeVoucher { get; set; }
        public string CreatedOn { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string secretId { get; set; }
        public string secretCode { get; set; }
    }
    public class CartModelIndex
    {
        public List<Order> Orders { get; set; }
        public List<StatusOrder> StatusOrders { get; set; }
        public string secret { get; set; }
        public Order Order { get; set; }
        public Customer Customer { get; set; }
    }
    public class OrderModelIndex : CartModelIndex
    {
        public CompanyAdminInfo Company { get; set; }
    }
    public class CartItemViewModel
    {
        public string secretId { get; set; }
    }

}
