using Application.Constants;
using Application.Enums;
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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SystemVariable;

namespace Application.Features.OrderTablePos.Commands
{

    public class CheckOutOrderCommand : IRequest<Result<string>>
    {
        public EnumTypeProduct TypeUpdate { get; set; }
        public Guid IdOrder { get; set; }
        public decimal discountPayment { get; set; }
        public decimal discount { get; set; }
        public decimal? cuspayAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal VATAmount { get; set; }
        public bool removeOrder { get; set; }
        public int Idpayment { get; set; }
        public int ComId { get; set; }
        public string Cashername { get; set; }
        public string IdCasher { get; set; }
        public bool vat { get; set; }
        public int? Vatrate { get; set; }
            public int? ManagerPatternEInvoices { get; set; }
        public class CheckOutOrderHandler : IRequestHandler<CheckOutOrderCommand, Result<string>>
        {
            private readonly ITemplateInvoiceRepository<TemplateInvoice> _templateInvoicerepository;
            private readonly IOrderTableRepository _Repository;
            private readonly ICompanyAdminInfoRepository _companyProductRepository;
            private readonly IRepositoryAsync<Product> _ProductRepository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public CheckOutOrderHandler(IRepositoryAsync<Product> ProductRepository, ICompanyAdminInfoRepository companyProductRepository,
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
            public async Task<Result<string>> Handle(CheckOutOrderCommand command, CancellationToken cancellationToken)
            {
                var product = await _Repository.CheckOutOrderAsync(
                    command.ComId,
                    command.Idpayment,
                    command.IdOrder,
                    command.discountPayment,
                    command.discount,
                    command.cuspayAmount,
                    command.Amount,
                    command.VATAmount,
                    command.Cashername,
                    command.IdCasher,
                    command.vat,
                    command.Vatrate,
                    command.ManagerPatternEInvoices,
                    command.TypeUpdate
                    );
                if (product.Succeeded)
                {
                    CompanyAdminInfo company = _companyProductRepository.GetCompany(command.ComId);
                    TemplateInvoice templateInvoice = await _templateInvoicerepository.GetTemPlate(command.ComId);
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
                            comname = !string.IsNullOrEmpty(company.Title)? company.Title.Trim(): company.Name,
                            comaddress = company?.Address,

                            tongtien = product.Data.Invoice.Amonut.ToString("N0"),
                            tientruocthue = product.Data.Invoice.Total.ToString("N0"),
                            tienthue = product.Data.Invoice.VATAmount.ToString("N0"),
                            thuesuat = product.Data.Invoice.VATRate?.ToString("N0"),
                            giamgia = product.Data.Invoice.DiscountAmount.ToString("N0"),
                            khachcantra = (product.Data.Invoice.Amonut).ToString("N0"),
                            khachthanhtoan = product.Data.Invoice.AmountCusPayment?.ToString("N0"),
                            tienthuatrakhach = product.Data.Invoice.AmountChangeCus?.ToString("N0"),
                          
                        };
                        if (product.Data.IsSuccess)
                        {
                            templateInvoiceParameter.kyhieuhoadon = GetEInvoiceNoFormat.get_kyhieu(product.Data.Pattern, product.Data.Serial);
                            templateInvoiceParameter.sohoadon = GetEInvoiceNoFormat.get_no(product.Data.InvoiceNo);
                        }
                        string tableProduct = string.Empty;
                        foreach (var item in product.Data.Invoice.InvoiceItems)
                        {
                            tableProduct += $"<tr>" +
                                                $"<td colspan=\"4\"><span style=\"display: block;font-size: 11px\">{item.Name}</span></td>" +
                                            "</tr>" +
                                            "<tr style='border-botom-style: dotted;border-width: 0.3px'>" +
                                                $"<td><span style=\"display: block;  text-align: left;font-size: 11px\">{item.Price.ToString("N0")}</span></td>" +
                                                $"<td style='text-align: right'><span style=\"display: block; font-size: 11px\">{item.Quantity.ToString("N0")}</span></td>" +
                                                $"<td><span style=\"display: block; text-align: center;font-size: 11px\">{item.Unit}</span></td>" +
                                                $"<td><span style=\"display: block; text-align: right;font-size: 11px\">{item.Total.ToString("N0")}</span></td>" +
                                            "</tr>";

                        }

                        string thongtinthue = string.Empty;
                        if (product.Data.Invoice.VATRate!= (int)VATRateInv.KHONGVAT)
                        {
                            thongtinthue =  $"<tr style='font-size: 11px; text-align: left'>" +
                                            $"<td colspan=\"3\">Tiền thuế: {templateInvoiceParameter.thuesuat}%</td>\r\n\t\t\t<td style=\"text-align: right;\">{templateInvoiceParameter.tienthue}</td>" +
                                            $"</tr>";
                           
                        }
                        templateInvoiceParameter.tableProduct = tableProduct;
                        string thongtintracuuhoadon = string.Empty;
                        
                        if (!product.Data.IsSuccess)
                        {
                            //đoạn này là xóa đi vì chưa phát hành hóa đơn điện tử dc
                            string regex = @"<.*?{kyhieuhoadon}.*?>";
                             Regex rg = new Regex(regex);
                             var match = rg.Match(templateInvoice.Template);
                           // MatchCollection coll = Regex.Matches(templateInvoice.Template, regex);
                            //String result = coll[0].Groups[1].Value;
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
                        }
                        else
                        {

                            if (!string.IsNullOrEmpty(product.Data.Fkey))
                            {
                                string url = InfoSeachInvCons.thong_tin_tra_cuu(product.Data.UrlDomain, product.Data.Fkey, product.Data.MCQT);
                                thongtintracuuhoadon = $"<hr />" +
                                                        $"<table style='margin-top: 0mm;width: 100%;'>" +
                                                        $"<tr style='width: 100%;'>" +
                                                        $"<td style='text-align: center; font-size: 11px;'>" +
                                                        $"<span style='display: block;'>{url}</span>" +
                                                        $"</td>" +
                                                        $"</tr>" +
                                                        $"</table>";
                            }
                        }
                        templateInvoiceParameter.thongtinthue = thongtinthue;
                        templateInvoiceParameter.thongtintracuuhoadon = thongtintracuuhoadon;
                        string content = LibraryCommon.GetTemplate(templateInvoiceParameter, templateInvoice.Template, EnumTypeTemplate.INVOICEPOS);
                     
                        return Result<string>.Success(content, HeperConstantss.SUS014);
                    }
                    return Result<string>.Success("Công ty chưa cấu hình mẫu in", HeperConstantss.SUS014);
                }
                return Result<string>.Fail(product.Message);
            }
        }
    }
}
