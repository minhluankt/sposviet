using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using HelperLibrary;
using Library;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SystemVariable;

namespace Application.Features.OrderTablePos.Querys
{
    public class PrintOrderTaleQuery : IRequest<Result<string>>
    {
        public string casherName { get; set; }
        public bool vat { get; set; }
        public int VATRate { get; set; }
        public int Comid { get; set; }
        public Guid? IdOrder { get; set; }
        public class PrintOrderTaleQueryHandler : IRequestHandler<PrintOrderTaleQuery, Result<string>>
        {
            private readonly ITemplateInvoiceRepository<TemplateInvoice> _templateInvoicerepository;
            private readonly ICompanyAdminInfoRepository _companyProductRepository;
            private readonly IRepositoryAsync<OrderTable> _repository;
            public PrintOrderTaleQueryHandler(IRepositoryAsync<OrderTable> repository,
                ICompanyAdminInfoRepository companyProductRepository, ITemplateInvoiceRepository<TemplateInvoice> templateInvoicerepository)
            {
                _templateInvoicerepository = templateInvoicerepository;
                _companyProductRepository = companyProductRepository;
                _repository = repository;
            }
            public async Task<Result<string>> Handle(PrintOrderTaleQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetAllQueryable().AsNoTracking().Include(x => x.OrderTableItems).Include(x => x.Customer).Include(x => x.RoomAndTable).SingleOrDefaultAsync(x => x.ComId == query.Comid && x.IdGuid== query.IdOrder);

                CompanyAdminInfo company = _companyProductRepository.GetCompany(query.Comid);
                TemplateInvoice templateInvoice = await _templateInvoicerepository.GetTemPlate(query.Comid,EnumTypeTemplatePrint.IN_TAM_TINH);
                if (templateInvoice!=null)
                {
                    var checkvatrate = product.OrderTableItems.Where(x => x.IsVAT).Select(x => x.VATRate).Distinct().ToArray();
                    if (checkvatrate.Count() > 1)
                    {
                        return await Result<string>.FailAsync(GeneralMess.ConvertStatusToString(HeperConstantss.ERR050));
                    }
                    else if (checkvatrate != null && checkvatrate.Count() > 0 && query.vat)
                    {
                        if (checkvatrate[0] != query.VATRate)
                        {
                            return await Result<string>.FailAsync($"Chi tiết hàng hóa có thuế suất là {checkvatrate[0].ToString("#,0.##", LibraryCommon.GetIFormatProvider())}%, không khớp thuế suất {query.VATRate}% bạn chọn, vui lòng chọn lại!");
                        }
                    }

                    TemplateInvoiceParameter templateInvoiceParameter = new TemplateInvoiceParameter()
                    {
                        giovao = product.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss"),
                        lienhehotline = SystemVariableHelper.lienhehotline,
                        invoiceNo = product.OrderTableCode,
                        buyer = product.Buyer,
                        casherName = query.casherName,
                        staffName = product.StaffName,
                        cusPhone = product.Customer?.PhoneNumber,
                        cusAddress = product.Customer?.Address,
                        cuscode = product.Customer?.Code,
                        tenbanphong = product.RoomAndTable!=null? product.RoomAndTable.Name:"Mang về",

                        comname = !string.IsNullOrEmpty(company.Title) ? company.Title.Trim() : company.Name,
                        comaddress = company?.Address,
                        comphone = company?.PhoneNumber,
                        comemail = company?.Email,

                        giamgia = "0",
                        TypeTemplatePrint = EnumTypeTemplatePrint.IN_TAM_TINH,
                        tientruocthue = product.Amonut.ToString("#,0.###", LibraryCommon.GetIFormatProvider()),
                        khachcantra = (product.Amonut).ToString("#,0.###", LibraryCommon.GetIFormatProvider()),
                        //khachthanhtoan = product.AmountCusPayment?.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                        //tienthuatrakhach = product.AmountChangeCus?.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                    };

                    bool IsProductVAT = false;
                    bool IsServiceDate = false;
                   
                    var neworderitem = new List<OrderTableItem>();
                    foreach (var item in product.OrderTableItems.GroupBy(x=>x.IdProduct))
                    {
                        var _item = item.First().CloneJson();
                        _item.Quantity = item.Sum(x => x.Quantity);
                        _item.Total = item.Sum(x => x.Total);
                        _item.VATAmount = item.Sum(x => x.VATAmount);//_item.Total* (_item.VATRate/100.0M);
                        _item.Amount = item.Sum(x => x.Amount);
                        if (!_item.IsVAT && query.vat)//nếu hàng hóa k có thuế mà hóa đơn có thuế thì xử lý thêm thuế cho hàng hóa đó
                        {
                            _item.VATAmount = _item.Total* (query.VATRate / 100.0M);
                            _item.Amount =  _item.Total + _item.VATAmount;
                        }
                        else if (_item.IsVAT)//nếu sp có thuế thì đánh dấu
                        {
                            IsProductVAT = true;
                        }

                        // xử lý hàng hóa khi có dịch vụ là san phẩm tính tiền theo giờ mà vẫn đang tính giờ chưa dừng
                        if (_item.IsServiceDate && _item.DateEndService==null)
                        {
                            IsServiceDate = true;
                            DateTime enddate = _item.DateEndService ?? DateTime.Now;
                            var timespan = enddate.Subtract(_item.DateCreateService.Value);
                            _item.Quantity = (timespan.Days*24) + timespan.Hours + Math.Round((decimal)timespan.Minutes / 60, 2);
                            _item.Total = _item.Quantity * (_item.IsVAT ? _item.PriceNoVAT : _item.Price);
                            _item.VATAmount = _item.IsVAT ? _item.Total * (_item.VATRate / 100.0M) : 0;
                            _item.Amount = _item.VATAmount + _item.Total;
                            if (_item.DateEndService==null)
                            {
                                _item.DateEndService = DateTime.Now;
                            }
                        }
                        neworderitem.Add(_item);
                    }
                    if (IsServiceDate)//xử lý tổng tiền trước nếu có tính dịch vụ
                    {
                        product.Amonut= neworderitem.Sum(x => x.Amount);
                        templateInvoiceParameter.tientruocthue = product.Amonut.ToString("#,0.###", LibraryCommon.GetIFormatProvider());
                        templateInvoiceParameter.khachcantra = (product.Amonut).ToString("#,0.###", LibraryCommon.GetIFormatProvider());
                    }

                    if (query.vat)
                    {
                        templateInvoiceParameter.thuesuat = query.VATRate.ToString();
                        templateInvoiceParameter.isVAT = query.vat;
                        if (IsProductVAT)
                        {
                            var tienthue = neworderitem.Sum(x=>x.VATAmount);
                            templateInvoiceParameter.tienthue = tienthue.ToString("#,0.###", LibraryCommon.GetIFormatProvider());
                            templateInvoiceParameter.khachcantra = Math.Round(neworderitem.Sum(x => x.Amount),MidpointRounding.AwayFromZero).ToString("#,0.##", LibraryCommon.GetIFormatProvider());
                        }
                        else
                        {
                            var vatrate = (query.VATRate / 100.0M);
                            var tienthue = vatrate * product.Amonut;
                            templateInvoiceParameter.tienthue = tienthue.ToString("#,0.###", LibraryCommon.GetIFormatProvider());
                            templateInvoiceParameter.khachcantra = (tienthue + product.Amonut).ToString("#,0.##", LibraryCommon.GetIFormatProvider());
                        }
                    }
                    
                    if (IsProductVAT)//check trường hợp nếu sản phẩm có dòng đơn giá đã gồm thuế
                    {
                        if (query.vat)//nếu hóa đơn có thuế thì hiển thị tiền trước thuế phải là tiền trước thuế của sản phẩm có và k có thuế
                        {
                            templateInvoiceParameter.tientruocthue = neworderitem.Sum(x => x.Total).ToString("#,0.###", LibraryCommon.GetIFormatProvider());//update lại tiền trước thuế cho đúng
                        }
                        else//hóa đơn k có thuế mà sp có thuế thì hiển tiền sau thuế
                        {
                            templateInvoiceParameter.tientruocthue = neworderitem.Sum(x => x.Amount).ToString("#,0.###", LibraryCommon.GetIFormatProvider());//update lại tiền trước thuế cho đúng
                        }

                    }
                    else
                    {
                        if (query.vat)//nếu hóa đơn có thuế
                        {
                            foreach (var item in neworderitem)
                            {
                                if (!item.IsVAT)//là sp đơn giá không có thuế
                                {
                                    item.Amount = item.Total;//update lại amount để hiển thị lên bill cho đúng là tiền trước thuế của sp đó
                                }
                            }
                        }
                    }



                    string content = PrintTemplate.PrintOrder(templateInvoiceParameter, neworderitem, templateInvoice.Template);

                    //string tableProduct = string.Empty;
                    //foreach (var item in product.OrderTableItems)
                    //{
                    //    tableProduct += $"<tr>" +
                    //                        $"<td colspan=\"4\"><span style=\"display: block;font-size: 11px\">{item.Name}</span></td>" +
                    //                    "</tr>" +
                    //                    "<tr>" +
                    //                        $"<td><span style=\"display: block;  text-align: left;font-size: 11px\">{item.Price.ToString("#,0.##", LibraryCommon.GetIFormatProvider())}</span></td>" +
                    //                        $"<td style='text-align: center'><span style=\"display: block;font-size: 11px\">{item.Quantity.ToString("#,0.##", LibraryCommon.GetIFormatProvider())}</span></td>" +
                    //                        $"<td><span style=\"display: block; text-align: center;font-size: 11px\">{item.Unit}</span></td>" +
                    //                        $"<td><span style=\"display: block; text-align: right;font-size: 11px\">{item.Total.ToString("#,0.##", LibraryCommon.GetIFormatProvider())}</span></td>" +
                    //                    "</tr>";

                    //}
                    //templateInvoiceParameter.tableProduct = tableProduct;
                    //string thongtinthue = string.Empty;
                    //if (query.VATRate != (int)VATRateInv.KHONGVAT)
                    //{
                    //    thongtinthue = $"<tr style='font-size: 11px; text-align: left'>" +
                    //            $"<td colspan=\"3\">Tiền thuế: {templateInvoiceParameter.thuesuat}%</td>\r\n\t\t\t<td style=\"text-align: right;\">{templateInvoiceParameter.tienthue}</td>" +
                    //            $"</tr>";

                    //}
                    //templateInvoiceParameter.thongtinthue = thongtinthue;
                    //templateInvoice.Template = templateInvoice.Template.Replace("HÓA ĐƠN BÁN HÀNG","HOA ĐƠN TẠM TÍNH");


                    //đoạn này là xóa đi vì chưa phát hành hóa đơn điện tử dc
                    //string regex = @"<.*?{kyhieuhoadon}.*?>";
                    //Regex rg = new Regex(regex);
                    //var match = rg.Match(templateInvoice.Template);
                    //String result = match.Groups[0].Value;
                    //if (!string.IsNullOrEmpty(result))
                    //{
                    //    templateInvoice.Template = templateInvoice.Template.Replace(result, "");
                    //}
                    //string regexsohoadon = @"<.*?{sohoadon}.*?>";
                    //rg = new Regex(regexsohoadon);
                    //match = rg.Match(templateInvoice.Template);
                    //result = match.Groups[0].Value;
                    //if (!string.IsNullOrEmpty(result))
                    //{
                    //    templateInvoice.Template = templateInvoice.Template.Replace(result, "");
                    //}

                    // string content = LibraryCommon.GetTemplate(templateInvoiceParameter, templateInvoice.Template, EnumTypeTemplate.INVOICEPOS);
                    return Result<string>.Success(content, HeperConstantss.SUS014);
                }

                return await Result<string>.FailAsync("Vui lòng cấu hình mẫu in tạm tính!");
            }
        }
    }
}
