using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.ViewModel;
using HelperLibrary;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ReportPoss.Query
{
  
    public class GetReportDashboardQuery : SearchReportPosModel, IRequest<Result<ResponseReport>>
    {
        public class GetReportsProductsQueryHandler : IRequestHandler<GetReportDashboardQuery, Result<ResponseReport>>
        {
            private readonly IReportPosRepository _repository;
            public GetReportsProductsQueryHandler(IReportPosRepository repository)
            {
                _repository = repository;
            }
            public async Task<Result<ResponseReport>> Handle(GetReportDashboardQuery query, CancellationToken cancellationToken)
            {
                ResponseReport responseReport = new ResponseReport();
                var getdt = await _repository.GetRevenue(query);
                switch (query.TypeReportDashboard)
                {
                    case Enums.EnumTypeReportDashboard.DOANHTHU:
                        ReportPosModel reportPosModel = new ReportPosModel();
                        //lấy theo ngày
                        var lstitem = getdt.GroupBy(x => x.CreatedOn.Date);
                        List<ListItemReport> ListItemReport = new List<ListItemReport>();
                        foreach (var item in lstitem)
                        {
                            ListItemReport.Add(new ListItemReport()
                            {
                                Amount = item.Where(x => x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN || x.Status == Enums.EnumStatusInvoice.HOAN_TIEN_MOT_PHAN).Sum(x => x.Amonut),
                                VATAmount = item.Where(x => x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN || x.Status == Enums.EnumStatusInvoice.HOAN_TIEN_MOT_PHAN).Sum(x => x.VATAmount),
                                TotalCancel = item.Where(x => x.Status == Enums.EnumStatusInvoice.HUY_BO || x.Status == Enums.EnumStatusInvoice.HOAN_TIEN).Sum(x => x.Amonut),//tiền hủy nên lấy của hóa đơn
                                Total = item.Sum(x => x.Total),
                                DiscountAmount = item.Where(x => x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN || x.Status == Enums.EnumStatusInvoice.HOAN_TIEN_MOT_PHAN).Sum(x => x.DiscountAmount),
                                ServiceChargeAmount = item.Where(x => x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN || x.Status == Enums.EnumStatusInvoice.HOAN_TIEN_MOT_PHAN).Sum(x => x.ServiceChargeAmount),
                                CreateDate = item.Key.ToString("dd/MM/yyyy"),
                                Date = item.Key,
                                RankName = LibraryCommon.DateInWeek(item.Key)
                            });
                        }
                        var dates = new List<DateTime>();
                        var Charts = new List<Chart>();
                        // hiển thị chart lấy danh dách ngày nhé
                        for (var dt = query.srartDate.Value; dt <= query.endDate.Value; dt = dt.AddDays(1))
                        {
                            dates.Add(dt);
                        }

                        if (dates.Count() == 1)//chỉ chọn 1 ngày thì báo cáo theo giờ 24
                        {
                            //nếu bằng 1 thì hiển thị biểu đồ theo giwof trong ngày đó
                            var queryhour = getdt
                            .GroupBy(row => row.CreatedOn.Hour);
                            List<ListItemReport> ListItemReportbyhour = new List<ListItemReport>();
                            foreach (var item in queryhour)
                            {
                                ListItemReportbyhour.Add(new ListItemReport()
                                {
                                    Amount = item.Where(x => x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN || x.Status == Enums.EnumStatusInvoice.HOAN_TIEN_MOT_PHAN).Sum(x => x.Amonut),
                                    VATAmount = item.Where(x => x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN || x.Status == Enums.EnumStatusInvoice.HOAN_TIEN_MOT_PHAN).Sum(x => x.VATAmount),
                                    Hour = item.Key
                                });
                            }
                            var hours = new List<int>();
                            for (var dt = 0; dt <= 23; dt++)
                            {
                                hours.Add(dt);
                            }
                            foreach (var item in hours)
                            {
                                var getdata = ListItemReportbyhour.SingleOrDefault(x => x.Hour == item);
                                if (getdata != null)
                                {
                                    Charts.Add(new Chart() { Key = $"{item.ToString("00")}:00", Value = getdata.Amount, Value2 = getdata.VATAmount  });
                                }
                                else
                                {
                                    Charts.Add(new Chart() { Key = $"{item.ToString("00")}:00", Value = 0 });
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in dates)
                            {
                                var getdata = ListItemReport.SingleOrDefault(x => x.Date.Date == item.Date);
                                if (getdata != null)
                                {
                                    Charts.Add(new Chart() { Key = item.ToString("dd/MM/yyyy"), Value = getdata.Amount, Value2 = getdata.VATAmount });
                                }
                                else
                                {
                                    Charts.Add(new Chart() { Key = item.ToString("dd/MM/yyyy"), Value = 0,Value2=0});
                                }
                            }
                        }
                        reportPosModel.Charts = Charts;
                        //reportPosModel.ListItemReports = ListItemReport.OrderByDescending(x => x.Date).ToList();
                        responseReport.ReportPosModel = reportPosModel;
                        break;
                    default:
                        return await Result<ResponseReport>.FailAsync(HeperConstantss.ERR000);

                }

                return await Result<ResponseReport>.SuccessAsync(responseReport);
            }
        }
    }
}
