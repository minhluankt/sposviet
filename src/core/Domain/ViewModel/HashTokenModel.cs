using Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class HashTokenModel
    {
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }
        public string pattern { get; set; }
        public string serial { get; set; }
        public string serialCert { get; set; }
        public string dataxmlhash { get; set; }
    }
}
