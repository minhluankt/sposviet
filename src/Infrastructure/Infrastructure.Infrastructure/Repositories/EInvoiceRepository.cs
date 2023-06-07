﻿using Application.Constants;
using Application.EInvoices.Interfaces.VNPT;
using Application.Enums;
using Application.Interfaces.Repositories;
using Application.Providers;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive.Joins;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Linq.Dynamic.Core;
using System.Linq;
using Application.Hepers;
using Infrastructure.Infrastructure.Migrations;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.AspNetCore.SignalR;
using System.Security.Policy;
using Library;
using Domain.XmlDataModel;
using Spire.Pdf.Exporting.XPS.Schema;
using JetBrains.Annotations;
using Joker.Extensions;
using Hangfire.Logging;
using NStandard;
using Infrastructure.Infrastructure.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using HelperLibrary;
using Org.BouncyCastle.Utilities;
using Application.CacheKeys;
using X.PagedList;
using Spire.Doc;
using static System.Data.Entity.Infrastructure.Design.Executor;
using AspNetCoreHero.Abstractions.Repository;
using Telegram.Bot.Types;
using Domain.Identity;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using Joker.Contracts.Data;

namespace Infrastructure.Infrastructure.Repositories
{
    public class EInvoiceRepository : IEInvoiceRepository<EInvoice>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IUnitOfWork _unitOfWork { get; set; }

        

        private readonly IManagerInvNoRepository _managerInvNoRepository;
        private UserManager<ApplicationUser> _userManager;
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IRepositoryAsync<Customer> _repositoryCusomer;
        private readonly IVNPTHKDApiRepository _hkdvnptrepository;
        private readonly IVNPTPublishServiceRepository _vnptrepository;
        private readonly IVNPTBusinessServiceRepository _vnptbusinessrepository;
        private readonly IVNPTPortalServiceRepository _vnptportalrepository;
        private readonly ISupplierEInvoiceRepository<SupplierEInvoice> _supplierEInvoicerepository;
        private readonly IRepositoryAsync<EInvoice> _repository;
        private readonly IRepositoryAsync<EInvoiceItem> _einvoiceitemrepository;
        private readonly IRepositoryAsync<Invoice> _Invoicerepository;
        private readonly IRepositoryAsync<HistoryEInvoice> _HistoryEInvoicerepository;
        private readonly ILogger<EInvoiceRepository> _log;
        public EInvoiceRepository(IRepositoryAsync<EInvoice> repository, IVNPTHKDApiRepository hkdvnptrepository,
            IRepositoryAsync<HistoryEInvoice> HistoryEInvoicerepository,
            IVNPTPortalServiceRepository vnptportalrepository, IUnitOfWork unitOfWork, IServiceScopeFactory serviceScopeFactory,
            IManagerInvNoRepository managerInvNoRepository, IVNPTBusinessServiceRepository vnptbusinessrepository,
            UserManager<ApplicationUser> userManager, IRepositoryAsync<Invoice> Invoicerepository,
               IRepositoryAsync<Customer> repositoryCusomer, IOptions<CryptoEngine.Secrets> config,
        ISupplierEInvoiceRepository<SupplierEInvoice> supplierEInvoicerepository, IRepositoryAsync<EInvoiceItem> einvoiceitemrepository,
            IVNPTPublishServiceRepository vnptrepository, ILogger<EInvoiceRepository> log
            )
        {
            _hkdvnptrepository = hkdvnptrepository;
            _serviceScopeFactory = serviceScopeFactory;
            _HistoryEInvoicerepository = HistoryEInvoicerepository;
            _vnptbusinessrepository = vnptbusinessrepository;
            _Invoicerepository = Invoicerepository;
            _managerInvNoRepository = managerInvNoRepository;
            _unitOfWork = unitOfWork;
            _vnptportalrepository = vnptportalrepository;
            _userManager = userManager;
            _config = config;
            _repositoryCusomer = repositoryCusomer;
            _log = log;
            _supplierEInvoicerepository = supplierEInvoicerepository;
            _einvoiceitemrepository = einvoiceitemrepository;
            _vnptrepository = vnptrepository;
            _repository = repository;
        }
        public IQueryable<EInvoice> Entities => _repository.Entities;
        public async Task CreateAsync(EInvoice Entity, string Carsher, string IdCarsher)
        {
            await _repository.AddAsync(Entity);
            await _unitOfWork.SaveChangesAsync();
            AddHistori(new HistoryEInvoice() { Carsher = Carsher, StatusEvent = StatusStaffEventEInvoice.TaoMoiHoaDon, IdCarsher = IdCarsher, EInvoiceCode = Entity.EInvoiceCode, IdEInvoice = Entity.Id, Name = $"Tạo mới hóa đơn" });
        }
        public async Task CreateRangeAsync(List<EInvoice> Entity, string Carsher, string IdCarsher)
        {
            await _repository.AddRangeAsync(Entity);
            await _unitOfWork.SaveChangesAsync();
            foreach (var item in Entity)
            {
                AddHistori(new HistoryEInvoice() { 
                    Carsher = Carsher,
                    StatusEvent = StatusStaffEventEInvoice.TaoMoiHoaDon, 
                    IdCarsher = IdCarsher,
                    EInvoiceCode = item.EInvoiceCode, 
                    IdEInvoice = item.Id,
                    Name = $"Tạo mới hóa đơn" });
            }
            
        }

