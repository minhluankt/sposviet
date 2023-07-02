using AspNetCoreHero.Abstractions.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Order : AuditableEntity
    {
        public Order()
        {
            this.OrderDetailts = new HashSet<OrderDetailts>();
        }
        [Required]
        public string OrderCode { get; set; }
        [Required]
        [ForeignKey("IdCustomer")]
        public int IdCustomer { get; set; }
        [ForeignKey("IdDeliveryCompany")]
        public int? IdDeliveryCompany { get; set; }// chọn nhà vận chuyển nếu có
        public string ContentTransportFee { get; set; }// nội dung vận chuyển nếu có
        public decimal Total { get; set; }
        public decimal Amount { get; set; }
        public decimal TransportFeeAmount { get; set; } // phí vận chuyển
        public decimal DiscountAmonnt { get; set; }
        public float DiscountRate { get; set; }
        public float VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public int Quantity { get; set; }
        public int? IdPharmaceutical { get; set; }// nhân viên sell
        public string PharmaceuticalName { get; set; }// nhân viên sell
        public string AmountInWord { get; set; }
        public string Note { get; set; }
        public string CodeVoucher { get; set; }



        public int Status { get; set; }
        //lưu thông tin khách để lỡ khách đổi thì vẫn giữ đúng lúc đặt đơn, khi cần check lại thì rõ hơn, và có nút cập nhật thông tin khách hàng
        public string CusName { get; set; }
        public string CusTaxcode { get; set; }
        public string CusCode { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        [ForeignKey("IdPaymentMethod")]
        public int? IdPaymentMethod { get; set; }
        public string PaymentMethodContent { get; set; }
        [ForeignKey("IdBankAccount")]
        public int? IdBankAccount { get; set; }// nếu chuyển khoản thì sau đó admin xác nhận và chọn đúng ngân hàng nếu có
        public string BankAccountContent { get; set; }
        public virtual DeliveryCompany DeliveryCompany { get; set; }
        public Customer Customer { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual ICollection<OrderDetailts> OrderDetailts { get; set; }
    }
}
