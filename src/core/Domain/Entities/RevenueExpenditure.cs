using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RevenueExpenditure : AuditableEntity
    {
        public EnumTypeObjectRevenueExpenditure ObjectRevenueExpenditure { get; set; }//loại người nộp, khách hay nhà ucng cấp, đối tượng khác
        public EnumTypeRevenueExpenditure Type { get; set; }//là loij thu hay chi
        public EnumTypeCategoryThuChi Typecategory { get; set; }//loại thu chi mặc định khi thu từ bán hàng hoặc nhập hàng
        public string Title { get; set; }//nội dung thu
        public int ComId { get; set; }
        public int? IdCategoryCevenue { get; set; }//danh mục thu
        public int GroupSubmiter { get; set; }//đối tượng thu là khashc hàng là đôi tác hay đối tượng khác, làm enum
        [StringLength(30, ErrorMessage = "Mã chứng từ không quá 30 ký tự")]
        public string CodeOriginaldocument { get; set; }// chứng từ gốc là mã ở đâu có của khách
        public decimal Amount { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;
        [StringLength(20)]
        public string Code { get; set; }
        public string Content { get; set; }
        [StringLength(50, ErrorMessage = "Tên người nộp không quá 30 ký tự")]
        public string CustomerName { get; set; }//tên người nộp
        public EnumStatusRevenueExpenditure Status { get; set; }//trạng thái
        public int? IdCustomer { get; set; }//id người nộp
        public int? IdInvoice { get; set; }//id của hóa đơn bán hàng dành cho thu từ hóa đơn
        public int? IdPurchaseOrder { get; set; }//id của nhập hàng khi nhập hàng
        public int? IdPayment { get; set; }// 
        [NotMapped]
        public string IdString { get; set; }//
        [NotMapped]
        public string CategoryCevenueName { get; set; }//tên danh mục
        [NotMapped] 
        public string TypeName { get; set; }//tên danh mục
        [NotMapped]
        public string PaymentName { get; set; }//
        public virtual CategoryCevenue CategoryCevenue { get; set; }
    }
}
