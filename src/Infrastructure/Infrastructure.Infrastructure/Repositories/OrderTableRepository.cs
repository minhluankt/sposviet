using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using Hangfire.Logging;
using HelperLibrary;
using Infrastructure.Infrastructure.Migrations;
using Infrastructure.Infrastructure.Migrations.Identity;
using Joker.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NStandard;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
namespace Infrastructure.Infrastructure.Repositories
{
    public class OrderTableRepository : IOrderTableRepository
    {
        private readonly IMapper _map;

        private readonly IRevenueExpenditureRepository<RevenueExpenditure> _revenueExpenditureRepository;
        private readonly IDetailtKitchenRepository<DetailtKitchen> _detailtKitchenRepository;
        private readonly IHistoryOrderRepository<HistoryOrder> _historyOrderRepository;
        private readonly IRepositoryAsync<HistoryInvoice> _historyInvoiceRepository;
        private readonly IRepositoryAsync<RoomAndTable> _roomadntableRepository;
        private readonly IInvoicePepository<Invoice> _InvoiceRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IRepositoryAsync<Customer> _customerRepository;
        private readonly IProductPepository<Product> _productrepository;
        private readonly IRepositoryAsync<ComponentProduct> _comboproductrepository;
        private readonly IManagerInvNoRepository _managerInvNorepository;
        private readonly INotifyChitkenRepository _notifyChitkenRepository;
        private readonly ILogger<OrderTableRepository> _log;
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IRepositoryAsync<OrderTable> _repository;
        private readonly IRepositoryAsync<OrderTableItem> _OrderTableItemrepository;
        public OrderTableRepository(IProductPepository<Product> productrepository,
            ILogger<OrderTableRepository> log,
            IRepositoryAsync<ComponentProduct> comboproductrepository,
            IPaymentMethodRepository paymentMethodRepository,
            IRevenueExpenditureRepository<RevenueExpenditure> revenueExpenditureRepository,
            IHistoryOrderRepository<HistoryOrder> historyOrderRepository,
            INotifyChitkenRepository notifyChitkenRepository,
            IRepositoryAsync<RoomAndTable> roomadntableRepository,
            IRepositoryAsync<Customer> customerRepository,
            IMapper map,
            IRepositoryAsync<HistoryInvoice> historyInvoiceRepository,
            IInvoicePepository<Invoice> InvoiceRepository,
            IDetailtKitchenRepository<DetailtKitchen> detailtKitchenRepository,
            IManagerInvNoRepository managerInvNorepository, IRepositoryAsync<OrderTableItem> OrderTableItemrepository,
            IUnitOfWork unitOfWork, IRepositoryAsync<OrderTable> repository)
        {
            _log = log;
            _comboproductrepository = comboproductrepository;
            _paymentMethodRepository = paymentMethodRepository;
            _revenueExpenditureRepository = revenueExpenditureRepository;
            _historyInvoiceRepository = historyInvoiceRepository;
            _notifyChitkenRepository = notifyChitkenRepository;
            _detailtKitchenRepository = detailtKitchenRepository;
            _map = map;
            _historyOrderRepository = historyOrderRepository;
            _roomadntableRepository = roomadntableRepository;
            _customerRepository = customerRepository;
            _OrderTableItemrepository = OrderTableItemrepository;
            _managerInvNorepository = managerInvNorepository;
            _productrepository = productrepository;
            _unitOfWork = unitOfWork;
            _InvoiceRepository = InvoiceRepository;
            _repository = repository;
        }

