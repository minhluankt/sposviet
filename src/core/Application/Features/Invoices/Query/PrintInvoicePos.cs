﻿using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using BankService.Model;
using BankService.VietQR;
using Domain.Entities;
using Domain.ViewModel;
using HelperLibrary;
using Library;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SystemVariable;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.Features.Invoices.Query
{

    public class PrintInvoicePos : IRequest<Result<string>>
    {

        public Guid Id { get; set; }
        public int ComId { get; set; }
        public bool IncludeCustomer { get; set; } = true;
        public bool IncludeRoomAndTable { get; set; }
        public bool PrintByVAT { get; set; }

        public class PrintInvoicePosHandler : IRequestHandler<PrintInvoicePos, Result<string>>
        {
            private readonly IBankAccountRepository _bankaccountQRrepository;
            private readonly IFormFileHelperRepository _iFormFileHelperRepository;
            private readonly IVietQRService _vietQservice;
            private readonly IVietQRRepository<VietQR> _vietQRrepository;
            private readonly ISupplierEInvoiceRepository<SupplierEInvoice> _supplierEInvoicerepository;
            private readonly ITemplateInvoiceRepository<TemplateInvoice> _templateInvoicerepository;
            private readonly ICompanyAdminInfoRepository _companyProductRepository;
            private readonly IRepositoryAsync<Invoice> _repository;
            private readonly IInvoicePepository<Invoice> _Invoicerepository;
            private readonly IEInvoiceRepository<EInvoice> _eInvoicerepository;
            public PrintInvoicePosHandler(IRepositoryAsync<Invoice> repository,
                IEInvoiceRepository<EInvoice> eInvoicerepository, ISupplierEInvoiceRepository<SupplierEInvoice> supplierEInvoicerepository,
                ICompanyAdminInfoRepository companyProductRepository, ITemplateInvoiceRepository<TemplateInvoice> templateInvoicerepository,
                IInvoicePepository<Invoice> Invoicerepository, IBankAccountRepository bankaccountQRrepository, IFormFileHelperRepository iFormFileHelperRepository, IVietQRService vietQservice, IVietQRRepository<VietQR> vietQRrepository)
            {
                _supplierEInvoicerepository = supplierEInvoicerepository;
                _templateInvoicerepository = templateInvoicerepository;
                _eInvoicerepository = eInvoicerepository;
                _companyProductRepository = companyProductRepository;
                _Invoicerepository = Invoicerepository;
                _repository = repository;
                _bankaccountQRrepository = bankaccountQRrepository;
                _iFormFileHelperRepository = iFormFileHelperRepository;
                _vietQservice = vietQservice;
                _vietQRrepository = vietQRrepository;
            }
            public async Task<Result<string>> Handle(PrintInvoicePos query, CancellationToken cancellationToken)
            {
                var Invoice = _repository.Entities.AsNoTracking().Where(m => m.IdGuid == query.Id && m.ComId == query.ComId);

                
                Invoice = Invoice.Include(x => x.InvoiceItems);
                if (query.IncludeCustomer)
                {
                    Invoice = Invoice.Include(x => x.Customer);
                } if (query.IncludeRoomAndTable)
                {
                    Invoice = Invoice.Include(x => x.RoomAndTable);
                }
                var InvoiceData = await Invoice.SingleOrDefaultAsync();

                if (InvoiceData == null)
                {
                    return await Result<string>.FailAsync(HeperConstantss.ERR012);
                }
                
                CompanyAdminInfo company = _companyProductRepository.GetCompany(query.ComId);
                TemplateInvoice templateInvoice = await _templateInvoicerepository.GetTemPlate(query.ComId);
                if (templateInvoice != null)
                {
                    bool IsProductVAT = InvoiceData.InvoiceItems.Where(x => x.PriceNoVAT > 0).Any();
                    TemplateInvoiceParameter templateInvoiceParameter = new TemplateInvoiceParameter()
                    {
                        giovao = InvoiceData.ArrivalDate.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                        ngaythangnamxuat = InvoiceData.PurchaseDate.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                        lienhehotline = SystemVariableHelper.lienhehotline,
                        TypeTemplatePrint = EnumTypeTemplatePrint.IN_BILL,
                        invoiceNo = InvoiceData.InvoiceCode,
                        buyer = !string.IsNullOrEmpty(InvoiceData.CusName)? InvoiceData.CusName : InvoiceData.Buyer,
                        casherName = InvoiceData.CasherName,
                        staffName = InvoiceData.StaffName,
                        cusPhone = InvoiceData.Customer?.PhoneNumber,
                        cusAddress = InvoiceData.Customer?.Address,
                        cuscode = InvoiceData.Customer?.Code,
                        tenbanphong = InvoiceData.RoomAndTable != null ? InvoiceData.RoomAndTable.Name : "Mang về",
                        comname = !string.IsNullOrEmpty(company.Title) ? company.Title.Trim() : company.Name,
                        comaddress = company?.Address,
                        comphone = company?.PhoneNumber,
                        comemail = company?.Email,

                        tongtien = InvoiceData.Amonut.ToString("#,0.###", LibraryCommon.GetIFormatProvider()),
                        tientruocthue = IsProductVAT ? (InvoiceData.VATAmount + InvoiceData.Total).ToString("#,0.###", LibraryCommon.GetIFormatProvider()) : InvoiceData.Total.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                        tienthue = InvoiceData.VATAmount.ToString("#,0.###", LibraryCommon.GetIFormatProvider()),
                        thuesuat = InvoiceData.VATRate?.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                        giamgia = (InvoiceData.DiscountAmount??0 + InvoiceData.DiscountOther??0).ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                        khachcantra = (InvoiceData.Amonut).ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                        khachthanhtoan = InvoiceData.AmountCusPayment?.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                        tienthuatrakhach = InvoiceData.AmountChangeCus?.ToString("#,0.##", LibraryCommon.GetIFormatProvider()),
                       
                    }; 
                    var listitemnew = InvoiceData.InvoiceItems;
                    if (IsProductVAT)//check trường hợp nếu sản phẩm có dòng đơn giá đã gồm thuế
                    {
                        if (InvoiceData.VATRate !=(float)NOVAT.NOVAT)//nếu hóa đơn có thuế thì hiển thị tiền trước thuế phải là tiền trước thuế của sản phẩm có và k có thuế
                        {
                            //templateInvoiceParameter.tientruocthue = listitemnew.Sum(x => x.Total).ToString("#,0.##", LibraryCommon.GetIFormatProvider());//update lại tiền trước thuế cho đúng
                            templateInvoiceParameter.tientruocthue = InvoiceData.Total.ToString("#,0.###", LibraryCommon.GetIFormatProvider());//update lại tiền trước thuế cho đúng
                        }
                        else//hóa đơn k có thuế mà sp có thuế thì hiển 
                        {
                            templateInvoiceParameter.tientruocthue = listitemnew.Sum(x => x.Amonut).ToString("#,0.###", LibraryCommon.GetIFormatProvider());//update lại tiền trước thuế cho đúng
                        }

                    }
                    else
                    {
                        if (InvoiceData.VATRate != (float)NOVAT.NOVAT)//nếu hóa đơn có thuế
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

                    if (InvoiceData.IdEInvoice!=null)
                    {
                   
                        var inv = await _eInvoicerepository.FindByIdAsync(InvoiceData.IdEInvoice.Value,true);
                        if (inv!=null)
                        {
                            templateInvoiceParameter.kyhieuhoadon = GetEInvoiceNoFormat.get_kyhieu(inv.Pattern,inv.Serial);
                            templateInvoiceParameter.sohoadon = GetEInvoiceNoFormat.get_no(inv.InvoiceNo);

                            string thongtintracuuhoadon = string.Empty;
                            if (inv.TypeSupplierEInvoice==ENumSupplierEInvoice.VNPT)
                            {
                                var companyvnpt = await _supplierEInvoicerepository.GetByIdAsync(query.ComId, inv.TypeSupplierEInvoice);
                                if (companyvnpt!=null) {
                                    string urlportal = ConvertSupport.ConverDoaminVNPTPortal<string>(companyvnpt.DomainName);
                                    //thongtintracuuhoadon = InfoSeachInvCons.thong_tin_tra_cuu(urlportal, inv.FkeyEInvoice, inv.MCQT);
                                    templateInvoiceParameter.linktracuu = urlportal;
                                    templateInvoiceParameter.matracuu = inv.FkeyEInvoice;
                                    templateInvoiceParameter.macoquanthue = inv.MCQT;
                                }
                            }
                            //string thongtinthue = string.Empty;
                            if (InvoiceData.VATRate != (int)NOVAT.NOVAT)
                            {
                                templateInvoiceParameter.isVAT = true;
                                //thongtinthue = $"<tr style='font-size: 11px; text-align: left'>" +
                                //        $"<td colspan=\"3\">Tiền thuế: {templateInvoiceParameter.thuesuat}%</td>\r\n\t\t\t<td style=\"text-align: right;\">{templateInvoiceParameter.tienthue}</td>" +
                                //        $"</tr>";

                            }
                            //-----------kiểm tra nếu hóa đơn mà xuất hóa đơn điện tử sau khi bán, tùy chọn để in
                            else if (query.PrintByVAT)
                            {
                                templateInvoiceParameter.isVAT = true;
                                templateInvoiceParameter.tongtien = inv.Amount.ToString("#,0.###", LibraryCommon.GetIFormatProvider());
                                templateInvoiceParameter.tientruocthue = inv.Total.ToString("#,0.###", LibraryCommon.GetIFormatProvider());
                                templateInvoiceParameter.tienthue = inv.VATAmount.ToString("#,0.###", LibraryCommon.GetIFormatProvider());
                                templateInvoiceParameter.thuesuat = inv.VATRate.ToString("#,0.##", LibraryCommon.GetIFormatProvider());
                                templateInvoiceParameter.giamgia = (inv.DiscountAmount ?? 0 + inv.DiscountOther ?? 0).ToString("#,0.##", LibraryCommon.GetIFormatProvider());
                                templateInvoiceParameter.khachcantra = (inv.Amount).ToString("#,0.##", LibraryCommon.GetIFormatProvider());
                            }
                        }
                        else
                        {
                           
                        }
                    }
                    try
                    {
                        if (templateInvoice.IsShowQrCodeVietQR && !string.IsNullOrEmpty(templateInvoice.HtmlQrCodeVietQR))
                        {
                            var getvietqr = await _vietQRrepository.GetByFirstAsync(query.ComId);
                            if (getvietqr.Succeeded)
                            {
                                InfoPayQrcode infoPayQrcode = new InfoPayQrcode()
                                {
                                    accountName = getvietqr.Data.BankAccount.AccountName,
                                    accountNo = getvietqr.Data.BankAccount.BankNumber,
                                    acqId = getvietqr.Data.BankAccount.BinVietQR,
                                    template = getvietqr.Data.Template,
                                    amount = templateInvoiceParameter.khachcantra.Replace(",", ""),
                                    addInfo = InvoiceData.InvoiceCode.Replace("-", "")
                                };
                                var qrcode = await _vietQservice.GetQRCode(infoPayQrcode);
                                if (!qrcode.isError)
                                {
                                    string path = _iFormFileHelperRepository.GetFileTemplate(FileConstants.logAppsposviet, string.Empty, FolderUploadConstants.Images);

                                    string qrcodedata = string.Empty;
                                    Bitmap image1 = null;
                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        image1 = (Bitmap)Image.FromFile(path, true);
                                    }
                                    var data = ConvertSupport.ConverJsonToModel<VietQRData>(qrcode.data);
                                    templateInvoiceParameter.chu_tai_khoan = infoPayQrcode.accountName;
                                    templateInvoiceParameter.so_tai_khoan = infoPayQrcode.accountNo;
                                    templateInvoiceParameter.ten_ngan_hang = getvietqr.Data.BankAccount?.BankName;
                                    templateInvoiceParameter.infoqrcodethanhtoan = templateInvoice.HtmlQrCodeVietQR.Replace("{qrDataURL}", ConvertSupport.ConverStringToQrcode(data.qrCode, 20, image1));
                                }
                            }
                        }
                        else
                        {
                            var bank = await _bankaccountQRrepository.GetDefaultAsync(query.ComId);
                            if (bank.Succeeded)
                            {
                                templateInvoiceParameter.chu_tai_khoan = bank.Data.AccountName;
                                templateInvoiceParameter.so_tai_khoan = bank.Data.BankNumber;
                                templateInvoiceParameter.ten_ngan_hang = bank.Data.BankName;
                            }
                        }
                    }
                    catch (Exception e)
                    {


                    }
                    string content = PrintTemplate.PrintInvoice(templateInvoiceParameter, listitemnew.ToList(), templateInvoice.Template);
                    return Result<string>.Success(content, HeperConstantss.SUS014);
                }
                return await Result<string>.FailAsync("Không tìm thấy mẫu hóa đơn");
            }
        }
    }
}
