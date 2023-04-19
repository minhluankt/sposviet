using Application.Enums;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class RevenueExpenditureModel: RevenueExpenditure
    {
  
        public List<SelectListItem> ObjectRevenueExpenditures { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public List<CategoryCevenue> CategoryCevenues { get; set; }
    }
    public class ReportRevenueExpenditureModel
    {
        public decimal BeginningFund { get; set; }//quỹ đầu kỳ
        public decimal TotalRevenue { get; set; }//tổng thu
        public decimal TotalExpenditure { get; set; }//tổng chi
        public decimal FundsBalance { get; set; }//tồn quỹ
      
    }
    public class ReportRevenueExpenditureDetailt
    {
        public decimal Amount { get; set; }
        public EnumTypeRevenueExpenditure Type { get; set; }

    }
}