        public async Task UpdateCustomerOrder(int comid, Guid idOrder, Customer customer, EnumTypeProduct enumTypeProduct = EnumTypeProduct.AMTHUC)
        {
            var order = await _repository.Entities.SingleOrDefaultAsync(x => x.IdGuid == idOrder && x.ComId == comid && x.Status == EnumStatusOrderTable.DANG_DAT && x.TypeProduct == enumTypeProduct);
            if (order == null)
            {
                throw new Exception("không tìm thấy đơn đặt bàn");
            }
            if (customer == null)
            {
                order.IsRetailCustomer = true;
                order.IdCustomer = null;
                order.Buyer = "Khách lẻ";
                order.CusCode = "";
            }
            else
            {
                order.IsRetailCustomer = false;
                order.IdCustomer = customer.Id;
                order.Buyer = customer.Name;
                order.CusCode = customer.Code;
            }
            await _repository.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }
        private void AddHistoryOrder(List<HistoryOrder> model)
        {
            _historyOrderRepository.AddHistoryOrder(model);
        }
        public async Task<Result<OrderTable>> UpdateItemOrderAsync(int? IdCustomer, string CusCode, int comid, Guid idOrder, Guid idItem, Guid? idTable, bool IsBringBack, decimal Quantity, string Cashername, string IdCasher, string Note = "", bool IsRemoverow = false, bool IsCancelItem = false)
        {
            _unitOfWork.CreateTransaction();

            try
            {
                var orders = _repository.Entities.Include(x => x.OrderTableItems).Where(x => x.IdGuid == idOrder && x.ComId == comid && x.Status == EnumStatusOrderTable.DANG_DAT);
                if (idTable != null)
                {
                    orders = orders.Where(x => x.IdRoomAndTableGuid == idTable.Value);
                }
                else if (IsBringBack)
                {
                    orders = orders.Where(x => x.IsBringBack);
                }
                if (orders.Count() == 0)
                {
                    //throw new Exception("Đơn không tồn tại");
                    return Result<OrderTable>.Fail("Đơn không tồn tại");
                }
                var order = orders.Include(x => x.OrderTableItems).SingleOrDefault();
                var getItem = order.OrderTableItems.Where(x => x.IdOrderTable == order.Id && x.IdGuid == idItem).SingleOrDefault();
                if (getItem != null)
                {
                    bool checkupdatequantity = false;//
                    if (Quantity != -1)
                    {
                        checkupdatequantity = true;
                    }
                    if (IsCancelItem)//nếu hủy món mà đã thông báo bép thì luuw lịch sử lại
                    {
                        decimal _newquantity = Quantity;
                        if (checkupdatequantity)
                        {
                            _newquantity = (getItem.QuantityNotifyKitchen - (getItem.Quantity - (Quantity * -1)))*-1;
                        }
                        string randowm = LibraryCommon.RandomString(8);
                        var lst = new List<HistoryOrder>();
                      
                        var his = new HistoryOrder()
                        {
                            IdOrderTable = order.Id,
                            Carsher = Cashername,
                            Code = randowm,
                            IsNotif = true,
                            CreateDate = DateTime.Now,
                            Name = $"{Quantity.ToString("0.###")} {getItem.Name}",
                        };
                        lst.Add(his);
                        await _historyOrderRepository.AddHistoryOrder(lst);//add lịch sử
                        // thông báo cho bếp nữa nhé
                        await _notifyChitkenRepository.UpdateNotifyKitchenCancelAsync(comid, idOrder, getItem.IdProduct.Value, _newquantity, Cashername,IdCasher);

                    }
                    if (IsRemoverow || ((getItem.Quantity * -1) == Quantity))// trường hợp xóa dòng hoặc giảm tất cả
                    {
                        order.OrderTableItems.Remove(getItem);
                    }
                    else
                    {
                        var checksl  = Math.Round(getItem.Quantity + (Quantity), 3);
                        if (checksl<=0)
                        {
                            // throw new Exception("Món được chọn số lượng tối thiểu là 1");
                            return Result<OrderTable>.Fail("Món được chọn số lượng không được bằng nhỏ hơn hoặc bằng không");
                        }
                        order.OrderTableItems.ToList().ForEach(x =>
                        {
                            if (x.IdOrderTable == order.Id && x.IdGuid == idItem)
                            {
                                x.Quantity = Math.Round(x.Quantity + (Quantity),3);
                                x.Total = Convert.ToDecimal(x.Quantity) * x.Price;
                                if (Quantity < 0 && IsCancelItem)
                                {
                                    x.QuantityNotifyKitchen = x.Quantity;
                                }
                            }

                        });

                    }
                    if (order.OrderTableItems.Count() == 0)
                    {
                        await _repository.DeleteAsync(order);
                    }
                    else
                    {
                        if (order.IdCustomer != IdCustomer && IdCustomer != null)
                        {
                            var customer = await _customerRepository.Entities.Where(x => x.Code == CusCode && x.Id == IdCustomer && x.Comid == comid).SingleOrDefaultAsync();
                            if (customer == null)
                            {
                                return await Result<OrderTable>.FailAsync("Không tìm thấy khách hàng phù hợp");
                            }

                            order.IdCustomer = customer.Id;
                            order.IsRetailCustomer = false;
                            order.Buyer = customer.Name;
                        }

                        order.Amonut = Math.Round(order.OrderTableItems.Sum(x => x.Total), MidpointRounding.AwayFromZero);
                        order.Quantity = order.OrderTableItems.Sum(x => x.Quantity);
                        await _repository.UpdateAsync(order);
                    }

                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    return Result<OrderTable>.Fail("Món chọn không tồn tại");
                    // throw new Exception("Món chọn không tồn tại");
                }
                await _unitOfWork.CommitAsync();
                _unitOfWork.Dispose();
                return Result<OrderTable>.Success(order);
            }
            catch (Exception e)
            {

                await _unitOfWork.RollbackAsync();
                throw new Exception(e.Message);
            }

        }
        public async Task<Result<OrderTable>> AddOrUpdateOrderTable(bool IsNewOrder, OrderTable model, OrderTableItem item)
        {
            var product = new Product();
            if (model.TypeProduct == EnumTypeProduct.BAN_LE || model.TypeProduct == EnumTypeProduct.TAPHOA_SIEUTHI)
            {
                if (item.IdProduct > 0)
                {
                    product = await _productrepository.GetByIdAsync(model.ComId,item.IdProduct.Value,true);
                }
                else
                {
                    product = await _productrepository.GetByCodeAsync(model.ComId, item.Code, true);
                }
            }
            else
            {
                product = await _productrepository.GetByIdAsync(model.ComId, item.IdProduct.Value, true);
            }

            if (product == null)
            {
                return Result<OrderTable>.Fail("Không tìm thấy sản phẩm");
                //throw new Exception("Không tìm thấy sản phẩm");
            }

            if (!IsNewOrder)// đã có order
            {
                _unitOfWork.CreateTransaction();
                try
                {
                    var orders = _repository.Entities.Include(x => x.OrderTableItems).Where(x => x.IdGuid == model.IdGuid && x.Status == EnumStatusOrderTable.DANG_DAT);// điều kiện này là đủ nhưng cần thêm dưới để chính xác hơn
                    if (model.IdRoomAndTable != null && model.IdRoomAndTable > 0)
                    {
                        orders = orders.Where(x => x.IdRoomAndTable == model.IdRoomAndTable);
                    }
                    else if (model.IsBringBack)
                    {
                        orders = orders.Where(x => x.IsBringBack);
                    }
                    if (orders.Count() == 0)
                    {
                        return Result<OrderTable>.Fail("Không tìm thấy đơn đặt bàn");
                        // throw new Exception("không tìm thấy đơn đặt bàn");
                    }
                    var order = await orders.SingleOrDefaultAsync();
                    var getItem = order.OrderTableItems.Where(x => x.IdOrderTable == order.Id && x.IdProduct == product.Id).SingleOrDefault();
                    if (getItem != null)
                    {
                        if (getItem.IsServiceDate)
                        {
                            return Result<OrderTable>.Fail("Sản phẩm là dịch vụ tính tiền theo giờ chỉ được thêm 1 lần");
                        }
                        //phải chạy lai list đó vì nếu k sẽ k update dc list đó
                        order.OrderTableItems.ToList().ForEach(x =>
                        {
                            if (x.IdOrderTable == order.Id && x.IdProduct == product.Id)
                            {
                                x.Quantity = x.Quantity + 1;
                                x.Total = Convert.ToDecimal(x.Quantity) * x.Price;
                                x.TypeProductCategory = product.TypeProductCategory;
                            }
                        });
                    }
                    else
                    {
                        order.OrderTableItems.Add(this.MapOrderTableItem(product, item.DateCreateService));
                    }
                    // var amount = await UpdateItem(order.Id, product);

                    order.Amonut = Math.Round(order.OrderTableItems.Sum(x => x.Total), MidpointRounding.AwayFromZero);
                    order.Quantity = order.OrderTableItems.Sum(x => x.Quantity);
                    order.IdCustomer = model.IdCustomer;
                    order.IsBringBack = model.IsBringBack;
                    order.IsRetailCustomer = model.IsRetailCustomer;
                    order.Buyer = model.Buyer;
                    await _repository.UpdateAsync(order);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    return await Result<OrderTable>.SuccessAsync(order);
                }
                catch (Exception e)
                {
                    await _unitOfWork.RollbackAsync();
                    throw new Exception("Lỗi updateitem table" + e.ToString());
                }
            }
            else // chưa có đơn
            {
                _unitOfWork.CreateTransaction();
                try
                {
                    if (model.IsBringBack)
                    {
                        model.IdRoomAndTable = null;
                    }
                    string cn = "en-US"; //Vietnamese
                    var _cultureInfo = new CultureInfo(cn);
                    var checkOrder = await _managerInvNorepository.UpdateInvNo(model.ComId, ENumTypeManagerInv.OrderTable,false);
                    //model.IdGuid = Guid.NewGuid();
                    model.OrderSort = checkOrder;
                    if (model.TypeProduct == EnumTypeProduct.BAN_LE || model.TypeProduct == EnumTypeProduct.TAPHOA_SIEUTHI)
                    {
                        model.OrderTableCode = $"HD-{model.OrderSort}";
                    }
                    else
                    {
                        model.OrderTableCode = $"OD-{model.OrderSort}";
                    }


                    item = this.MapOrderTableItem(product, item.DateCreateService);
                    model.Amonut = Math.Round(product.Price * item.Quantity, MidpointRounding.AwayFromZero);
                    model.Quantity = item.Quantity;
                    item.TypeProductCategory = product.TypeProductCategory;
                    model.OrderTableItems.Add(item);
                    await _repository.AddAsync(model);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    return await Result<OrderTable>.SuccessAsync(model);
                }
                catch (Exception e)
                {
                    await _unitOfWork.RollbackAsync();
                    throw new Exception("Lỗi thêm  order table" + e.ToString());
                }

            }
        }
        private OrderTableItem MapOrderTableItem(Product product,DateTime? DateCreateService)
        {
            OrderTableItem item = new OrderTableItem();
            item.IdProduct = product.Id;
            item.Quantity = 1;
            item.Price = product.Price;
            item.EntryPrice = product.RetailPrice;// giá nhập vào
            item.Total = product.Price;
            item.Name = product.Name;
            item.IsServiceDate = product.IsServiceDate;
            item.DateCreateService = DateCreateService;
            item.Code = product.Code;
            item.Unit = product.UnitType?.Name;
            return item;
        }