        public async Task<IResult<PublishInvoiceModelView>> CancelEInvoiceAsync(int[] lst, int ComId, string Carsher, string IdCarsher)
        {
            SupplierEInvoice company = new SupplierEInvoice();
            List<DetailInvoice> ListDetailInvoice = new List<DetailInvoice>();
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                bool update = false;
                var getall = await _repository.Entities.Where(x => lst.Contains(x.Id) && x.ComId == ComId).ToListAsync();

                foreach (var item in getall)
                {
                    try
                    {
                        if (item.TypeSupplierEInvoice == ENumSupplierEInvoice.VNPT)
                        {
                            if (company.TypeSupplierEInvoice != item.TypeSupplierEInvoice)
                            {
                                company = await _supplierEInvoicerepository.GetByIdAsync(ComId,item.TypeSupplierEInvoice);
                            }
                            if (company == null)
                            {
                                ListDetailInvoice.Add(new DetailInvoice()
                                {
                                    code = item.EInvoiceCode,
                                    note = $"Hóa đơn có mã {item.EInvoiceCode}, không tìm thấy nhà cung cấp",
                                    TypePublishEinvoice = ENumTypePublishEinvoice.KHONGTONTAINHACUNGCAP,
                                });
                            }
                            else
                            {
                                if (item.StatusEinvoice == StatusEinvoice.NewInv)
                                {
                                    ListDetailInvoice.Add(new DetailInvoice()
                                    {
                                        code = item.EInvoiceCode,
                                        note = $"Hóa đơn có trạng thái là: {GeneralMess.GeneralMessStatusEInvoice(item.StatusEinvoice).ToLower()}, không thể hủy hóa đơn",
                                        TypePublishEinvoice = ENumTypePublishEinvoice.TRANGTHAIPHATHANHKHONGHOPLE,
                                    });
                                }
                                else
                                {
                                    var pub = await _vnptbusinessrepository.cancelInvNoPayAsync(company.UserNameAdmin, company.PassWordAdmin, item.FkeyEInvoice, company.UserNameService, company.PassWordService, company.DomainName);
                                    if (pub.Equals("OK:"))
                                    {
                                        AddHistori(new HistoryEInvoice() { Carsher = Carsher, StatusEvent = StatusStaffEventEInvoice.HuyHoaDon, IdCarsher = IdCarsher, EInvoiceCode = item.EInvoiceCode, IdEInvoice = item.Id, Name = $"Hủy hóa đơn" });
                                        await this.UpdateStatusPublishInvoice(item.ComId,item.IdInvoice,item.InvoiceCode, EnumStatusPublishInvoiceOrder.CANCEL);
                                        item.StatusEinvoice = StatusEinvoice.CanceledInv;
                                        update = true;
                                        ListDetailInvoice.Add(new DetailInvoice()
                                        {
                                            code = item.EInvoiceCode,
                                            note = $"Hủy hóa đơn thành công",
                                            TypePublishEinvoice = ENumTypePublishEinvoice.HUYHOADONOK,
                                        });
                                    }
                                    else
                                    {
                                        ListDetailInvoice.Add(new DetailInvoice()
                                        {
                                            code = item.EInvoiceCode,
                                            note = $"Hủy hóa đơn thất bại , mã lỗi từ VNPT: {pub}",
                                            TypePublishEinvoice = ENumTypePublishEinvoice.HUYHOADONTHATBAI,
                                        });
                                        _log.LogError($"Hủy hóa đơn thất bại , mã lôi từ VNPT: {pub}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            ListDetailInvoice.Add(new DetailInvoice()
                            {
                                code = item.EInvoiceCode,
                                note = $"Hóa đơn có mã {item.EInvoiceCode}, không tìm thấy nhà cung cấp",
                                TypePublishEinvoice = ENumTypePublishEinvoice.KHONGTONTAINHACUNGCAP,
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        ListDetailInvoice.Add(new DetailInvoice()
                        {
                            code = item.EInvoiceCode,
                            note = $"Hủy hóa đơn thất bại, lỗi từ cung cấp dữ liệu Exception: {e.ToString()}",
                            TypePublishEinvoice = ENumTypePublishEinvoice.DONGBOTHATBAI,
                        });
                        _log.LogError($"Hủy hóa đơn thất bại, lỗi từ cung cấp dữ liệu Exception-> {e.ToString()}");
                    }
                }
                if (update)
                {
                    await _repository.UpdateRangeAsync(getall);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    _unitOfWork.Dispose();
                }
                var PublishInvoiceModelView = new PublishInvoiceModelView();
                PublishInvoiceModelView.DetailInvoices = ListDetailInvoice;
                return await Result<PublishInvoiceModelView>.SuccessAsync(PublishInvoiceModelView);
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                _log.LogError($"Lỗi conver dữ liệu Exception -> {e.ToString()}");
                return await Result<PublishInvoiceModelView>.FailAsync(e.ToString());
            }
        }

        public IQueryable<EInvoice> GetAllAsync()
        {
            return _repository.Entities.AsNoTracking();
        }
        public async Task<IResult<string>> CheckConnectWebserviceAsync(string doamin, string userservice, string passservice, string useradmin, string passadmin)
        {
            var pub = await _vnptrepository.ImportAndPublishInvMTTAsync(doamin, string.Empty, userservice, passservice, useradmin, passadmin, string.Empty, string.Empty);
            if (pub.Contains("ERR:") && !pub.Equals("ERR:1"))
            {
                return await Result<string>.SuccessAsync($"Kết nối thành công");
            }
            else
            {
                return await Result<string>.FailAsync($"Kết nối thất bại sai tên đăng nhập hoặc mật khẩu {pub}");
            }
        }
        public async Task<IResult<string>> GetHashInvWithTokenVNPTAsync(List<EInvoice> einvoices, SupplierEInvoice company)
        {
            try
            {
                string xmlstring = string.Empty;
                try
                {
                    xmlstring = this.GenerateXML32List_VNPT(einvoices);
                }
                catch (Exception e)
                {
                    _log.LogError($"Mã {string.Join(",", einvoices.Select(x => x.EInvoiceCode))}, {e.Message}, Lỗi sinh XMl khi phát hành hóa đơn điện tử" + e.ToString());
                    return await Result<string>.FailAsync($"{CommonException.ExceptionXML}Lỗi sinh XMl khi phát hành hóa đơn điện tử {e.Message}");
                }
                string gettoken = await _vnptrepository.GetHashInvWithToken(company.DomainName, xmlstring, company.UserNameService, company.PassWordService, company.UserNameAdmin, company.PassWordAdmin, company.SerialCert, ENumTypePublishServiceEInvoice.PHATHANH, string.Empty, einvoices.First().Pattern, einvoices.First().Serial);
                if (gettoken.Contains("ERR:"))
                {
                    return await Result<string>.FailAsync($"Lỗi lấy chuỗi hash hóa đơn điện tử thất bại {gettoken}");
                }
                else if (gettoken.Contains("Invoices"))
                {
                    return await Result<string>.SuccessAsync(gettoken,"Lấy hash thành công");
                }
                return await Result<string>.FailAsync($"Lỗi phát hành hóa đơn điện tử không xác định {gettoken}");
            }
            catch (Exception e)
            {
                _log.LogError(e.ToString());
                return await Result<string>.FailAsync($"Lỗi phát hành hóa đơn điện tử {e.Message}");
            }
        }
        public async Task<IResult<string>> ImportAndPublishInvMTTAsync(EInvoice einvoice, SupplierEInvoice company, string Carsher, string IdCarsher)
        {
            if (einvoice != null)
            {
                if (company == null)
                {
                    company = await _supplierEInvoicerepository.GetByIdAsync(einvoice.ComId,einvoice.TypeSupplierEInvoice);
                    if (company == null)
                    {
                        return await Result<string>.FailAsync("Chưa cấu hình phát hành hóa đơn");
                    }
                }

                return await ImportAndPublishInvMTTAsync(einvoice, company, einvoice.Pattern, einvoice.Serial, Carsher, IdCarsher);
            }
            return await Result<string>.FailAsync($"Không tìm thấy hóa đơn cần phát hành");

        }
      
        private async Task<IResult<string>> ImportAndPublishInvMTTAsync(EInvoice einvoice, SupplierEInvoice company, string pattern, string serial, string Carsher, string IdCarsher)
        {
            var checkserial = LibraryCommon.IsHDDTMayTinhTien(serial);
            switch (einvoice.TypeSupplierEInvoice)
            {
                case ENumSupplierEInvoice.VNPT:

                    try
                    {
                        if (einvoice.EInvoiceItems.Count() == 0)
                        {
                            return await Result<string>.FailAsync("Sản phẩm không được trống khi phát hành hóa đơn điện tử");
                        }

                        string xmlData = string.Empty;
                        try
                        {
                            if (checkserial)//là máy tính tiền
                            {
                                xmlData = this.GenerateXMLMTT_VNPT(einvoice);
                            }
                            else {
                                xmlData = this.GenerateXML32_VNPT(einvoice);
                            }
                                
                        }
                        catch (Exception e)
                        {
                            _log.LogError($"Mã {einvoice.EInvoiceCode}, {e.Message}, Lỗi sinh XMl khi phát hành hóa đơn điện tử" + e.ToString());
                            return await Result<string>.FailAsync($"{CommonException.ExceptionXML}Lỗi sinh XMl khi phát hành hóa đơn điện tử");
                        }
                        string pub = string.Empty;
                       
                        if (checkserial)
                        {
                            pub = await _vnptrepository.ImportAndPublishInvMTTAsync(company.DomainName, xmlData, company.UserNameService, company.PassWordService, company.UserNameAdmin, company.PassWordAdmin, pattern, serial);
                        }
                        else
                        {
                            pub = await _vnptrepository.ImportAndPublishInvAsync(company.DomainName, xmlData, company.UserNameService, company.PassWordService, company.UserNameAdmin, company.PassWordAdmin, pattern, serial);
                        }
                        
                        if (pub.Contains("OK:"))
                        {
                            AddHistori(new HistoryEInvoice() { Carsher = Carsher, StatusEvent = StatusStaffEventEInvoice.PhatHanhHoaDon, IdCarsher = IdCarsher, EInvoiceCode = einvoice.EInvoiceCode, IdEInvoice = einvoice.Id, Name = $"Phát hành hóa đơn VNPT" });
                            return await Result<string>.SuccessAsync(pub, HeperConstantss.SUS006);
                        }
                        else
                        {
                            _log.LogError($"Lỗi phát hành hóa đơn đến nhà cung cấp VNPT -> {pub}");
                            _log.LogError($"XML VNPT -> {xmlData}");
                            return await Result<string>.FailAsync($"{pub}");
                        }

                    }
                    catch (Exception e)
                    {
                        return await Result<string>.FailAsync($"{CommonException.Exception} Lỗi phát hành hóa đơn đến nhà cung cấp VNPT -> {e.Message}");
                    }
                default:
                    return await Result<string>.FailAsync($"Không tìm thấy nhà cung cấp nào phù hợp {einvoice.TypeSupplierEInvoice}");

            }
        }
        
        private string GenerateXML32_VNPT(EInvoice model)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode contentDSHDon = xmlDoc.CreateElement("Invoices");
            xmlDoc.AppendChild(contentDSHDon);
            this.SerializeToXML32(xmlDoc, contentDSHDon, model);
            return xmlDoc.InnerXml;
        }
        private string GenerateXML32List_VNPT(List<EInvoice> model)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode contentDSHDon = xmlDoc.CreateElement("Invoices");
            xmlDoc.AppendChild(contentDSHDon);
            foreach (var item in model)
            {
                this.SerializeToXML32(xmlDoc, contentDSHDon, item);
            }
            return xmlDoc.InnerXml;
        }
        private  string SerializeToXML32(XmlDocument xmlDoc, XmlNode xmlDSHD, EInvoice model)
        {
            XmlNode contentHd = xmlDoc.CreateElement("Inv");
            xmlDSHD.AppendChild(contentHd);

            //tao kêy node
            XmlNode InvNode = xmlDoc.CreateElement("key");
            InvNode.InnerText = model.FkeyEInvoice;
            contentHd.AppendChild(InvNode);

            XmlNode xmlContentInv = xmlDoc.CreateElement("Invoice");
            contentHd.AppendChild(xmlContentInv);
            if (model.ExchangeRate == null)
            {
                model.ExchangeRate = 1;
            }
            XmlNode xmlExchangeRate = xmlDoc.CreateElement("ExchangeRate");
            xmlExchangeRate.InnerText = model.ExchangeRate.Value.ToString("N0", LibraryCommon.GetIFormatProvider());
            xmlContentInv.AppendChild(xmlExchangeRate);

            if (string.IsNullOrEmpty(model.CurrencyUnit))
            {
                model.CurrencyUnit = "VND";
            }
            XmlNode xmlCurrencyUnit = xmlDoc.CreateElement("CurrencyUnit");
            xmlCurrencyUnit.InnerText = model.CurrencyUnit;
            xmlContentInv.AppendChild(xmlCurrencyUnit);


            XmlNode xmlCusCode = xmlDoc.CreateElement("CusCode");
            xmlCusCode.InnerText = model.CusCode;
            xmlContentInv.AppendChild(xmlCusCode);

            XmlNode xmlCusName = xmlDoc.CreateElement("CusName");
            xmlCusName.InnerText = model.CusName;
            xmlContentInv.AppendChild(xmlCusName);

            XmlNode xmlBuyer = xmlDoc.CreateElement("Buyer");
            xmlBuyer.InnerText = model.Buyer;
            xmlContentInv.AppendChild(xmlBuyer);

            XmlNode xmlCusAddress = xmlDoc.CreateElement("CusAddress");
            xmlCusAddress.InnerText = model.Address;
            xmlContentInv.AppendChild(xmlCusAddress);

            XmlNode xmlCusTaxCode = xmlDoc.CreateElement("CusTaxCode");
            xmlCusTaxCode.InnerText = model.CusTaxCode;
            xmlContentInv.AppendChild(xmlCusTaxCode);

            XmlNode xmlPaymentMethod = xmlDoc.CreateElement("PaymentMethod");
            xmlPaymentMethod.InnerText = model.PaymentMethod;
            xmlContentInv.AppendChild(xmlPaymentMethod);


            if (!string.IsNullOrEmpty(model.CCCD))
            {
                XmlNode xmlExtra1 = xmlDoc.CreateElement("CCCD");
                xmlExtra1.InnerText = model.CCCD;
                xmlContentInv.AppendChild(xmlExtra1);
            }
            if (!string.IsNullOrEmpty(model.EmailDeliver))
            {
                XmlNode xmlExtra1 = xmlDoc.CreateElement("EmailDeliver");
                xmlExtra1.InnerText = model.CCCD;
                xmlContentInv.AppendChild(xmlExtra1);
            }
            //if (model.Customer != null)
            //{
            //    XmlNode xmlExtra1 = xmlDoc.CreateElement("Extra1");
            //    xmlExtra1.InnerText = model.Customer?.Nationality;
            //    xmlContentInv.AppendChild(xmlExtra1);

            //    XmlNode xmlExtra2 = xmlDoc.CreateElement("Extra2");
            //    xmlExtra2.InnerText = model.Customer?.Passport;
            //    xmlContentInv.AppendChild(xmlExtra2);

            //}


            //product
            //van de ve san pham
            XmlNode xmlProducts = xmlDoc.CreateElement("Products");
            xmlContentInv.AppendChild(xmlProducts);
           

            foreach (var el in model.EInvoiceItems)
            {
                //<IsSum> Tính chất * (0-Hàng hóa, dịch vụ; 1-Khuyến mại; 2-Chiết khấu thương mại (trong trường hợp muốn thể hiện thông tin chiết khấu theo dòng); 4-Ghi chú/diễn giải)</IsSum>
                XmlNode xmlProduct = xmlDoc.CreateElement("Product");
                xmlProducts.AppendChild(xmlProduct);

                //begin product
                XmlNode xmlProdCode = xmlDoc.CreateElement("Code");
                xmlProdCode.InnerText = el.ProCode;
                xmlProduct.AppendChild(xmlProdCode);

                string issum = string.Empty;
                switch (el.IsSum)
                {
                    case TCHHDVuLoai.HHoa:
                        issum = "0";
                        break;
                    case TCHHDVuLoai.KMai:
                        issum = "1";
                        break;
                    case TCHHDVuLoai.CKhau:
                        issum = "2";
                        break;
                    case TCHHDVuLoai.GChu:
                        issum = "4";
                        break;
                    default:
                        break;
                }
                XmlNode xmlIsSum = xmlDoc.CreateElement("IsSum");
                xmlIsSum.InnerText = issum;
                xmlProduct.AppendChild(xmlIsSum);

                XmlNode xmlProdName = xmlDoc.CreateElement("ProdName");
                xmlProdName.InnerText = el.ProName;
                xmlProduct.AppendChild(xmlProdName);

                XmlNode xmlProdUnit = xmlDoc.CreateElement("ProdUnit");
                xmlProdUnit.InnerText = el.Unit;
                xmlProduct.AppendChild(xmlProdUnit);

                XmlNode xmlProdQuantity = xmlDoc.CreateElement("ProdQuantity");
                xmlProdQuantity.InnerText = el.Quantity.ToString();
                xmlProduct.AppendChild(xmlProdQuantity);

                if (el.DiscountAmount!=null)
                {
                    XmlNode xmlDiscountAmountpro = xmlDoc.CreateElement("DiscountAmount");
                    xmlDiscountAmountpro.InnerText = el.DiscountAmount.ToString();
                    xmlProduct.AppendChild(xmlDiscountAmountpro);
                }

                if (el.Discount != null)
                {
                    XmlNode xmlDiscountpro = xmlDoc.CreateElement("Discount");
                    xmlDiscountpro.InnerText = el.Discount.ToString();
                    xmlProduct.AppendChild(xmlDiscountpro);
                }

                XmlNode xmlProdPrice = xmlDoc.CreateElement("ProdPrice");
                xmlProdPrice.InnerText = el.Price.ToString();
                xmlProduct.AppendChild(xmlProdPrice);

                XmlNode xmlProdVATRate = xmlDoc.CreateElement("VATRate");
                xmlProdVATRate.InnerText = el.VATRate.ToString();
                xmlProduct.AppendChild(xmlProdVATRate);

                XmlNode xmlProdVATAmount = xmlDoc.CreateElement("VATAmount");
                xmlProdVATAmount.InnerText = el.VATAmount.ToString();
                xmlProduct.AppendChild(xmlProdVATAmount);

                XmlNode xmlProdTotal = xmlDoc.CreateElement("Total");
                xmlProdTotal.InnerText = el.Total.ToString();
                xmlProduct.AppendChild(xmlProdTotal);

                //XmlNode xmlProdRemark = xmlDoc.CreateElement("Remark");
                //xmlProdRemark.InnerText = el.Remark.ToString();
                //xmlProduct.AppendChild(xmlProdRemark);

                XmlNode xmlProdAmount = xmlDoc.CreateElement("Amount");
                xmlProdAmount.InnerText = el.Amount.ToString();
                xmlProduct.AppendChild(xmlProdAmount);
              
            }
            decimal? GrossValue0 =  model.EInvoiceItems.Where(x => x.VATRate == 0).ToList().Sum(x=>x.Total);
            decimal? GrossValue5 = model.EInvoiceItems.Where(x => x.VATRate == 5).ToList().Sum(x => x.Total); 
            decimal? GrossValue8 = model.EInvoiceItems.Where(x => x.VATRate == 8).ToList().Sum(x => x.Total); 
            decimal? GrossValue10 = model.EInvoiceItems.Where(x => x.VATRate == 10).ToList().Sum(x => x.Total); 
            decimal? VatAmount0 = model.EInvoiceItems.Where(x => x.VATRate == 0).ToList().Sum(x => x.VATAmount); 
            decimal? VatAmount5 = model.EInvoiceItems.Where(x => x.VATRate == 5).ToList().Sum(x => x.VATAmount);
            decimal? VatAmount10 = model.EInvoiceItems.Where(x => x.VATRate == 10).ToList().Sum(x => x.VATAmount);
            decimal? VatAmount8 = model.EInvoiceItems.Where(x => x.VATRate == 8).ToList().Sum(x => x.VATAmount);

            decimal? _discountAmount = model.EInvoiceItems.Where(x=>x.DiscountAmount.HasValue).ToList().Sum(x => x.DiscountAmount);

            if (GrossValue0!=null && GrossValue0!=0)
            {
                XmlNode xmlGrossValue0 = xmlDoc.CreateElement("GrossValue0");
                xmlGrossValue0.InnerText = GrossValue0.Value.ToString("F3", LibraryCommon.GetIFormatProvider());
                xmlContentInv.AppendChild(xmlGrossValue0);
            }
            if (GrossValue5 != null && GrossValue5 != 0)
            {
                XmlNode xmlGrossValue0 = xmlDoc.CreateElement("GrossValue5");
                xmlGrossValue0.InnerText = GrossValue5.Value.ToString("F3", LibraryCommon.GetIFormatProvider());
                xmlContentInv.AppendChild(xmlGrossValue0);
            }
            if (GrossValue8 != null && GrossValue8 != 0)
            {
                XmlNode xmlGrossValue0 = xmlDoc.CreateElement("GrossValue8");
                xmlGrossValue0.InnerText = GrossValue8.Value.ToString("F3", LibraryCommon.GetIFormatProvider());
                xmlContentInv.AppendChild(xmlGrossValue0);
            }
            if (GrossValue10 != null && GrossValue10 != 0)
            {
                XmlNode xmlGrossValue0 = xmlDoc.CreateElement("GrossValue10");
                xmlGrossValue0.InnerText = GrossValue10.Value.ToString("F3", LibraryCommon.GetIFormatProvider());
                xmlContentInv.AppendChild(xmlGrossValue0);
            }

            if (VatAmount0 != null && VatAmount0 != 0)
            {
                XmlNode xmlVatAmountxxx = xmlDoc.CreateElement("VatAmount0");
                xmlVatAmountxxx.InnerText = VatAmount0.Value.ToString("F3", LibraryCommon.GetIFormatProvider());
                xmlContentInv.AppendChild(xmlVatAmountxxx);
            }

              if (VatAmount5 != null && VatAmount5 != 0)
            {
                XmlNode xmlVatAmountxxx = xmlDoc.CreateElement("VatAmount5");
                xmlVatAmountxxx.InnerText = VatAmount5.Value.ToString("F3", LibraryCommon.GetIFormatProvider());
                xmlContentInv.AppendChild(xmlVatAmountxxx);
            }

              if (VatAmount8 != null && VatAmount8 != 0)
            {
                XmlNode xmlVatAmountxxx = xmlDoc.CreateElement("VatAmount8");
                xmlVatAmountxxx.InnerText = VatAmount8.Value.ToString("F3", LibraryCommon.GetIFormatProvider());
                xmlContentInv.AppendChild(xmlVatAmountxxx);
            }

              if (VatAmount10 != null && VatAmount10 != 0)
            {
                XmlNode xmlVatAmountxxx = xmlDoc.CreateElement("VatAmount10");
                xmlVatAmountxxx.InnerText = VatAmount10.Value.ToString("F3", LibraryCommon.GetIFormatProvider());
                xmlContentInv.AppendChild(xmlVatAmountxxx);
            }

            if (_discountAmount!=null && _discountAmount!=0)
            {
                _discountAmount = _discountAmount + model.DiscountAmount;
                XmlNode xmlDiscountAmount = xmlDoc.CreateElement("DiscountAmount");
                xmlDiscountAmount.InnerText = _discountAmount.Value.ToString("F3", LibraryCommon.GetIFormatProvider());
                xmlContentInv.AppendChild(xmlDiscountAmount);
            }
            else
            {
                XmlNode xmlDiscountAmount = xmlDoc.CreateElement("DiscountAmount");
                xmlDiscountAmount.InnerText = model.DiscountAmount.Value.ToString("F3", LibraryCommon.GetIFormatProvider());
                xmlContentInv.AppendChild(xmlDiscountAmount);
            }

            if (model.Discount!=null && model.Discount!=0)
            {
                XmlNode xmlDiscountRate = xmlDoc.CreateElement("DiscountRate");
                xmlDiscountRate.InnerText = model.Discount.Value.ToString("N1", LibraryCommon.GetIFormatProvider());
                xmlContentInv.AppendChild(xmlDiscountRate);

            }
             if (model.DiscountNonTax!=null && model.DiscountNonTax != 0)
            {
                XmlNode xmlDiscountRate = xmlDoc.CreateElement("DiscountNonTax");
                xmlDiscountRate.InnerText = model.DiscountNonTax.Value.ToString("F3", LibraryCommon.GetIFormatProvider());
                xmlContentInv.AppendChild(xmlDiscountRate);

            }
             if (model.DiscountOther != null && model.DiscountOther != 0)
            {
                XmlNode xmlDiscountRate = xmlDoc.CreateElement("DiscountOther");
                xmlDiscountRate.InnerText = model.DiscountOther.Value.ToString("F3", LibraryCommon.GetIFormatProvider());
                xmlContentInv.AppendChild(xmlDiscountRate);

            }
             
           
            //tong tien dich vu
            XmlNode xmlTotal = xmlDoc.CreateElement("Total");
            xmlTotal.InnerText = model.Total.ToString("F3", LibraryCommon.GetIFormatProvider());
            xmlContentInv.AppendChild(xmlTotal);

            // tong tien VAT_Amount
            XmlNode xmlVAT_Amount = xmlDoc.CreateElement("VATAmount");
            xmlVAT_Amount.InnerText = model.VATAmount.ToString("F3", LibraryCommon.GetIFormatProvider());
            xmlContentInv.AppendChild(xmlVAT_Amount);


            //thue gia tri gia tang  VAT_Rate
            XmlNode xmlVAT_Rate = xmlDoc.CreateElement("VATRate");
            xmlVAT_Rate.InnerText = model.VATRate.ToString("N0", LibraryCommon.GetIFormatProvider());
            xmlContentInv.AppendChild(xmlVAT_Rate);

        
            //Amount
            XmlNode xmlamount = xmlDoc.CreateElement("Amount");
            xmlamount.InnerText = model.Amount.ToString("F3", LibraryCommon.GetIFormatProvider());
            xmlContentInv.AppendChild(xmlamount);


            //hien thi so tien bang chu
            XmlNode xmlAmountInWords = xmlDoc.CreateElement("AmountInWords");
            xmlAmountInWords.InnerText = model.AmountInWords;
            xmlContentInv.AppendChild(xmlAmountInWords);

            if (model.ArisingDate!=null)
            {
                //ngay hóa đơn
                XmlNode xmlArisingDate = xmlDoc.CreateElement("ArisingDate");
                xmlArisingDate.InnerText = model.ArisingDate.Value.ToString("dd/MM/yyyy");
                xmlContentInv.AppendChild(xmlArisingDate);
            }

            //end other
            //End sanpham
            return xmlDoc.OuterXml;
        }
        private string GenerateXMLMTT_VNPT(EInvoice model)
        {
            XmlDocument xmlDocz = new XmlDocument();
            XmlNode contentDSHDon = xmlDocz.CreateElement("DSHDon");
            xmlDocz.AppendChild(contentDSHDon);
            this.SerializeToXMLMTT(xmlDocz, contentDSHDon, model);
            return xmlDocz.InnerXml;
        }
        private void SerializeToXMLMTT(XmlDocument xmlDoc, XmlNode xmlDSHD, EInvoice model)
        {
            string cn = "en-US"; //Vietnamese
            var _cultureInfo = new CultureInfo(cn);
            try
            {
                XmlNode contentHd = xmlDoc.CreateElement("HDon");
                xmlDSHD.AppendChild(contentHd);

                //tao kêy node
                XmlNode InvNode = xmlDoc.CreateElement("key");
                InvNode.InnerText = model.FkeyEInvoice;
                contentHd.AppendChild(InvNode);

                XmlNode contentInv = xmlDoc.CreateElement("DLHDon");
                // tao id tag DLHDon
                contentHd.AppendChild(contentInv);
                // thong tin chung
                XmlNode ttChung = xmlDoc.CreateElement("TTChung");
                contentInv.AppendChild(ttChung);
               
                //this.AddTTKhac(xmlDoc, ttChung, "TTChung", model);

                //end

                if (model.ArisingDate != null)
                {
                    XmlNode thDon = xmlDoc.CreateElement("NLap");
                    thDon.InnerText = model.ArisingDate.Value.ToString("yyyy-MM-dd");
                    ttChung.AppendChild(thDon);
                }

                if (string.IsNullOrEmpty(model.CurrencyUnit))
                {
                    model.CurrencyUnit = "VND";
                }
                XmlNode thDVTTe = xmlDoc.CreateElement("DVTTe");
                thDVTTe.InnerText = model.CurrencyUnit;
                ttChung.AppendChild(thDVTTe);

                if (model.ExchangeRate == null)
                {
                    model.ExchangeRate = 1;
                }
                XmlNode thTGia = xmlDoc.CreateElement("TGia");
                thTGia.InnerText = model.ExchangeRate.Value.ToString(_cultureInfo);
                ttChung.AppendChild(thTGia);

                if (!string.IsNullOrEmpty(model.PaymentMethod))
                {
                    XmlNode thDon = xmlDoc.CreateElement("HTTToan");
                    thDon.InnerText = model.PaymentMethod;
                    ttChung.AppendChild(thDon);
                }
                // hết thông tin chung
                //đến nội dung hóa đơn
                XmlNode ndhDon = xmlDoc.CreateElement("NDHDon");
                contentInv.AppendChild(ndhDon);

                // phần người mua
                XmlNode nMua = xmlDoc.CreateElement("NMua");
                ndhDon.AppendChild(nMua);

                XmlNode nMuaTen = xmlDoc.CreateElement("Ten");
                nMuaTen.AppendChild(xmlDoc.CreateCDataSection(!string.IsNullOrEmpty(model.CusName) ? model.CusName : model.Buyer));
                //nMuaTen.InnerText = !string.IsNullOrEmpty(model.CusName) ? model.CusName : model.Buyer;
                nMua.AppendChild(nMuaTen);


                if (!string.IsNullOrEmpty(model.CusCode))
                {
                    XmlNode nMuaMKHang = xmlDoc.CreateElement("MKHang");
                    nMuaMKHang.InnerText = model.CusCode;
                    nMua.AppendChild(nMuaMKHang);
                }
                 

                if (!string.IsNullOrEmpty(model.CusTaxCode))
                {
                    XmlNode nMuaMst = xmlDoc.CreateElement("MST");
                    nMuaMst.InnerText = model.CusTaxCode;
                    nMua.AppendChild(nMuaMst);
                }


                if (!string.IsNullOrEmpty(model.Buyer) && !string.IsNullOrEmpty(model.CusName))
                {
                    XmlNode nMuaHVTNMHang = xmlDoc.CreateElement("HVTNMHang");
                    nMuaHVTNMHang.AppendChild(xmlDoc.CreateCDataSection(model.Buyer));
                    nMua.AppendChild(nMuaHVTNMHang);

                }
                if (!string.IsNullOrEmpty(model.Address))
                {
                    XmlNode nMuaDChi = xmlDoc.CreateElement("DChi");
                    //nMuaDChi.InnerText = model.Address;
                    nMuaDChi.AppendChild(xmlDoc.CreateCDataSection(model.Address));
                    nMua.AppendChild(nMuaDChi);
                }
                if (!string.IsNullOrEmpty(model.CusPhone))
                {
                    XmlNode nMuaDChi = xmlDoc.CreateElement("SDThoai");
                    nMuaDChi.InnerText = model.CusPhone;
                    nMua.AppendChild(nMuaDChi);
                }
                if (!string.IsNullOrEmpty(model.CusBankNo))
                {
                    XmlNode nMuaSTKNHang = xmlDoc.CreateElement("STKNHang");
                    nMuaSTKNHang.InnerText = model.CusBankNo;
                    nMua.AppendChild(nMuaSTKNHang);
                }
                 if (!string.IsNullOrEmpty(model.CusBankName))
                {
                    XmlNode nMuaTNHang = xmlDoc.CreateElement("TNHang");
                    // nMuaTNHang.InnerText = model.CusBankName;
                    nMuaTNHang.AppendChild(xmlDoc.CreateCDataSection(model.CusBankName));
                    nMua.AppendChild(nMuaTNHang);
                }

                XmlNode nCCCDan = xmlDoc.CreateElement("CCCDan");
                nCCCDan.InnerText = model.CCCD;
                nMua.AppendChild(nCCCDan);

                XmlNode nDCTDTu = xmlDoc.CreateElement("DCTDTu");
                //nDCTDTu.InnerText = model.EmailDeliver;
                nDCTDTu.AppendChild(xmlDoc.CreateCDataSection(model.EmailDeliver));
                nMua.AppendChild(nDCTDTu);

                // hàng hóa dịch vụ
                XmlNode products = xmlDoc.CreateElement("DSHHDVu");
                ndhDon.AppendChild(products);




                int sttProduct = 0;

                foreach (var el in model.EInvoiceItems)
                {
                    sttProduct++;
                    XmlNode product = xmlDoc.CreateElement("HHDVu");
                    products.AppendChild(product);

                    //begin product
                    XmlNode TCHAT = xmlDoc.CreateElement("TChat");
                    TCHAT.InnerText = ((int)el.IsSum).ToString();
                    product.AppendChild(TCHAT);

                    XmlNode xmlStt = xmlDoc.CreateElement("STT");
                    xmlStt.InnerText = sttProduct.ToString(CultureInfo.InvariantCulture);
                    product.AppendChild(xmlStt);

                    if (!string.IsNullOrEmpty(el.ProCode))
                    {
                        XmlNode MHHDVu = xmlDoc.CreateElement("MHHDVu");
                        MHHDVu.InnerText = el.ProCode;
                        product.AppendChild(MHHDVu);
                    }

                    if (!string.IsNullOrEmpty(el.ProName))
                    {
                        XmlNode prodName = xmlDoc.CreateElement("THHDVu");
                        prodName.AppendChild(xmlDoc.CreateCDataSection(el.ProName));
                        //prodName.InnerText = el.ProName;
                        product.AppendChild(prodName);
                    }
                    else
                    {
                        throw new Exception("Tên sản phẩm không được để trống khi phát hành hddt");
                    }

                    if (!string.IsNullOrEmpty(el.Unit))
                    {
                        XmlNode prodUnit = xmlDoc.CreateElement("DVTinh");
                        prodUnit.InnerText = el.Unit;
                        product.AppendChild(prodUnit);
                    }

                    XmlNode prodQuantity = xmlDoc.CreateElement("SLuong");
                    prodQuantity.InnerText = el.Quantity.ToString(_cultureInfo);
                    product.AppendChild(prodQuantity);

                    XmlNode prodPrice = xmlDoc.CreateElement("DGia");
                    prodPrice.InnerText = el.Price.ToString(_cultureInfo);
                    product.AppendChild(prodPrice);
                    if (el.Discount != null)
                    {
                        XmlNode prodTlcKhau = xmlDoc.CreateElement("TLCKhau");
                        prodTlcKhau.InnerText = el.Discount.Value.ToString(_cultureInfo);
                        product.AppendChild(prodTlcKhau);
                    }
                    if (el.DiscountAmount != null)
                    {
                        XmlNode prodTlcKhau = xmlDoc.CreateElement("STCKhau");
                        prodTlcKhau.InnerText = el.DiscountAmount.Value.ToString(_cultureInfo);
                        product.AppendChild(prodTlcKhau);
                    }

                    XmlNode xmlProdVatRate = xmlDoc.CreateElement("TSuat");
                    xmlProdVatRate.InnerText = el.VATRate.ToString(_cultureInfo);
                    product.AppendChild(xmlProdVatRate);

                    XmlNode xmlProdTotal = xmlDoc.CreateElement("ThTien");
                    xmlProdTotal.InnerText = el.Total.ToString(_cultureInfo);
                    product.AppendChild(xmlProdTotal);

                    XmlNode xmlProdVatAmount = xmlDoc.CreateElement("TThue");
                    xmlProdVatAmount.InnerText = el.VATAmount.ToString(_cultureInfo);
                    product.AppendChild(xmlProdVatAmount);

                    XmlNode xmlProdAmount = xmlDoc.CreateElement("TSThue");
                    xmlProdAmount.InnerText = el.Amount.ToString(_cultureInfo);
                    product.AppendChild(xmlProdAmount);


                    //XmlNode xmlProdTtKhac = xmlDoc.CreateElement("TTKhac");
                    //product.AppendChild(xmlProdTtKhac);
                    //if (true)
                    //{
                    //    XmlNode prodTtKhacTtinAmount = xmlDoc.CreateElement("TTin");
                    //    xmlProdTtKhac.AppendChild(prodTtKhacTtinAmount);

                    //    XmlNode prodTtKhacAmount = xmlDoc.CreateElement("TTruong");
                    //    prodTtKhacAmount.InnerText = "Amount";
                    //    prodTtKhacTtinAmount.AppendChild(prodTtKhacAmount);

                    //    XmlNode kdLieuAmount = xmlDoc.CreateElement("KDLieu");
                    //    kdLieuAmount.InnerText = "numeric";
                    //    prodTtKhacTtinAmount.AppendChild(kdLieuAmount);

                    //    XmlNode dLieuAmount = xmlDoc.CreateElement("DLieu");
                    //    dLieuAmount.InnerText = el.Amount.ToString(_cultureInfo);
                    //    prodTtKhacTtinAmount.AppendChild(dLieuAmount);
                    //}
                    ////if (el.VATAmount > 0)
                    //if (true)
                    //{
                    //    XmlNode prodTtKhacTtinVatAmount = xmlDoc.CreateElement("TTin");
                    //    xmlProdTtKhac.AppendChild(prodTtKhacTtinVatAmount);

                    //    XmlNode prodTtKhacVatAmount = xmlDoc.CreateElement("TTruong");
                    //    prodTtKhacVatAmount.InnerText = "VATAmount";
                    //    prodTtKhacTtinVatAmount.AppendChild(prodTtKhacVatAmount);

                    //    XmlNode kdLieuVatAmount = xmlDoc.CreateElement("KDLieu");
                    //    kdLieuVatAmount.InnerText = "numeric";
                    //    prodTtKhacTtinVatAmount.AppendChild(kdLieuVatAmount);

                    //    XmlNode dLieuVatAmount = xmlDoc.CreateElement("DLieu");
                    //    dLieuVatAmount.InnerText = el.VATAmount.ToString(_cultureInfo);
                    //    prodTtKhacTtinVatAmount.AppendChild(dLieuVatAmount);
                    //}


                }

                // thong tin thanh toan
                XmlNode thanhToan = xmlDoc.CreateElement("TToan");
                ndhDon.AppendChild(thanhToan);
                //TTKhac
                //if (IsGenerateTTKhacMTT)
                //{
                //    this.AddTTKhac(xmlDoc, thanhToan, "TToan", model);
                //}


                XmlNode thttltSuat = xmlDoc.CreateElement("THTTLTSuat");
                thanhToan.AppendChild(thttltSuat);
                float[] vatRate = model.EInvoiceItems.Select(p => p.VATRate).Distinct().ToArray();

                foreach (float itemVatrate in vatRate)
                {

                    if (itemVatrate == 0)
                    {
                        var vatamount0 = model.EInvoiceItems.Where(p => p.VATRate == 0).Sum(x => x.VATAmount);
                        var GrossValue0 = model.EInvoiceItems.Where(p => p.VATRate == 0).Sum(x => x.Total);
                        XmlNode ltSuat = xmlDoc.CreateElement("LTSuat");
                        thttltSuat.AppendChild(ltSuat);

                        XmlNode tsuat = xmlDoc.CreateElement("TSuat");
                        tsuat.InnerText = itemVatrate.ToString(_cultureInfo);
                        ltSuat.AppendChild(tsuat);

                        XmlNode tthue = xmlDoc.CreateElement("TThue");
                        tthue.InnerText = vatamount0.ToString(_cultureInfo);
                        ltSuat.AppendChild(tthue);

                        XmlNode thTien = xmlDoc.CreateElement("ThTien");
                        thTien.InnerText = GrossValue0.ToString(_cultureInfo);
                        ltSuat.AppendChild(thTien);
                    }

                    else if (itemVatrate == 5)
                    {
                        var VatAmount5 = model.EInvoiceItems.Where(p => p.VATRate == 5).Sum(x => x.VATAmount);
                        var GrossValue5 = model.EInvoiceItems.Where(p => p.VATRate == 5).Sum(x => x.Total);

                        XmlNode ltSuat = xmlDoc.CreateElement("LTSuat");
                        thttltSuat.AppendChild(ltSuat);

                        XmlNode tsuat = xmlDoc.CreateElement("TSuat");
                        tsuat.InnerText = "5";
                        ltSuat.AppendChild(tsuat);

                        XmlNode tThue = xmlDoc.CreateElement("TThue");
                        tThue.InnerText = VatAmount5.ToString(_cultureInfo);
                        ltSuat.AppendChild(tThue);

                        XmlNode thTien = xmlDoc.CreateElement("ThTien");
                        thTien.InnerText = GrossValue5.ToString(_cultureInfo);
                        ltSuat.AppendChild(thTien);
                    }
                    else if (itemVatrate == 8)
                    {
                        var VatAmount8 = model.EInvoiceItems.Where(p => p.VATRate == 8).Sum(x => x.VATAmount);
                        var GrossValue8 = model.EInvoiceItems.Where(p => p.VATRate == 8).Sum(x => x.Total);

                        XmlNode ltSuat = xmlDoc.CreateElement("LTSuat");
                        thttltSuat.AppendChild(ltSuat);

                        XmlNode tsuat = xmlDoc.CreateElement("TSuat");
                        tsuat.InnerText = "8";
                        ltSuat.AppendChild(tsuat);

                        XmlNode tThue = xmlDoc.CreateElement("TThue");
                        tThue.InnerText = VatAmount8.ToString(_cultureInfo);
                        ltSuat.AppendChild(tThue);

                        XmlNode thTien = xmlDoc.CreateElement("ThTien");
                        thTien.InnerText = GrossValue8.ToString(_cultureInfo);
                        ltSuat.AppendChild(thTien);
                    }

                    else if (itemVatrate == 10)
                    {
                        var VatAmount10 = model.EInvoiceItems.Where(p => p.VATRate == 10).Sum(x => x.VATAmount);
                        var GrossValue10 = model.EInvoiceItems.Where(p => p.VATRate == 10).Sum(x => x.Total);
                        XmlNode ltSuat = xmlDoc.CreateElement("LTSuat");
                        thttltSuat.AppendChild(ltSuat);

                        XmlNode tsuat = xmlDoc.CreateElement("TSuat");
                        tsuat.InnerText = "10";
                        ltSuat.AppendChild(tsuat);


                        XmlNode tThue = xmlDoc.CreateElement("TThue");
                        tThue.InnerText = VatAmount10.ToString(_cultureInfo);
                        ltSuat.AppendChild(tThue);

                        XmlNode thTien = xmlDoc.CreateElement("ThTien");
                        thTien.InnerText = GrossValue10.ToString(_cultureInfo);
                        ltSuat.AppendChild(thTien);
                    }

                    else if (itemVatrate == -1)
                    {
                        var GrossValueKCT = model.EInvoiceItems.Where(p => p.VATRate == -1).Sum(x => x.Total);

                        XmlNode ltSuat = xmlDoc.CreateElement("LTSuat");
                        thttltSuat.AppendChild(ltSuat);

                        XmlNode tsuat = xmlDoc.CreateElement("TSuat");
                        tsuat.InnerText = "-1";
                        ltSuat.AppendChild(tsuat);

                        XmlNode tThue = xmlDoc.CreateElement("TThue");
                        tThue.InnerText = "0";
                        ltSuat.AppendChild(tThue);

                        XmlNode thTien = xmlDoc.CreateElement("ThTien");
                        thTien.InnerText = GrossValueKCT.ToString(_cultureInfo);
                        ltSuat.AppendChild(thTien);
                    }

                    else if (itemVatrate == -2)
                    {
                        var GrossValueKKKNT = model.EInvoiceItems.Where(p => p.VATRate == -2).Sum(x => x.Total);

                        XmlNode ltSuat = xmlDoc.CreateElement("LTSuat");
                        thttltSuat.AppendChild(ltSuat);

                        XmlNode tsuat = xmlDoc.CreateElement("TSuat");
                        tsuat.InnerText = "-2";
                        ltSuat.AppendChild(tsuat);

                        XmlNode tThue = xmlDoc.CreateElement("TThue");
                        tThue.InnerText = "0";
                        ltSuat.AppendChild(tThue);

                        XmlNode thTien = xmlDoc.CreateElement("ThTien");
                        thTien.InnerText = GrossValueKKKNT.ToString(_cultureInfo);
                        ltSuat.AppendChild(thTien);
                    }
                    else
                    {
                        XmlNode ltSuat = xmlDoc.CreateElement("LTSuat");
                        thttltSuat.AppendChild(ltSuat);

                        XmlNode tsuat = xmlDoc.CreateElement("TSuat");
                        tsuat.InnerText = itemVatrate.ToString(CultureInfo.InvariantCulture);
                        ltSuat.AppendChild(tsuat);

                        XmlNode tthue = xmlDoc.CreateElement("TThue");
                        tthue.InnerText = model.VATAmount.ToString(CultureInfo.InvariantCulture);
                        ltSuat.AppendChild(tthue);

                        XmlNode thTien = xmlDoc.CreateElement("ThTien");
                        thTien.InnerText = model.Total.ToString(CultureInfo.InvariantCulture);
                        ltSuat.AppendChild(thTien);
                    }
                }
                XmlNode tgTcThue = xmlDoc.CreateElement("TgTCThue");
                tgTcThue.InnerText = model.Total.ToString(CultureInfo.InvariantCulture);
                thanhToan.AppendChild(tgTcThue);

                XmlNode tgTThue = xmlDoc.CreateElement("TgTThue");
                tgTThue.InnerText = model.VATAmount.ToString(CultureInfo.InvariantCulture);
                thanhToan.AppendChild(tgTThue);

                if (model.DiscountAmount != null && model.DiscountAmount != 0)
                {
                    XmlNode TTCKTMai = xmlDoc.CreateElement("TTCKTMai");
                    TTCKTMai.InnerText = model.DiscountAmount.Value.ToString(CultureInfo.InvariantCulture);
                    thanhToan.AppendChild(TTCKTMai);
                } 
                if (model.DiscountNonTax != null && model.DiscountNonTax != 0)
                {
                    XmlNode TTCKTMai = xmlDoc.CreateElement("TGTKCThue");
                    TTCKTMai.InnerText = model.DiscountNonTax.Value.ToString(CultureInfo.InvariantCulture);
                    thanhToan.AppendChild(TTCKTMai);
                }
                 if (model.DiscountOther != null&&model.DiscountOther != 0)
                {
                    XmlNode TTCKTMai = xmlDoc.CreateElement("TGTKhac");
                    TTCKTMai.InnerText = model.DiscountOther.Value.ToString(CultureInfo.InvariantCulture);
                    thanhToan.AppendChild(TTCKTMai);
                }

                XmlNode tgTttbSo = xmlDoc.CreateElement("TgTTTBSo");
                tgTttbSo.InnerText = model.Amount.ToString(CultureInfo.InvariantCulture);
                thanhToan.AppendChild(tgTttbSo);

                XmlNode tgTttbChu = xmlDoc.CreateElement("TgTTTBChu");
                tgTttbChu.InnerText = model.AmountInWords;
                thanhToan.AppendChild(tgTttbChu);

                // return xmlDoc;


            }
            catch (Exception e)
            {
                throw new Exception("Lỗi sinh XML MTT: " + e.ToString());
            }

        }
        private void AddTTKhac(XmlDocument xmlDoc, XmlNode xmlDSHD, string Name, EInvoice model)
        {
            if (Name == "TTChung")
            {
                //thông tin khác
                XmlNode xmlTtKhacNMua = xmlDoc.CreateElement("TTKhac");
                xmlDSHD.AppendChild(xmlTtKhacNMua);

                if (model.Customer!=null)
                {
                    if (!string.IsNullOrEmpty(model.Customer.Passport))
                    {
                        //  Add trường mở rộng
                        XmlNode ttinTtKhac = xmlDoc.CreateElement("TTin");
                        XmlNode tTruong = xmlDoc.CreateElement("TTruong");
                        tTruong.InnerText = "Extra5";
                        ttinTtKhac.AppendChild(tTruong);
                        XmlNode kdLieu = xmlDoc.CreateElement("KDLieu");
                        kdLieu.InnerText = "string";
                        ttinTtKhac.AppendChild(kdLieu);
                        XmlNode dLieu = xmlDoc.CreateElement("DLieu");
                        dLieu.InnerText = model.Customer.Passport;
                        ttinTtKhac.AppendChild(dLieu);
                        xmlTtKhacNMua.AppendChild(ttinTtKhac);
                    }
                    if (!string.IsNullOrEmpty(model.Customer.Nationality))
                    {
                        //  Add trường mở rộng
                        XmlNode ttinTtKhac = xmlDoc.CreateElement("TTin");
                        XmlNode tTruong = xmlDoc.CreateElement("TTruong");
                        tTruong.InnerText = "Extra6";
                        ttinTtKhac.AppendChild(tTruong);
                        XmlNode kdLieu = xmlDoc.CreateElement("KDLieu");
                        kdLieu.InnerText = "string";
                        ttinTtKhac.AppendChild(kdLieu);
                        XmlNode dLieu = xmlDoc.CreateElement("DLieu");
                        dLieu.InnerText = model.Customer.Nationality;
                        ttinTtKhac.AppendChild(dLieu);
                        xmlTtKhacNMua.AppendChild(ttinTtKhac);
                    }
                }
            }
            //else if (Name == "TTChung")
            //{
            //    //thông tin khác
            //    XmlNode xmlTtKhac = xmlDoc.CreateElement("TTKhac");
            //    xmlDSHD.AppendChild(xmlTtKhac);

                

            //}
            //else if (Name == "TToan")
            //{
            //    //thông tin khác
            //    XmlNode xmlTtKhacTToan = xmlDoc.CreateElement("TTKhac");
            //    xmlDSHD.AppendChild(xmlTtKhacTToan);

               

            //}
        }
        public async Task CreateRangeAsync(List<EInvoice> Entity)
        {
            await _repository.AddRangeAsync(Entity);
        }

        public async Task UpdateAsync(EInvoice Entity)
        {
            await _repository.UpdateAsync(Entity);
        }
         public async Task UpdateRangeAsync(List<EInvoice> Entity)
        {
            await _repository.UpdateRangeAsync(Entity);
        }

        public async Task<PaginatedList<EInvoice>> GetAllDatatableAsync(int? Comid, InvoiceModel textSearch, string sortColumn, string sortColumnDirection, int pageSize, int skip, EnumTypeProduct enumTypeProduct = EnumTypeProduct.THOITRANG)
        {
            var datalist = _repository.GetAllQueryable().AsNoTracking().Where(x => x.ComId == Comid && !x.IsDelete);
            //if (Comid != null)
            //{
            //    datalist = datalist.Where(x => x.ComId == Comid);
            //}
            if (!string.IsNullOrEmpty(textSearch.Code))
            {
                datalist = datalist.Where(m => m.InvoiceCode.Contains(textSearch.Code) || m.CusCode.Contains(textSearch.Code) || m.MCQT == textSearch.Code);
            }
            if (textSearch.InvoiceNo != null)
            {
                datalist = datalist.Where(m => m.InvoiceNo== textSearch.InvoiceNo);
            }if (textSearch.Status != StatusEinvoice.Null)
            {
                datalist = datalist.Where(m => m.StatusEinvoice== textSearch.Status);
            }
            if (!string.IsNullOrEmpty(textSearch.RangesDate))
            {
                var _split = textSearch.RangesDate.Split('-');
                DateTime? sratdate = Common.ConvertStringToDateTime(_split[0].Trim());
                DateTime? enddate = Common.ConvertStringToDateTime(_split[1].Trim());
                if (sratdate == null || enddate == null)
                {
                    return null;
                }
                datalist = datalist.Where(m => m.CreatedOn >= sratdate && m.CreatedOn < enddate.Value.AddDays(1));
            }
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                datalist = datalist.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            else
            {
                datalist = datalist.OrderByDescending(x => x.Id);
            }
            // count = datalist.Count();
            datalist = datalist.Select(x => new EInvoice()
            {
                Id = x.Id,
                Fkey = x.Fkey,
                EInvoiceCode = x.EInvoiceCode,
                Pattern = x.Pattern,
                Serial = x.Serial,
                InvoiceNo = x.InvoiceNo,
                InvoiceCode = x.InvoiceCode,
                PublishDate = x.PublishDate,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                Buyer = x.Buyer,
                Total = x.Total,
                StatusEinvoice = x.StatusEinvoice,
                DiscountAmount = x.DiscountAmount,
                Amount = x.Amount,
                CusName = x.CusName,
                CusCode = x.CusCode,
                CasherName = x.CasherName
            });
            var data = await PaginatedList<EInvoice>.ToPagedListAsync(datalist, textSearch.Currentpage, pageSize);
            data.Items.ForEach(x =>
            {
                var values = "id=" + x.Id;
                var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                x.Secret = secret;
                // x.CasherName = _userManager.FindByIdAsync(x.CreatedBy).Result?.FullName;
                // x.CasherName = _userManager.FindByIdAsync(x.CreatedBy).Result?.FullName;
            });
            return data;
        }

        public async Task<IResult<string>> GetInvViewFkeyAsync(int IdEInvoice, int Comid, string Carsher, string IdCarsher)
        {
            var get = await _repository.Entities.Where(x => x.ComId == Comid && x.Id == IdEInvoice).AsNoTracking().SingleOrDefaultAsync();
            if (get.TypeSupplierEInvoice == ENumSupplierEInvoice.VNPT)
            {
                var company = await _supplierEInvoicerepository.GetByIdAsync(Comid,get.TypeSupplierEInvoice);
                if (company == null)
                {
                    return await Result<string>.FailAsync("Chưa cấu hình phát hành hóa đơn");
                }
                var pub = await _vnptportalrepository.getInvViewFkeyNoPayAsync(get.FkeyEInvoice, company.UserNameService, company.PassWordService, company.DomainName);
                if (!pub.Contains("ERR:"))
                {
                    AddHistori(new HistoryEInvoice() { Carsher = Carsher, StatusEvent = StatusStaffEventEInvoice.InHoaDon, IdCarsher = IdCarsher, EInvoiceCode = get.EInvoiceCode, IdEInvoice = get.Id, Name = $"In hóa đơn" });
                    return await Result<string>.SuccessAsync(pub, HeperConstantss.ERR000);
                }
                else
                {
                    _log.LogError($"Lỗi xem hóa đơn getInvViewFkeyNoPayAsync đến nhà cung cấp VNPT -> {pub}");
                    return await Result<string>.FailAsync($"Lỗi phát hành hóa đơn đến nhà cung cấp VNPT -> {pub}");
                }
            }
            return await Result<string>.FailAsync("Không tồn tại cấu hình nhà cung cấp hóa đơn nào ");
        }
        public async Task<IResult<string>> DownloadInvFkeyNoPayAsync(int IdEInvoice, int Comid, string Carsher, string IdCarsher)
        {
            var get = await _repository.Entities.Where(x => x.ComId == Comid && x.Id == IdEInvoice).AsNoTracking().SingleOrDefaultAsync();
            if (get.TypeSupplierEInvoice == ENumSupplierEInvoice.VNPT)
            {
                var company = await _supplierEInvoicerepository.GetByIdAsync(Comid,get.TypeSupplierEInvoice);
                if (company == null)
                {
                    return await Result<string>.FailAsync("Chưa cấu hình phát hành hóa đơn");
                }
                var pub = await _vnptportalrepository.downloadInvFkeyNoPayAsync(get.FkeyEInvoice, company.UserNameService, company.PassWordService, company.DomainName);
                if (!pub.Contains("ERR:"))
                {
                    AddHistori(new HistoryEInvoice() { Carsher = Carsher, StatusEvent = StatusStaffEventEInvoice.InHoaDon, IdCarsher = IdCarsher, EInvoiceCode = get.EInvoiceCode, IdEInvoice = get.Id, Name = $"Dow xml hóa đơn" });
                    return await Result<string>.SuccessAsync(pub, HeperConstantss.ERR000);
                }
                else
                {
                    _log.LogError($"Lỗi xem hóa đơn getInvViewFkeyNoPayAsync đến nhà cung cấp VNPT -> {pub}");
                    return await Result<string>.FailAsync($"Lỗi phát hành hóa đơn đến nhà cung cấp VNPT -> {pub}");
                }
            }
            return await Result<string>.FailAsync("Không tồn tại cấu hình nhà cung cấp hóa đơn nào ");
        }
        public async Task<IResult<string>> ConvertForStoreFkeyAsync(int IdEInvoice, int Comid, string Carsher, string IdCarsher)
        {
            var get = await _repository.Entities.Where(x => x.ComId == Comid && x.Id == IdEInvoice).AsNoTracking().SingleOrDefaultAsync();
            if (get.TypeSupplierEInvoice == ENumSupplierEInvoice.VNPT)
            {
                var company = await _supplierEInvoicerepository.GetByIdAsync(Comid,get.TypeSupplierEInvoice);
                if (company == null)
                {
                    return await Result<string>.FailAsync("Chưa cấu hình phát hành hóa đơn");
                }
                var pub = await _vnptportalrepository.convertForStoreFkeyAsync(get.FkeyEInvoice, company.UserNameService, company.PassWordService, company.DomainName);
                if (!pub.Contains("ERR:"))
                {
                    AddHistori(new HistoryEInvoice() { Carsher = Carsher, StatusEvent = StatusStaffEventEInvoice.InHoaDon, IdCarsher = IdCarsher, EInvoiceCode = get.EInvoiceCode, IdEInvoice = get.Id, Name = $"In hóa đơn" });
                    return await Result<string>.SuccessAsync(pub, HeperConstantss.ERR000);
                }
                else if (pub.Contains("ERR:12"))
                {
                    return await this.GetInvViewFkeyAsync(IdEInvoice, Comid, Carsher, IdCarsher);
                }
                else
                {
                    _log.LogError($"Lỗi xem hóa đơn getInvViewFkeyNoPayAsync đến nhà cung cấp VNPT -> {pub}");
                    return await Result<string>.FailAsync($"Lỗi phát hành hóa đơn đến nhà cung cấp VNPT -> {pub}");
                }
            }
            return await Result<string>.FailAsync("Không tồn tại cấu hình nhà cung cấp hóa đơn nào ");
        }

        public async Task<IResult<PublishInvoiceModelView>> PublishInvoiceAsync(int[] lst, int ComId, string Carsher, string IdCarsher)
        {
            await _unitOfWork.CreateTransactionAsync();
            var getall = await _repository.Entities.Where(x => x.ComId == ComId && lst.Contains(x.Id)).Include(x => x.EInvoiceItems).ToListAsync();
            SupplierEInvoice supplierEInvoice = new SupplierEInvoice();
            List<DetailInvoice> ListDetailInvoice = new List<DetailInvoice>();
            try
            {
                foreach (var item in getall)
                {
                    if (item.IdCustomer.HasValue)
                    {
                        var cus = _repositoryCusomer.GetById(item.IdCustomer.Value);
                        item.Customer = cus;
                    }
                 
                    if (item.TypeSupplierEInvoice == ENumSupplierEInvoice.VNPT && item.StatusEinvoice == StatusEinvoice.NewInv)
                    {
                        if (supplierEInvoice.TypeSupplierEInvoice != item.TypeSupplierEInvoice)
                        {
                            supplierEInvoice = await _supplierEInvoicerepository.GetByIdAsync(ComId,item.TypeSupplierEInvoice);
                        }

                        if (supplierEInvoice == null)
                        {
                            ListDetailInvoice.Add(new DetailInvoice()
                            {
                                code = item.EInvoiceCode,
                                note = $"Chưa cấu hình nhà cung cấp",
                                TypePublishEinvoice = ENumTypePublishEinvoice.KHONGTONTAINHACUNGCAP,
                            });
                            //return await Result<PublishInvoiceModelView>.FailAsync("Chưa cấu hình phát hành hóa đơn");
                        }
                        else
                        {
                            if (item.EInvoiceItems.Count() == 0)
                            {
                                _log.LogError($"Hóa đơn không có sản phẩm {item.EInvoiceCode}");

                                ListDetailInvoice.Add(new DetailInvoice()
                                {
                                    TypePublishEinvoice = ENumTypePublishEinvoice.KHONGCOSANPHAM,
                                    code = item.EInvoiceCode,
                                    note = $"Hóa đơn không có sản phẩm",
                                });
                                continue;
                            }
                            var publish = await this.ImportAndPublishInvMTTAsync(item, supplierEInvoice, Carsher, IdCarsher);
                            if (publish.Succeeded)
                            {

                                string[] getinvoice = publish.Data.Split(';');
                                if (getinvoice.Count() < 2)
                                {
                                    _log.LogError($"Lỗi phát hành hóa đơn đến nhà cung cấp VNPT -> {publish}");

                                    ListDetailInvoice.Add(new DetailInvoice()
                                    {
                                        TypePublishEinvoice = ENumTypePublishEinvoice.PHATHANHLOI,
                                        code = item.EInvoiceCode,
                                        note = $"Phát hành lỗi, mã lỗi từ nhà cung cấp {publish.Message}",
                                    });
                                }
                                else
                                {
                                    var getinvno = await _managerInvNoRepository.UpdateInvNo(item.ComId, ENumTypeManagerInv.EInvoice, false);
                                    string[] getinvoicesplit = getinvoice[1].Split('_');
                                    string invoiceno = getinvoicesplit[1];
                                    string MCQT = getinvoicesplit[2];
                                    item.MCQT = MCQT;
                                    item.InvoiceNo = int.Parse(invoiceno);
                                    item.StatusEinvoice = StatusEinvoice.SignedInv;
                                    item.PublishDate = DateTime.Now;
                                    await this.UpdateStatusPublishInvoice(item.ComId,item.IdInvoice, item.InvoiceCode);

                                    await _unitOfWork.SaveChangesAsync();

                                    ListDetailInvoice.Add(new DetailInvoice()
                                    {
                                        TypePublishEinvoice = ENumTypePublishEinvoice.PHATHANHOK,
                                        code = item.EInvoiceCode,
                                        note = $"Phát hành thành công hóa đơn điện tử, mẫu số {item.Pattern}, ký hiệu {item.Serial},số {item.InvoiceNo.ToString("00000000")}",
                                    });
                                }
                            }
                            else
                            {
                                _log.LogError($"Lỗi phát hành hóa đơn đến nhà cung cấp VNPT -> {publish}");

                                ListDetailInvoice.Add(new DetailInvoice()
                                {
                                    TypePublishEinvoice = ENumTypePublishEinvoice.PHATHANHLOI,
                                    code = item.EInvoiceCode,
                                    note = $"Phát hành lỗi, mã lỗi từ nhà cung cấp: {publish.Message}",
                                });
                            }
                        }

                    }
                    else
                    {
                        ListDetailInvoice.Add(new DetailInvoice()
                        {
                            TypePublishEinvoice = ENumTypePublishEinvoice.TRANGTHAIPHATHANHKHONGHOPLE,
                            code = item.EInvoiceCode,
                            note = $"Hóa đơn có trạng thái là: {GeneralMess.GeneralMessStatusEInvoice(item.StatusEinvoice)}, không thể phát hành ",
                        });
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                var PublishInvoiceModelView = new PublishInvoiceModelView();
                PublishInvoiceModelView.DetailInvoices = ListDetailInvoice;
                PublishInvoiceModelView.TypeEventInvoice = EnumTypeEventInvoice.PublishEInvoice;
                return await Result<PublishInvoiceModelView>.SuccessAsync(PublishInvoiceModelView);
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                _log.LogError("Phát hành lỗi: " + e.ToString());
                _log.LogError(e.ToString());

                return await Result<PublishInvoiceModelView>.FailAsync("Phát hành lỗi: " + e.ToString());
            }
        }
        private async Task UpdateStatusPublishInvoice(int Comid,int idInvoice, string invoicecode = null, EnumStatusPublishInvoiceOrder staus = EnumStatusPublishInvoiceOrder.PUBLISH)
        {
            var get = await _Invoicerepository.GetByIdAsync(idInvoice);
            if (get != null)
            {
                get.StatusPublishInvoiceOrder = staus;
                await _Invoicerepository.UpdateAsync(get);
            }
            else if (!string.IsNullOrEmpty(invoicecode))
            {
                var getcodeeinvoice = await _repository.Entities.SingleOrDefaultAsync(x => x.ComId == Comid && x.IdInvoice == idInvoice &&!x.IsDelete&& x.StatusEinvoice!=StatusEinvoice.CanceledInv);
                var getcode = await _Invoicerepository.Entities.SingleOrDefaultAsync(x => x.ComId == Comid && x.InvoiceCode == invoicecode);
                if (getcode != null)
                {
                    if (getcodeeinvoice!=null && getcode.IdEInvoice!=null)
                    {
                        getcode.IdEInvoice = getcodeeinvoice.Id;
                    }
                    getcode.StatusPublishInvoiceOrder = staus;
                    await _Invoicerepository.UpdateAsync(get);
                }
            }
        }

        public async Task<IResult<string>> ConvertForStoreFkeyMutiInvoiceAsync(int[] lstid, int Comid, ENumTypePrint typePrint, string Carsher, string IdCarsher)
        {
            var getall = _repository.Entities.Where(x => lstid.Contains(x.Id) && x.ComId == Comid).AsNoTracking().ToList();
            string html = string.Empty;
            string mess = string.Empty;
            int deminvok = 0;
            SupplierEInvoice company = new SupplierEInvoice();
            foreach (var item in getall)
            {
                if (item.TypeSupplierEInvoice == ENumSupplierEInvoice.VNPT)
                {
                    if (company.TypeSupplierEInvoice != item.TypeSupplierEInvoice)
                    {
                        company = await _supplierEInvoicerepository.GetByIdAsync(Comid,item.TypeSupplierEInvoice);
                    }
                    if (company == null)
                    {
                        mess += $"Hóa đơn có mã {item.EInvoiceCode}, không tìm thấy nhà cung cấp";
                    }
                    var pub = string.Empty;
                    if (typePrint == ENumTypePrint.PrintConvert)
                    {
                        pub = await _vnptportalrepository.convertForStoreFkeyAsync(item.FkeyEInvoice, company.UserNameService, company.PassWordService, company.DomainName);
                    }
                    else
                    {
                        pub = await _vnptportalrepository.getInvViewFkeyNoPayAsync(item.FkeyEInvoice, company.UserNameService, company.PassWordService, company.DomainName);
                    }

                    if (!pub.Contains("ERR:"))
                    {
                        html += pub;
                        deminvok += 1;
                        AddHistori(new HistoryEInvoice() { Carsher = Carsher, StatusEvent = StatusStaffEventEInvoice.InHoaDon, IdCarsher = IdCarsher, EInvoiceCode = item.EInvoiceCode, IdEInvoice = item.Id, Name = $"In hóa đơn" });
                    }
                    else if (pub.Contains("ERR:12"))
                    {
                        mess += $"Hóa đơn có mã {item.EInvoiceCode}, lỗi từ nhà cung cấp {pub}";
                    }
                    else
                    {
                        _log.LogError($"Lỗi xem hóa đơn ConvertForStoreFkeyMutiInvoiceAsync đến nhà cung cấp VNPT -> {pub}");
                        mess += $"Lỗi get hóa đơn từ nhà cung cấp VNPT -> {pub}";
                    }
                }

            }
            if (deminvok == 0)
            {
                await _unitOfWork.SaveChangesAsync();
                return await Result<string>.FailAsync(mess);
            }
            else
            {
                return await Result<string>.SuccessAsync(html, mess);
            }
        }

        public async Task<IResult<PublishInvoiceModelView>> SycnInvoiceAsync(int[] lst, int ComId, string Carsher, string IdCarsher, EnumTypeSyncEInvoice TypeSyncEInvoice)
        {
            SupplierEInvoice company = new SupplierEInvoice();
            List<DetailInvoice> ListDetailInvoice = new List<DetailInvoice>();
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                bool update = false;
                var getall = await _repository.Entities.Where(x => lst.Contains(x.Id) && x.ComId == ComId).ToListAsync();
                if (getall.Count==0)
                {
                    _log.LogError($"Không tìm thấy hóa đơn");
                    return await Result<PublishInvoiceModelView>.FailAsync("Không tìm thấy hóa đơn");
                }
                if (TypeSyncEInvoice == EnumTypeSyncEInvoice.TRANG_THAI_HOA_DON)
                {
                    foreach (var item in getall)
                    {
                        try
                        {
                            if (item.TypeSupplierEInvoice == ENumSupplierEInvoice.VNPT)
                            {
                                if (company.TypeSupplierEInvoice != item.TypeSupplierEInvoice)
                                {
                                    company = await _supplierEInvoicerepository.GetByIdAsync(ComId, item.TypeSupplierEInvoice);
                                }
                                if (company == null)
                                {
                                    ListDetailInvoice.Add(new DetailInvoice()
                                    {
                                        code = item.EInvoiceCode,
                                        note = $"Hóa đơn có mã {item.EInvoiceCode}, không tìm thấy nhà cung cấp",
                                        TypePublishEinvoice = ENumTypePublishEinvoice.KHONGTONTAINHACUNGCAP,
                                    });
                                }
                                else
                                {
                                    var pub = await _vnptportalrepository.listInvByCusFkeyAsync(item.FkeyEInvoice, string.Empty, string.Empty, company.UserNameService, company.PassWordService, company.DomainName);
                                    if (!pub.Contains(CommonERREinvoice.ERR))
                                    {
                                        try
                                        {
                                            var model = ConvertSupport.ConvertXMLToModel<XmlListInvByCusFkey>(pub);
                                            if (model.Item.Count() == 0)
                                            {
                                                ListDetailInvoice.Add(new DetailInvoice()
                                                {
                                                    code = item.EInvoiceCode,
                                                    note = $"Mã đơn này chưa phát hành không thể đồng bộ",
                                                    TypePublishEinvoice = ENumTypePublishEinvoice.HOADONCHUAPHATHANH,
                                                });
                                                _log.LogError($"Mã đơn này chưa phát hành không thể đồng bộ -> {item.EInvoiceCode} -> {pub}");

                                            }
                                            else
                                            {
                                                item.InvoiceNo = model.Item[0].InvNum;
                                                item.StatusEinvoice = (StatusEinvoice)model.Item[0].Status;
                                                if (model.Item[0].PublishDate.Split(" ").Count() > 0)
                                                {
                                                    item.PublishDate = Common.ConvertStringToDateTime(model.Item[0].PublishDate.Split(" ")[0], "M/d/yyyy").Value;
                                                }
                                                await this.UpdateStatusPublishInvoice(item.ComId,item.IdInvoice, item.InvoiceCode);
                                                AddHistori(new HistoryEInvoice() { Carsher = Carsher, StatusEvent = StatusStaffEventEInvoice.DongBoHoaDon, IdCarsher = IdCarsher, EInvoiceCode = item.EInvoiceCode, IdEInvoice = item.Id, Name = $"Đồng bộ hóa đơn" });
                                                update = true;
                                                ListDetailInvoice.Add(new DetailInvoice()
                                                {
                                                    code = item.EInvoiceCode,
                                                    note = $"Đồng bộ hóa đơn điện tử  thành công",
                                                    TypePublishEinvoice = ENumTypePublishEinvoice.DONGBOTHANHCONG,
                                                });
                                            }

                                        }
                                        catch (Exception e)
                                        {
                                            ListDetailInvoice.Add(new DetailInvoice()
                                            {
                                                code = item.EInvoiceCode,
                                                note = $"Đồng bộ hóa đơn điện tử thất bại ",
                                                TypePublishEinvoice = ENumTypePublishEinvoice.DONGBOTHATBAI,
                                            });
                                            _log.LogError($"Lỗi conver dữ liệu -> {pub}");
                                            _log.LogError($"Lỗi conver dữ liệu -> {e.ToString()}");
                                        }

                                    }
                                    else
                                    {
                                        ListDetailInvoice.Add(new DetailInvoice()
                                        {
                                            code = item.EInvoiceCode,
                                            note = $"Đồng bộ hóa đơn điện tử thất bại, mã lỗi từ nhà cung cấp VNPT: {pub}",
                                            TypePublishEinvoice = ENumTypePublishEinvoice.DONGBOTHATBAI,
                                        });
                                        _log.LogError($"Đồng bộ hóa đơn điện tử thất bại, mã lỗi từ nhà cung cấp VNPT -> {pub}");
                                    }



                                }
                            }
                            else
                            {
                                ListDetailInvoice.Add(new DetailInvoice()
                                {
                                    code = item.EInvoiceCode,
                                    note = $"Hóa đơn có mã {item.EInvoiceCode}, không tìm thấy nhà cung cấp",
                                    TypePublishEinvoice = ENumTypePublishEinvoice.KHONGTONTAINHACUNGCAP,
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            ListDetailInvoice.Add(new DetailInvoice()
                            {
                                code = item.EInvoiceCode,
                                note = $"Đồng bộ hóa đơn điện tử thất bại, lỗi convert dữ liệu: {e.ToString()}",
                                TypePublishEinvoice = ENumTypePublishEinvoice.DONGBOTHATBAI,
                            });
                            _log.LogError($"Lỗi conver dữ liệu Exception -> {e.ToString()}");
                        }
                    }
                }
                else if (TypeSyncEInvoice == EnumTypeSyncEInvoice.TRANG_THAI_CQT)
                {
                    var frrstinvoice = getall.FirstOrDefault();
                    if (frrstinvoice.TypeSupplierEInvoice == ENumSupplierEInvoice.VNPT)
                    {
                        if (company.TypeSupplierEInvoice != frrstinvoice.TypeSupplierEInvoice)
                        {
                            company = await _supplierEInvoicerepository.GetByIdAsync(ComId, frrstinvoice.TypeSupplierEInvoice);
                        }
                        if (company == null)
                        {
                            ListDetailInvoice.Add(new DetailInvoice()
                            {
                                code = frrstinvoice.EInvoiceCode,
                                note = $"Hóa đơn có mã {frrstinvoice.EInvoiceCode}, không tìm thấy nhà cung cấp",
                                TypePublishEinvoice = ENumTypePublishEinvoice.KHONGTONTAINHACUNGCAP,
                            });

                        }
                        else
                        {
                            string fkeys = string.Join(";", getall.Select(x => x.FkeyEInvoice).ToArray());
                            string getVNPT = await _vnptrepository.GetMCCQThueByFkeys(company.UserNameAdmin, company.PassWordAdmin, company.UserNameService, company.PassWordService, frrstinvoice.Pattern, fkeys, company.DomainName);
                            if (getVNPT.Contains("ERR:"))
                            {
                                _log.LogError("Đồng bộ lấy trạng thái thất bại " + getVNPT + "__" + fkeys);
                            }
                            else
                            {
                                var result = GenXMLToModelDSHDon(getVNPT);//chuyển thành xml
                                var lstHistoryEInvoice = new List<HistoryEInvoice>();

                                foreach (var item in getall)
                                {
                                    var getfkey = result.HDons.SingleOrDefault(x => x.Fkey == item.FkeyEInvoice);
                                    if (getfkey != null)
                                    {
                                        switch (getfkey.Status)
                                        {
                                            case ENumTypeStausSendCQT.CHUAGUI_CQT:
                                                ListDetailInvoice.Add(new DetailInvoice()
                                                {
                                                    code = item.EInvoiceCode,
                                                    note = $"Đồng bộ hóa đơn điện tử chưa gửi CQT",
                                                    TypePublishEinvoice = ENumTypePublishEinvoice.UnSentInv,
                                                });
                                                break;
                                            case ENumTypeStausSendCQT.DAGUI_CQT:
                                                item.StatusEinvoice = StatusEinvoice.SentInv;
                                                lstHistoryEInvoice.Add(new HistoryEInvoice()
                                                {
                                                    Carsher = "Hệ thống",
                                                    StatusEvent = StatusStaffEventEInvoice.SendCQT,
                                                    IdCarsher = string.Empty,
                                                    EInvoiceCode = item.EInvoiceCode,
                                                    IdEInvoice = item.Id,
                                                    Name = $"Hóa đơn đã được gửi CQT"
                                                });

                                                ListDetailInvoice.Add(new DetailInvoice()
                                                {
                                                    code = item.EInvoiceCode,
                                                    note = $"Đồng bộ hóa đơn điện tử đã gửi CQT",
                                                    TypePublishEinvoice = ENumTypePublishEinvoice.SendCQT,
                                                });
                                                // update = true;
                                                break;
                                            case ENumTypeStausSendCQT.CQT_CHAPNHAN:
                                                item.StatusEinvoice = StatusEinvoice.AcceptedInv;
                                                lstHistoryEInvoice.Add(new HistoryEInvoice()
                                                {
                                                    Carsher = "Hệ thống",
                                                    StatusEvent = StatusStaffEventEInvoice.SendCQT,
                                                    IdCarsher = string.Empty,
                                                    EInvoiceCode = item.EInvoiceCode,
                                                    IdEInvoice = item.Id,
                                                    Name = $"Hóa đơn đã được cơ quan thuế chấp nhận"
                                                });

                                                ListDetailInvoice.Add(new DetailInvoice()
                                                {
                                                    code = item.EInvoiceCode,
                                                    note = $"Đồng bộ hóa đơn điện tử CQT đã chấp nhập",
                                                    TypePublishEinvoice = ENumTypePublishEinvoice.SendCQTOK,
                                                });

                                                update = true;
                                                break;
                                            case ENumTypeStausSendCQT.CQT_TUCHOI:
                                                item.StatusEinvoice = StatusEinvoice.RejectedInv;
                                                lstHistoryEInvoice.Add(new HistoryEInvoice()
                                                {
                                                    Carsher = "Hệ thống",
                                                    StatusEvent = StatusStaffEventEInvoice.SendCQT,
                                                    IdCarsher = string.Empty,
                                                    EInvoiceCode = item.EInvoiceCode,
                                                    IdEInvoice = item.Id,
                                                    Name = getfkey.MTLoi
                                                });

                                                ListDetailInvoice.Add(new DetailInvoice()
                                                {
                                                    code = item.EInvoiceCode,
                                                    note = $"Đồng bộ hóa đơn điện tử bị CQT từ chối",
                                                    TypePublishEinvoice = ENumTypePublishEinvoice.SendCQTFail,
                                                });
                                                update = true;
                                                break;
                                            default:
                                                ListDetailInvoice.Add(new DetailInvoice()
                                                {
                                                    code = item.EInvoiceCode,
                                                    note = $"Lỗi không xác định, khong tìm thấy trạng thái phù hợp" + getfkey.TThai,
                                                    TypePublishEinvoice = ENumTypePublishEinvoice.ERROR,
                                                });
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ListDetailInvoice.Add(new DetailInvoice()
                        {
                            code = frrstinvoice.EInvoiceCode,
                            note = $"Hóa đơn có mã {frrstinvoice.EInvoiceCode}, không tìm thấy nhà cung cấp phù hợp",
                            TypePublishEinvoice = ENumTypePublishEinvoice.KHONGTONTAINHACUNGCAP,
                        });
                    }
                }
                   
                if (update)
                {
                    await _repository.UpdateRangeAsync(getall);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    _unitOfWork.Dispose();
                }
                var PublishInvoiceModelView = new PublishInvoiceModelView();
                PublishInvoiceModelView.DetailInvoices = ListDetailInvoice;
                PublishInvoiceModelView.TypeEventInvoice = EnumTypeEventInvoice.SycnEInvoice;
                return await Result<PublishInvoiceModelView>.SuccessAsync(PublishInvoiceModelView);
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                _log.LogError($"Lỗi conver dữ liệu Exception -> {e.ToString()}");
                return await Result<PublishInvoiceModelView>.FailAsync(e.ToString());
            }

        }
     
        private async void AddHistori(HistoryEInvoice entity)
        {
            await _HistoryEInvoicerepository.AddAsync(entity);
        } 
        private async void AddHistori(List<HistoryEInvoice> entity)
        {
            await _HistoryEInvoicerepository.AddRangeAsync(entity);
        }
        public async Task<IResult<PublishInvoiceModelView>> RemoveEInvoiceAsync(int[] lst, int ComId, string Carsher, string IdCarsher)
        {
            SupplierEInvoice company = new SupplierEInvoice();
            List<DetailInvoice> ListDetailInvoice = new List<DetailInvoice>();

            await _unitOfWork.CreateTransactionAsync();
            try
            {
                bool update = false;
                var getall = await _repository.Entities.AsNoTracking().Where(x => lst.Contains(x.Id) && x.ComId == ComId).ToListAsync();

                foreach (var item in getall)
                {
                    try
                    {
                        if (item.TypeSupplierEInvoice == ENumSupplierEInvoice.VNPT)
                        {
                            if (item.StatusEinvoice != StatusEinvoice.NewInv)
                            {
                                ListDetailInvoice.Add(new DetailInvoice()
                                {
                                    code = item.EInvoiceCode,
                                    note = $"Hóa  đơn  này có trạng thái: {GeneralMess.GeneralMessStatusEInvoice(item.StatusEinvoice).ToLower()}, không thể xóa bỏ",
                                    TypePublishEinvoice = ENumTypePublishEinvoice.TRANGTHAIPHATHANHKHONGHOPLE,
                                });
                            }
                            else
                            {
                                if (company.TypeSupplierEInvoice != item.TypeSupplierEInvoice)
                                {
                                    company = await _supplierEInvoicerepository.GetByIdAsync(ComId,item.TypeSupplierEInvoice);
                                }
                                if (company == null)
                                {
                                    await UpdateStatusPublishInvoice(item.ComId, item.IdInvoice, item.InvoiceCode, EnumStatusPublishInvoiceOrder.NONE);
                                    //item.IsDelete = true;
                                    //item.InvoiceCode = item.InvoiceCode + "-D";

                                    await _repository.DeleteAsync(item);
                                    update = true;
                                    ListDetailInvoice.Add(new DetailInvoice()
                                    {
                                        code = item.EInvoiceCode,
                                        note = $"Hóa đơn có mã {item.EInvoiceCode}, không tìm thấy nhà cung cấp hóa đơn, đã xóa thành công",
                                        TypePublishEinvoice = ENumTypePublishEinvoice.KHONGTONTAINHACUNGCAP,
                                    });
                                }
                                if (!company.IsHKD)
                                {
                                    //---------------------------------------xem hóa đơn theo webservice
                                    var pub = await _vnptportalrepository.getInvViewFkeyNoPayAsync(item.FkeyEInvoice, company.UserNameService, company.PassWordService, company.DomainName);
                                    if (pub.Equals("ERR:6"))
                                    {
                                        await UpdateStatusPublishInvoice(item.ComId, item.IdInvoice, item.InvoiceCode, EnumStatusPublishInvoiceOrder.NONE);
                                        // item.IsDelete = true;
                                        //  item.InvoiceCode = item.InvoiceCode + "-D";
                                        await _repository.DeleteAsync(item);
                                        update = true;
                                        ListDetailInvoice.Add(new DetailInvoice()
                                        {
                                            code = item.EInvoiceCode,
                                            note = $"Xóa bỏ thành công",
                                            TypePublishEinvoice = ENumTypePublishEinvoice.XOADONTHANHCONG,
                                        });
                                        AddHistori(new HistoryEInvoice() { Carsher = Carsher, StatusEvent = StatusStaffEventEInvoice.XoaHoaDon, IdCarsher = IdCarsher, EInvoiceCode = item.EInvoiceCode, IdEInvoice = item.Id, Name = $"Xóa hóa đơn" });
                                    }
                                    else
                                    {
                                        ListDetailInvoice.Add(new DetailInvoice()
                                        {
                                            code = item.EInvoiceCode,
                                            note = $"Hóa đơn này đã tồn tại hóa đơn điện tử VNPT, vui lòng đồng bộ, mã lỗi: {pub}",
                                            TypePublishEinvoice = ENumTypePublishEinvoice.XOADONTHATBAI,
                                        });

                                    }
                                }
                                else
                                {
                                    //-----------------xem hóa đơn theo API HKD
                                    if (item.IdHoaDonHKD==null)
                                    {
                                        await UpdateStatusPublishInvoice(item.ComId, item.IdInvoice, item.InvoiceCode, EnumStatusPublishInvoiceOrder.NONE);
                                        item.IsDelete = true;
                                        item.InvoiceCode = item.InvoiceCode + "-D";
                                        update = true;
                                    }
                                    else
                                    {
                                        ListDetailInvoice.Add(new DetailInvoice()
                                        {
                                            code = item.EInvoiceCode,
                                            note = $"Hệ thống chưa hỗ trợ hóa đơn VNPT HKD",
                                            TypePublishEinvoice = ENumTypePublishEinvoice.XOADONTHATBAI,
                                        });


                                        //var gettoken = _hkdvnptrepository.GetTokenCache(ComId);
                                        //if (string.IsNullOrEmpty(gettoken))
                                        //{
                                        //    var login = await _hkdvnptrepository.Login(ComId, company.DomainName, company.UserNameAdmin, company.PassWordAdmin);
                                        //    if (!login.success)
                                        //    {
                                        //        ListDetailInvoice.Add(new DetailInvoice()
                                        //        {
                                        //            code = item.EInvoiceCode,
                                        //            note = $"{string.Join(",", login.errors?.ToArray())}",
                                        //            TypePublishEinvoice = ENumTypePublishEinvoice.XOADONTHATBAI,
                                        //        });
                                        //    }
                                        //    else
                                        //    {
                                        //        gettoken = login.Token;
                                        //    }
                                        //}
                                        //var callgetinv = await _hkdvnptrepository.XemHoaDon(company.DomainName, gettoken,0);


                                    }

                                }
                            }
                        }
                        else
                        {
                            await UpdateStatusPublishInvoice(item.ComId, item.IdInvoice, item.InvoiceCode, EnumStatusPublishInvoiceOrder.NONE);
                            // item.IsDelete = true;
                            //item.InvoiceCode = item.InvoiceCode + "-D";
                            await _repository.DeleteAsync(item);
                            update = true;
                            ListDetailInvoice.Add(new DetailInvoice()
                            {
                                code = item.EInvoiceCode,
                                note = $"Xóa bỏ thành công",
                                TypePublishEinvoice = ENumTypePublishEinvoice.XOADONTHANHCONG,
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        ListDetailInvoice.Add(new DetailInvoice()
                        {
                            code = item.EInvoiceCode,
                            note = $"Xóa hóa đơn điện tử thất bại, có lỗi hệ thống: {e.ToString()}",
                            TypePublishEinvoice = ENumTypePublishEinvoice.XOADONTHATBAI,
                        });
                        _log.LogError($"Lỗi conver dữ liệu Exception -> {e.ToString()}");
                    }
                }
                if (update)
                {
                    await _repository.UpdateRangeAsync(getall);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    _unitOfWork.Rollback();
                }
                var PublishInvoiceModelView = new PublishInvoiceModelView();
                PublishInvoiceModelView.DetailInvoices = ListDetailInvoice;
                PublishInvoiceModelView.TypeEventInvoice = EnumTypeEventInvoice.SycnEInvoice;
                return await Result<PublishInvoiceModelView>.SuccessAsync(PublishInvoiceModelView);
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                _log.LogError($"Lỗi conver dữ liệu Exception -> {e.ToString()}");
                return await Result<PublishInvoiceModelView>.FailAsync(e.ToString());
            }
        }

        public async Task<EInvoice> FindByIdAsync(int id, bool asNotracking = false)
        {
            if (asNotracking)
            {
                var getall = _repository.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            }
            return await _repository.GetByIdAsync(id);
        }
        public async Task<IResult<string>> GetHashTokenVNPTAsync(int[] lstid, int Comid)
        {
            _log.LogInformation($"Lấy hash"+ Comid);
            if (Comid == 0 || lstid == null)
            {
                return await Result<string>.FailAsync(HeperConstantss.ERR000);
            }
            var getall = _repository.Entities.Where(x => lstid.Contains(x.Id) && x.ComId == Comid && x.StatusEinvoice==StatusEinvoice.SignedInv).ToList().GroupBy(x => x.TypeSupplierEInvoice);
            if (getall.Count() == 0)
            {
                _log.LogError($"Không tìm thấy hóa đơn gửi CQT");
                return await Result<string>.FailAsync(HeperConstantss.ERR012);
            }
            if (getall.Count() > 1)
            {
                _log.LogError($"Vui lòng chọn hóa đơn của một nhà cung cấp để phát hành, để không xảy ra sai sót!");
                return await Result<string>.FailAsync("Vui lòng chọn hóa đơn của một nhà cung cấp để phát hành, để không xảy ra sai sót!");
            }
            var getFirstEinvoice = getall.FirstOrDefault().FirstOrDefault();
            var company = await _supplierEInvoicerepository.GetByIdAsync(Comid, getFirstEinvoice.TypeSupplierEInvoice);
            if (company==null)
            {
                _log.LogError($"Công ty chưa cấu hình kết nối hóa đơn điện tử");
                return await Result<string>.FailAsync(HeperConstantss.ERR012);
            }
            if (string.IsNullOrEmpty(company.SerialCert))
            {
                _log.LogError($"Công ty chưa có chứng thư");
                return await Result<string>.FailAsync(HeperConstantss.ERR012);
            }
            var getlstfkey = getall.FirstOrDefault().Select(x => x.FkeyEInvoice).ToArray();//lấy fkey
            string fkeys = string.Join("_", getlstfkey);

            string getVNPT = await _vnptbusinessrepository.GetHashInvMTTFkeyByTokenAsync(company.UserNameAdmin, company.PassWordAdmin, fkeys, company.UserNameService, company.PassWordService, getFirstEinvoice.Pattern, getFirstEinvoice.Serial, company.SerialCert, company.DomainName);
            if (getVNPT.Contains(CommonERREinvoice.ERR))
            {
                return await Result<string>.FailAsync(GeneralMess.GeneralMessStartPublishEInvoice(getVNPT));
            }
            _log.LogInformation("Lấy hash thành công" + Comid);
            return await Result<string>.SuccessAsync(getVNPT,HeperConstantss.SUS006);
        }
        public async Task<IResult<PublishInvoiceModelView>> SendCQTTokenAsync(int[] lstid, string dataXml, int Comid, string Carsher, string IdCarsher)
        {
            List<DetailInvoice> ListDetailInvoice = new List<DetailInvoice>();
            var PublishInvoiceModelView = new PublishInvoiceModelView();
            var getAll = await _repository.Entities.Where(x => lstid.Contains(x.Id) && x.ComId == Comid && x.StatusEinvoice == StatusEinvoice.SignedInv).ToListAsync();
            if (getAll.Count()==0)
            {
                _log.LogError($"Không tìm thấy hóa đơn gửi CQT");
                return await Result<PublishInvoiceModelView>.FailAsync("Không tìm thấy hóa đơn gửi CQT");
            }
            var getfirst = getAll.First();
            var  company = await _supplierEInvoicerepository.GetByIdAsync(Comid, getfirst.TypeSupplierEInvoice);

            //đoạn này là gửi lên thuế sau khi đã lấy hash và ký ở plugin rồi
            string getVNPT = await _vnptbusinessrepository.SendInvMTTFkeyByTokenAsync(company.UserNameAdmin, company.PassWordAdmin, dataXml, company.UserNameService, company.PassWordService, getfirst.Pattern, getfirst.Serial, company.DomainName);
            if (getVNPT.Contains(CommonOKEinvoice.OK))
            {
                getAll.ForEach(x =>
                {
                    if (x.StatusEinvoice== StatusEinvoice.SignedInv)
                    {
                        x.StatusEinvoice = StatusEinvoice.SentInv;
                    }
                });
                ListDetailInvoice.Add(new DetailInvoice()
                {
                    code = string.Join(" ", getAll.Select(x => x.EInvoiceCode).ToArray()),
                    note = $"Gửi hóa đơn cơ quan thành công, tổng <b>{getAll.Count()}</b> hóa đơn, nhà cung cấp {(LibraryCommon.GetDisplayNameEnum(company.TypeSupplierEInvoice))}",
                    TypePublishEinvoice = ENumTypePublishEinvoice.GUICQTTHANHCONG,
                });

                foreach (var inv in getAll)
                {
                    AddHistori(new HistoryEInvoice()
                    {
                        Carsher = Carsher,
                        StatusEvent = StatusStaffEventEInvoice.SendCQT,
                        IdCarsher = IdCarsher,
                        EInvoiceCode = inv.EInvoiceCode,
                        IdEInvoice = inv.Id,
                        Name = $"Gửi hóa đơn lên cơ quan thuế thành công mã giao dịch: {getVNPT.Split(':')[1]}"
                    });
                }

                await _repository.UpdateRangeAsync(getAll);
                await _unitOfWork.SaveChangesAsync();
                var getlstfkey = getAll.Select(x => x.FkeyEInvoice).ToArray();
                string fkeys = string.Join("_", getlstfkey);
                //check trạng thái cqt
                _log.LogInformation("cb lấy kq" + Comid);
                var t = new Thread(async () => await CheckStatusEInvoiceSendCQT(getlstfkey, company, getfirst.Pattern, Comid));
                t.Start();

            }
            else
            {
                foreach (var inv in getAll)
                {
                    AddHistori(new HistoryEInvoice()
                    {
                        Carsher = Carsher,
                        StatusEvent = StatusStaffEventEInvoice.SendCQT,
                        IdCarsher = IdCarsher,
                        EInvoiceCode = inv.EInvoiceCode,
                        IdEInvoice = inv.Id,
                        Name = $"Gửi hóa đơn lên cơ quan thuế thất bại"
                    });
                }
                ListDetailInvoice.Add(new DetailInvoice()
                {
                    code = string.Join("_", getAll.Select(x => x.EInvoiceCode).ToArray()),
                    note = $"Gửi hóa đơn cơ quan thuế lỗi {getVNPT}",
                    TypePublishEinvoice = ENumTypePublishEinvoice.GUICQTLOI,
                });
                _log.LogError($"Gửi hóa đơn cơ quan thuế lỗi -> {getVNPT}");
            }
            PublishInvoiceModelView.DetailInvoices = ListDetailInvoice;
            return await Result<PublishInvoiceModelView>.SuccessAsync(PublishInvoiceModelView);
        }
        public async Task<IResult<PublishInvoiceModelView>>  SendCQTAsync(int[] lstid, int Comid, string Carsher, string IdCarsher)
        {
            //isTokenSmartCASend tức là nếu true là gửi thuế chứ k phải lấy hash nữa
            var PublishInvoiceModelView = new PublishInvoiceModelView();
            PublishInvoiceModelView.TypeEventInvoice = EnumTypeEventInvoice.SendCQT;
            List<DetailInvoice> ListDetailInvoice = new List<DetailInvoice>();
            SupplierEInvoice company = new SupplierEInvoice();
          
            var getall =  _repository.Entities.Where(x => lstid.Contains(x.Id) && x.ComId == Comid).ToList().GroupBy(x=>x.TypeSupplierEInvoice);
            if (getall.Count()==0)
            {
                _log.LogError($"Không tìm thấy hóa đơn gửi CQT");
                return await Result<PublishInvoiceModelView>.FailAsync("Không tìm thấy hóa đơn gửi CQT");
            }
            if (getall.Count()>1)
            {
                _log.LogError($"Không tìm thấy hóa đơn gửi CQT");
                return await Result<PublishInvoiceModelView>.FailAsync("Vui lòng chọn hóa đơn của một nhà cung cấp để phát hành, để không xảy ra sai sót!");
            }
            foreach (var item in getall)
            {
                company = await _supplierEInvoicerepository.GetByIdAsync(Comid, item.FirstOrDefault().TypeSupplierEInvoice);
                if (company == null)
                {
                    _log.LogError($"Công ty chưa cấu hình kết nối hóa đơn điện tử");
                    return await Result<PublishInvoiceModelView>.FailAsync(HeperConstantss.ERR012);
                }
                PublishInvoiceModelView.TypeSeri = company.TypeSeri;//gán loại ký số
                if (item.Select(x=>x.IdManagerPatternEInvoice).Distinct().Count()>1)
                {
                    _log.LogError($"Vui lòng chọn mẫu số ký hiệu");
                    return await Result<PublishInvoiceModelView>.FailAsync($"Vui lòng chọn mẫu số ký hiệu");
                }

                var getlstfkey = item.Select(x => x.FkeyEInvoice).ToArray();
                string fkeys = string.Join("_", getlstfkey);
                try
                {
                    var firstEivnoice = item.First();
                    if (company.TypeSeri==ENumTypeSeri.NONE)
                    {
                        _log.LogError($"Bạn chưa cấu hình chữ ký số");
                        return await Result<PublishInvoiceModelView>.FailAsync("Bạn chưa cấu hình chữ ký số");
                    }
                    if (company.TypeSeri==ENumTypeSeri.HSM)//dành cho ký HSM
                    {
                        string getVNPT = await _vnptbusinessrepository.SendInvMTTFkeyAsync(company.UserNameAdmin, company.PassWordAdmin, fkeys, company.UserNameService, company.PassWordService, firstEivnoice.Pattern, firstEivnoice.Serial, company.SerialCert, company.DomainName);
                        if (getVNPT.Contains(CommonOKEinvoice.OK))
                        {
                            item.ForEach(x => x.StatusEinvoice = StatusEinvoice.SentInv);
                            ListDetailInvoice.Add(new DetailInvoice()
                            {
                                code = string.Join(" ", item.Select(x => x.EInvoiceCode).ToArray()),
                                note = $"Gửi hóa đơn cơ quan thành công, tổng <b>{getlstfkey.Count()}</b> hóa đơn, nhà cung cấp {(company.TypeSupplierEInvoice == ENumSupplierEInvoice.VNPT ? "VNPT" : "")}",
                                TypePublishEinvoice = ENumTypePublishEinvoice.GUICQTTHANHCONG,
                            });

                            foreach (var inv in item)
                            {
                                AddHistori(new HistoryEInvoice()
                                {
                                    Carsher = Carsher,
                                    StatusEvent = StatusStaffEventEInvoice.SendCQT,
                                    IdCarsher = IdCarsher,
                                    EInvoiceCode = inv.EInvoiceCode,
                                    IdEInvoice = inv.Id,
                                    Name = $"Gửi hóa đơn lên cơ quan thuế thành công mã giao dịch: {getVNPT.Split(':')[1]}"
                                });
                            }

                            await _repository.UpdateRangeAsync(item);
                            await _unitOfWork.SaveChangesAsync();

                            //check trạng thái cqt
                            _log.LogInformation("cb lấy kq" + Comid);
                            var t = new Thread(async () => await CheckStatusEInvoiceSendCQT(getlstfkey, company, firstEivnoice.Pattern, Comid));
                            t.Start();

                        }
                        else
                        {
                            foreach (var inv in item)
                            {
                                AddHistori(new HistoryEInvoice()
                                {
                                    Carsher = Carsher,
                                    StatusEvent = StatusStaffEventEInvoice.SendCQT,
                                    IdCarsher = IdCarsher,
                                    EInvoiceCode = inv.EInvoiceCode,
                                    IdEInvoice = inv.Id,
                                    Name = $"Gửi hóa đơn lên cơ quan thuế thất bại"
                                });
                            }
                            ListDetailInvoice.Add(new DetailInvoice()
                            {
                                code = string.Join("_", item.Select(x => x.EInvoiceCode).ToArray()),
                                note = $"Gửi hóa đơn cơ quan thuế lỗi {getVNPT}",
                                TypePublishEinvoice = ENumTypePublishEinvoice.GUICQTLOI,
                            });
                            _log.LogError($"Gửi hóa đơn cơ quan thuế lỗi -> {getVNPT}");
                        }
                    }
                    else
                    {
                        ListDetailInvoice.Add(new DetailInvoice()
                        {
                            code = string.Join("_", item.Select(x => x.EInvoiceCode).ToArray()),
                            note = $"Gửi hóa đơn cơ quan thuế lỗi, không đúng chữ ký số",
                            TypePublishEinvoice = ENumTypePublishEinvoice.GUICQTLOI,
                        });
                        _log.LogError($"Gửi hóa đơn cơ quan thuế lỗi ->  không đúng chữ ký số");
                    }
                   
                }
                catch (Exception e)
                {
                    ListDetailInvoice.Add(new DetailInvoice()
                    {
                        code = string.Join("_", item.Select(x => x.EInvoiceCode).ToArray()),
                        note = $"Gửi hóa đơn cơ quan thuế lỗi {e.Message}",
                        TypePublishEinvoice = ENumTypePublishEinvoice.GUICQTLOI,
                    });
                    _log.LogError($"Gửi hóa đơn cơ quan thuế lỗi -> {e.Message}");
                }
            }
            PublishInvoiceModelView.DetailInvoices = ListDetailInvoice;
            return await Result<PublishInvoiceModelView>.SuccessAsync(PublishInvoiceModelView);

        }
        private async Task CheckStatusEInvoiceSendCQT(string[] lstid, SupplierEInvoice company, string pattern, int Comid)
        {
            try
            {
                Thread.Sleep(30000);//chờ 30s đã gọi
                _log.LogInformation("Băt đầu lấy kq");
               // if (lstid.Count() > 100)
               // {
                    var newlst = Common.Split(lstid, 2);
                    foreach (var items in newlst)
                    {
                        string fkeys = string.Join(";", items);
                        string getVNPT = await _vnptrepository.GetMCCQThueByFkeys(company.UserNameAdmin, company.PassWordAdmin, company.UserNameService, company.PassWordService, pattern, fkeys, company.DomainName);
                        if (getVNPT.Contains("ERR:"))
                        {
                            _log.LogError("Đồng bộ lấy trạng thái thất bại " + getVNPT + "__" + fkeys);
                        }
                        else
                        {
                            var result = GenXMLToModelDSHDon(getVNPT);
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                                string[] fkeysres = result.HDons.Select(x => x.Fkey).ToArray();
                                bool update = false;
                                var getall = await db.EInvoice.Where(x => fkeysres.Contains(x.FkeyEInvoice) && x.ComId == Comid).ToListAsync();
                                var lstHistoryEInvoice = new List<HistoryEInvoice>();
                                foreach (var item in getall)
                                {
                                    var getfkey = result.HDons.SingleOrDefault(x => x.Fkey == item.FkeyEInvoice);
                                    if (getfkey != null)
                                    {
                                        switch (getfkey.Status)
                                        {
                                            case ENumTypeStausSendCQT.CHUAGUI_CQT:
                                                break;
                                            case ENumTypeStausSendCQT.DAGUI_CQT:
                                                item.StatusEinvoice = StatusEinvoice.SentInv;
                                                // update = true;
                                                break;
                                            case ENumTypeStausSendCQT.CQT_CHAPNHAN:
                                                item.StatusEinvoice = StatusEinvoice.AcceptedInv;
                                                lstHistoryEInvoice.Add(new HistoryEInvoice()
                                                {
                                                    Carsher = "Hệ thống",
                                                    StatusEvent = StatusStaffEventEInvoice.SendCQT,
                                                    IdCarsher = string.Empty,
                                                    EInvoiceCode = item.EInvoiceCode,
                                                    IdEInvoice = item.Id,
                                                    Name = $"Hóa đơn đã được cơ quan thuế chấp nhận"
                                                });

                                                update = true;
                                                break;
                                            case ENumTypeStausSendCQT.CQT_TUCHOI:
                                                item.StatusEinvoice = StatusEinvoice.RejectedInv;
                                                lstHistoryEInvoice.Add(new HistoryEInvoice()
                                                {
                                                    Carsher = "Hệ thống",
                                                    StatusEvent = StatusStaffEventEInvoice.SendCQT,
                                                    IdCarsher = string.Empty,
                                                    EInvoiceCode = item.EInvoiceCode,
                                                    IdEInvoice = item.Id,
                                                    Name = $"Hóa đơn bị cơ quan thuế từ chối: lỗi {getfkey.MTLoi}"
                                                });

                                                update = true;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                if (update)
                                {
                                    await db.HistoryEInvoice.AddRangeAsync(lstHistoryEInvoice);
                                    db.UpdateRange(getall);
                                    await db.SaveChangesAsync();
                                }
                                db.Dispose();
                            }
                            _log.LogError("Đồng bộ lấy trạng thái thất bại " + getVNPT + "__" + fkeys);
                        }
                    }
               // }
                _log.LogInformation("Lấy kqxong");
            }
            catch (Exception e)
            {
                _log.LogInformation("Lấy lỗi");
                _log.LogInformation(e.ToString());
            }

        }
      
        private DSHDon GenXMLToModelDSHDon(string base64)
        {
            string xml = Common.Base64Decode(base64);
            var DSHDon = ConvertSupport.ConvertXMLToModel<DSHDon>(xml);
            return DSHDon;
        }

        public async Task<DashboardEInvoiceModel> GetDashboardAsync(int Comid,DateTime date)
        {
            var getall = await _repository.Entities.Where(x => x.ComId == Comid && x.CreatedOn.Date == date.Date).AsNoTracking().ToListAsync();
            int daphathanh = getall.Count(x=>x.StatusEinvoice== StatusEinvoice.SignedInv);
            int daguithue = getall.Count(x => x.StatusEinvoice == StatusEinvoice.SentInv);
            int chuaguithue = getall.Count(x => x.StatusEinvoice == StatusEinvoice.UnSendInv);
            int thuechapnhan = getall.Count(x => x.StatusEinvoice == StatusEinvoice.AcceptedInv);
            int thuetuchoi = getall.Count(x => x.StatusEinvoice == StatusEinvoice.ReplacedInv);
            DashboardEInvoiceModel model = new DashboardEInvoiceModel()
            {
                SignedInv = daphathanh,
                SentInv = daguithue,
                UnSendInv = chuaguithue,
                AcceptedInv = thuechapnhan,
                RejectedInv = thuetuchoi,
            };
            return model;
        }

        public async Task<List<EInvoice>> GetReportMonth(DateTime todate, DateTime enddate, int ComId)
        {
           return await _repository.Entities.Where(x=>x.ComId==ComId && x.CreatedOn >= todate && x.CreatedOn<enddate && x.InvoiceNo>0).ToListAsync();
        }
        public async Task<List<ReportMonthProductEInvoice>> GetReportMonthProduct(DateTime todate, DateTime enddate, int ComId)
        {
          
            var InvoiceData = await _repository.Entities.Where(x => x.ComId == ComId && x.CreatedOn >= todate && x.CreatedOn < enddate && x.InvoiceNo > 0)
                .Join(_einvoiceitemrepository.Entities, inv => inv.Id, einv => einv.IdInvoice, (invoice, einvitem) => new ReportMonthProductEInvoice()
                {
                    Patern = invoice.Pattern,
                    Serial = invoice.Serial,
                    InvoiceNo = invoice.InvoiceNo,
                    Status = invoice.StatusEinvoice,
                    SignDate = invoice.CreatedOn,
                    Buyer = invoice.Buyer,
                    CusName = invoice.CusName,
                    Address = invoice.Address,
                    TaxCode = invoice.CusTaxCode,
                    Payment = invoice.PaymentMethod,
                    ProductName = einvitem.ProName,
                    ProductCode = einvitem.ProCode,
                    ProductQuantity = einvitem.Quantity,
                    ProductPrice = einvitem.Price,
                    ProductUnit = einvitem.Unit,
                    ProductTotal = einvitem.Total,
                    ProductVATAmount = einvitem.VATAmount,
                    ProductVATRate = einvitem.VATRate,
                }).ToListAsync();

            return InvoiceData;
        }

        public async Task SendCQTAutoAsync(List<HistoryAutoSendTimer> history,int[] lstPattern, int Comid, ENumSupplierEInvoice SupplierEInvoice)
        {
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                var getdata = await _repository.Entities.Where(x => x.ComId == Comid && x.TypeSupplierEInvoice == SupplierEInvoice && lstPattern.Contains(x.IdManagerPatternEInvoice) && x.InvoiceNo > 0 && (x.StatusEinvoice == StatusEinvoice.SignedInv || x.StatusEinvoice == StatusEinvoice.ReplacedInv || x.StatusEinvoice == StatusEinvoice.AdjustedInv)).ToListAsync();
                SupplierEInvoice company = await _supplierEInvoicerepository.GetByIdAsync(Comid, SupplierEInvoice);
                if (company == null)
                {
                    _log.LogError($"Công ty chưa cấu hình kết nối hóa đơn điện tử");
                }
                else
                {

                    if (company.TypeSeri != ENumTypeSeri.HSM)
                    {
                        _log.LogError($"Chữ ký số công ty không phải là HSM , không thể gửi hóa đơn lên CQT");
                    }
                    else
                    {
                        bool update = false;
                        var groupByPattern = await getdata.GroupBy(x => x.IdManagerPatternEInvoice).ToListAsync();
                        List<HistoryEInvoice> HistoryEInvoice = new List<HistoryEInvoice>();
                        foreach (var item in groupByPattern)
                        {
                            var getlstfkey = item.Select(x => x.FkeyEInvoice).ToArray();
                            string fkeys = string.Join("_", getlstfkey);
                            try
                            {
                                string getVNPT = await _vnptbusinessrepository.SendInvMTTFkeyAsync(company.UserNameAdmin, company.PassWordAdmin, fkeys, company.UserNameService, company.PassWordService, item.First().Pattern, item.First().Serial, company.SerialCert, company.DomainName);
                                if (getVNPT.Contains(CommonOKEinvoice.OK))
                                {
                                    item.ForEach(x => x.StatusEinvoice = StatusEinvoice.SentInv);
                                    update = true;
                                    foreach (var inv in item)
                                    {
                                        HistoryEInvoice.Add(new HistoryEInvoice()
                                        {
                                            Carsher = "Tự động",
                                            StatusEvent = StatusStaffEventEInvoice.SendCQT,
                                            IdCarsher = "",
                                            EInvoiceCode = inv.EInvoiceCode,
                                            IdEInvoice = inv.Id,
                                            Name = $"Gửi hóa đơn lên cơ quan thuế thành công mã giao dịch: {getVNPT.Split(':')[1]}"
                                        });
                                    }
                                    history.Add(new HistoryAutoSendTimer() { Name = $"Gửi hóa đơn cơ quan thuế thành công mẫu số ký hiệu: {item.First().Pattern}-{item.First().Serial}, tổng {item.Count()} hóa đơn" });
                                }
                                else
                                {
                                    _log.LogError($"Gửi hóa đơn lên CQT thất bại, lỗi nhà cung cấp: {getVNPT}");
                                    history.Add(new HistoryAutoSendTimer() { Name = $"Gửi hóa đơn cơ quan thuế thất bại mẫu số ký hiệu: {item.First().Pattern}-{item.First().Serial}, tổng {item.Count()} hóa đơn", Error = getVNPT });
                                }
                                if (update)
                                {
                                    await _repository.UpdateRangeAsync(item);
                                }
                            }
                            catch (Exception e)
                            {
                                _log.LogError($"Gửi hóa đơn lên CQT thất bại, lỗi Exception" + e.ToString());
                            }

                        }
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitAsync();
                    }
                }
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                _log.LogError($"Lỗi trong quá trình lưu",e.ToString());
            }
        }

        public async Task<IResult<PublishInvoiceModelView>> PublishInvoiceByTokenVNPTAsync(int Comid,ENumSupplierEInvoice SupplierEInvoice, string serial, string pattern, string dataxml, string IdCasher, string CasherName)
        {
            PublishInvoiceModelView publishInvoiceModelView =   new PublishInvoiceModelView();
            SupplierEInvoice company = await _supplierEInvoicerepository.GetByIdAsync(Comid, SupplierEInvoice);
            if (company == null)
            {
               return  Result<PublishInvoiceModelView>.Fail(HeperConstantss.ERR019);
            }
            List<DetailInvoice> ListDetailInvoice = new List<DetailInvoice>();
            var publish = await _vnptrepository.PublishInvWithTokenAsync(company.DomainName,dataxml,company.UserNameService, company.PassWordService, company.UserNameAdmin, company.PassWordAdmin, pattern,serial);
            if (publish.Contains("OK:"))
            {
                string[] data = publish.Split("-");
                string[] datalisteinv = data[1].Split(",");
                try
                {
                    List<string> getlstfkey = new List<string>();
                    foreach (var item in datalisteinv)
                    {
                        string[] newdata = item.Split("_");
                        string fkey = newdata[0];
                        string invoiceno = newdata[1];
                        var einv = await _repository.Entities.SingleOrDefaultAsync(x => x.ComId == Comid && x.FkeyEInvoice == fkey);
                        if (einv != null)
                        {
                            getlstfkey.Add(fkey);
                            einv.InvoiceNo = int.Parse(invoiceno);
                            einv.StatusEinvoice = StatusEinvoice.SignedInv;
                            einv.PublishDate = DateTime.Now;
                            await _repository.UpdateAsync(einv);
                            //xử lý bên invoice
                            var getupđateinvoice = await _Invoicerepository.GetByIdAsync(einv.IdInvoice);
                            getupđateinvoice.StatusPublishInvoiceOrder = EnumStatusPublishInvoiceOrder.PUBLISH;

                            ListDetailInvoice.Add(new DetailInvoice()
                            {
                                TypePublishEinvoice = ENumTypePublishEinvoice.PHATHANHOK,
                                code = getupđateinvoice.InvoiceCode,
                                note = $"Phát hành thành công hóa đơn điện tử, mẫu số {einv.Pattern}, ký hiệu {einv.Serial},số {einv.InvoiceNo.ToString("00000000")}",
                            });
                        }

                    }

                    //---đồng bộ mã cơ quan thuế
                    _log.LogInformation("đồng bộ lấy kq" + Comid);
                    var t = new Thread(async () => await CheckStatusEInvoiceSendCQT(getlstfkey.ToArray(), company, pattern, Comid));
                    t.Start();
                    //-------
                    publishInvoiceModelView.DetailInvoices = ListDetailInvoice;
                    publishInvoiceModelView.TypeEventInvoice = EnumTypeEventInvoice.PublishEInvoice;
                    await _unitOfWork.SaveChangesAsync();
                    return await Result<PublishInvoiceModelView>.SuccessAsync(publishInvoiceModelView);
                }
                catch (Exception e)
                {
                    _log.LogError(e.ToString());
                    return await Result<PublishInvoiceModelView>.FailAsync(e.Message);
                }
                
            }
            else
            {
                return await Result<PublishInvoiceModelView>.FailAsync(publish);
            }
        }
    }
}
