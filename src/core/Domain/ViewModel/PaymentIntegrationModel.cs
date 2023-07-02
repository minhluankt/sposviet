using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class PaymentIntegrationModel
    {
        public PaymentIntegrationModel() {
            this.VietQRs = new List<VietQR>();
        }
        public List<VietQR> VietQRs { get; set; }
    }
}