        public async Task<OrderTable> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public IQueryable<OrderTable> GetOrderByBringback(int ComId, EnumStatusOrderTable enumStatusOrderTable, EnumTypeProduct enumTypeProduct)
        {
            return _repository.Entities.Where(x => x.ComId == ComId && x.Status == enumStatusOrderTable && x.IsBringBack && x.TypeProduct== enumTypeProduct);
        }

        public async Task<Result<bool>> RemoveOrder(int comid, Guid idOrder,string CasherName, string IdCashername, EnumTypeProduct enumTypeProduct = EnumTypeProduct.AMTHUC)
        {
            var checkOrder = await _repository.Entities.SingleOrDefaultAsync(x => x.ComId == comid && x.IdGuid == idOrder && x.TypeProduct == enumTypeProduct);
            if (checkOrder != null)
            {
                await _repository.DeleteAsync(checkOrder);
                List<Kitchen> lstId = new List<Kitchen>();
                lstId.Add(new Kitchen()
                {
                    IdOrder = idOrder
                });
                await _notifyChitkenRepository.UpdateNotifyKitchenCancelListAsync(lstId, comid, CasherName, IdCashername);
                await _unitOfWork.SaveChangesAsync();
                return await Result<bool>.SuccessAsync(true);
            }
            return await Result<bool>.FailAsync();
        }

