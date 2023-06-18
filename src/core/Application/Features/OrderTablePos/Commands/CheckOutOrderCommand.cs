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
using System.Collections.Generic;
using System.Linq;
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
        public decimal discountOther { get; set; }
        public decimal discountPayment { get; set; }
        public decimal discount { get; set; }
        public decimal? cuspayAmount { get; set; }
        public decimal Total { get; set; }
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
            private readonly IManagerPatternEInvoiceRepository<ManagerPatternEInvoice> _managerPatternEInvoicerepository;
            private readonly ITemplateInvoiceRepository<TemplateInvoice> _templateInvoicerepository;
            private readonly IOrderTableRepository _Repository;
            private readonly ICompanyAdminInfoRepository _companyProductRepository;
            private readonly IRepositoryAsync<Product> _ProductRepository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public CheckOutOrderHandler(IRepositoryAsync<Product> ProductRepository,
                IManagerPatternEInvoiceRepository<ManagerPatternEInvoice> managerPatternEInvoicerepository,
                ICompanyAdminInfoRepository companyProductRepository,
                IOrderTableRepository brandRepository, ITemplateInvoiceRepository<TemplateInvoice> templateInvoicerepository,

                IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {

                _companyProductRepository = companyProductRepository;
                _templateInvoicerepository = templateInvoicerepository;
                _ProductRepository = ProductRepository;
                _managerPatternEInvoicerepository = managerPatternEInvoicerepository;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<string>> Handle(CheckOutOrderCommand command, CancellationToken cancellationToken)
            {
                if (command.vat)
                {
                    if (command.ManagerPatternEInvoices==null)
                    {
                        return await Result<string>.FailAsync(HeperConstantss.ERR049);
                    }
                    var getpattern = await _managerPatternEInvoicerepository.GetbyIdAsync(command.ManagerPatternEInvoices.Value, true);
                    if (getpattern == null)
                    {
                        return await Result<string>.FailAsync(HeperConstantss.ERR049);
                    }
                    else
                    {
                        if (!LibraryCommon.IsHDDTMayTinhTien(getpattern.Serial))
                        {
                            return await Result<string>.FailAsync(HeperConstantss.ERR049);
                        }
                    }
                }

                var product = await _Repository.CheckOutOrderAsync(
                    command.ComId,
                    command.Idpayment,
                    command.IdOrder,
                    command.discountPayment,
                    command.discount,
                    command.discountOther,
                    command.cuspayAmount,
                    command.Total,
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
                    TemplateInvoice templateInvoice = await _templateInvoicerepository.GetTemPlate(command.ComId,EnumTypeTemplatePrint.IN_BILL);
                    if (templateInvoice != null)
                    {
                        TemplateInvoiceParameter templateInvoiceParameter = new TemplateInvoiceParameter()
                        {
                            giovao = product.Data.Invoice.ArrivalDate.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                            ngaythangnamxuat = product.Data.Invoice.PurchaseDate.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                            TypeTemplatePrint = EnumTypeTemplatePrint.IN_BILL,
                            lienhehotline = SystemVariableHelper.lienhehotline,
                            invoiceNo = product.Data.Invoice.InvoiceCode,
                            buyer = !string.IsNullOrEmpty(product.Data.Invoice.CusName)? product.Data.Invoice.CusName : product.Data.Invoice.Buyer,
                            casherName = product.Data.Invoice.CasherName,
                            staffName = product.Data.Invoice.StaffName,
                            cusPhone = product.Data.Invoice.PhoneNumber,
                            cusAddress = product.Data.Invoice.Address,
                            cuscode = product.Data.Invoice.CusCode,
                            tenbanphong = product.Data.Invoice.RoomAndTable != null ? product.Data.Invoice.RoomAndTable.Name : "Mang về",
                            comname = !string.IsNullOrEmpty(company.Title)? company.Title.Trim(): company.Name,
                            comaddress = company?.Address,
                            comphone = company?.PhoneNumber,
                            comemail = company?.Email,

                            isVAT = command.vat,
                            tongtien = product.Data.Invoice.Amonut.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            tientruocthue = (product.Data.IsProductVAT) ? (product.Data.Invoice.VATAmount+ product.Data.Invoice.Total).ToString("#,0.##", LibraryCommon.GetIFormatProvider()): product.Data.Invoice.Total.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            tienthue = product.Data.Invoice.VATAmount.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            thuesuat = product.Data.Invoice.VATRate?.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            giamgia = ((product.Data.Invoice.DiscountAmount ?? 0) + (product.Data.Invoice.DiscountOther??0)).ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            khachcantra = (product.Data.Invoice.Amonut).ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            khachthanhtoan = product.Data.Invoice.AmountCusPayment?.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            tienthuatrakhach = product.Data.Invoice.AmountChangeCus?.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                          
                        };
                        var listitemnew = product.Data.Invoice.InvoiceItems;
                        if (product.Data.IsProductVAT)//check trường hợp nếu sản phẩm có dòng đơn giá đã gồm thuế
                        {
                            if (product.Data.Invoice.VATRate != (float)NOVAT.NOVAT)//nếu hóa đơn có thuế thì hiển thị tiền trước thuế phải là tiền trước thuế của sản phẩm có và k có thuế
                            {
                                //foreach (var item in listitemnew)
                                //{
                                //    if (item.PriceNoVAT == 0)//là sp đơn giá không có thuế
                                //    {
                                //        item.Amonut = item.Total;//update lại amount để hiển thị lên bill cho đúng là tiền trước thuế của sp đó
                                //    }
                                //}
                                templateInvoiceParameter.tientruocthue = listitemnew.Sum(x => x.Total).ToString("#,0.##", LibraryCommon.GetIFormatProvider());//update lại tiền trước thuế cho đúng
                            }
                            else
                            {
                                templateInvoiceParameter.tientruocthue = listitemnew.Sum(x => x.Amonut).ToString("#,0.##", LibraryCommon.GetIFormatProvider());//update lại tiền trước thuế cho đúng
                            }

                        }
                        else
                        {
                            if (product.Data.Invoice.VATRate != (float)NOVAT.NOVAT)//nếu hóa đơn có thuế
                            {
                                foreach (var item in listitemnew)
                                {
                                    if (item.PriceNoVAT == 0)//là sp đơn giá không có thuế
                                    {
                                        item.Amonut = item.Total;//update lại amount để hiển thị lên bill cho đúng là tiền trước thuế của sp đó
                                    }
                                }
                            }
                        }
                        

                        if (product.Data.IsSuccess)
                        {
                            templateInvoiceParameter.kyhieuhoadon = GetEInvoiceNoFormat.get_kyhieu(product.Data.Pattern, product.Data.Serial);
                            templateInvoiceParameter.sohoadon = GetEInvoiceNoFormat.get_no(product.Data.InvoiceNo);
                            if (!string.IsNullOrEmpty(product.Data.Fkey))
                            {
                                templateInvoiceParameter.linktracuu = product.Data.UrlDomain;
                                templateInvoiceParameter.matracuu = product.Data.Fkey;
                                templateInvoiceParameter.macoquanthue = product.Data.MCQT;
                            }
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
                        //if (product.Data.Invoice.VATRate!= (int)VATRateInv.KHONGVAT)
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
                        //        templateInvoiceParameter.UrlDomain = product.Data.UrlDomain;
                        //        templateInvoiceParameter.Fkey = product.Data.Fkey;
                        //        templateInvoiceParameter.MCQT = product.Data.MCQT;
                        //        //string url = InfoSeachInvCons.thong_tin_tra_cuu(product.Data.UrlDomain, product.Data.Fkey, product.Data.MCQT);
                        //        //thongtintracuuhoadon = $"<hr />" +
                        //        //                        $"<table style='margin-top: 0mm;width: 100%;'>" +
                        //        //                        $"<tr style='width: 100%;'>" +
                        //        //                        $"<td style='text-align: center; font-size: 11px;'>" +
                        //        //                        $"<span style='display: block;'>{url}</span>" +
                        //        //                        $"</td>" +
                        //        //                        $"</tr>" +
                        //        //                        $"</table>";
                        //    }
                        //}
                        //templateInvoiceParameter.thongtinthue = thongtinthue;
                        // templateInvoiceParameter.thongtintracuuhoadon = thongtintracuuhoadon;
                        // string content = LibraryCommon.GetTemplate(templateInvoiceParameter, templateInvoice.Template, EnumTypeTemplate.INVOICEPOS);
                        try
                        {
                            string content = PrintTemplate.PrintInvoice(templateInvoiceParameter, listitemnew.ToList(), templateInvoice.Template);
                            return Result<string>.Success(content, HeperConstantss.SUS014);
                        }
                        catch (Exception e)
                        {
                            return Result<string>.Success("Lỗi khi tìm mẫu in", HeperConstantss.SUS014);
                        }
                       
                    }
                    return Result<string>.Success("Công ty chưa cấu hình mẫu in", HeperConstantss.SUS014);
                }
                return Result<string>.Fail(product.Message);
            }
        }
    }
}
