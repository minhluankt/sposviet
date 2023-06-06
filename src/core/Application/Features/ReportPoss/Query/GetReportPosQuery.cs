using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using HelperLibrary;
using MediatR;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ReportPoss.Query
{

    public class GetReportPosQuery : SearchReportPosModel,IRequest<Result<ResponseReport>>
    {
        public class GetReportsProductsQueryHandler : IRequestHandler<GetReportPosQuery, Result<ResponseReport>>
        {
            private readonly IReportPosRepository _repository;
            public GetReportsProductsQueryHandler(IReportPosRepository repository)
            {
                _repository = repository;
            }
            public async Task<Result<ResponseReport>> Handle(GetReportPosQuery query, CancellationToken cancellationToken)
            {
                ResponseReport responseReport = new ResponseReport();
                var getdt = await _repository.GetRevenue(query);
                switch (query.typeReportPos)
                {
                    case Enums.EnumTypeReportPos.NONE:
                        break;
                    case Enums.EnumTypeReportPos.DOANHTHU:
                        ReportPosModel reportPosModel = new ReportPosModel();
                        reportPosModel.InvoiceAll = getdt.Count();
                        reportPosModel.InvoiceCancel = getdt.Where(x=>x.Status==Enums.EnumStatusInvoice.HUY_BO).Count();
                        reportPosModel.Product = getdt.Sum(x => x.Quantity);
                        reportPosModel.DiscountAmount = getdt.Where(x=> x.DiscountAmount.HasValue).ToList().Sum(x => x.DiscountAmount)??0;
                        reportPosModel.Amount = getdt.Where(x=>x.Status==Enums.EnumStatusInvoice.DA_THANH_TOAN || x.Status == Enums.EnumStatusInvoice.HOAN_TIEN_MOT_PHAN).Sum(x => x.Amonut);
                        // chi tiết theo hóa đơn
                        List<ListItemReport> ListItemReportsByInvoice = new List<ListItemReport>();
                        var lstOrder = getdt.OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Id);//theo hóa đơn
                        foreach (var item in lstOrder)
                        {
                            ListItemReportsByInvoice.Add(new ListItemReport()
                            {
                                StatusName = item.Status.GetDisplayNameEnum(),
                                CasherName = item.CasherName,
                                InvoiceCode = item.InvoiceCode,
                                InvoiceId = item.Id,
                                RoomName = item.RoomAndTable!=null? item.RoomAndTable.Name:"Mang về",
                                Buyer = item.Buyer,
                                Status = item.Status,
                                Amount = item.Amonut,
                                Total = item.Total,
                                VATAmount = item.VATAmount,
                                DiscountAmount = item.DiscountAmount??0,
                                ServiceChargeAmount = item.ServiceChargeAmount,
                                CreateDate = item.CreatedOn.ToString("dd/MM/yyyy HH:mm"),
                                RankName = LibraryCommon.DateInWeek(item.CreatedOn)
                            });
                        }
                        reportPosModel.ListItemReportsByInvoice = ListItemReportsByInvoice;

                        //lấy theo ngày
                        var lstitem = getdt.GroupBy(x => x.CreatedOn.Date);
                        List<ListItemReport> ListItemReport = new List<ListItemReport>();
                        foreach (var item in lstitem)
                        {
                            ListItemReport.Add(new ListItemReport() {
                                Amount = item.Where(x => x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN || x.Status == Enums.EnumStatusInvoice.HOAN_TIEN_MOT_PHAN).Sum(x=>x.Amonut),
                                VATAmount = item.Where(x => x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN || x.Status == Enums.EnumStatusInvoice.HOAN_TIEN_MOT_PHAN).Sum(x=>x.VATAmount),
                                TotalCancel = item.Where(x => x.Status == Enums.EnumStatusInvoice.HUY_BO ||  x.Status == Enums.EnumStatusInvoice.HOAN_TIEN).Sum(x=>x.Amonut),//tiền hủy nên lấy của hóa đơn
                                Total = item.Sum(x=>x.Total),
                                DiscountAmount = item.Where(x => x.DiscountAmount.HasValue&&( x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN || x.Status == Enums.EnumStatusInvoice.HOAN_TIEN_MOT_PHAN)).ToList().Sum(x=>x.DiscountAmount)??0,
                                ServiceChargeAmount = item.Where(x => x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN || x.Status == Enums.EnumStatusInvoice.HOAN_TIEN_MOT_PHAN).Sum(x=>x.ServiceChargeAmount),
                                CreateDate = item.Key.ToString("dd/MM/yyyy"),
                                Date = item.Key,
                                RankName = LibraryCommon.DateInWeek(item.Key)
                            });
                        }
                        var dates = new List<DateTime>();
                        var Charts = new List<Chart>();
                        // lấy danh dách ngày nhé
                        for (var dt = query.srartDate.Value; dt <= query.endDate.Value; dt = dt.AddDays(1))
                        {
                            dates.Add(dt);
                        }

                        if (dates.Count()==1)//chỉ chọn 1 ngày thì báo cáo theo giờ 24
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
                                    Charts.Add(new Chart() { Key = $"{item.ToString("00")}:00", Value = getdata.Amount });
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
                                if (getdata!=null)
                                {
                                    Charts.Add(new Chart() { Key = item.ToString("dd/MM/yyyy"),Value= getdata.Amount});
                                }
                                else
                                {
                                    Charts.Add(new Chart() { Key = item.ToString("dd/MM/yyyy"), Value = 0 });
                                }
                            }
                        }

                        reportPosModel.Charts = Charts;
                        reportPosModel.ListItemReports = ListItemReport.OrderByDescending(x => x.Date).ToList();
                        responseReport.ReportPosModel = reportPosModel;
                        break;
                    case Enums.EnumTypeReportPos.HUYDON:
                        ReportHuyDon ReportHuyDons = new ReportHuyDon();
                        List<ListItemReportHuyDon> ListItemReportReportHuyDon = new List<ListItemReportHuyDon>();
                        var lstitemhuydon = getdt.Where(x=>x.Status == Enums.EnumStatusInvoice.HUY_BO).ToList();
                        foreach (var item in lstitemhuydon)
                        {
                            ListItemReportReportHuyDon.Add(new ListItemReportHuyDon() { 
                                CasherName= item.CasherName,
                                CreatDate = item.CreatedOn.ToString("dd/MM/yyyy HH:mm"),
                                Date= item.CreatedOn,
                                InvoiceNo= item.InvoiceCode,
                                InvoiceId = item.Id,
                                VATAmount = item.VATAmount,
                                Note = !string.IsNullOrEmpty(item.Note)? item.Note:string.Empty,
                                Amount = item.Amonut
                            });
                        }

                        // chart
                        var Chartshuy = new List<Chart>();
                        var dateshuy = new List<DateTime>();
                        for (var dt = query.srartDate.Value; dt <= query.endDate.Value; dt = dt.AddDays(1))
                        {
                            dateshuy.Add(dt);
                        }
                        if (dateshuy.Count()>1)
                        {
                            foreach (var item in dateshuy)
                            {
                                var getdata = ListItemReportReportHuyDon.Where(x => x.Date.Date == item.Date).ToList();
                                if (getdata.Count()>0)
                                {
                                    Chartshuy.Add(new Chart() { Key = item.ToString("dd/MM/yyyy"), Value = getdata.Sum(x=>x.Amount) });
                                }
                                else
                                {
                                    Chartshuy.Add(new Chart() { Key = item.ToString("dd/MM/yyyy"), Value = 0 });
                                }
                            }
                            responseReport.isShowChart = true;
                        }

                        ReportHuyDons.ListItemReports = ListItemReportReportHuyDon;
                        ReportHuyDons.Charts = Chartshuy;
                        responseReport.ReportHuyDon = ReportHuyDons;
                        break;
                    //case Enums.EnumTypeReportPos.HINHTHUCTHANHTOAN:
                    //    break;  
                    case Enums.EnumTypeReportPos.HINHTHUCPHUVU:

                        ReportHinhThucPhucVu ReportHinhThucPhucVumodle = new ReportHinhThucPhucVu();
                        var lstitemHINHTHUCPHUVU = getdt.GroupBy(x => x.IsBringBack);//lấy theo HÌNH THỨc ăn tại bàn hày về
                        List<ListItemReportHinhThucPhucVu> ListItemReports = new List<ListItemReportHinhThucPhucVu>();
                        var ChartsHINHTHUCPHUVU = new List<Chart>();
                        foreach (var item in lstitemHINHTHUCPHUVU)
                        {
                            var newDt = new ListItemReportHinhThucPhucVu()
                            {
                                InvoicePaymentCount = item.Where(x => x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN || x.Status == Enums.EnumStatusInvoice.HOAN_TIEN_MOT_PHAN).Count(),
                                InvoiceCancelCount = item.Where(x => x.Status == Enums.EnumStatusInvoice.HOAN_TIEN || x.Status == Enums.EnumStatusInvoice.HUY_BO).Count(),
                                Amount = item.Where(x => x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN || x.Status == Enums.EnumStatusInvoice.HOAN_TIEN_MOT_PHAN).Sum(x=>x.Amonut),
                                VATAmount = item.Where(x => x.Status == Enums.EnumStatusInvoice.DA_THANH_TOAN || x.Status == Enums.EnumStatusInvoice.HOAN_TIEN_MOT_PHAN).Sum(x=>x.VATAmount)
                            };
                            if (item.Key)
                            {
                                ChartsHINHTHUCPHUVU.Add(new Chart() { Key = "Mang về",Value = newDt.Amount });
                                newDt.Name = "Mang về";
                                ListItemReports.Add(newDt);

                            }
                            else
                            {
                                ChartsHINHTHUCPHUVU.Add(new Chart() { Key = "Ăn tại bàn", Value = newDt.Amount });
                                newDt.Name = "Ăn tại bàn";
                                ListItemReports.Add(newDt);
                            }
                           
                        }
                        //làm chast

                        ReportHinhThucPhucVumodle.ListItemReports = ListItemReports;
                        ReportHinhThucPhucVumodle.Charts = ChartsHINHTHUCPHUVU;
                        responseReport.ReportHinhThucPhucVu = ReportHinhThucPhucVumodle;

                        break;
                    //case Enums.EnumTypeReportPos.HOADONCHUATHANHTOAN:
                    //    break;
                    default:
                        return await Result<ResponseReport>.FailAsync(HeperConstantss.ERR000);
   
                }

                return await Result<ResponseReport>.SuccessAsync(responseReport);
            }
        }
    }
}