        public async Task<Result<PublishInvoiceResponse>> CheckOutOrderAsync(int comid, int Idpayment, Guid idOrder,
            decimal discountPayment,  decimal discount, decimal? AmountCusPayment,
            decimal Amount, decimal VATAmount, string Cashername,
            string IdCasher, bool vat, int? Vatrate, int? ManagerPatternEInvoices, EnumTypeProduct enumType = EnumTypeProduct.AMTHUC)
        {
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                var checkOrder = await _repository.Entities.Include(x => x.OrderTableItems)
                    .Include(x => x.Customer)
                    .Include(x => x.RoomAndTable)
                    .ThenInclude(x=>x.Area)
                    .SingleOrDefaultAsync(x => x.ComId == comid && x.IdGuid == idOrder && x.TypeProduct == enumType);
                if (checkOrder != null)
                {
                    checkOrder.Status = EnumStatusOrderTable.DA_THANH_TOAN;
                    checkOrder.CasherName = Cashername;
                    checkOrder.IdCasher = IdCasher;
                    checkOrder.PurchaseDate = DateTime.Now;
                    await _repository.UpdateAsync(checkOrder);

                    var inv = _map.Map<Invoice>(checkOrder);
                    inv.ArrivalDate = checkOrder.CreatedOn;
                    var getInv = await _managerInvNorepository.UpdateInvNo(comid, ENumTypeManagerInv.Invoice,false);

                    inv.InvoiceCode = $"HD-{getInv.ToString("00000000")}";
                    inv.Id = 0;
                    inv.PurchaseDate = checkOrder.PurchaseDate;
                    inv.IdPaymentMethod = Idpayment;
                    inv.IdOrderTable = checkOrder.Id;
                    inv.CasherName = Cashername;
                    inv.IdCasher = IdCasher;
                    var invitem = _map.Map<List<InvoiceItem>>(checkOrder.OrderTableItems);
                    invitem.ForEach(x => x.Id = 0);
                    
                    inv.InvoiceItems = invitem;
                    inv.Status = EnumStatusInvoice.DA_THANH_TOAN;
                    inv.DiscountAmount = discountPayment;
                    inv.Discount = (float)discount;
                    inv.Total = inv.Amonut;// tong tien chưa giam, gán amount bởi vì là giá trị gốc của hóa đơn ban đầu
                    inv.Amonut = inv.Amonut - discountPayment;// tiền  cần thanh toán đoạn này là sau khi hiển thị bill khách có nhập giảm giá hay k
                    inv.AmountCusPayment = AmountCusPayment.HasValue ? AmountCusPayment.Value : 0;// tieefn khasch đưa
                    if (vat && Vatrate!=null && Vatrate>0)
                    {
                        inv.VATAmount = VATAmount;
                        inv.Amonut = Amount;
                        inv.VATRate = Vatrate;
                    }
                    else
                    {
                        inv.VATRate = (int)VATRateInv.KHONGVAT;
                    }

                    if (inv.Amonut < inv.AmountCusPayment)
                    {
                        inv.AmountChangeCus = AmountCusPayment - inv.Amonut;//tiền thừa
                    }
                    if (checkOrder.Customer!=null)
                    {
                        inv.Buyer = checkOrder.Customer.Buyer?.Trim();
                        inv.CusName = checkOrder.Customer.Name?.Trim();
                        inv.Taxcode = checkOrder.Customer.Taxcode?.Trim();
                        inv.CusCode = checkOrder.Customer.Code?.Trim();
                        inv.PhoneNumber = checkOrder.Customer.PhoneNumber?.Trim();
                        inv.CCCD = checkOrder.Customer.CCCD?.Trim();
                        inv.Address = checkOrder.Customer.Address?.Trim();
                        inv.Email = checkOrder.Customer.Email?.Trim();
                        inv.CusBankNo = checkOrder.Customer.CusBankNo?.Trim();
                        inv.CusBankName = checkOrder.Customer.CusBankName?.Trim();
                    }
                    if (checkOrder.RoomAndTable!=null&&!checkOrder.IsBringBack)
                    {
                        inv.TableNameArea = $"{checkOrder.RoomAndTable.Name}";
                        if (checkOrder.RoomAndTable.Area!=null)
                        {
                            inv.TableNameArea += $",{checkOrder.RoomAndTable.Area.Name}";
                        }
                    }
                    else if (checkOrder.IsBringBack)
                    {
                        inv.TableNameArea = $"Mang về";
                    }
                    //add bảng
                    await _InvoiceRepository.AddAsync(inv);
                    await _unitOfWork.SaveChangesAsync();
                    inv.Customer = checkOrder.Customer;

                    //giwof tạo phiếu thu nhé
                    RevenueExpenditure revenueExpenditure = new RevenueExpenditure()
                    {
                        ComId = inv.ComId,
                        Amount = inv.Amonut,
                        IdInvoice = inv.Id,
                        ObjectRevenueExpenditure=EnumTypeObjectRevenueExpenditure.KHACHHANG,
                        IdCustomer= inv.IdCustomer,
                        Type = EnumTypeRevenueExpenditure.THU,
                        Typecategory = EnumTypeCategoryThuChi.TIENHANG,
                        Title =$"Thu {LibraryCommon.GetDisplayNameEnum(EnumTypeCategoryThuChi.TIENHANG).ToLower()}" ,
                        Date = inv.CreatedOn,
                        Code = $"PT{inv.InvoiceCode}",
                        CustomerName = inv.Buyer,
                        Status = EnumStatusRevenueExpenditure.HOANTHANH,
                        IdPayment = inv.IdPaymentMethod,
                    };
                    await _revenueExpenditureRepository.AddAsync(revenueExpenditure);
                    //
                    // his tory đơn
                    var his = new HistoryInvoice();
                    his.Carsher = Cashername;
                    his.InvoiceCode = inv.InvoiceCode;
                    his.IdInvoice = inv.Id;
                    his.Name = $"Thanh toán đơn";
                    await _historyInvoiceRepository.AddAsync(his);
                  
                    //-----------------------------------//
                    // update lại sản phẩm tồn kho
                    var list = new List<KeyValuePair<string, decimal>>();
                    foreach (var item in inv.InvoiceItems)
                    {
                        if (item.TypeProductCategory!=EnumTypeProductCategory.SERVICE && item.TypeProductCategory != EnumTypeProductCategory.COMBO)//lấy ra mấy csai k phải là combo và dịch vụ
                        {
                            list.Add(new KeyValuePair<string, decimal>(item.Code, item.Quantity * -1));
                        }
                        if (item.TypeProductCategory == EnumTypeProductCategory.COMBO)//nếu là combo nó sẽ có các thành phần cần lôi ra
                        {
                            //nếu là combo thì phải tìm ra các thành phần để update vào tồn kho
                            var getlistprobycombo =  _comboproductrepository.Entities.AsNoTracking().Where(x=>x.IdProduct== item.IdProduct).ToList();
                            if (getlistprobycombo.Count()>0)
                            {
                                var getlstid = getlistprobycombo.Select(x => x.IdPro).ToArray();
                                var listproductcombo = await _productrepository.GetProductbyListId(getlstid, inv.ComId);
                                foreach (var combo in getlistprobycombo)
                                {
                                    var getprobycombo = listproductcombo.SingleOrDefault(x=>x.Id== combo.IdPro);
                                    // kquarn lý tồn kho,k phải dịch vụ k phải combo mới update
                                    if (!getprobycombo.IsInventory && item.TypeProductCategory != EnumTypeProductCategory.SERVICE && item.TypeProductCategory != EnumTypeProductCategory.COMBO)
                                    {
                                        var quantity = (combo.Quantity * -1) * item.Quantity;
                                        list.Add(new KeyValuePair<string, decimal>(getprobycombo.Code, quantity));
                                    }
                                }
                            }
                        }
                    }

                   
                    await _productrepository.UpdateQuantity(list,comid);
                    //end
                    //--------------------------------//
                    await _unitOfWork.SaveChangesAsync();
                    _unitOfWork.Commit();
                    var getpayment = await _paymentMethodRepository.GetAll(inv.ComId).Where(x=>x.Id==inv.IdPaymentMethod).AsNoTracking().SingleOrDefaultAsync();
                    inv.PaymentMethod = getpayment;
                    string token = string.Empty;
                    PublishInvoiceResponse publishInvoiceResponse = new PublishInvoiceResponse();
                   
                    if (vat)
                    {
                        try
                        {
                            PublishInvoiceModel modelein = new PublishInvoiceModel();
                            modelein.ComId = inv.ComId;
                            modelein.IdManagerPatternEInvoice = ManagerPatternEInvoices.Value;
                            modelein.isVAT = vat;
                            modelein.VATRate = Vatrate.Value;
                            modelein.VATAmount = VATAmount;
                            modelein.Amount = Amount;
                            modelein.TypeSupplierEInvoice = ENumSupplierEInvoice.VNPT;
                            modelein.CasherName = Cashername;
                            modelein.IdCarsher = IdCasher;
                            //modelein.lstid = ids.ToArray();
                            var publishinv = await _InvoiceRepository.PublishInvoice(inv, modelein, inv.ComId, IdCasher, Cashername);
                            if (publishinv.Succeeded)
                            {
                                publishInvoiceResponse = publishinv.Data;
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }

                    if (!inv.IsBringBack && inv.IdCustomer.HasValue)
                    {
                        inv.Customer = _customerRepository.GetById(inv.IdCustomer.Value);
                    }
                    publishInvoiceResponse.Invoice = inv;
                    return await Result<PublishInvoiceResponse>.SuccessAsync(publishInvoiceResponse);
                }
                return await Result<PublishInvoiceResponse>.FailAsync(HeperConstantss.ERR043);

            }
            catch (Exception e)
            {
                _log.LogError("Thanh toán đơn lỗi: "+e.ToString());
                await _unitOfWork.RollbackAsync();
                return await Result<PublishInvoiceResponse>.FailAsync(e.ToString());
            }
        }

        public async Task<Result<string>> AddNote(int comid, Guid idOrder, string note)
        {
            var checkOrder = await _repository.Entities.SingleOrDefaultAsync(x => x.ComId == comid && x.IdGuid == idOrder);
            if (checkOrder == null)
            {
                return await Result<string>.FailAsync(HeperConstantss.ERR043);
            }
            checkOrder.Note = note;
            await _repository.UpdateAsync(checkOrder);
            await _unitOfWork.SaveChangesAsync();
            return await Result<string>.SuccessAsync(HeperConstantss.SUS006);
        }

        public async Task<Result<bool>> SplitOrderAsync(int comid, Guid idOrderOld, List<Guid> lstIdoder, EnumTypeSpitOrder Type, string CasherName, string IdCashername)//ghép
        {
            if (lstIdoder.Count() == 0)
            {
                return await Result<bool>.FailAsync(HeperConstantss.ERR044);
            }
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                List<Guid> guids = new List<Guid>();//list danh sách idorder cần ghép vào bàn mới
                guids = lstIdoder;
                guids.Add(idOrderOld);

                var checkOrder = await _repository.Entities.Include(x => x.OrderTableItems).SingleOrDefaultAsync(x => x.ComId == comid && x.IdGuid == idOrderOld);
                if (checkOrder == null)
                {
                    return await Result<bool>.FailAsync(HeperConstantss.ERR043);
                }
                var lstitemordernguyenthuy = checkOrder.OrderTableItems.ToList();

                List<OrderTableItem> orderTablesNewOrder = new List<OrderTableItem>();// danh sách các sp chưa bấm thông báo ở các đơn cần ghép
                List<OrderTableItem> orderTables = new List<OrderTableItem>();
                orderTables = checkOrder.OrderTableItems.ToList();

                await _repository.DeleteAsync(checkOrder);
                await _unitOfWork.SaveChangesAsync();
                var lstod = await _repository.Entities.Include(x => x.OrderTableItems).Where(x => x.ComId == comid && lstIdoder.Contains(x.IdGuid)).ToListAsync();
                var newOrder = lstod.FirstOrDefault();// đây là bàn mới nè lựa chọn đại 1 cái
                newOrder.RoomAndTable = await _roomadntableRepository.GetByIdAsync(newOrder.IdRoomAndTable.Value);
                guids.Remove(newOrder.IdGuid);// xóa đi cái bàn cần ghép vào nè
                if (lstod.Count() > 1)
                {
                    // add các item của đơn k phải đơn ms vào 1 danh sách
                    foreach (var item in lstod)
                    {
                        if (item.Id != newOrder.Id)
                        {
                            // lọc lấy sp mới chưa thông báo ở các đơn chọn để ghép

                            orderTables.AddRange(item.OrderTableItems.ToList());
                        }
                    }
                    var newOrderremove = lstod.Where(x => x.Id != newOrder.Id);
                    await _repository.DeleteRangeAsync(newOrderremove);
                    await _unitOfWork.SaveChangesAsync();
                }
                foreach (var itemorder in orderTables)
                {
                    if (itemorder.Quantity > itemorder.QuantityNotifyKitchen)
                    {
                        var _dt = itemorder.CloneJson();
                        _dt.Quantity = itemorder.Quantity - itemorder.QuantityNotifyKitchen;
                        _dt.IdOrderTable = newOrder.Id;
                        orderTablesNewOrder.Add(_dt);
                    }

                }


                orderTables.ForEach(x => { x.IdOrderTable = 0; x.Id = 0; });
                //update cái sp nào dg có Quantity và tiền
                foreach (var x in newOrder.OrderTableItems)
                {
                    // lọc lấy sp mới chưa thông báo tiếp tục add

                    if (x.Quantity > x.QuantityNotifyKitchen)
                    {
                        var _dt = x.CloneJson();
                        _dt.Quantity = x.Quantity - x.QuantityNotifyKitchen;
                        _dt.IdOrderTable = newOrder.Id;
                        orderTablesNewOrder.Add(_dt);
                    }


                    var getitem = orderTables.SingleOrDefault(z => z.Code == x.Code);
                    if (getitem != null)
                    {
                        x.Quantity += getitem.Quantity;
                        x.QuantityNotifyKitchen = x.Quantity;
                        x.Total += getitem.Total;
                    }
                }

                //lấy cái k có trong đơn mới để add vào
                var lstNew = orderTables.Where(z => !newOrder.OrderTableItems.Select(x => x.Code).ToArray().Contains(z.Code));
                if (lstNew.Count() > 0)
                {
                    foreach (var item in lstNew)
                    {
                        item.IdOrderTable = 0;
                        item.Id = 0;
                        item.QuantityNotifyKitchen = item.Quantity;
                        newOrder.OrderTableItems.Add(item);
                    }
                }

                newOrder.Amonut = newOrder.OrderTableItems.Sum(x => x.Total);
                newOrder.Quantity = newOrder.OrderTableItems.Sum(x => x.Quantity);
                newOrder.CasherName = CasherName;
                newOrder.IdCasher = IdCashername;
                await _repository.UpdateAsync(newOrder);

                await _notifyChitkenRepository.UpdateNotifyKitchenSpitOrderGraftAsync(comid, guids, newOrder, orderTablesNewOrder);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();


                return await Result<bool>.SuccessAsync(HeperConstantss.SUS006);
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                return await Result<bool>.FailAsync(e.Message);
            }


        }

