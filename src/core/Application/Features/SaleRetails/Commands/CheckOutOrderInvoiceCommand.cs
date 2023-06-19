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

                            tongtien = product.Data.Invoice.Amonut.ToString("#,0.###", LibraryCommon.GetIFormatProvider()),
                            tientruocthue = (product.Data.IsProductVAT) ? (product.Data.Invoice.VATAmount + product.Data.Invoice.Total).ToString("#,0.###", LibraryCommon.GetIFormatProvider()) : product.Data.Invoice.Total.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            tienthue = product.Data.Invoice.VATAmount.ToString("#,0.###", LibraryCommon.GetIFormatProvider()),
                            thuesuat = product.Data.Invoice.VATRate?.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            giamgia = (product.Data.Invoice.DiscountAmount??0 + product.Data.Invoice.DiscountOther??0).ToString("#,0.###", LibraryCommon.GetIFormatProvider()),
                            khachcantra = (product.Data.Invoice.Amonut).ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            khachthanhtoan = product.Data.Invoice.AmountCusPayment?.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                            tienthuatrakhach = product.Data.Invoice.AmountChangeCus?.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                          
                        };

                        var listitemnew = product.Data.Invoice.InvoiceItems;
                        if (product.Data.IsProductVAT)//check trường hợp nếu sản phẩm có dòng đơn giá đã gồm thuế
                        {
                            if (product.Data.Invoice.VATRate != (float)NOVAT.NOVAT)//nếu hóa đơn có thuế thì hiển thị tiền trước thuế phải là tiền trước thuế của sản phẩm có và k có thuế
                            {
                                templateInvoiceParameter.tientruocthue = listitemnew.Sum(x => x.Total).ToString("#,0.###", LibraryCommon.GetIFormatProvider());//update lại tiền trước thuế cho đúng
                            }
                            else
                            {
                                templateInvoiceParameter.tientruocthue = listitemnew.Sum(x => x.Amonut).ToString("#,0.###", LibraryCommon.GetIFormatProvider());//update lại tiền trước thuế cho đúng
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
                        }
                        if (!string.IsNullOrEmpty(product.Data.Fkey))
                        {
                            templateInvoiceParameter.linktracuu = product.Data.UrlDomain;
                            templateInvoiceParameter.matracuu = product.Data.Fkey;
                            templateInvoiceParameter.macoquanthue = product.Data.MCQT;
                        }



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
