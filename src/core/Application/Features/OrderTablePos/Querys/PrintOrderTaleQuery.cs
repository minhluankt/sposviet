using Application.Constants;
using Application.Enums;
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
                var product = await _repository.GetAllQueryable().AsNoTracking().Include(x => x.OrderTableItems).SingleOrDefaultAsync(x => x.ComId == query.Comid && x.IdGuid== query.IdOrder);

                CompanyAdminInfo company = _companyProductRepository.GetCompany(query.Comid);
                TemplateInvoice templateInvoice = await _templateInvoicerepository.GetTemPlate(query.Comid);
                if (templateInvoice!=null)
                {
                    TemplateInvoiceParameter templateInvoiceParameter = new TemplateInvoiceParameter()
                    {
                        lienhehotline = SystemVariableHelper.lienhehotline,
                        invoiceNo = product.OrderTableCode,
                        buyer = product.Buyer,
                        casherName = product.CasherName,
                        staffName = product.StaffName,
                        cusPhone = product.Customer?.PhoneNumber,
                        cusAddress = product.Customer?.Address,
                        comname = !string.IsNullOrEmpty(company.Title) ? company.Title.Trim() : company.Name,
                        comaddress = company?.Address,
                        tientruocthue = product.Amonut.ToString("N0"),
                        khachcantra = (product.Amonut).ToString("N0"),
                        //khachthanhtoan = product.AmountCusPayment?.ToString("N0"),
                        //tienthuatrakhach = product.AmountChangeCus?.ToString("N0"),
                    };
                    if (query.vat)
                    {
                        var vatrate = (query.VATRate / 100.0M);
                        var tienthue = vatrate * product.Amonut;
                        templateInvoiceParameter.tienthue = tienthue.ToString("N0");
                        templateInvoiceParameter.thuesuat = query.VATRate.ToString();
                        templateInvoiceParameter.khachcantra = (tienthue + product.Amonut).ToString("N0");
                    }
                    string tableProduct = string.Empty;
                    foreach (var item in product.OrderTableItems)
                    {
                        tableProduct += $"<tr>" +
                                            $"<td colspan=\"4\"><span style=\"display: block;font-size: 11px\">{item.Name}</span></td>" +
                                        "</tr>" +
                                        "<tr>" +
                                            $"<td><span style=\"display: block;  text-align: left;font-size: 11px\">{item.Price.ToString("N0")}</span></td>" +
                                            $"<td style='text-align: center'><span style=\"display: block;font-size: 11px\">{item.Quantity.ToString("N0")}</span></td>" +
                                            $"<td><span style=\"display: block; text-align: center;font-size: 11px\">{item.Unit}</span></td>" +
                                            $"<td><span style=\"display: block; text-align: right;font-size: 11px\">{item.Total.ToString("N0")}</span></td>" +
                                        "</tr>";

                    }
                    templateInvoiceParameter.tableProduct = tableProduct;
                    string thongtinthue = string.Empty;
                    if (query.VATRate != (int)VATRateInv.KHONGVAT)
                    {
                        thongtinthue = $"<tr style='font-size: 11px; text-align: left'>" +
                                $"<td colspan=\"3\">Tiền thuế: {templateInvoiceParameter.thuesuat}%</td>\r\n\t\t\t<td style=\"text-align: right;\">{templateInvoiceParameter.tienthue}</td>" +
                                $"</tr>";

                    }
                    templateInvoiceParameter.thongtinthue = thongtinthue;
                    templateInvoice.Template = templateInvoice.Template.Replace("HÓA ĐƠN BÁN HÀNG","HOA ĐƠN TẠM TÍNH");

                    //đoạn này là xóa đi vì chưa phát hành hóa đơn điện tử dc
                    string regex = @"<.*?{kyhieuhoadon}.*?>";
                    Regex rg = new Regex(regex);
                    var match = rg.Match(templateInvoice.Template);
                    String result = match.Groups[0].Value;
                    if (!string.IsNullOrEmpty(result))
                    {
                        templateInvoice.Template = templateInvoice.Template.Replace(result, "");
                    }
                    string regexsohoadon = @"<.*?{sohoadon}.*?>";
                    rg = new Regex(regexsohoadon);
                    match = rg.Match(templateInvoice.Template);
                    result = match.Groups[0].Value;
                    if (!string.IsNullOrEmpty(result))
                    {
                        templateInvoice.Template = templateInvoice.Template.Replace(result, "");
                    }

                    string content = LibraryCommon.GetTemplate(templateInvoiceParameter, templateInvoice.Template, EnumTypeTemplate.INVOICEPOS);
                    return Result<string>.Success(content, HeperConstantss.SUS014);
                }

                return await Result<string>.FailAsync("Vui lòng cấu hình mẫu in tạm tính!");
            }
        }
    }
}