        public async Task<Result<bool>> SplitOrderSeparateAsync(int comid, Guid idOrderOld, List<DetailtSpitModel> lstitem, bool IsCreate, bool IsBringBack, Guid? IdOrderNew, Guid? IdTable, string CasherName, string IdCasher)
        {
            //tách
            await _unitOfWork.CreateTransactionAsync();
            try
            {

                OrderTable model = new OrderTable();
                model.ComId = comid;
                //lấy bàn cũ cần chyển đi
                var getBr = await _repository.Entities.Include(x => x.OrderTableItems).Include(x => x.RoomAndTable).SingleOrDefaultAsync(x => x.IdGuid == idOrderOld && x.ComId == comid && x.Status == EnumStatusOrderTable.DANG_DAT);
                if (getBr == null)
                {
                    return await Result<bool>.FailAsync(HeperConstantss.ERR012);
                }
                var orderitemNguyenthuy = getBr.OrderTableItems.ToList();//giữ cái gốc để tìm kiếm
                bool isValid = false;
                lstitem.ForEach(x =>
                {
                    var getdt = orderitemNguyenthuy.SingleOrDefault(z => z.IdGuid == x.idOrderItem);
                    if (getdt == null)
                    {
                        isValid = true;
                    }
                    x.Code = getdt.Code;
                    x.IdProduct = getdt.IdProduct;
                });// danh sachs item cần tách

                if (isValid)
                {
                    return await Result<bool>.FailAsync(HeperConstantss.ERR001);
                }
                List<OrderTableItem> OrderTableItemold = new List<OrderTableItem>();
                List<OrderTableItem> OrderTableItemnewOld = new List<OrderTableItem>(); // item mới của đơn cũ còn chưa báo bếp    
                List<OrderTableItem> OrderTableItemremove = new List<OrderTableItem>();
                foreach (var item in getBr.OrderTableItems)
                {
                    var getitem = lstitem.SingleOrDefault(x => x.idOrderItem == item.IdGuid);// lấy item trong list từ view đưa vào
                    if (getitem != null)
                    {
                        if (item.Quantity != getitem.Quantity && getitem.Quantity < item.Quantity)// có lấy đi một ít
                        {

                            decimal quannotify = 0;
                            if (item.Quantity == item.QuantityNotifyKitchen)//TH =
                            {
                                item.QuantityNotifyKitchen = item.QuantityNotifyKitchen - getitem.Quantity.Value;

                            }
                            else if (item.Quantity > item.QuantityNotifyKitchen)//TH  đã thông báo 1 ít
                            {
                                var qut = item.Quantity - item.QuantityNotifyKitchen;// số lượng còn chưa thông báo bếp
                                if (qut >= getitem.Quantity.Value)//nếu lớn hơn số lượng  cần tách thì lấy số lượng còn lại báo cho đơn cũ và đơn mới vs sl mới
                                {
                                    quannotify = getitem.Quantity.Value;//sl báo cho đơn mới k phải là sl chuyển, sl còn lại thì tự hệ thống check báo new list dưới
                                    if (qut - quannotify > 0)
                                    {
                                        var _dtnew = item.CloneJson();
                                        _dtnew.Quantity = qut - quannotify;
                                        OrderTableItemnewOld.Add(_dtnew);
                                    }
                                    //item.QuantityNotifyKitchen = item.Quantity - getitem.Quantity.Value;
                                }
                                else
                                {
                                    quannotify = qut;// nếu như k đủ thì cứ chuyển hết cho cái mới thôi
                                                     // qut = getitem.Quantity.Value - qut;
                                                     //item.QuantityNotifyKitchen = qut+ getitem.Quantity.Value;
                                    item.QuantityNotifyKitchen = item.Quantity - getitem.Quantity.Value;

                                }
                            }
                            //quannotify = getitem.Quantity.Value;
                            item.Quantity = item.Quantity - getitem.Quantity.Value;
                            item.Total = Convert.ToDecimal(item.Quantity) * item.Price;

                            lstitem.ForEach(x =>
                            {
                                if (x.Code == item.Code)
                                {
                                    x.QuantityNotifyKitchen = quannotify;// dùng số này báo cho đơn mới

                                }
                            });// danh sachs item cần tách

                            OrderTableItemold.Add(item);//danh sách sau khi  đã up
                        }
                        else
                        {
                            // khi = nhau tức là tách hết sản phẩm luôn nha item.Quantity = getitem.Quantity 
                            OrderTableItemremove.Add(item);
                        }
                    }
                    else
                    {
                        OrderTableItemold.Add(item);
                    }
                }
                if (OrderTableItemremove.Count() > 0)
                {
                    if (OrderTableItemremove.Count() == getBr.OrderTableItems.Count())
                    {
                        return await Result<bool>.FailAsync(HeperConstantss.ERR046);
                    }
                    await _OrderTableItemrepository.DeleteRangeAsync(OrderTableItemremove);
                    await _unitOfWork.SaveChangesAsync();
                }

                getBr.Amonut = getBr.OrderTableItems.Sum(x => x.Total);
                getBr.Quantity = getBr.OrderTableItems.Sum(x => x.Quantity);
                await _repository.UpdateAsync(getBr); //update lại đơn cũ 
                await _unitOfWork.SaveChangesAsync();
                if (IsCreate)// tường hơp tạo ra đơn mới chứ k tách vào 1 đươn nào đó dg có
                {
                    model.CasherName = CasherName;
                    model.IdCasher = IdCasher;
                    model.IsBringBack = IsBringBack;
                    model.IdCustomer = getBr.IdCustomer;
                    model.IdCasher = getBr.IdCasher;
                    model.IsRetailCustomer = getBr.IsRetailCustomer;
                    model.Buyer = getBr.Buyer;
                    model.TypeProduct = getBr.TypeProduct;

                    List<OrderTableItem> OrderTableItemS = new List<OrderTableItem>();
                    List<OrderTableItem> OrderTableItemNewNotifyOrder = new List<OrderTableItem>();// list cho đơn mới cần báo số lượng này chưa báo bếp lần nào mà  chuyeenren từ đơn dc chọn qua
                    foreach (var item in lstitem)//DANH SÁch MỚi
                    {
                        var getItme = orderitemNguyenthuy.SingleOrDefault(x => x.IdGuid == item.idOrderItem);
                        OrderTableItem orderTableItem = new OrderTableItem();
                        orderTableItem.IdGuid = Guid.NewGuid();
                        orderTableItem.Code = getItme.Code;
                        orderTableItem.Name = getItme.Name;
                        orderTableItem.Unit = getItme.Unit;
                        orderTableItem.Price = getItme.Price;
                        orderTableItem.IdProduct = getItme.IdProduct;
                        orderTableItem.Discount = getItme.Discount;
                        orderTableItem.DiscountAmount = getItme.DiscountAmount;
                        orderTableItem.EntryPrice = getItme.EntryPrice;
                        orderTableItem.Quantity = item.Quantity.Value;
                        orderTableItem.QuantityNotifyKitchen = item.Quantity.Value;
                        orderTableItem.Total = Convert.ToDecimal(orderTableItem.Quantity) * getItme.Price;
                        OrderTableItemS.Add(orderTableItem);
                      //  if (item.QuantityNotifyKitchen > 0)
                        //{
                            var cloneitem = orderTableItem.CloneJson();
                            cloneitem.QuantityNotifyKitchen = item.QuantityNotifyKitchen;
                            if (item.QuantityNotifyKitchen == 0)//mục đich nếu đã báo bếp thì lấy số lượng từ đơn gốc là clone, còn chưa báo gì lấy số lượng từ truyền vào
                            {
                                cloneitem.Quantity = (item.Quantity == null ? 0 : item.Quantity.Value);
                            }
                            OrderTableItemNewNotifyOrder.Add(cloneitem);
                       // }
                       
                    }

                    if (IsBringBack)
                    {
                        model.IdRoomAndTable = null;
                    }
                    else
                    {
                        if (IdTable == null)
                        {
                            return await Result<bool>.FailAsync(HeperConstantss.ERR045);
                        }
                        var getphong = await _roomadntableRepository.Entities.SingleOrDefaultAsync(x => x.IdGuid == IdTable.Value && x.ComId == comid);
                        model.IdRoomAndTable = getphong.Id;
                        model.IdRoomAndTableGuid = getphong.IdGuid;
                    }

                    string cn = "en-US"; //Vietnamese
                    var _cultureInfo = new CultureInfo(cn);
                    var checkOrder = await _managerInvNorepository.UpdateInvNo(model.ComId, ENumTypeManagerInv.OrderTable,false);
                    //model.IdGuid = Guid.NewGuid();
                    model.OrderSort = checkOrder;
                    model.OrderTableCode = $"OD-{model.OrderSort}";
                    model.OrderTableItems = OrderTableItemS;
                    model.Amonut = OrderTableItemS.Sum(x => x.Total);
                    model.Quantity = OrderTableItemS.Sum(x => x.Quantity);
                    await _repository.AddAsync(model);
                    await _unitOfWork.SaveChangesAsync();
                    string randowm = LibraryCommon.RandomString(8);

                    List<OrderTableItem> lstItemNewInDonMoi = new List<OrderTableItem>();// danh sách item mới cho đơn mới, là sl chưa thông báo ở bàn cũ mà chuyển qua
                    List<HistoryOrder> lsthsi = new List<HistoryOrder>();
                    DateTime CreateDateHis = DateTime.Now;
                    foreach (var item in OrderTableItemNewNotifyOrder)//báo cho đơn mới
                    {

                        var his = new HistoryOrder()
                        {
                            ProductName = item.Name,
                            IdProduct = item.IdProduct,
                            Code = randowm,
                            IsNotif = true,
                            CreateDate = CreateDateHis,
                            NewTableName = model.RoomAndTable != null ? model.RoomAndTable.Name : "mang về",
                            OrderCode = model.OrderTableCode,
                            IdOrderTable = model.Id,
                            Carsher = CasherName,
                            Quantity = item.Quantity,
                        };
                        if (0 == item.QuantityNotifyKitchen)
                        {
                            his.TypeKitchenOrder = EnumTypeKitchenOrder.THEM;
                            his.Name = $"+ {item.Quantity.ToString("0.###")} {item.Name}";
                            //his.Name = $"+ {item.Quantity.ToString("0.###")} {item.Name} (được chuyển từ {(getBr.RoomAndTable != null ? getBr.RoomAndTable.Name : "mang về")} sang)".ToLower();
                            //vì là chưa báo bếp là món mới từ bàn kia chuyển qua nên báo bếp nhé add vào  danh sách new của đơn mới 
                            var newitem = item.CloneJson();
                            newitem.Quantity = item.Quantity;
                            lstItemNewInDonMoi.Add(newitem);
                        }
                        else
                        {
                            his.TypeKitchenOrder = EnumTypeKitchenOrder.THEM;
                            decimal newqua = item.Quantity - item.QuantityNotifyKitchen;
                            if (newqua > 0)
                            {


                                his.Name = $"+ {item.QuantityNotifyKitchen} {item.Name}";
                                var newitem = item.CloneJson();
                                newitem.Quantity = item.QuantityNotifyKitchen;
                                lstItemNewInDonMoi.Add(newitem);
                                var his2 = new HistoryOrder()
                                {
                                    TypeKitchenOrder = EnumTypeKitchenOrder.CHUYEN,
                                    ProductName = item.Name,
                                    IdProduct = item.IdProduct,
                                    Code = randowm,
                                    IsNotif = true,
                                    CreateDate = his.CreateDate,// cho cùng lúc
                                    NewTableName = model.RoomAndTable != null ? model.RoomAndTable.Name : "mang về",
                                    OrderCode = model.OrderTableCode,
                                    IdOrderTable = model.Id,
                                    Carsher = CasherName,
                                    Quantity = item.Quantity,
                                };

                                his2.Name = $"+ {newqua.ToString("0.###")} {item.Name} (được chuyển từ {(getBr.RoomAndTable != null ? getBr.RoomAndTable.Name : "mang về")} sang đã báo bếp trước đó)".ToLower();
                                lsthsi.Add(his2);
                            }
                            else
                            {
                                lstItemNewInDonMoi.Add(item);
                                his.Name = $"+ {item.Quantity.ToString("0.###")} {item.Name}";
                            }

                        }
                        lsthsi.Add(his);
                    }
                    this.AddHistoryOrder(lsthsi);
                    bool isnew = false;

                    //if (lstItemNewInDonMoi.Count() > 0)
                    //{
                    //    isnew = true;

                    //}
                    await _notifyChitkenRepository.UpdateNotifyKitchenSpitOrderAsync(getBr, OrderTableItemold, comid, model, model.OrderTableItems.ToList(), isnew, CreateDateHis);


                    if (lstItemNewInDonMoi.Count() > 0)
                    {
                        await _notifyChitkenRepository.NotifyOrderByItem(lstItemNewInDonMoi, model, CasherName, IdCasher);

                    }
                    //if (OrderTableItemnewOld.Count()>0)
                    //{
                    //    await _notifyChitkenRepository.NotifyOrderByItem(OrderTableItemnewOld, getBr,CasherName,IdCasher);
                    //}

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    await _notifyChitkenRepository.NotifyOrder(comid, getBr.IdGuid,CasherName);

                }
                else  //tách vào 1 bàn khác một ít
                {
                    //lấy bàn mới đã chọn cần tách vào
                    var getod = await _repository.Entities.Include(x => x.OrderTableItems).Include(x => x.RoomAndTable).SingleOrDefaultAsync(x => x.IdGuid == IdOrderNew && x.ComId == comid && x.Status == EnumStatusOrderTable.DANG_DAT);
                    if (getod == null)
                    {
                        return await Result<bool>.FailAsync(HeperConstantss.ERR012);
                    }


                    var lstnoex = new List<Guid>();
                    foreach (var x in getod.OrderTableItems)
                    {
                        var geti = lstitem.SingleOrDefault(z => z.Code == x.Code);// tìm cái trpong list mới có thì update vào cái danh sách hiện tại của đơn mới đó
                        if (geti != null)
                        {
                            lstnoex.Add(geti.idOrderItem.Value);
                            x.Quantity = x.Quantity + geti.Quantity.Value;
                            x.QuantityNotifyKitchen = x.QuantityNotifyKitchen + geti.Quantity.Value;// mục đích để tí kiểm tra và báo lại 
                            x.Total = Convert.ToDecimal(x.Quantity) * x.Price;
                        }

                    }// update các món dg có 


                    var newlstitem = lstitem.Where(x => !lstnoex.Contains(x.idOrderItem.Value)).ToList();//tìm cái còn lại k có, vì cái có đã update bên trên
                    var lstNew = new List<OrderTableItem>();
                    if (newlstitem.Count() > 0)
                    {
                        var getNew = orderitemNguyenthuy.Where(x => newlstitem.Select(x => x.idOrderItem).ToArray().Contains(x.IdGuid)).ToList();// danh sasch item cos  trong order cũ

                        foreach (var item in getNew)
                        {
                            var geti = newlstitem.SingleOrDefault(z => z.idOrderItem == item.IdGuid);// tìm cái trpong list mới có thì update vào
                            OrderTableItem orderTableItem = new OrderTableItem();
                            orderTableItem.Code = item.Code;
                            orderTableItem.Name = item.Name;
                            orderTableItem.Unit = item.Unit;
                            orderTableItem.IdProduct = item.IdProduct;
                            orderTableItem.Price = item.Price;
                            orderTableItem.EntryPrice = item.EntryPrice;
                            orderTableItem.Quantity = geti.Quantity.Value;
                            orderTableItem.QuantityNotifyKitchen = geti.Quantity.Value;
                            orderTableItem.Total = Convert.ToDecimal(geti.Quantity.Value) * item.Price;
                            getod.OrderTableItems.Add(orderTableItem);// add thèm vào cái mới với item chwua có
                        }

                    }
                    // getod.OrderTableItems = tl;
                    getod.Amonut = getod.OrderTableItems.Sum(x => x.Total);
                    getod.Quantity = getod.OrderTableItems.Sum(x => x.Quantity);
                    

                    //danh sách món mà có chứa các món chưa thông báo hết cho bép, ví dụ, có 5 món mà thông báo 3, còn 2 chưa thông báo mà đã dc tách từ bàn kahsc hêm vào

                    var newlist = lstitem.Select(x => new OrderTableItem()
                    {
                        IdProduct = x.IdProduct,
                        IdGuid = x.idOrderItem.Value,
                        Quantity = x.Quantity.Value,
                    }).ToList();


                    await _notifyChitkenRepository.UpdateNotifyKitchenTachdonVaoDonDacoAsync(getBr, OrderTableItemold, comid, getod, newlist, OrderTableItemremove, CasherName, IdCasher);
                    await _repository.UpdateAsync(getod);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();


                }
                return await Result<bool>.SuccessAsync(HeperConstantss.SUS006);
            }

            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                return await Result<bool>.FailAsync(e.Message);
            }

        }

