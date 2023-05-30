using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using Domain.XmlDataModel;
using HelperLibrary;
using JetBrains.Annotations;
using MediatR;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ReportPoss.Query
{
    public class GetRepostEInvoiceQuery : SearchReportPosModel, IRequest<Result<ResponseReport>>
    {
        public class GetRepostEInvoiceQueryHandler : IRequestHandler<GetRepostEInvoiceQuery, Result<ResponseReport>>
        {
            private readonly IFormFileHelperRepository _fileHelper;
            private readonly IReportPosRepository _repository;
            private readonly ICompanyAdminInfoRepository _companyrepository;
            private readonly IEInvoiceRepository<EInvoice> _einvoiceRepository;
            public GetRepostEInvoiceQueryHandler(IReportPosRepository repository, 
                ICompanyAdminInfoRepository companyrepository,
                IEInvoiceRepository<EInvoice> einvoiceRepository,
                IFormFileHelperRepository fileHelper)
            {
                _repository = repository;
                _companyrepository = companyrepository;
                _einvoiceRepository = einvoiceRepository;
                _fileHelper = fileHelper;
            }
            public async Task<Result<ResponseReport>> Handle(GetRepostEInvoiceQuery query, CancellationToken cancellationToken)
            {
                if (!query.srartDate.HasValue || !query.endDate.HasValue)
                {
                    return Result<ResponseReport>.Fail(HeperConstantss.ERR012);
                }
                DateTime endDate = query.endDate.Value.AddDays(1);
                if (query.TypeReportEInvoice == Enums.EnumTypeReportEInvoice.DOANHtHU_HOADON)
                {
                    var getreport = (await _einvoiceRepository.GetReportMonth(query.srartDate.Value, endDate, query.Comid)).OrderByDescending(x => x.InvoiceNo).GroupBy(x => x.VATRate).OrderBy(x => x.Key).ToList();
                    if (getreport.Count() == 0)
                    {
                        return await Result<ResponseReport>.FailAsync(HeperConstantss.ERR012);
                    }
                    var getcompany = _companyrepository.GetCompany(query.Comid);
                    string path = _fileHelper.GetFileTemplate("ReportEInvoiceMonth.xlsx", "EInvoice");
                    FileInfo newFile = new FileInfo(path);
                    using (ExcelPackage package = new ExcelPackage(newFile))
                    {
                        // add a new worksheet to the empty workbook
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        ////Add the headers
                        //worksheet.Cells[1, 1].Value = "ID";
                        //worksheet.Cells[1, 2].Value = "Product";
                        //worksheet.Cells[1, 3].Value = "Quantity";
                        //worksheet.Cells[1, 4].Value = "Price";
                        //worksheet.Cells[1, 5].Value = "Value";
                        worksheet.Cells[3, 4].Value = $"[1]Kỳ tính thuế: Tháng {query.endDate.Value.Month} năm {query.endDate.Value.Year}";
                        worksheet.Cells[5, 2].Value = getcompany?.Name;
                        worksheet.Cells[6, 2].Value = getcompany?.CusTaxCode;

                        //add item
                        int row = 12;
                        decimal totals = 0;
                        decimal vatamounts = 0;
                        foreach (var invoices in getreport)
                        {
                            totals += invoices.Where(x=>x.StatusEinvoice!=Enums.StatusEinvoice.CanceledInv).Sum(x => x.Total);
                            vatamounts += invoices.Where(x => x.StatusEinvoice != Enums.StatusEinvoice.CanceledInv).Sum(x => x.VATAmount);
                            string title = string.Empty;
                            switch (invoices.Key)
                            {
                                case -1:
                                    title = $"1. Hàng hoá, dịch vụ không chịu thuế giá trị gia tăng (GTGT):";
                                    break;
                                case 0:
                                    title = $"2. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 0%:";
                                    break;
                                case 5:
                                    title = $"3. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 5%:";
                                    break;
                                case 8:
                                    title = $"4. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 8%:";
                                    break;
                                case 10:
                                    title = $"5. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 10%:";
                                    break;
                                default:
                                    break;
                            }
                            using (ExcelRange rng = worksheet.Cells[$"A{row}:J{row}"])
                            {
                                rng.Merge = true;
                                rng.Value = title;
                            }
           

                          //  worksheet.Cells[row, 9].Merge = true;
                          //  worksheet.Cells[row, 1].Value = title;
                            row++;
                            int i = 1;
                            foreach (var item in invoices.OrderBy(x=>x.InvoiceNo))
                            {
                                worksheet.Cells[row, 1].Value = i++;
                                worksheet.Cells[row, 2].Value = item.Pattern;
                                worksheet.Cells[row, 3].Value = item.Serial;
                                worksheet.Cells[row, 4].Value = item.InvoiceNo.ToString("00000000");
                                worksheet.Cells[row, 5].Value = item.CreatedOn.ToString("dd/MM/yyyy");
                                worksheet.Cells[row, 6].Value = !string.IsNullOrEmpty(item.Buyer) ? item.Buyer : item.CusName;
                                worksheet.Cells[row, 7].Value = item.CusTaxCode;
                                worksheet.Cells[row, 8].Value = item.Total;
                                worksheet.Cells[row, 9].Value = item.VATAmount;
                                worksheet.Cells[row, 10].Value = item.StatusEinvoice == Enums.StatusEinvoice.CanceledInv ? "Hóa đơn hủy bỏ" : "";
                                row++;
                            }
                           // row++;
                        }
                        worksheet.Cells[$"A{12}:J{row}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[$"A{12}:J{row}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[$"A{12}:J{row}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[$"A{12}:J{row}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                        row++;

                        worksheet.Cells[row, 1].Value = $"Tổng doanh thu hàng hoá, dịch vụ bán ra: ";
                        worksheet.Cells[row, 2].Value = totals;
                        worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[row+1, 1].Value = $"Tổng thuế GTGT của hàng hóa, dịch vụ bán ra:";
                        worksheet.Cells[row+1, 2].Value = vatamounts;
                        worksheet.Cells[row+1, 2].Style.Numberformat.Format = "#,##0";
                        //Add a formula for the value-column
                        // worksheet.Cells["E2:E4"].Formula = "C2*D2";
                        //Ok now format the values;
                        //using (var range = worksheet.Cells[1, 1, 1, 5])
                        //{Tổng doanh thu hàng hoá, dịch vụ bán ra:   37.221.199.984,000


                        //    range.Style.Font.Bold = true;
                        //    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //    range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                        //    range.Style.Font.Color.SetColor(Color.White);
                        //}


                        //        worksheet.Cells[5, 3, 5, 5].Formula = string.Format("SUBTOTAL(9,{0})",
                        //new ExcelAddress(2, 3, 4, 3).Address);
                        worksheet.Cells[$"H{12}:H{row}"].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[$"I{12}:I{row}"].Style.Numberformat.Format = "#,##0";
                        //Create an autofilter for the range
                        //worksheet.Cells[$"A{12}:J{row}"].AutoFilter = true;
                        worksheet.Cells[$"A{10}:J{row}"].Style.WrapText = false;
                        worksheet.Cells[$"A{12}:J{row}"].AutoFitColumns(10);
                       
                        ResponseReport responseReport = new ResponseReport();
                        responseReport.dataExcel = package.GetAsByteArray();
                        return await Result<ResponseReport>.SuccessAsync(responseReport);

                    }
                }
                else if (query.TypeReportEInvoice==Enums.EnumTypeReportEInvoice.DOANHtHU_SANPHAM_HOADON)
                {
                    var getreport = (await _einvoiceRepository.GetReportMonthProduct(query.srartDate.Value, endDate, query.Comid)).OrderByDescending(x => x.InvoiceNo).GroupBy(x => x.ProductVATRate).OrderBy(x => x.Key).ToList();
                    if (getreport.Count() == 0)
                    {
                        return await Result<ResponseReport>.FailAsync(HeperConstantss.ERR012);
                    }
                    var getcompany = _companyrepository.GetCompany(query.Comid);
                    string path = _fileHelper.GetFileTemplate("ReportEInvoiceMonthProduct.xlsx", "EInvoice");
                    FileInfo newFile = new FileInfo(path);
                    using (ExcelPackage package = new ExcelPackage(newFile))
                    {
                        // add a new worksheet to the empty workbook
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        ////Add the headers
                        //worksheet.Cells[1, 1].Value = "ID";
                        //worksheet.Cells[1, 2].Value = "Product";
                        //worksheet.Cells[1, 3].Value = "Quantity";
                        //worksheet.Cells[1, 4].Value = "Price";
                        //worksheet.Cells[1, 5].Value = "Value";
                        worksheet.Cells[3, 4].Value = $"[1]Kỳ tính thuế: Tháng {query.endDate.Value.Month} năm {query.endDate.Value.Year}";
                        worksheet.Cells[5, 2].Value = getcompany?.Name;
                        worksheet.Cells[6, 2].Value = getcompany?.CusTaxCode;
                        decimal totals = 0;
                        decimal vatamounts = 0;
                        //add item
                        int row = 12;
                        foreach (var invoices in getreport)
                        {
                            totals = invoices.Where(x => x.Status != Enums.StatusEinvoice.CanceledInv).Sum(x => x.ProductTotal);
                            vatamounts = invoices.Where(x => x.Status != Enums.StatusEinvoice.CanceledInv).Sum(x => x.ProductVATAmount);
                            string title = string.Empty;
                            switch (invoices.Key)
                            {
                                case -1:
                                    title = $"1. Hàng hoá, dịch vụ không chịu thuế giá trị gia tăng (GTGT):";
                                    break;
                                case 0:
                                    title = $"2. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 0%:";
                                    break;
                                case 5:
                                    title = $"3. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 5%:";
                                    break;
                                case 8:
                                    title = $"4. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 8%:";
                                    break;
                                case 10:
                                    title = $"5. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 10%:";
                                    break;
                                default:
                                    break;
                            }
                            using (ExcelRange rng = worksheet.Cells[$"A{row}:J{row}"])
                            {
                                rng.Merge =true;
                                rng.Value = title;
                            }
                            row++;
                            int i = 1;
                            foreach (var item in invoices)
                            {
                                worksheet.Cells[row, 1].Value = i++;
                                worksheet.Cells[row, 2].Value = item.Patern;
                                worksheet.Cells[row, 3].Value = item.Serial;
                                worksheet.Cells[row, 4].Value = item.InvoiceNo.ToString("00000000");
                                worksheet.Cells[row, 5].Value = item.SignDate.ToString("dd/MM/yyyy");
                                worksheet.Cells[row, 6].Value = !string.IsNullOrEmpty(item.Buyer) ? item.Buyer : item.CusName;
                                worksheet.Cells[row, 7].Value = item.TaxCode;
                                worksheet.Cells[row, 8].Value = item.ProductName;
                                worksheet.Cells[row, 9].Value = item.ProductQuantity;
                                worksheet.Cells[row, 10].Value = item.ProductUnit;
                                worksheet.Cells[row, 11].Value = item.ProductPrice;
                                worksheet.Cells[row, 12].Value = item.ProductTotal;
                                worksheet.Cells[row, 13].Value = item.ProductVATAmount;
                                worksheet.Cells[row, 14].Value = (item.Status == Enums.StatusEinvoice.CanceledInv ? "Hóa đơn hủy bỏ" : "");
                                row++;
                            }
                          
                        }
                        worksheet.Cells[$"A{12}:N{row}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[$"A{12}:N{row}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[$"A{12}:N{row}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[$"A{12}:N{row}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                        row++;

                        worksheet.Cells[row, 1].Value = $"Tổng doanh thu hàng hoá, dịch vụ bán ra: ";
                        worksheet.Cells[row, 2].Value = totals;
                        worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[row + 1, 1].Value = $"Tổng thuế GTGT của hàng hóa, dịch vụ bán ra:";
                        worksheet.Cells[row + 1, 2].Value = vatamounts;
                        worksheet.Cells[row + 1, 2].Style.Numberformat.Format = "#,##0";
                        //Add a formula for the value-column
                        // worksheet.Cells["E2:E4"].Formula = "C2*D2";
                        //Ok now format the values;
                        //using (var range = worksheet.Cells[1, 1, 1, 5])
                        //{
                        //    range.Style.Font.Bold = true;
                        //    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //    range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                        //    range.Style.Font.Color.SetColor(Color.White);
                        //}
                  

                        //        worksheet.Cells[5, 3, 5, 5].Formula = string.Format("SUBTOTAL(9,{0})",
                        //new ExcelAddress(2, 3, 4, 3).Address);
                        worksheet.Cells[$"K{12}:K{row}"].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[$"L{12}:L{row}"].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[$"M{12}:M{row}"].Style.Numberformat.Format = "#,##0";
                        //Create an autofilter for the range
                        //worksheet.Cells[$"A{12}:J{row}"].AutoFilter = true;
                        worksheet.Cells[$"A{10}:J{row}"].Style.WrapText = false;
                        worksheet.Cells[$"A{12}:J{row}"].AutoFitColumns(10);
                        // lets set the header text
                        //worksheet.HeaderFooter.oddHeader.CenteredText = "&24&U&\"Arial,Regular
                        // Bold\" Inventory";
                        // add the page number to the footer plus the total number of pages
                        ////// worksheet.HeaderFooter.OddFooter.RightAlignedText =
                        //string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                        // add the sheet name to the footer
                        // worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
                        // add the file path to the footer
                        //worksheet.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath
                        //+ ExcelHeaderFooter.FileName;
                        //  worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:2"];
                        //    worksheet.PrinterSettings.RepeatColumns = worksheet.Cells["A:G"];
                        // Change the sheet view to show it in page layout mode
                        //worksheet.View.PageLayoutView = true;
                        // set some document properties
                        //        package.Workbook.Properties.Title = "Invertory";
                        //        package.Workbook.Properties.Author = "Jan Källman";
                        //        package.Workbook.Properties.Comments = "This sample demonstrates how
                        //         to create an Excel 2007 workbook using EPPlus";
                        //                 // set some extended property values
                        //                 package.Workbook.Properties.Company = "AdventureWorks Inc.";
                        //        // set some custom property values
                        //        package.Workbook.Properties.SetCustomPropertyValue("Checked by", "Jan

                        //Källman");

                        //        package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");
                        // save our new workbook and we are done!
                        ResponseReport responseReport = new ResponseReport();
                        responseReport.dataExcel = package.GetAsByteArray();
                        return await Result<ResponseReport>.SuccessAsync(responseReport);

                    }
                }
                return Result<ResponseReport>.Fail(HeperConstantss.ERR012);
            }
        }
    }
}
