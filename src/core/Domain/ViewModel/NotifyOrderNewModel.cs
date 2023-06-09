using Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class NotifyOrderNewModel
    {
        public string StaffName { get; set; }
        public string IdStaff { get; set; }
        public string RoomTableName { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Code { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string Note { get; set; }
        public EnumTypeTemplatePrint TypeNotifyOrder { get; set; }
    }
}