        public async Task<Result<string>> ConvertInvoice(int comid, Guid idOrder, EnumTypeProduct enumTypeProduct = EnumTypeProduct.BAN_LE)
        {
            var get = await _repository.Entities.SingleOrDefaultAsync(x => x.ComId == comid && x.IdGuid == idOrder && x.TypeProduct == enumTypeProduct);
            if (get != null)
            {
                if (get.TypeInvoice == EnumTypeInvoice.INVOICE)
                {
                    get.TypeInvoice = EnumTypeInvoice.INVOICE_ORDER;
                    get.OrderTableCode = $"DH-{get.OrderSort}";
                }
                else
                {
                    get.TypeInvoice = EnumTypeInvoice.INVOICE;
                    get.OrderTableCode = $"HD-{get.OrderSort}";
                }

                await _repository.UpdateAsync(get);
                await _unitOfWork.SaveChangesAsync();
                return await Result<string>.SuccessAsync(get.OrderTableCode, HeperConstantss.SUS006);
            }
            return await Result<string>.FailAsync();
        }

        public async Task<Result<bool>> RemoveCustomerOrder(int comid, Guid idOrder, EnumTypeProduct enumTypeProduct)
        {
            var get = await _repository.Entities.SingleOrDefaultAsync(x => x.ComId == comid && x.IdGuid == idOrder && x.TypeProduct == enumTypeProduct);
            if (get != null)
            {
                get.IdCustomer = null;
                get.IsRetailCustomer = true;
                get.Buyer = "Khách lẻ";

                await _repository.UpdateAsync(get);
                await _unitOfWork.SaveChangesAsync();
                return await Result<bool>.SuccessAsync(true);
            }
            return await Result<bool>.FailAsync();
        }

