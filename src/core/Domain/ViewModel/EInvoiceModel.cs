using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
   
    public class EInvoiceModel
    {
        public string CusName { get; set; }
        public string Buyer { get; set; }
        public string Address { get; set; }
        public string CusTaxCode { get; set; }
        public string CusCode { get; set; }
        public string Email { get; set; }
        public string CCCD { get; set; }
        public int IdPayment { get; set; }
        public int? IdCustomer { get; set; }
    }

    public class DashboardEInvoiceModel
    {
        public int SignedInv { get; set; }//đã phát hành
        public int UnSendInv { get; set; }//chưa gưi thuế
        public int SentInv { get; set; }// đã gửi thuế
        public int AcceptedInv { get; set; }//thuế chấp nhận
        public int RejectedInv { get; set; }//thues từ chối
    }
}
