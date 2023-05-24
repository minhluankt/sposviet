using Application.Constants;
using Application.EInvoices.Interfaces.VNPT;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Providers;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using Domain.XmlDataModel;
using Hangfire.Logging;
using HelperLibrary;
using Infrastructure.Infrastructure.Identity.Models;
using Infrastructure.Webservice.Repository.VNPT;
using Library;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NStandard.Evaluators;
using Spire.License;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class InvoicePepository : IInvoicePepository<Invoice>
    {
        private readonly IVNPTPortalServiceRepository _vnptportalrepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRevenueExpenditureRepository<RevenueExpenditure> _revenueExpenditureRepository;
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly ILogger<InvoicePepository> _log;
        private readonly IRepositoryAsync<HistoryInvoice> _historyInvoiceRepository;
        private readonly IManagerInvNoRepository _managerInvNoRepository;
        private readonly IRepositoryAsync<Customer> _repositoryCusomer;
        private readonly IManagerPatternEInvoiceRepository<ManagerPatternEInvoice> _managerPatternEInvoicerepository;
        private UserManager<ApplicationUser> _userManager;
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IRepositoryAsync<Invoice> _invoiceRepository;
        private readonly IRepositoryAsync<OrderTable> _orderTableRepository;
        private readonly IEInvoiceRepository<EInvoice> _einvoiceRepository;
        private readonly ISupplierEInvoiceRepository<SupplierEInvoice> _supplierEInvoiceRepository;
        public InvoicePepository(IRepositoryAsync<Invoice> invoiceRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IManagerInvNoRepository managerInvNoRepository,
            IRepositoryAsync<OrderTable> orderTableRepository,
            ICustomerRepository customerRepository,
            IVNPTPortalServiceRepository vnptportalrepository,
            ISupplierEInvoiceRepository<SupplierEInvoice> supplierEInvoiceRepository,
            IRevenueExpenditureRepository<RevenueExpenditure> revenueExpenditureRepository,
            IManagerPatternEInvoiceRepository<ManagerPatternEInvoice> managerPatternEInvoicerepository,
            IRepositoryAsync<Customer> repositoryCusomer, IUnitOfWork unitOfWork,
            IEInvoiceRepository<EInvoice> einvoiceRepository, ILogger<InvoicePepository> log,
            UserManager<ApplicationUser> userManager, IRepositoryAsync<HistoryInvoice> historyInvoiceRepository,
            IOptions<CryptoEngine.Secrets> config)
        {
            _log = log;
            _orderTableRepository = orderTableRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _vnptportalrepository = vnptportalrepository;
            _customerRepository = customerRepository;
            _revenueExpenditureRepository = revenueExpenditureRepository;
            _supplierEInvoiceRepository = supplierEInvoiceRepository;
            _managerInvNoRepository = managerInvNoRepository;
            _einvoiceRepository = einvoiceRepository;
            _managerPatternEInvoicerepository = managerPatternEInvoicerepository;
            _historyInvoiceRepository = historyInvoiceRepository;
            _repositoryCusomer = repositoryCusomer;
            _userManager = userManager;
            _config = config;
            _unitOfWork = unitOfWork;
            _invoiceRepository = invoiceRepository;
        }
        public async Task<PaginatedList<Invoice>> GetAllDatatableAsync(int? Comid, InvoiceModel textSearch, string sortColumn, string sortColumnDirection, int pageSize, int skip, EnumTypeProduct enumTypeProduct = EnumTypeProduct.THOITRANG)
        {
            var datalist = _invoiceRepository.Entities.AsNoTracking().Include(x => x.Customer).Where(x => x.TypeProduct == enumTypeProduct && x.Status != EnumStatusInvoice.XOA_BO);
            if (Comid != null)
            {
                datalist = datalist.Where(x => x.ComId == Comid);
            }
            if (!string.IsNullOrEmpty(textSearch.Code))
            {
                datalist = datalist.Where(m => m.InvoiceCode.ToLower().Contains(textSearch.Code.ToLower()));
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
            datalist = datalist.Select(x => new Invoice()
            {
                Id = x.Id,
                IdEInvoice = x.IdEInvoice,
                IdGuid = x.IdGuid,
                InvoiceCodePatern = x.InvoiceCodePatern,
                InvoiceCode = x.InvoiceCode,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                IsBringBack = x.IsBringBack,
                IsMerge = x.IsMerge,
                IsDeleteMerge = x.IsDeleteMerge,
                Buyer = x.Buyer,
                CusName = x.CusName,
                Total = x.Total,
                VATAmount = x.VATAmount,
                VATRate = x.VATRate,
                Status = x.Status,
                DiscountAmount = x.DiscountAmount,
                DiscountOther = x.DiscountOther,
                TableNameArea = x.TableNameArea,
                StatusPublishInvoiceOrder = x.StatusPublishInvoiceOrder,
                Amonut = x.Amonut
            });
            var data = await PaginatedList<Invoice>.ToPagedListAsync(datalist, textSearch.Currentpage, pageSize);
            data.Items.ForEach(x =>
            {
                if (x.IdCustomer != null)
                {
                    x.Buyer = _repositoryCusomer.GetByIdAsync(x.IdCustomer.Value).Result?.Name;
                }
                var values = "id=" + x.IdGuid;
                var valuesEinvoice = "id=" + x.IdEInvoice;
                var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                var secretEinvoice = CryptoEngine.Encrypt(valuesEinvoice, _config.Value.Key);
                x.Secret = secret;
                x.secretEinvoice = secretEinvoice;
                x.CasherName = _userManager.FindByIdAsync(x.CreatedBy).Result?.FullName;
            });
            return data;
        }

        public async Task<Result<PublishInvoiceModelView>> CancelInvoice(Guid IdInvoice, int ComId, string CasherName, string Note, EnumTypeEventInvoice TypeEventInvoice)
        {
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                var _invoice = await _invoiceRepository.Entities.Where(x => x.IdGuid == IdInvoice && x.ComId == ComId).SingleOrDefaultAsync();
                if (_invoice == null)
                {
                    return await Result<PublishInvoiceModelView>.FailAsync("Không tìm thấy đơn để hủy!");
                }
                if (_invoice.StatusPublishInvoiceOrder == EnumStatusPublishInvoiceOrder.CREATE || _invoice.StatusPublishInvoiceOrder == EnumStatusPublishInvoiceOrder.PUBLISH)
                {
                    return await Result<PublishInvoiceModelView>.FailAsync("Đơn đã tạo hóa đơn điện tử không thể hủy!");
                }
                var his = new HistoryInvoice();
                his.IdInvoice = _invoice.Id;
                his.InvoiceCode = _invoice.InvoiceCode;
                his.Carsher = CasherName;
                if (TypeEventInvoice == EnumTypeEventInvoice.Cancel)
                {
                    _invoice.Status = EnumStatusInvoice.HUY_BO;
                    _invoice.Note = $"{Note}, Thời gian {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}";
                    his.Name = $"Đã hủy hóa đơn bởi {CasherName}, lý do: {Note}";

                }
                else if (TypeEventInvoice == EnumTypeEventInvoice.Restore)
                {
                    _invoice.Status = EnumStatusInvoice.DA_THANH_TOAN;
                    his.Name = $"Đã khôi phục lại hóa đơn bởi {CasherName}, lý do: {Note}";
                }
                else
                {
                    return await Result<PublishInvoiceModelView>.FailAsync(HeperConstantss.ERR042);
                }
                //add his
                await _historyInvoiceRepository.AddAsync(his);

                await _invoiceRepository.UpdateAsync(_invoice);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return Result<PublishInvoiceModelView>.Success(HeperConstantss.SUS006);
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                return await Result<PublishInvoiceModelView>.FailAsync(e.Message);
            }
        }

        public async Task<Result<PublishInvoiceModelView>> PublishInvoice(PublishInvoiceModel model)
        {
            //await _unitOfWork.CreateTransactionAsync();
            try
            {
                var _invoice = await _invoiceRepository.Entities.Where(x => model.lstid.Contains(x.Id)).Include(x => x.InvoiceItems).Include(x => x.Customer).Include(x => x.PaymentMethod).AsNoTracking().ToListAsync();
                if (_invoice.Count() == 0)
                {
                    return await Result<PublishInvoiceModelView>.FailAsync(HeperConstantss.ERR012);
                }

                PublishInvoiceModelView lstketqua = new PublishInvoiceModelView();//kết quả
                List<Invoice> lstinvoicecancel = new List<Invoice>();//đã hủy
                List<Invoice> lstinvoiceok = new List<Invoice>();//ok để phát  hành
                List<Invoice> lstinvoicefail = new List<Invoice>();//danh sách k phù hợp, k có sản phẩm hoặc trạng thái k đủ điều kiện phát hành
                List<EInvoice> lsteinvoice = new List<EInvoice>();
                foreach (var item in _invoice)
                {
                    if (item.IsMerge)
                    {
                        lstinvoicefail.Add(item);
                    }
                    else if (item.StatusPublishInvoiceOrder != EnumStatusPublishInvoiceOrder.NONE)
                    {
                        lstinvoicefail.Add(item);
                    }
                    else
                    {
                        if (item.Status == EnumStatusInvoice.HUY_BO || item.Status == EnumStatusInvoice.HOAN_TIEN)
                        {
                            lstinvoicecancel.Add(item);
                        }
                        else if (item.Status == EnumStatusInvoice.DA_THANH_TOAN || item.Status == EnumStatusInvoice.HOAN_TIEN_MOT_PHAN)
                        {
                            if (item.InvoiceItems.Count() == 0)
                            {
                                lstinvoicefail.Add(item);
                            }
                            else
                            {
                                lstinvoiceok.Add(item);
                            }
                        }
                        else
                        {
                            lstinvoicefail.Add(item);
                        }
                    }
                }
                var getpattern = await _managerPatternEInvoicerepository.GetbyIdAsync(model.IdManagerPatternEInvoice, true);
                if (getpattern == null)
                {
                    return await Result<PublishInvoiceModelView>.FailAsync(HeperConstantss.ERR049);
                }
                foreach (var item in lstinvoiceok)
                {
                    this.AddEInvoice(lsteinvoice, item, model, getpattern);
                }
                List<DetailInvoice> ListDetailInvoice = new List<DetailInvoice>();
                var suplcompany = new SupplierEInvoice();
                //tạo và phát hành hóa đơn
                foreach (var item in lsteinvoice)
                {
                    //await  _unitOfWork.CreateTransactionAsync();
                    try
                    {
                        var getinvno = await _managerInvNoRepository.UpdateInvNo(item.ComId, ENumTypeManagerInv.EInvoice, false);
                        var getupđateinvoice = await _invoiceRepository.GetByIdAsync(item.IdInvoice);

                        if (model.TypeEventInvoice == EnumTypeEventInvoice.PublishEInvoice)
                        {
                            getupđateinvoice.StatusPublishInvoiceOrder = EnumStatusPublishInvoiceOrder.PUBLISH;
                        }
                        else
                        {
                            getupđateinvoice.StatusPublishInvoiceOrder = EnumStatusPublishInvoiceOrder.CREATE;
                        }


                        item.EInvoiceCode = $"EINV{getinvno.ToString("00000000")}";

                        await _einvoiceRepository.CreateAsync(item, model.CasherName, model.IdCarsher);
                        getupđateinvoice.IdEInvoice = item.Id;
                        if (item.VATRate != getupđateinvoice.VATRate)//nếu bên gốc invoice mà khác thì update lại
                        {
                            //tạm đóng lại đã
                            //getupđateinvoice.VATRate = item.VATRate;
                            //getupđateinvoice.VATAmount = item.VATAmount;
                            //getupđateinvoice.Amonut = item.Amount;
                        }


                        //await _unitOfWork.SaveChangesAsync();
                        //await _unitOfWork.CommitAsync();
                        //phát hành hóa đơn diện tử
                        if (model.TypeEventInvoice == EnumTypeEventInvoice.PublishEInvoice)
                        {
                            try
                            {
                                if (item.TypeSupplierEInvoice != suplcompany.TypeSupplierEInvoice)
                                {
                                    suplcompany = await _supplierEInvoiceRepository.GetByIdAsync(item.ComId, item.TypeSupplierEInvoice);
                                }
                                if (suplcompany == null)
                                {
                                    ListDetailInvoice.Add(new DetailInvoice()
                                    {
                                        TypePublishEinvoice = ENumTypePublishEinvoice.KHONGTONTAINHACUNGCAP,
                                        code = getupđateinvoice.InvoiceCode,
                                        note = $"Chưa cấu hình nhà cung cấp",
                                    });
                                }
                                else
                                {
                                    var publish = await _einvoiceRepository.ImportAndPublishInvMTTAsync(item, suplcompany, model.CasherName, model.IdCarsher);
                                    if (publish.Succeeded)
                                    {
                                        // loại 1 OK:1/001;C23MMT-ABCDEF1000000000t300u6_6_M1-23-41849-00100000006
                                        // loai 2 OK:1/001;C23MMT-ABCDEF10gfg00000000t300u6_7_M1-23-41849-00100000007,ABCDEF10gfg00000000t300pu6_8_M1-23-41849-00100000008
                                        string[] getinvoice = publish.Data.Split(';');
                                        if (getinvoice.Count() < 2)
                                        {
                                            ListDetailInvoice.Add(new DetailInvoice()
                                            {
                                                TypePublishEinvoice = ENumTypePublishEinvoice.TAOMOIPHATHANHLOI,
                                                code = getupđateinvoice.InvoiceCode,
                                                token = publish.Data,
                                                note = $"Tạo mới thành công hóa hóa đơn điện tử, phát hành thất bại, mã lỗi từ nhà cung cấp: {publish.Data}",
                                            });
                                        }
                                        else
                                        {
                                            string[] getinvoicesplit = getinvoice[1].Split('_');
                                            string invoiceno = getinvoicesplit[1];
                                            string MCQT = getinvoicesplit[2];
                                            item.MCQT = MCQT;
                                            item.InvoiceNo = int.Parse(invoiceno);
                                            item.StatusEinvoice = StatusEinvoice.SignedInv;
                                            item.PublishDate = DateTime.Now;

                                            await _einvoiceRepository.UpdateAsync(item);
                                            await _unitOfWork.SaveChangesAsync();

                                            ListDetailInvoice.Add(new DetailInvoice()
                                            {
                                                TypePublishEinvoice = ENumTypePublishEinvoice.PHATHANHOK,
                                                code = getupđateinvoice.InvoiceCode,
                                                note = $"Phát hành thành công hóa đơn điện tử, mẫu số {item.Pattern}, ký hiệu {item.Serial},số {item.InvoiceNo.ToString("00000000")}",
                                            });
                                        }
                                    }
                                    else
                                    {
                                        ListDetailInvoice.Add(new DetailInvoice()
                                        {
                                            TypePublishEinvoice = ENumTypePublishEinvoice.TAOMOIPHATHANHLOI,
                                            code = getupđateinvoice.InvoiceCode,
                                            note = $"Tạo mới thành công hóa hóa đơn điện tử mã hóa đơn:{item.EInvoiceCode}, phát hành thất bại, mã lỗi từ nhà cung cấp: {publish.Message}",
                                        });
                                    }
                                }


                            }
                            catch (Exception e)
                            {
                                _log.LogError($"Tạo mới thành công hóa hóa đơn điện tử nhưng không phát hành được  fkey {item.Fkey} mã lỗi Exception");
                                _log.LogError(e.ToString());
                                ListDetailInvoice.Add(new DetailInvoice()
                                {
                                    TypePublishEinvoice = ENumTypePublishEinvoice.TAOMOIPHATHANHLOI,
                                    code = getupđateinvoice.InvoiceCode,
                                    note = $"Tạo mới thành công hóa hóa đơn điện tử mã Fkey:{item.Fkey} nhưng không phát hành được mã lỗi hệ thống {e.ToString()}",
                                });

                            }

                        }
                        else
                        {
                            ListDetailInvoice.Add(new DetailInvoice()
                            {
                                TypePublishEinvoice = ENumTypePublishEinvoice.TAOMOIOK,
                                code = getupđateinvoice.InvoiceCode,
                                note = $"Đã tạo mới hóa đơn điện tử thành công",
                            });
                        }


                    }
                    catch (Exception e)
                    {
                        _log.LogError("xử ly phát hành hóa đơn" + item.InvoiceCode);
                        _log.LogError(e.ToString());
                        ListDetailInvoice.Add(new DetailInvoice()
                        {
                            TypePublishEinvoice = ENumTypePublishEinvoice.ERROR,
                            code = item.InvoiceCode,
                            note = $"Dữ liệu lỗi không thể xử lý hóa đơn điện tử",
                        });
                    }


                }
                foreach (var item in lstinvoicecancel)
                {
                    ListDetailInvoice.Add(new DetailInvoice()
                    {
                        TypePublishEinvoice = ENumTypePublishEinvoice.DONDAHUY,
                        code = item.InvoiceCode,
                        note = $"Mã đơn đã được hủy không thể phát hành hóa đơn điện tử",
                    });
                }
                foreach (var item in lstinvoicefail)
                {
                    if (item.IsMerge)
                    {
                        ListDetailInvoice.Add(new DetailInvoice()
                        {
                            TypePublishEinvoice = ENumTypePublishEinvoice.PHATHANHLOI,
                            code = item.InvoiceCode,
                            note = $"Mã đơn này đã phát hành gộp cho hóa đơn có mã {item.InvoiceCodePatern}",
                        });
                    }
                    else if(item.StatusPublishInvoiceOrder == EnumStatusPublishInvoiceOrder.CREATE)
                    {
                        ListDetailInvoice.Add(new DetailInvoice()
                        {
                            TypePublishEinvoice = ENumTypePublishEinvoice.DAPHATHANHROI,
                            code = item.InvoiceCode,
                            note = $"Mã đơn này đã tồn tại hóa đơn điện tử trạng thái tạo mới",
                        });
                    }
                    else if (item.StatusPublishInvoiceOrder == EnumStatusPublishInvoiceOrder.PUBLISH)
                    {
                        ListDetailInvoice.Add(new DetailInvoice()
                        {
                            TypePublishEinvoice = ENumTypePublishEinvoice.DAPHATHANHROI,
                            code = item.InvoiceCode,
                            note = $"Mã đơn này đã được phát hành hóa đơn điện tử",
                        });
                    }
                    else if (item.InvoiceItems.Count() == 0)
                    {
                        ListDetailInvoice.Add(new DetailInvoice()
                        {
                            TypePublishEinvoice = ENumTypePublishEinvoice.KHONGCOSANPHAM,
                            code = item.InvoiceCode,
                            note = $"Mã đơn này không có sản phẩm không thể phát hành hóa đơn điện tử",
                        });
                    }
                    else
                    {
                        ListDetailInvoice.Add(new DetailInvoice()
                        {
                            TypePublishEinvoice = ENumTypePublishEinvoice.ERROR,
                            code = item.InvoiceCode,
                            note = $"Mã đơn này có trạng thái là {GeneralMess.GeneralMessEnumStatusInvoice(item.Status)}",
                        });
                    }

                }
                lstketqua.DetailInvoices = ListDetailInvoice;
                lstketqua.TypeEventInvoice = model.TypeEventInvoice;

                return Result<PublishInvoiceModelView>.Success(lstketqua);
            }
            catch (Exception e)
            {
                _log.LogError("Phát hành tạo mới hóa đơn điện tử lỗi");
                _log.LogError(e.ToString());
                return await Result<PublishInvoiceModelView>.FailAsync(e.Message);
            }
        }
        private void AddEInvoice(List<EInvoice> lsteinvoice, Invoice invoice, PublishInvoiceModel model, ManagerPatternEInvoice getpattern)
        {
            //int size = 8 - invoice.ComId.ToString().Length;
            var newmodel = new EInvoice()
            {
                FkeyEInvoice = KeyGenerator.GetUniqueKey(8) + invoice.ComId,
                InvoiceCode = invoice.InvoiceCode,
                IdCustomer = invoice.IdCustomer,
                TypeSupplierEInvoice = model.TypeSupplierEInvoice,
                IdManagerPatternEInvoice = getpattern.Id,
                IdInvoice = invoice.Id,
                ComId = invoice.ComId,
                CasherName = model.CasherName,
                PaymentMethod = invoice.PaymentMethod?.Name,
                Pattern = getpattern.Pattern,
                Serial = getpattern.Serial,
                ArisingDate = DateTime.Now,
                PublishDate = DateTime.Now,
                CurrencyUnit = "VND",
                Discount = invoice.Discount,
                DiscountAmount = invoice.DiscountAmount,
                DiscountOther = invoice.DiscountOther,
                StatusEinvoice = StatusEinvoice.NewInv,
                Total = invoice.Total,
            };

            if (invoice.Customer != null)
            {
                newmodel.Buyer = invoice.Customer.Buyer?.Trim();
                newmodel.CusName = invoice.Customer.Name?.Trim();
                newmodel.CusTaxCode = invoice.Customer.Taxcode?.Trim();
                newmodel.CusCode = invoice.Customer.Code?.Trim();
                newmodel.CusPhone = invoice.Customer.PhoneNumber?.Trim();
                newmodel.CCCD = invoice.Customer.CCCD?.Trim();
                newmodel.Address = invoice.Customer.Address?.Trim();
                newmodel.EmailDeliver = invoice.Customer.Email?.Trim();
                newmodel.CusBankNo = invoice.Customer.CusBankNo?.Trim();
                newmodel.CusBankName = invoice.Customer.CusBankName?.Trim();
            }
            else
            {
                newmodel.Buyer = invoice.Buyer?.Trim();
                newmodel.CusName = invoice.CusName?.Trim();
                newmodel.CusTaxCode = invoice.Taxcode?.Trim();
                newmodel.CusCode = invoice.CusCode?.Trim();
                newmodel.CusPhone = invoice.PhoneNumber?.Trim();
                newmodel.CCCD = invoice.CCCD?.Trim();
                newmodel.Address = invoice.Address?.Trim();
                newmodel.EmailDeliver = invoice.Email?.Trim();
                newmodel.CusBankNo = invoice.CusBankNo?.Trim();
                newmodel.CusBankName = invoice.CusBankName?.Trim();
            }
            //kiểm có có xuất hóa đơn GTGT không
            string gettypeinvoice = getpattern.Pattern.Split("/")[0];
            int typeinv = 0;
            bool isGTGT = int.TryParse(gettypeinvoice, out typeinv);
            //sẩn phẩm
            List<EInvoiceItem> eInvoiceItems = new List<EInvoiceItem>();
            foreach (var item in invoice.InvoiceItems)
            {
                var itemeinvoice = new EInvoiceItem()
                {
                    ProCode = item.Code,
                    ProName = item.Name,
                    Unit = item.Unit,
                    Quantity = item.Quantity,
                    Price = (item.VATRate.Value != (int)NOVAT.NOVAT&& item.PriceNoVAT>0) ? item.PriceNoVAT : item.Price,
                    Total = item.Total,
                    VATAmount = item.VATAmount,
                    VATRate = (item.VATRate.Value!= (int)NOVAT.NOVAT? item.VATRate.Value: (int)NOVAT.NOVAT),//để novat vì xuống dưới check
                    Amount = item.Amonut
                };
                if (item.Total > 0)
                {
                    itemeinvoice.IsSum = TCHHDVuLoai.HHoa;
                }
                else if (item.Total == 0)
                {
                    itemeinvoice.IsSum = TCHHDVuLoai.KMai;
                } 
                else
                {
                    itemeinvoice.IsSum = TCHHDVuLoai.CKhau;
                }
                if (isGTGT && (ENumTypeEInvoice)typeinv == ENumTypeEInvoice.GTGT)
                {
                    if (itemeinvoice.VATRate == (int)NOVAT.NOVAT && model.VATRate!= (int)NOVAT.NOVAT)//thèn sp k có thuế mà trư
                    {
                        itemeinvoice.VATRate = model.VATRate;
                        itemeinvoice.VATAmount = Math.Round(itemeinvoice.Total * Convert.ToDecimal((model.VATRate < 0 ? 0 : model.VATRate) / 100), MidpointRoundingCommon.Three);
                        itemeinvoice.Amount = itemeinvoice.Total + itemeinvoice.VATAmount;
                    }
                    else if (itemeinvoice.VATRate != model.VATRate)
                    {
                        throw new Exception($"Hàng hóa {itemeinvoice.ProName} có thuế suất là {itemeinvoice.VATRate}, nhưng bạn chọn thuế suất là {model.VATRate}, vui lòng điều chỉnh lại để đồng nhất thuế suất");
                    }
                } 
                else if(!isGTGT)
                {
                    itemeinvoice.VATRate = (int)VATRateInv.KHONGVAT;
                    itemeinvoice.VATAmount = 0;
                    itemeinvoice.Total = item.Amonut;
                    itemeinvoice.Amount = item.Amonut;
                }

                //if (isGTGT && (ENumTypeEInvoice)typeinv == ENumTypeEInvoice.GTGT && itemeinvoice.VATRate != model.VATRate &&  itemeinvoice.VATRate >(int)NOVAT.NOVAT)
                //{
                //    itemeinvoice.VATRate = model.VATRate;
                //    itemeinvoice.VATAmount = itemeinvoice.Total * Convert.ToDecimal((model.VATRate < 0 ? 0 : model.VATRate) / 100);
                //    itemeinvoice.Amount = itemeinvoice.Total + itemeinvoice.VATAmount;
                //}
                //else if(!isGTGT)
                //{
                //    itemeinvoice.VATRate = (int)VATRateInv.KHONGVAT;
                //    itemeinvoice.VATAmount = 0;
                //    itemeinvoice.Amount = item.Amonut;
                //}

                eInvoiceItems.Add(itemeinvoice);
            }
            newmodel.EInvoiceItems = eInvoiceItems;
            //update tiền và thuế
            if (model.VATRate != invoice.VATRate.Value && model.VATRate > (int)NOVAT.NOVAT && invoice.VATRate > (int)NOVAT.NOVAT)
            {
                throw new Exception($"Hóa đơn có thuế suất là {invoice.VATRate.Value}, không thế tính thuế suất {model.VATRate}");
            }
            if (model.VATRate != invoice.VATRate.Value && model.VATRate > (int)NOVAT.NOVAT && invoice.VATRate.Value == (int)NOVAT.NOVAT)
            {
                var checkprovat = invoice.InvoiceItems.Where(x=>x.VATRate!= (int)NOVAT.NOVAT).ToList();
                if (checkprovat.Count()>0)//th các sản phẩm giá đã có thuế thfi phải tính lại total tiền trucosw thuế khác
                {
                  
                    newmodel.VATRate = model.VATRate;
                    if (checkprovat.Count()== eInvoiceItems.Count())//tức là nếu tất cả sản phẩm đã có thuế thì k cần tính lại lấy nguyên giá trị qua
                    {
                        newmodel.Total = invoice.Total;
                        newmodel.VATAmount = Math.Round(eInvoiceItems.Sum(x=>x.VATAmount), MidpointRounding.AwayFromZero);
                        newmodel.Amount = eInvoiceItems.Sum(x=>x.Amount) - newmodel.DiscountOther;
                    }
                    else
                    {
                        newmodel.Total = Math.Round(invoice.InvoiceItems.ToList().Sum(x => x.Total), MidpointRounding.AwayFromZero);
                        float vatr = (model.VATRate < 0 ? 0 : model.VATRate) / 100;
                        newmodel.VATAmount = Math.Round(newmodel.Total * Convert.ToDecimal(vatr), MidpointRounding.AwayFromZero);
                        newmodel.Amount = newmodel.VATAmount + newmodel.Total;
                    }
                    newmodel.AmountInWords = LibraryCommon.So_chu(newmodel.Amount);
                }
                else// th k có sp nào có thuế thì làm như bt lấy đúng với order
                {
                    newmodel.VATRate = model.VATRate;
                    float vatr = (model.VATRate < 0 ? 0 : model.VATRate) / 100;
                    newmodel.VATAmount = (invoice.Total - invoice.DiscountAmount) * Convert.ToDecimal(vatr);
                    newmodel.Amount = newmodel.VATAmount + newmodel.Total;
                    newmodel.AmountInWords = LibraryCommon.So_chu(newmodel.Amount);
                }
            }
            else
            {
                newmodel.Amount = invoice.Amonut;
                newmodel.VATRate = invoice.VATRate.Value;
                newmodel.VATAmount = invoice.VATAmount;
                newmodel.AmountInWords = LibraryCommon.So_chu(newmodel.Amount);
            }

            lsteinvoice.Add(newmodel);

        }

        public async Task<Result<PublishInvoiceModelView>> CancelInvoice(int[] lstid, int ComId, string CasherName, string Note, EnumTypeEventInvoice TypeEventInvoice, bool IsDelete, bool IsDeletePT = false)
        {
            List<Invoice> ListInvoicedelete = new List<Invoice>();
            List<DetailInvoice> ListDetailInvoice = new List<DetailInvoice>();
            if (TypeEventInvoice != EnumTypeEventInvoice.Cancel && TypeEventInvoice != EnumTypeEventInvoice.Delete)
            {
                return await Result<PublishInvoiceModelView>.FailAsync(HeperConstantss.ERR042);
            }
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                var _invoices = await _invoiceRepository.Entities.Where(x => lstid.Contains(x.Id) && x.ComId == ComId && x.Status == EnumStatusInvoice.DA_THANH_TOAN || x.Status == EnumStatusInvoice.HOAN_TIEN_MOT_PHAN).ToListAsync();
                if (_invoices.Count() == 0)
                {
                    return await Result<PublishInvoiceModelView>.FailAsync(HeperConstantss.ERR012);
                }
                foreach (var _invoice in _invoices)
                {
                    if (_invoice.StatusPublishInvoiceOrder == EnumStatusPublishInvoiceOrder.CREATE || _invoice.StatusPublishInvoiceOrder == EnumStatusPublishInvoiceOrder.PUBLISH)
                    {
                        ListDetailInvoice.Add(new DetailInvoice()
                        {
                            code = _invoice.InvoiceCode,
                            note = $"Trạng thái hóa đơn không hợp lệ, {(_invoice.StatusPublishInvoiceOrder == EnumStatusPublishInvoiceOrder.CREATE ? " đã tạo mới HDDT" : " đã phát hành HDDT")}",
                            TypePublishEinvoice = ENumTypePublishEinvoice.TRANGTHAIPHATHANHKHONGHOPLE,
                        });
                    }
                    else
                    {
                        var his = new HistoryInvoice();
                        his.IdInvoice = _invoice.Id;
                        his.InvoiceCode = _invoice.InvoiceCode;
                        his.Carsher = CasherName;
                        if (TypeEventInvoice == EnumTypeEventInvoice.Cancel)
                        {
                            _invoice.Status = EnumStatusInvoice.HUY_BO;
                            his.Name = $"Đã hủy hóa đơn bởi {CasherName}, lý do: {Note}";
                            ListDetailInvoice.Add(new DetailInvoice()
                            {
                                code = _invoice.InvoiceCode,
                                note = $"Hủy đơn thành công",
                                TypePublishEinvoice = ENumTypePublishEinvoice.HUYHOADONOK,
                            });
                            if (IsDeletePT)
                            {
                                //có xóa phiếu thu liên quan, lưu ý trường hợp đã trả hàng thì k cho xóa hóa đơn này
                                await _revenueExpenditureRepository.DeleteAsync(_invoice.Id, _invoice.ComId);
                            }
                            _invoice.Note += $"{Note}, Thời gian {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}";
                            //add his
                            await _historyInvoiceRepository.AddAsync(his);
                        }
                        else if (TypeEventInvoice == EnumTypeEventInvoice.Delete)
                        {
                            ListDetailInvoice.Add(new DetailInvoice()
                            {
                                code = _invoice.InvoiceCode,
                                note = $"Xóa hóa đơn thành công",
                                TypePublishEinvoice = ENumTypePublishEinvoice.XOADONTHANHCONG,
                            });
                            if (_invoice.IsMerge)
                            {
                                ListInvoicedelete.Add(_invoice);
                            }
                            else
                            {
                                _invoice.Status = EnumStatusInvoice.XOA_BO;
                                _invoice.IsDelete = IsDelete;
                                his.Name = $"Đã xóa hóa đơn bởi {CasherName}, lý do: {Note}";
                               
                                _invoice.Note += $"{Note}, Thời gian {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}";
                                //add his
                                await _historyInvoiceRepository.AddAsync(his);
                            }
                            if (IsDelete)//cái này dành cho loại xóa hóa đơn nhưng có k ghi nhận doanh thu
                            {
                                //có xóa phiếu thu liên quan, lưu ý trường hợp đã trả hàng thì k cho xóa hóa đơn này,điều chỉnh sau
                                await _revenueExpenditureRepository.DeleteAsync(_invoice.Id, _invoice.ComId);
                            }
                        }
                        else
                        {
                            return await Result<PublishInvoiceModelView>.FailAsync(HeperConstantss.ERR042);
                        }
                    }
                }

                await _invoiceRepository.DeleteRangeAsync(ListInvoicedelete);
                await _invoiceRepository.UpdateRangeAsync(_invoices);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                PublishInvoiceModelView publishInvoiceModelView = new PublishInvoiceModelView();
                publishInvoiceModelView.DetailInvoices = ListDetailInvoice;

                return Result<PublishInvoiceModelView>.Success(publishInvoiceModelView, HeperConstantss.SUS006);
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                return await Result<PublishInvoiceModelView>.FailAsync(e.Message);
            }
        }

        public async Task AddAsync(Invoice entity)
        {
            await _invoiceRepository.AddAsync(entity);
        }

        public async Task<Result<PublishInvoiceResponse>> PublishInvoice(Invoice Invoice, PublishInvoiceModel model, int ComId, string IdCasher, string CasherName)
        {
            try
            {
                List<EInvoice> lsteinvoice = new List<EInvoice>();
                var getpattern = await _managerPatternEInvoicerepository.GetbyIdAsync(model.IdManagerPatternEInvoice);
                if (getpattern == null)
                {
                    return await Result<PublishInvoiceResponse>.FailAsync(HeperConstantss.ERR049);
                }
                this.AddEInvoice(lsteinvoice, Invoice, model, getpattern);
                foreach (var item in lsteinvoice)
                {
                    var getinvno = await _managerInvNoRepository.UpdateInvNo(item.ComId, ENumTypeManagerInv.EInvoice, false);
                    var getupđateinvoice = await _invoiceRepository.GetByIdAsync(item.IdInvoice);
                    getupđateinvoice.StatusPublishInvoiceOrder = EnumStatusPublishInvoiceOrder.CREATE;

                    item.EInvoiceCode = $"EINV{getinvno.ToString("00000000")}";
                    await _einvoiceRepository.CreateAsync(item, model.CasherName, model.IdCarsher);
                    var suplcompany = await _supplierEInvoiceRepository.GetByIdAsync(ComId, item.TypeSupplierEInvoice);

                    if (suplcompany == null)
                    {
                        _log.LogError("Chưa cấu hình nhà cung cấp");
                        _log.LogError(item.EInvoiceCode);
                    }
                    var publish = await _einvoiceRepository.ImportAndPublishInvMTTAsync(item, suplcompany, model.CasherName, model.IdCarsher);
                    if (publish.Succeeded)
                    {
                        // loại 1 OK:1/001;C23MMT-ABCDEF1000000000t300u6_6_M1-23-41849-00100000006
                        // loai 2 OK:1/001;C23MMT-ABCDEF10gfg00000000t300u6_7_M1-23-41849-00100000007,ABCDEF10gfg00000000t300pu6_8_M1-23-41849-00100000008
                        string[] getinvoice = publish.Data.Split(';');
                        if (getinvoice.Count() < 2)
                        {
                            _log.LogError($"Tạo mới thành công hóa hóa đơn điện tử, phát hành thất bại, mã lỗi từ nhà cung cấp: {publish.Data} của mã {getupđateinvoice.InvoiceCode}");
                            _log.LogError(item.EInvoiceCode);
                            //gửi telegram
                            //return await Result<string>.FailAsync(publish.Message);
                        }
                        else
                        {
                            string[] getinvoicesplit = getinvoice[1].Split('_');
                            string invoiceno = getinvoicesplit[1];
                            string MCQT = getinvoicesplit[2];
                            item.MCQT = MCQT;
                            item.InvoiceNo = int.Parse(invoiceno);
                            item.StatusEinvoice = StatusEinvoice.SignedInv;
                            item.PublishDate = DateTime.Now;
                            getupđateinvoice.StatusPublishInvoiceOrder = EnumStatusPublishInvoiceOrder.PUBLISH;
                            getupđateinvoice.IdEInvoice = item.Id;
                            await _invoiceRepository.UpdateAsync(getupđateinvoice);
                            await _einvoiceRepository.UpdateAsync(item);
                            await _unitOfWork.SaveChangesAsync();
                            _log.LogError($"Phát hành thành công hóa đơn điện tử, mẫu số {item.Pattern}, ký hiệu {item.Serial},số {item.InvoiceNo.ToString("00000000")}");
                            _log.LogError(item.EInvoiceCode);
                            string urlportal = ConvertSupport.ConverDoaminVNPTPortal<string>(suplcompany.DomainName);

                            //string url = $"Tra cứu hóa đơn điện tử tại: {suplcompany.DomainName.Replace("admindemo.vnpt-invoice.com.vn", ".vnpt-invoice.com.vn").Replace("admin.vnpt-invoice.com.vn", ".vnpt-invoice.com.vn")}, mã tra cứu: {item.FkeyEInvoice.ToUpper()}";
                            var PublishInvoiceResponse = new PublishInvoiceResponse()
                            {
                                Pattern = item.Pattern,
                                Serial = item.Serial,
                                InvoiceNo = item.InvoiceNo,
                                Fkey = item.FkeyEInvoice,
                                UrlDomain = urlportal,
                                MCQT = MCQT,
                                IsSuccess = true

                            };
                            return await Result<PublishInvoiceResponse>.SuccessAsync(PublishInvoiceResponse, HeperConstantss.ERR049);
                        }
                    }
                    else
                    {
                        _log.LogError($"Tạo mới thành công hóa hóa đơn điện tử, phát hành thất bại, mã lỗi từ nhà cung cấp: {publish.Data} của mã {getupđateinvoice.InvoiceCode}");
                        _log.LogError(item.EInvoiceCode);
                    }
                }
                return await Result<PublishInvoiceResponse>.FailAsync(HeperConstantss.ERR000);
            }
            catch (Exception e)
            {
                _log.LogError(Invoice.InvoiceCode);
                _log.LogError(e.ToString());
                return await Result<PublishInvoiceResponse>.FailAsync(HeperConstantss.ERR000);
            }

        }
        private async Task<Result<PublishInvoiceModelView>> PublishEInvoiceByMerge(Invoice Invoice, PublishInvoiceMergeModel model, int ComId)
        {
            PublishInvoiceModelView publishInvoiceModelView = new PublishInvoiceModelView();
            List<EInvoice> lsteinvoice = new List<EInvoice>();
            PublishInvoiceModel publishInvoiceModel = new PublishInvoiceModel()
            {
                VATRate = Convert.ToInt32(model.VATRate),
                VATAmount = model.VATAmount,
                Amount = model.Amount,
                ComId = ComId,
                TypeSupplierEInvoice = model.TypeSupplierEInvoice,
            };
            var getpattern = await _managerPatternEInvoicerepository.GetbyIdAsync(model.ManagerPatternEInvoices);
            if (getpattern == null)
            {
                return await Result<PublishInvoiceModelView>.FailAsync(HeperConstantss.ERR049);
            }
            this.AddEInvoice(lsteinvoice, Invoice, publishInvoiceModel, getpattern);
            var item = lsteinvoice.SingleOrDefault();//lấy invoice vì có 1 cái thôi
                                                     //foreach (var item in lsteinvoice)
                                                     //  {
            var getinvno = await _managerInvNoRepository.UpdateInvNo(item.ComId, ENumTypeManagerInv.EInvoice, false);
            //var getupđateinvoice = await _invoiceRepository.GetByIdAsync(item.IdInvoice);
            Invoice.StatusPublishInvoiceOrder = EnumStatusPublishInvoiceOrder.CREATE;

            item.EInvoiceCode = $"EINV{getinvno.ToString("00000000")}";
            await _einvoiceRepository.CreateAsync(item, model.CasherName, model.IdCasher);
            var suplcompany = await _supplierEInvoiceRepository.GetByIdAsync(ComId, item.TypeSupplierEInvoice);

            if (suplcompany == null)
            {
                _log.LogError("Chưa cấu hình nhà cung cấp");
                _log.LogError(item.EInvoiceCode);
            }
            string error = string.Empty;
            int numberRetry2 = 0;
            RetryInvoice2:
            if (numberRetry2 < 3)
            {
                var publish = await _einvoiceRepository.ImportAndPublishInvMTTAsync(item, suplcompany, model.CasherName, model.IdCasher);
                if (publish.Succeeded)
                {
                    // loại 1 OK:1/001;C23MMT-ABCDEF1000000000t300u6_6_M1-23-41849-00100000006
                    // loai 2 OK:1/001;C23MMT-ABCDEF10gfg00000000t300u6_7_M1-23-41849-00100000007,ABCDEF10gfg00000000t300pu6_8_M1-23-41849-00100000008
                    string[] getinvoice = publish.Data.Split(';');
                    if (getinvoice.Count() < 2)
                    {
                        _log.LogError($"Tạo mới thành công hóa hóa đơn điện tử, phát hành thất bại, mã lỗi từ nhà cung cấp: {publish.Data} của mã {Invoice.InvoiceCode}");
                        _log.LogError(item.EInvoiceCode);
                        //gửi telegram
                        //return await Result<string>.FailAsync(publish.Message);
                    }
                    else
                    {
                        string[] getinvoicesplit = getinvoice[1].Split('_');
                        string invoiceno = getinvoicesplit[1];
                        string MCQT = getinvoicesplit[2];
                        item.MCQT = MCQT;
                        item.InvoiceNo = int.Parse(invoiceno);
                        item.StatusEinvoice = StatusEinvoice.SignedInv;
                        item.PublishDate = DateTime.Now;
                        Invoice.StatusPublishInvoiceOrder = EnumStatusPublishInvoiceOrder.PUBLISH;
                        
                        Invoice.IdEInvoice = item.Id;
                        await _invoiceRepository.UpdateAsync(Invoice);
                        await _einvoiceRepository.UpdateAsync(item);
                        await _unitOfWork.SaveChangesAsync();
                        _log.LogError($"Phát hành thành công hóa đơn điện tử, mẫu số {item.Pattern}, ký hiệu {item.Serial},số {item.InvoiceNo.ToString("00000000")}");
                        _log.LogError(item.EInvoiceCode);
                        string urlportal = ConvertSupport.ConverDoaminVNPTPortal<string>(suplcompany.DomainName);

                        //string url = $"Tra cứu hóa đơn điện tử tại: {suplcompany.DomainName.Replace("admindemo.vnpt-invoice.com.vn", ".vnpt-invoice.com.vn").Replace("admin.vnpt-invoice.com.vn", ".vnpt-invoice.com.vn")}, mã tra cứu: {item.FkeyEInvoice.ToUpper()}";
                        var PublishInvoiceResponse = new PublishInvoiceResponse()
                        {
                            Pattern = item.Pattern,
                            Serial = item.Serial,
                            InvoiceNo = item.InvoiceNo,
                            Fkey = item.FkeyEInvoice,
                            UrlDomain = urlportal,
                            MCQT = MCQT,
                            IsSuccess = true
                        };
                        publishInvoiceModelView.PublishInvoiceResponse = PublishInvoiceResponse;
                        return await Result<PublishInvoiceModelView>.SuccessAsync(publishInvoiceModelView, HeperConstantss.SUS006);
                    }
                }
                else
                {
                    error = publish.Message;
                    if (publish.Message.Contains(CommonException.ExceptionXML))
                    {
                        _log.LogError($"{publish.Message}");
                        return await Result<PublishInvoiceModelView>.FailAsync(publish.Message);
                    }
                    else if (publish.Message.Contains(CommonException.Exception))
                    {
                        _log.LogError($"{publish.Message}");
                        _log.LogError($"Thực hiện ngủ đông 2 giây");
                        numberRetry2++;
                        Thread.Sleep(2000);
                        _log.LogError($"Khởi chạy phát hành lại");
                        goto RetryInvoice2;
                        //return await Result<PublishInvoiceModelView>.FailAsync(HeperConstantss.ERR000);
                    }
                    else if (publish.Message.Contains(CommonERREinvoice.ERR13))
                    {
                        _log.LogError($"{publish.Message}");
                        var pub = await _vnptportalrepository.listInvByCusFkeyAsync(item.FkeyEInvoice, string.Empty, string.Empty, suplcompany.UserNameService, suplcompany.PassWordService, suplcompany.DomainName);
                        if (!pub.Contains(CommonERREinvoice.ERR))
                        {
                            var getxmlmodel = ConvertSupport.ConvertXMLToModel<XmlListInvByCusFkey>(pub);
                            if (getxmlmodel.Item.Count() == 0)
                            {
                                _log.LogError($"Đồng bộ gọi hàm listInvByCusFkeyAsync không tìm thấy dữ liệu -> {item.EInvoiceCode} -> {pub}");
                            }
                            else
                            {
                                var PublishInvoiceResponse = new PublishInvoiceResponse()
                                {
                                    Pattern = item.Pattern,
                                    Serial = item.Serial,
                                    InvoiceNo = getxmlmodel.Item.First().InvNum,
                                    Fkey = item.FkeyEInvoice,
                                    IsSuccess = true
                                };
                                publishInvoiceModelView.PublishInvoiceResponse = PublishInvoiceResponse;
                                return await Result<PublishInvoiceModelView>.SuccessAsync(publishInvoiceModelView, HeperConstantss.SUS006); 
                            }
                        }
                        else
                        {
                            _log.LogError($"Đồng bộ gọi hàm listInvByCusFkeyAsync không thành công lỗi: {pub}");
                        }
                        publishInvoiceModelView.PublishInvoiceResponse = new PublishInvoiceResponse()
                        {
                            Fkey = item.FkeyEInvoice,
                            Pattern = item.Pattern,
                            Serial = item.Serial,
                            IsSuccess = false,
                            Message = error
                        };
                        return await Result<PublishInvoiceModelView>.SuccessAsync(publishInvoiceModelView, HeperConstantss.SUS006);
                    }
                }
            }
            return await Result<PublishInvoiceModelView>.FailAsync(error);
        }
        public async Task<Result<PublishInvoiceModelView>> PublishEInvoiceMerge(PublishInvoiceMergeModel model, int ComId)
        {
            PublishInvoiceModelView publishInvoiceModelView = new PublishInvoiceModelView();
            List<DetailInvoice> ListDetailInvoice = new List<DetailInvoice>();
            if (string.IsNullOrEmpty(model.JsonInvoiceOld))
            {
                return await Result<PublishInvoiceModelView>.FailAsync(HeperConstantss.ERR000);
            }
            var lstinvoiccode = ConvertSupport.ConverJsonToModel<string[]>(model.JsonInvoiceOld);
            var getAllInvoice = await _invoiceRepository.Entities.AsNoTracking().Where(x => lstinvoiccode.Contains(x.InvoiceCode)).Include(x => x.InvoiceItems).ToListAsync();
            if (getAllInvoice.Count() != lstinvoiccode.Count())
            {
                _log.LogInformation($"old Json"+ model.JsonInvoiceOld);
                _log.LogInformation($"new Json"+ ConvertSupport.ConverObjectToJsonString(getAllInvoice.Select(x => x.InvoiceCode).ToArray()));
                return await Result<PublishInvoiceModelView>.FailAsync("Sản phẩm đã bị thay đổi vui lòng thực hiện lại!");
            }
            //----------------check xem đã có người phát hành chưa--------------//
            bool IsPublish = false;//tức là hóa đơn đã được phát hành chưa
            foreach (var item in getAllInvoice)
            {
                if (item.IsMerge)
                {
                    IsPublish = true;
                    ListDetailInvoice.Add(new DetailInvoice()
                    {
                        code = item.InvoiceCode,
                        note = $"Hóa đơn này đã được phát hành gộp cho hóa đơn có mã: {item.InvoiceCodePatern}",
                        TypePublishEinvoice = ENumTypePublishEinvoice.PHATHANHLOI,
                    });
                }
                else if (item.IdEInvoice != null)
                {
                    IsPublish = true;
                    ListDetailInvoice.Add(new DetailInvoice()
                    {
                        code = item.InvoiceCode,
                        note = $"Hóa đơn này đã được phát hành phát hành hóa đơn điện tử trước đó",
                        TypePublishEinvoice = ENumTypePublishEinvoice.PHATHANHLOI,
                    });
                }
            }

            //-------------------thực hiện map thành hóa đơn mới---------------------------//

            //------------------
            var listiteminvoice = new List<InvoiceItem>();
            foreach (var item in getAllInvoice)
            {
                if (item.InvoiceItems.Count() == 0)
                {
                    IsPublish = true;
                    ListDetailInvoice.Add(new DetailInvoice()
                    {
                        code = item.InvoiceCode,
                        note = $"Hóa đơn này không có dữ liệu sản phẩm không thể phát hành",
                        TypePublishEinvoice = ENumTypePublishEinvoice.PHATHANHLOI,
                    });
                }
                listiteminvoice.AddRange(item.InvoiceItems);
            }
            //---------------------------
            publishInvoiceModelView.IsError = IsPublish;
            if (IsPublish)//có lỗi thì trả về 
            {
                publishInvoiceModelView.DetailInvoices = ListDetailInvoice;
                return await Result<PublishInvoiceModelView>.SuccessAsync(publishInvoiceModelView);
            }
            //-------------------------

            listiteminvoice.ForEach(x => x.Id = 0);
            //var grinvocie = listiteminvoice.GroupBy(x => x.Code);
            //------------Tạo hóa đơn mới
            Invoice invoicenew = new Invoice();
            invoicenew.ComId = ComId;
            invoicenew.TypeProduct = model.TypeProduct;
            //-----------------tao lisst item , lấy luôn item từ view vào vì để người dùng thấy đúng giá trị đã xem
            var listiteminvoicenew = new List<InvoiceItem>();
            foreach (var item in model.PublishInvoiceItemModel)
            {
                var iteminvoice = new InvoiceItem()
                {
                    Code = item.Code,
                    Name = item.Name,
                    Unit = item.Unit,
                    Price = item.Price,
                    VATRate = item.VATRate,
                    VATAmount = item.VATAmount,
                    TypeProductCategory = item.TypeProductCategory,
                    Quantity = item.Quantity,
                    Total = item.Total,
                    Amonut = item.Amount
                };
                var getpro = listiteminvoice.FirstOrDefault(x=>x.Code==item.Code);
                if (getpro!=null)
                {
                    iteminvoice.IdProduct = getpro.IdProduct;
                    iteminvoice.PriceNoVAT = getpro.PriceNoVAT;
                    iteminvoice.Price = getpro.Price;
                    iteminvoice.Name = getpro.Name;
                }
                listiteminvoicenew.Add(iteminvoice);
            }
            //foreach (var item in grinvocie)
            //{
            //    var iteminvoice = new InvoiceItem()
            //    {
            //        Code = item.Key,
            //        Name = item.First().Name,
            //        Unit = item.First().Unit,
            //        Price = item.First().Price,
            //        VATRate = Convert.ToInt32(model.VATRate),
            //        TypeProductCategory = item.First().TypeProductCategory,
            //        Quantity = item.Sum(x => x.Quantity),
            //        Total = item.Sum(x => x.Quantity) * item.First().Price,
            //    };

            //    decimal vatrate = Convert.ToDecimal(model.VATRate == -1 ? 0 : model.VATRate);
            //    iteminvoice.VATAmount = Math.Round(iteminvoice.Total * (vatrate / 100), MidpointRounding.AwayFromZero);
            //    iteminvoice.Amonut = Math.Round(iteminvoice.Total + iteminvoice.VATAmount, MidpointRounding.AwayFromZero);
            //    listiteminvoice.Add(iteminvoice);
            //}
            invoicenew.InvoiceItems = listiteminvoicenew;
           
            // xử lý đưa vào db
            await _unitOfWork.CreateTransactionAsync();
            try
            { 
                //---------------xử lý khách hàng -------------------//
                if (model.IsCreateCustomer && !model.IsRetailCustomer)
                {
                    Customer customer = new Customer();
                    customer.Comid = ComId;
                    customer.Name = model.CusName?.Trim();
                    customer.Buyer = model.Buyer?.Trim();
                    customer.Code = model.CusCode?.Trim();
                    customer.Address = model.Address;
                    customer.Taxcode = model.Taxcode?.Trim();
                    customer.CCCD = model.CCCD;
                    customer.PhoneNumber = model.PhoneNumber?.Trim();
                    customer.CusBankNo = model.CusBankNo?.Trim();
                    customer.CusBankName = model.CusBankName?.Trim();
                    customer.Email = model.Email?.Trim();
                    customer.EmailConfirm = model.Email?.Trim();
                    customer.isEmailConfirm = true;
                    customer.UserName = model.CusCode?.Trim();
                    customer.Password = model.CusCode?.Trim();
                    var validatecode = await _customerRepository.ValidateCode(model.CusCode?.Trim(), ComId);
                    if (validatecode)
                    {
                        await _unitOfWork.RollbackAsync();
                        return await Result<PublishInvoiceModelView>.FailAsync("Mã khách hàng đã tồn tại trên hệ thống!" + model.CusCode?.Trim());
                    }
                    var validateemail = await _customerRepository.ValidatePhoneNumberAndEmail(model.PhoneNumber?.Trim(), model.Email?.Trim(), ComId, null);
                    if (validateemail)
                    {
                        await _unitOfWork.RollbackAsync();
                        return await Result<PublishInvoiceModelView>.FailAsync("Số điện thoại hoặc email đã tồn tại trên hệ thống, vui lòng đổi và thử lại");
                    }
                    var createcus = await _customerRepository.Crreate(customer);
                    if (createcus.Succeeded)
                    {
                        await _unitOfWork.SaveChangesAsync();
                        model.IdCustomer = createcus.Data.Id;
                    }
                }
                else if (!string.IsNullOrEmpty(model.CusCode))
                {
                    var getcus = await _repositoryCusomer.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.Code == model.CusCode);
                    if (getcus != null)
                    {
                       // return await Result<PublishInvoiceModelView>.FailAsync("Khách hàng bạn lựa chọn đã bị xóa khỏi hệ thống, vui lòng thử lại!");
                        model.IdCustomer = getcus.Id;
                    }
                }
                else if (model.IsRetailCustomer && string.IsNullOrEmpty(model.Buyer?.Trim()))
                {
                    model.Buyer = "Khách lẻ";
                }
                //--------map và sinh số hóa đơn----------------//
                MapToInvoiceNew(invoicenew, model);
                var getInv = await _managerInvNoRepository.UpdateInvNo(ComId, ENumTypeManagerInv.Invoice, true);
                invoicenew.InvoiceCode = $"HD-{getInv.ToString("00000000")}";
                invoicenew.IsMerge = true;//đánh dấu là hóa đơn gộp k có InvoiceCodePatern là thèn cha
                if (model.IsDelete)
                {
                    invoicenew.IsDeleteMerge = true;//--------------------đánh dâu là xóa các hóa đơn cũ nhé, nếu có cấu hình-------------------------
                }
                await _invoiceRepository.AddAsync(invoicenew);
                await _unitOfWork.SaveChangesAsync();//lưu hóa đơn bán hàng

                //------------------------lưu lịch sử cho hóa đơn---------------//
                HistoryInvoice history = new HistoryInvoice()
                { 
                    InvoiceCode = invoicenew.InvoiceCode,
                    IdInvoice = invoicenew.Id,
                    Carsher = model.CasherName,
                    Name = $"Tạo mới hóa đơn gộp, các hóa đơn gốc: {string.Join(",", lstinvoiccode)}"
                };
                //add lịch sử
                await _historyInvoiceRepository.AddAsync(history);


                if (model.IsDelete)     //--------------------xóa các hóa đơn cũ nhé, nếu có cấu hình-------------------------
                {
                    _log.LogError($"Lựa chọn xóa dữ liệu sau khi tạo đơn gộp bởi {model.CasherName}, hóa đơn đã xóa {string.Join(",", lstinvoiccode)}" );
                    HistoryInvoice history2 = new HistoryInvoice()
                    {
                        InvoiceCode = invoicenew.InvoiceCode,
                        IdInvoice = invoicenew.Id,
                        Carsher = model.CasherName,
                        Name = $"Xóa các hóa đơn đã gộp: {string.Join(",", lstinvoiccode)}"
                    };
                    //add lịch sử
                    await _historyInvoiceRepository.AddAsync(history2);

                    var updatedoncu = await _invoiceRepository.Entities.Where(x => lstinvoiccode.Contains(x.InvoiceCode)).ToListAsync();
                    await _invoiceRepository.DeleteRangeAsync(updatedoncu);
                }
                else     //--------------------lưu lại các hóa đơn cũ đã gộp là ghi nhận đã merge-------------------------
                {
                    var updatedoncu = await _invoiceRepository.Entities.Where(x => lstinvoiccode.Contains(x.InvoiceCode)).ToListAsync();
                    updatedoncu.ForEach(x => { x.IsMerge = true; x.InvoiceCodePatern = invoicenew.InvoiceCode; });
                }
                

                //---------------------- xử lý hóa đơn điện tử-----------------
                var getpayment = await _paymentMethodRepository.GetAll(ComId).AsNoTracking().SingleOrDefaultAsync(x=>x.Id== model.IdPaymentMethod);
                if (getpayment==null)
                {
                    await _unitOfWork.RollbackAsync();
                    return await Result<PublishInvoiceModelView>.FailAsync("Hình thức thanh toán bạn chọn đã bị xóa bỏ, vui lòng tải lại trang và thực hiện lại!");
                }
                invoicenew.PaymentMethod = getpayment;
                invoicenew.Status = EnumStatusInvoice.DA_THANH_TOAN;
                var publisheinvoice = await this.PublishEInvoiceByMerge(invoicenew, model, ComId);
                if (publisheinvoice.Succeeded)
                {
                    publishInvoiceModelView.PublishInvoiceResponse = publisheinvoice.Data.PublishInvoiceResponse;
                    if (publisheinvoice.Data.PublishInvoiceResponse.IsSuccess)
                    {
                        ListDetailInvoice.Add(new DetailInvoice()
                        {
                            code = invoicenew.InvoiceCode,
                            note = $"Phát hành hóa đơn điện tử thành công số: {publishInvoiceModelView.PublishInvoiceResponse.InvoiceNo.ToString("00000000")}, " +
                            $"mẫu số: {publishInvoiceModelView.PublishInvoiceResponse.Pattern}, ký hiệu: {publishInvoiceModelView.PublishInvoiceResponse.Serial}",
                            TypePublishEinvoice = ENumTypePublishEinvoice.PHATHANHOK,
                        });
                        HistoryInvoice history2 = new HistoryInvoice()
                        {
                            InvoiceCode = invoicenew.InvoiceCode,
                            IdInvoice = invoicenew.Id,
                            Carsher = model.CasherName,
                            Name = $"Phát hành hóa đơn điện tử thành công: {publisheinvoice.Data.PublishInvoiceResponse?.Pattern},{publisheinvoice.Data.PublishInvoiceResponse?.Serial},{publisheinvoice.Data.PublishInvoiceResponse?.InvoiceNo}"
                        };
                        //add lịch sử
                        await _historyInvoiceRepository.AddAsync(history2);
                    }
                    else
                    {
                        ListDetailInvoice.Add(new DetailInvoice()
                        {
                            code = invoicenew.InvoiceCode,
                            note = "Phát hành thành công hóa đơn điện tử không có kết quả, vui lòng truy cập danh sách hóa đơn điện tử để đồng bộ lại hóa đơn: " + publishInvoiceModelView.PublishInvoiceResponse.Message,
                            TypePublishEinvoice = ENumTypePublishEinvoice.PHATHANHOK,
                        });
                        HistoryInvoice history2 = new HistoryInvoice()
                        {
                            InvoiceCode = invoicenew.InvoiceCode,
                            IdInvoice = invoicenew.Id,
                            Carsher = model.CasherName,
                            Name = $"Phát hành hóa đơn điện tử thành công: {publisheinvoice.Data.PublishInvoiceResponse?.Fkey}"
                        };
                        //add lịch sử
                        await _historyInvoiceRepository.AddAsync(history2);
                    }

                    publishInvoiceModelView.DetailInvoices = ListDetailInvoice;
                    await _unitOfWork.SaveChangesAsync();//lưu các phiên nếu có
                    await _unitOfWork.CommitAsync();
                    return Result<PublishInvoiceModelView>.Success(publishInvoiceModelView, HeperConstantss.SUS006);
                }
                else
                {
                    await _unitOfWork.RollbackAsync();
                    return await Result<PublishInvoiceModelView>.FailAsync(GeneralMess.GeneralMessStartPublishEInvoice(publisheinvoice.Message));
                }
               // publishInvoiceModelView.DetailInvoices = ListDetailInvoice;
              
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                _log.LogError(e.ToString());
                return await Result<PublishInvoiceModelView>.FailAsync(e.Message);
            }

        }
        private void MapToInvoiceNew(Invoice invoice, PublishInvoiceMergeModel model)
        {
            invoice.CusCode = model.CusCode;
            invoice.Buyer = model.Buyer;
            invoice.IdCustomer = model.IdCustomer;
            invoice.Address = model.Address;
            invoice.PhoneNumber = model.PhoneNumber;
            invoice.CusName = model.CusName;
            invoice.CusBankNo = model.CusBankNo;
            invoice.CusBankName = model.CusBankName;
            invoice.Email = model.Email;
            invoice.CCCD = model.CCCD;
            invoice.IdPaymentMethod = model.IdPaymentMethod;
            invoice.IsRetailCustomer = model.IsRetailCustomer;
            invoice.Quantity = invoice.InvoiceItems.Sum(x => x.Quantity);
            invoice.VATRate = (float)model.VATRate;
            invoice.VATAmount = model.VATAmount;
            invoice.Amonut = model.Amount;
            invoice.Total = model.Total;
            invoice.ArrivalDate = DateTime.Now;
            invoice.PurchaseDate = invoice.ArrivalDate;
            invoice.DiscountAmount = model.DiscountAmount;
            invoice.StaffName = model.CasherName;
            invoice.CasherName = model.CasherName;
            invoice.IdStaff = model.IdCasher;
            invoice.IdCasher = model.IdCasher;
        }

        public async Task JobDeleteInvoiceAsync()
        {
            _log.LogInformation("Xóa hóa đơn định kỳ, hóa đơn xóa bỏ");
            try
            {
                var datatiem = DateTime.Now.AddDays(-30);
                var getall = _invoiceRepository.GetAllQueryable().Where(x => x.Status == EnumStatusInvoice.XOA_BO && x.CreatedOn < datatiem);
                if (getall.Count() > 0)
                {
                    var getlstIdOrderTable = getall.Where(x => x.IdOrderTable.HasValue).Select(x => x.IdOrderTable).ToArray();
                    await _invoiceRepository.DeleteRangeAsync(getall);
                    _log.LogInformation("Tổng xóa: "+ getall.Count());
                    await _orderTableRepository.DeleteRangeAsync(_orderTableRepository.Entities.Where(x => getlstIdOrderTable.Contains(x.Id)));
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                _log.LogInformation("Xóa hóa đơn định kỳ lỗi");
                _log.LogError(e.Message);
            }
           
        }

        public async Task<Result<PublishInvoiceModelView>> DeleteIsMergeInvoice(Guid IdInvoice, int ComId, string CasherName)
        {
            _log.LogInformation("Xóa hóa đơn gộp");
            try
            {
                PublishInvoiceModelView model = new PublishInvoiceModelView();
                var getall = await _invoiceRepository.GetAllQueryable().SingleOrDefaultAsync(x => x.IdGuid== IdInvoice);
                if (getall!=null)
                {
                    var getlstIdOrderTable = _invoiceRepository.Entities.Where(x => x.InvoiceCodePatern== getall.InvoiceCode && x.IsMerge && x.ComId==ComId);
                    var getlecect = getlstIdOrderTable.Select(x => x.InvoiceCode).ToArray();
                    foreach (var item in getlecect)
                    {
                        model.DetailInvoices.Add(new DetailInvoice()
                        {
                            code=item,
                            note= $"Xóa bỏ thành công đơn",
                            TypePublishEinvoice=ENumTypePublishEinvoice.XOADONTHANHCONG,
                        });
                    }
                    await _invoiceRepository.DeleteRangeAsync(getlstIdOrderTable);
                    getall.IsDeleteMerge = true;//--------------------update đơn gốc là đã xóa các đơn đã gộp-------------------------
                    HistoryInvoice history2 = new HistoryInvoice()
                    {
                        InvoiceCode = getall.InvoiceCode,
                        IdInvoice = getall.Id,
                        Carsher = CasherName,
                        Name = $"Xóa các hóa đơn đã gộp: {string.Join(",", getlecect)}"
                    };
                    //add lịch sử
                    await _historyInvoiceRepository.AddAsync(history2);
                    _log.LogInformation("Tổng xóa đơn gộp: " + getlstIdOrderTable.Count()+ "cửa đơn: "+ getall.InvoiceCode);
                    await _orderTableRepository.DeleteRangeAsync(_orderTableRepository.Entities.Where(x => getlstIdOrderTable.Where(x=>x.IdOrderTable!=null).Select(x=>x.IdOrderTable).ToArray().Contains(x.Id)));
                    _log.LogInformation("Xóa các đơn order của các đơn gộp");
                    await _invoiceRepository.UpdateAsync(getall);
                    await _unitOfWork.SaveChangesAsync();
                    return await Result<PublishInvoiceModelView>.SuccessAsync(model);
                }
                else
                {
                    return await Result<PublishInvoiceModelView>.FailAsync(HeperConstantss.ERR012);
                }
            }
            catch (Exception e)
            {
                _log.LogError("Xóa hóa đơn định kỳ lỗi");
                _log.LogError(e.Message);
                return await Result<PublishInvoiceModelView>.FailAsync(e.Message);
            }
        }
    }
}