        public async Task<Result<OrderTable>> UpdateAllQuantityOrderTable(int comid, Guid idOrder, Guid idOrderItem, decimal quantity)
        {
            var get = await _repository.Entities.Include(x => x.OrderTableItems).SingleOrDefaultAsync(x => x.ComId == comid && x.IdGuid == idOrder);
            if (get != null)
            {
                get.OrderTableItems.ForEach(x =>
                {
                    if (x.IdGuid == idOrderItem)
                    {
                        x.Quantity = quantity;
                        x.Total = Convert.ToDecimal(quantity) * x.Price;
                    }
                });
                get.Quantity = get.OrderTableItems.Sum(x => x.Quantity);
                get.Amonut = get.OrderTableItems.Sum(x => x.Total);
                await _repository.UpdateAsync(get);
                await _unitOfWork.SaveChangesAsync();
                return await Result<OrderTable>.SuccessAsync(get);
            }
            return await Result<OrderTable>.FailAsync();
        }

        public IQueryable<OrderTable> GetOrderInvoiceRetail(int ComId, EnumStatusOrderTable enumStatusOrderTable, EnumTypeProduct enumTypeProduct = EnumTypeProduct.BAN_LE)
        {
            return _repository.Entities.Where(x => x.ComId == ComId && x.Status == enumStatusOrderTable && x.TypeProduct == enumTypeProduct);
        }

