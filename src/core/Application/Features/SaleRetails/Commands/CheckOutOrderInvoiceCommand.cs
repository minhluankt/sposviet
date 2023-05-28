using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using Domain.XmlDataModel;
using HelperLibrary;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SystemVariable;

namespace Application.Features.SaleRetails.Commands
{

    public class CheckOutOrderInvoiceCommand : OrderInvoicePaymentSaleRetailModel,IRequest<Result<string>>
    {

        public class CheckOutOrderInvoiceHandler : IRequestHandler<CheckOutOrderInvoiceCommand, Result<string>>
        {
            private readonly ITemplateInvoiceRepository<TemplateInvoice> _templateInvoicerepository;
            private readonly IOrderTableRepository _Repository;
            private readonly ICompanyAdminInfoRepository _companyProductRepository;
            private readonly IRepositoryAsync<Product> _ProductRepository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public CheckOutOrderInvoiceHandler(IRepositoryAsync<Product> ProductRepository, ICompanyAdminInfoRepository companyProductRepository,
                IOrderTableRepository brandRepository, ITemplateInvoiceRepository<TemplateInvoice> templateInvoicerepository,

                IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {

                _companyProductRepository = companyProductRepository;
                _templateInvoicerepository = templateInvoicerepository;
                _ProductRepository = ProductRepository;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<string>> Handle(CheckOutOrderInvoiceCommand command, CancellationToken cancellationToken)
            {
                var product = await _Repository.CheckOutOrderInvoiceAsync(command, command.EnumTypeProduct);
                if (product.Succeeded)
                {
                    CompanyAdminInfo company = _companyProductRepository.GetCompany(command.ComId);
                    TemplateInvoice templateInvoice = await _templateInvoicerepository.GetTemPlate(command.ComId, EnumTypeTemplatePrint.IN_BILL);
                    if (templateInvoice != null)
                    {
                        TemplateInvoiceParameter templateInvoiceParameter = new TemplateInvoiceParameter()
                        {
                            lienhehotline = SystemVariableHelper.lienhehotline,
                            invoiceNo = product.Data.Invoice.InvoiceCode,
                            buyer = product.Data.Invoice.Buyer,
                            casherName = product.Data.Invoice.CasherName,
                            cusPhone = product.Data.Invoice.Customer?.PhoneNumber,
                            cusAddress = product.Data.Invoice.Customer?.Address,

                            comname = !string.IsNullOrEmpty(company.Title) ? company.Title.Trim() : company.Name,
                            comaddress = company?.Address,
                            comphone = company?.PhoneNumber,
                            comemail = company?.Email,
                            isVAT = command.VATMTT,

                            tongtien = product.Data.Invoice.Amonut.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            tientruocthue = (product.Data.IsProductVAT) ? (product.Data.Invoice.VATAmount + product.Data.Invoice.Total).ToString("#,0.##", LibraryCommon.GetIFormatProvider()) : product.Data.Invoice.Total.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            tienthue = product.Data.Invoice.VATAmount.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            thuesuat = product.Data.Invoice.VATRate?.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            giamgia = (product.Data.Invoice.DiscountAmount + product.Data.Invoice.DiscountOther).ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            khachcantra = (product.Data.Invoice.Amonut).ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            khachthanhtoan = product.Data.Invoice.AmountCusPayment?.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            tienthuatrakhach = product.Data.Invoice.AmountChangeCus?.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                          
                        };
                        if (product.Data.IsSuccess)
                        {
                            templateInvoiceParameter.kyhieuhoadon = GetEInvoiceNoFormat.get_kyhieu(product.Data.Pattern, product.Data.Serial);
                            templateInvoiceParameter.sohoadon = GetEInvoiceNoFormat.get_no(product.Data.InvoiceNo);
                        }
                        if (!string.IsNullOrEmpty(product.Data.Fkey))
                        {
                            templateInvoiceParameter.linktracuu = product.Data.UrlDomain;
                            templateInvoiceParameter.matracuu = product.Data.Fkey;
                            templateInvoiceParameter.macoquanthue = product.Data.MCQT;


                    
                        }

                        //string tableProduct = string.Empty;
                        //foreach (var item in product.Data.Invoice.InvoiceItems)
                        //{
                        //    tableProduct += $"<tr>" +
                        //                        $"<td colspan=\"4\"><span style=\"display: block;font-size: 11px\">{item.Name}</span></td>" +
                        //                    "</tr>" +
                        //                    "<tr style='border-botom-style: dotted;border-width: 0.3px'>" +
                        //                        $"<td><span style=\"display: block;  text-align: left;font-size: 11px\">{item.Price.ToString("#,0.##", LibraryCommon.GetIFormatProvider())}</span></td>" +
                        //                        $"<td style='text-align: right'><span style=\"display: block; font-size: 11px\">{item.Quantity.ToString("#,0.##", LibraryCommon.GetIFormatProvider())}</span></td>" +
                        //                        $"<td><span style=\"display: block; text-align: center;font-size: 11px\">{item.Unit}</span></td>" +
                        //                        $"<td><span style=\"display: block; text-align: right;font-size: 11px\">{item.Total.ToString("#,0.##", LibraryCommon.GetIFormatProvider())}</span></td>" +
                        //                    "</tr>";

                        //}

                        //string thongtinthue = string.Empty;
                        //if (product.Data.Invoice.VATRate!= (int)VATRateInv.KHONGVAT && product.Data.Invoice.VATRate!= (int)NOVAT.NOVAT)
                        //{
                        //    thongtinthue =  $"<tr style='font-size: 11px; text-align: left'>" +
                        //                    $"<td colspan=\"3\">Tiền thuế: {templateInvoiceParameter.thuesuat}%</td>\r\n\t\t\t<td style=\"text-align: right;\">{templateInvoiceParameter.tienthue}</td>" +
                        //                    $"</tr>";

                        //}
                        //templateInvoiceParameter.tableProduct = tableProduct;
                        //string thongtintracuuhoadon = string.Empty;

                        //if (!product.Data.IsSuccess)
                        //{
                        //    //đoạn này là xóa đi vì chưa phát hành hóa đơn điện tử dc
                        //    string regex = @"<.*?{kyhieuhoadon}.*?>";
                        //     Regex rg = new Regex(regex);
                        //     var match = rg.Match(templateInvoice.Template);
                        //   // MatchCollection coll = Regex.Matches(templateInvoice.Template, regex);
                        //    //String result = coll[0].Groups[1].Value;
                        //    String result = match.Groups[0].Value;
                        //    if (!string.IsNullOrEmpty(result))
                        //    {
                        //        templateInvoice.Template = templateInvoice.Template.Replace(result, "");
                        //    }
                        //    string regexsohoadon = @"<.*?{sohoadon}.*?>";
                        //    rg = new Regex(regexsohoadon);
                        //    match = rg.Match(templateInvoice.Template);
                        //    result = match.Groups[0].Value;
                        //    if (!string.IsNullOrEmpty(result))
                        //    {
                        //        templateInvoice.Template = templateInvoice.Template.Replace(result, "");
                        //    }
                        //}
                        //else
                        //{

                        //    if (!string.IsNullOrEmpty(product.Data.Fkey))
                        //    {
                        //        string url = InfoSeachInvCons.thong_tin_tra_cuu(product.Data.UrlDomain, product.Data.Fkey, product.Data.MCQT);
                        //        thongtintracuuhoadon = $"<hr />" +
                        //                                $"<table style='margin-top: 0mm;width: 100%;'>" +
                        //                                $"<tr style='width: 100%;'>" +
                        //                                $"<td style='text-align: center; font-size: 11px;'>" +
                        //                                $"<span style='display: block;'>{url}</span>" +
                        //                                $"</td>" +
                        //                                $"</tr>" +
                        //                                $"</table>";
                        //    }
                        //}
                        //templateInvoiceParameter.thongtinthue = thongtinthue;
                        //templateInvoiceParameter.thongtintracuuhoadon = thongtintracuuhoadon;
                        try
                        {
                            string content = PrintTemplate.PrintInvoice(templateInvoiceParameter, product.Data.Invoice.InvoiceItems.ToList(), templateInvoice.Template);
                            return Result<string>.Success(content, HeperConstantss.SUS014);
                        }
                        catch (Exception e)
                        {
                            
                            return Result<string>.Success("Lỗi khi tìm mẫu in", HeperConstantss.SUS014);
                        };
                    }
                    return Result<string>.Success("Công ty chưa cấu hình mẫu in", HeperConstantss.SUS014);
                }
                return Result<string>.Fail(product.Message);
            }
        }
    }
}