        public async Task<Result<OrderTable>> UpdateTableOrRoomOfOrder(int comid, bool isBringBack, Guid idOrder, Guid? idOldTableOrder, Guid? idRoomOrtable, EnumTypeProduct enumTypeProduct = EnumTypeProduct.AMTHUC)
        {
            RoomAndTable roomAndTable = new RoomAndTable();
            OrderTable get = await _repository.Entities.SingleOrDefaultAsync(x => x.ComId == comid && x.IdGuid == idOrder);
            if (get == null)
            {
                return await Result<OrderTable>.FailAsync(HeperConstantss.ERR012);
            }
            //if (isBringBack)
            //{
            //    get = 
            //}
            //else
            //{
            //    get = await _repository.Entities.SingleOrDefaultAsync(x => x.ComId == comid && x.IdGuid == idOrder && x.IdRoomAndTableGuid == idOldTableOrder);
            //}

            if (isBringBack)
            {
                get.IdRoomAndTableGuid = null;
                get.IdRoomAndTable = null;
                get.IsBringBack = true;
            }
            else
            {
                if (idRoomOrtable==null)
                {
                    return await Result<OrderTable>.FailAsync("Bàn không được để trống");
                }
                var getTable = await _roomadntableRepository.Entities.SingleOrDefaultAsync(x => x.IdGuid == idRoomOrtable.Value);
                if (getTable==null)
                {
                    return await Result<OrderTable>.FailAsync("Bàn không được để trống");
                }
                get.IdRoomAndTableGuid = getTable.IdGuid;
                get.IdRoomAndTable = getTable.Id;
                get.IsBringBack = false;
                roomAndTable = getTable;
            }
            await _repository.UpdateAsync(get);

            await _notifyChitkenRepository.UpdateNotifyAllByRoomTable(comid, get, get.TypeProduct);

            await _unitOfWork.SaveChangesAsync();
            get.RoomAndTable = roomAndTable;
            return await Result<OrderTable>.SuccessAsync(get);
        }
    }
}
