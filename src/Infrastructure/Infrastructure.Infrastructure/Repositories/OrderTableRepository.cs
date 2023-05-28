using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using Hangfire.Logging;
using HelperLibrary;
using Joker.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using NStandard;
using NStandard.Evaluators;
using Org.BouncyCastle.Asn1.Ocsp;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Infrastructure.Infrastructure.Repositories
{
    public class OrderTableRepository : IOrderTableRepository
    {
        private readonly ITemplateInvoiceRepository<TemplateInvoice> _templateInvoicerepository;
        private readonly IMapper _map;
        private readonly ICompanyAdminInfoRepository _companyProductRepository;
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
            ICompanyAdminInfoRepository companyProductRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IRevenueExpenditureRepository<RevenueExpenditure> revenueExpenditureRepository,
            IHistoryOrderRepository<HistoryOrder> historyOrderRepository,
            INotifyChitkenRepository notifyChitkenRepository,
            IRepositoryAsync<RoomAndTable> roomadntableRepository,
            IRepositoryAsync<Customer> customerRepository,
            IMapper map,
            IRepositoryAsync<HistoryInvoice> historyInvoiceRepository, 
            ITemplateInvoiceRepository<TemplateInvoice> templateInvoicerepository,
            IInvoicePepository<Invoice> InvoiceRepository,
            IDetailtKitchenRepository<DetailtKitchen> detailtKitchenRepository,
            IManagerInvNoRepository managerInvNorepository, IRepositoryAsync<OrderTableItem> OrderTableItemrepository,
            IUnitOfWork unitOfWork, IRepositoryAsync<OrderTable> repository)
        {
            _log = log;
            _templateInvoicerepository = templateInvoicerepository;
            _companyProductRepository = companyProductRepository;
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
            List<NotifyOrderNewModel> notifyOrderNewModels = new List<NotifyOrderNewModel>();
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
                var getItem = order.OrderTableItems.Where(x =>x.IdGuid == idItem).SingleOrDefault();
                if (getItem != null)
                {
                    bool checkupdatequantity = false;//
                    if (Quantity != -1)
                    {
                        checkupdatequantity = true;
                    }
                    if (IsCancelItem)//nếu hủy món mà đã thông báo bép thì luuw lịch sử lại IsCancelItem là đã check ở view nếu sl đã báo bếp lớn hơn sl sau khi đã hủy thì update true IsCancelItem
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
                        // in
                        notifyOrderNewModels.Add(new NotifyOrderNewModel()
                        {
                            Code= getItem.Code,
                            Name= getItem.Name,
                            Unit = getItem.Unit,
                            Price= getItem.Price,
                            Quantity= _newquantity<0?_newquantity*-1:_newquantity,
                            RoomTableName = order.IsBringBack ? "Mang về" : order.RoomAndTable?.Name,
                            StaffName = Cashername,
                        });
                        order.NotifyOrderNewModels = notifyOrderNewModels;
                        // thông báo cho bếp nữa nhé
                       // await _notifyChitkenRepository.UpdateNotifyKitchenCancelAsync(comid, idOrder, getItem.IdProduct.Value, _newquantity, Cashername,IdCasher);
                        await _notifyChitkenRepository.UpdateNotifyKitchenCancelAsync(comid, idOrder, getItem.Id, _newquantity, Cashername,IdCasher);

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

                                if (x.IsVAT)
                                {
                                    x.Total = x.Quantity * x.PriceNoVAT;
                                    x.VATAmount = Math.Round(x.Total * (x.VATRate / 100),  Application.Constants.MidpointRoundingCommon.Three); 
                                    x.Amount = x.Quantity * x.Price;
                                }
                                else
                                {
                                    x.Total = x.Quantity * x.Price;
                                    x.Amount = x.Quantity * x.Price;
                                }
                                //x.Total = Convert.ToDecimal(x.Quantity) * x.Price;

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

                        order.Amonut = Math.Round(order.OrderTableItems.Sum(x => x.Amount
                        ), MidpointRounding.AwayFromZero);
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
               // _unitOfWork.Dispose();
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
                    var getItem = order.OrderTableItems.Where(x => x.IdOrderTable == order.Id && x.IdProduct == product.Id).FirstOrDefault();
                    if (getItem != null)
                    {
                        if (getItem.IsServiceDate)
                        {
                            return Result<OrderTable>.Fail("Sản phẩm là dịch vụ tính tiền theo giờ chỉ được thêm 1 lần");
                        }
                        //phải chạy lai list đó vì nếu k sẽ k update dc list đó
                        order.OrderTableItems.ToList().ForEach(x =>
                        {
                            //if (x.IdOrderTable == order.Id && x.IdProduct == product.Id)
                            if (x.IdOrderTable == order.Id && x.Id == getItem.Id)
                            {
                                x.Quantity = x.Quantity + 1;
                                if (x.IsVAT)
                                {
                                    x.Total = x.Quantity * x.PriceNoVAT;
                                    x.VATAmount = Math.Round(x.Total * (x.VATRate / 100), Application.Constants.MidpointRoundingCommon.Three);
                                    x.Amount = x.Quantity * x.Price;
                                }
                                else
                                {
                                    x.Total = x.Quantity * x.Price;
                                    x.Amount = x.Quantity * x.Price;
                                }
                                x.TypeProductCategory = product.TypeProductCategory;
                            }
                        });
                    }
                    else
                    {
                        order.OrderTableItems.Add(this.MapOrderTableItem(product, item.DateCreateService));
                    }
                    // var amount = await UpdateItem(order.Id, product);

                    order.Amonut = Math.Round(order.OrderTableItems.Sum(x => x.Amount), MidpointRounding.AwayFromZero);
                    order.Quantity = order.OrderTableItems.Sum(x => x.Quantity);
                    if (IsNewOrder)
                    {
                        order.IdCustomer = model.IdCustomer;
                        order.IsBringBack = model.IsBringBack;
                        order.IsRetailCustomer = model.IsRetailCustomer;
                        order.Buyer = model.Buyer;
                    }

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
            item.VATRate = product.VATRate;
            item.IsVAT = product.IsVAT;
            item.PriceNoVAT = product.PriceNoVAT;
            item.EntryPrice = product.RetailPrice;// giá nhập vào
            if (product.IsVAT)
            {
                item.Total = product.PriceNoVAT;
                item.VATAmount = Math.Round(item.Total * (product.VATRate / 100), Application.Constants.MidpointRoundingCommon.Three);
                item.Amount = product.Price;
            }
            else
            {
                item.VATRate = (int)NOVAT.NOVAT;
                item.Total = product.Price;
                item.Amount = product.Price;
            }
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

        public async Task<Result<OrderTableModel>> RemoveOrder(int comid, Guid idOrder,string CasherName, string IdCashername, EnumTypeProduct enumTypeProduct = EnumTypeProduct.AMTHUC)
        {
            OrderTableModel orderTableModel = new OrderTableModel();
            var checkOrder = await _repository.Entities.Include(x=>x.OrderTableItems).SingleOrDefaultAsync(x => x.ComId == comid && x.IdGuid == idOrder && x.TypeProduct == enumTypeProduct);
            if (checkOrder != null)
            {
                await _repository.DeleteAsync(checkOrder);
                List<Kitchen> lstId = new List<Kitchen>();
                lstId.Add(new Kitchen()
                {
                    IdOrder = idOrder
                });
                if (enumTypeProduct == EnumTypeProduct.AMTHUC)
                {
                    var update = await _notifyChitkenRepository.UpdateNotifyKitchenCancelListAsync(lstId, comid, CasherName, IdCashername);
                    if (update.Count() > 0)
                    {
                        orderTableModel.NotifyOrderNewModels = update;

                    }
                }
               
              
                await _unitOfWork.SaveChangesAsync();
                orderTableModel.IdOrder = checkOrder.Id;
                orderTableModel.IdGuid = checkOrder.IdGuid;
                orderTableModel.IdRoomAndTableGuid = checkOrder.IdRoomAndTableGuid;
                orderTableModel.IsBringBack = checkOrder.IsBringBack;
                //update lại list notify
                orderTableModel.NotifyOrderNewModels.ForEach(x =>
                {
                    var getorder = orderTableModel.OrderTableItems.FirstOrDefault(z => z.Code == x.Code);
                    if (getorder != null)
                    {
                        x.Price = getorder.Price;
                        x.Unit = getorder.Unit;
                    }
                });

                return await Result<OrderTableModel>.SuccessAsync(orderTableModel, HeperConstantss.SUS007);
            }
            return await Result<OrderTableModel>.FailAsync();
        }

        public async Task<Result<PublishInvoiceResponse>> CheckOutOrderAsync(int comid, int Idpayment, Guid idOrder,
            decimal discountPayment,  decimal discount, decimal? AmountCusPayment,
            decimal Amount, decimal VATAmount, string Cashername,
            string IdCasher, bool vat, int? Vatrate, int? ManagerPatternEInvoices, EnumTypeProduct enumType = EnumTypeProduct.AMTHUC)
        {
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                bool IsProductVAT = false;//đánh dấu sản phẩm có thuế
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
                    checkOrder.PurchaseDate = DateTime.Now; //phát hành mà có xuất hóa đơn thì phải update lại tiền thuế và tiền sau thuế
                    await _repository.UpdateAsync(checkOrder);

                    var inv = _map.Map<Invoice>(checkOrder);
                    inv.ArrivalDate = checkOrder.CreatedOn;//giờ vào
                    var getInv = await _managerInvNorepository.UpdateInvNo(comid, ENumTypeManagerInv.Invoice,false);

                    inv.InvoiceCode = $"HD-{getInv.ToString("00000000")}";
                    inv.Id = 0;
                    inv.PurchaseDate = checkOrder.PurchaseDate;//giờ thanh toán
                    inv.IdPaymentMethod = Idpayment;
                    inv.IdOrderTable = checkOrder.Id;
                    inv.CasherName = Cashername;
                    inv.IdCasher = IdCasher;
                    //check item order và map, vì có 1 sản phẩm nhiều dòng
                    var newlistitem = new List<InvoiceItem>();
                    foreach (var item in checkOrder.OrderTableItems.GroupBy(x=>x.IdProduct))
                    {
                        var _item = item.First().CloneJson();
                        _item.Quantity = item.Sum(x => x.Quantity);
                        var invitem = _map.Map<InvoiceItem>(_item);
                        invitem.Total = item.Sum(x => x.Total); // phải làm vậy mới lấy cùng các sp
                        invitem.Amonut = item.Sum(x => x.Amount); // do lỗi trường nên phải làm ri
                        invitem.VATAmount = item.Sum(x => x.VATAmount); // do lỗi trường nên phải làm ri
                        invitem.DiscountAmount = item.Sum(x => x.DiscountAmount); // do lỗi trường nên phải làm ri

                        if (vat && Vatrate != null && Vatrate != (int)NOVAT.NOVAT && !_item.IsVAT) //sp k có thuế, mà có xuất hóa đơn hoặc có tính thuế
                        {
                            var thue = Vatrate.Value / 100.0m;
                            invitem.VATRate = Vatrate.Value;
                            invitem.VATAmount = Math.Round(invitem.Total * thue, MidpointRoundingCommon.Three);
                            invitem.Amonut = invitem.Total + invitem.VATAmount;
                        }
                        else if (!_item.IsVAT)
                        {
                            //invitem.Amonut = _item.Total;
                            invitem.VATRate = (int)NOVAT.NOVAT;
                        }
                        else
                        {
                            IsProductVAT = true;
                        }
                        //----
                        invitem.Id = 0;
                        newlistitem.Add(invitem);
                    }
                    //check item order và map
                    //var invitem = _map.Map<List<InvoiceItem>>(newlistitem);
                    //invitem.ForEach(x => x.Id = 0);
                    

                    inv.InvoiceItems = newlistitem;
                    inv.Status = EnumStatusInvoice.DA_THANH_TOAN;
                  
                    if (checkOrder.OrderTableItems.Where(x=>x.IsVAT).Count()>0)
                    {
                        inv.DiscountOther = discountPayment;//nếu là sản phẩm đã có thuế thì phải giảm giá sau thuế
                    }
                    else
                    {
                        inv.DiscountAmount = discountPayment;//ngược lại là giá sau thuế
                    }
                    inv.Discount = (float)discount;
                    inv.Total = Math.Round(newlistitem.Sum(x=>x.Total), MidpointRounding.AwayFromZero);// lấy tổng tiền trong sản phẩm chưa thuế vì có sp có thuế nên phải sum vậy, k lấy tổng bên order vì bên đó có thuế
                    inv.VATAmount = Math.Round(newlistitem.Sum(x=>x.VATAmount), MidpointRounding.AwayFromZero);// lấy tổng tiền trong sản phẩm chưa thuế vì có sp có thuế nên phải sum vậy, k lấy tổng bên order vì bên đó có thuế
                                                                                                               // lấy tổng tiền trong sản phẩm chưa thuế vì có sp có thuế nên phải sum vậy, k lấy tổng bên order vì bên đó có thuế

                    inv.Amonut = Amount;//inv.Amonut - discountPayment;// tiền  cần thanh toán đoạn này là sau khi hiển thị bill khách có nhập giảm giá hay k
                  
                    inv.AmountCusPayment = AmountCusPayment.HasValue ? AmountCusPayment.Value : 0;// tieefn khasch đưa
                    if (vat && Vatrate!=null && Vatrate>0)
                    {
                        inv.VATAmount = VATAmount;
                        inv.Amonut = Amount;
                        inv.VATRate = Vatrate;
                    }
                    else
                    {
                        inv.VATRate = (int)NOVAT.NOVAT;
                       // inv.VATRate = (int)VATRateInv.KHONGVAT;
                    }

                    if (inv.Amonut < inv.AmountCusPayment)
                    {
                        inv.AmountChangeCus = AmountCusPayment - inv.Amonut;//tiền thừa
                    }
                    if (checkOrder.Customer!=null)
                    {
                        // nếu có thêm gì nhớ thêm bên invoice khi xem chi tiết có update khách hàng nữa nhé
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
                    //------grby item lại vì nó có nhiều sản phẩm trên nhiều dòng
                    foreach (var item in inv.InvoiceItems)
                    {
                        if (item.TypeProductCategory!=EnumTypeProductCategory.SERVICE && item.TypeProductCategory != EnumTypeProductCategory.COMBO)//lấy ra mấy csai k phải là combo và dịch vụ
                        {
                            list.Add(new KeyValuePair<string, decimal>(item.Code, item.Quantity * -1));
                        }
                        if (item.TypeProductCategory == EnumTypeProductCategory.COMBO)//nếu là combo nó sẽ có các thành phần cần lôi ra
                        {
                            //nếu là combo thì phải tìm ra các thành phần để update vào tồn kho
                            var getlistprobycombo = await  _comboproductrepository.Entities.AsNoTracking().Where(x=>x.IdProduct== item.IdProduct).ToListAsync();
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
                        if (inv.Customer==null)
                        {
                            inv.Customer = await _customerRepository.Entities.AsNoTracking().SingleOrDefaultAsync(x=>x.Id== inv.IdCustomer.Value);
                        }
                    }
                    publishInvoiceResponse.Invoice = inv;
                    publishInvoiceResponse.IsProductVAT = IsProductVAT;
                    return await Result<PublishInvoiceResponse>.SuccessAsync(publishInvoiceResponse);
                }
                await _unitOfWork.RollbackAsync();
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
                var newOrder = lstod.FirstOrDefault();// đây là bàn mới nè lựa chọn đại 1 cái vì 1 bàn nếu có nhiều đơn mà chọn nhiều đơn đó thì ghép tất cả thành 1
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


                    var getitem = orderTables.Where(z => z.Code == x.Code).ToList();
                    if (getitem.Count()>0)
                    {
                        x.Quantity += getitem.Sum(x=>x.Quantity);
                        x.QuantityNotifyKitchen += getitem.Sum(x => x.Quantity);
                        x.Total += getitem.Sum(x => x.Total);
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
                //chạy for này để update các dữ liệu từ db vào các item đã chọn cho đầy đủ thông tin như note, hay số lượng thông báo
                lstitem.ForEach(x =>
                {
                    var getdt = orderitemNguyenthuy.SingleOrDefault(z => z.IdGuid == x.idOrderItem);
                    if (getdt == null)
                    {
                        isValid = true;
                    }
                    else
                    {
                        x.Note = getdt.QuantityNotifyKitchen == 0? getdt.Note:string.Empty;
                    }
                    x.Code = getdt?.Code;
                    x.IdProduct = getdt?.IdProduct;
                    x.idOrderItemInt = getdt?.Id;
                    x.QuantityNotifyKitchen = getdt.QuantityNotifyKitchen;
                    x.Note = getdt.Note;
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
                            if (item.Quantity == item.QuantityNotifyKitchen)//TH = nhau tức là chuyển k phải là dạng thêm và chuyển
                            {
                                item.QuantityNotifyKitchen = item.QuantityNotifyKitchen - getitem.Quantity.Value;
                                quannotify = getitem.Quantity.Value;//tức là nếu đã thông báo bếp hết rồi thì quannotify mới này phải = đúng sl chuyển, vì để báo chuyển chứ k phải là th
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
                            item.Quantity = item.Quantity - getitem.Quantity.Value;
                            item.Total = Convert.ToDecimal(item.Quantity) * item.Price;

                            lstitem.ForEach(x =>
                            {
                                //if (x.Code == item.Code)
                                if (x.idOrderItem == item.IdGuid)
                                {
                                    x.QuantityNotifyKitchen = quannotify;// dùng số này báo cho đơn mới

                                }
                            });// danh sachs item cần tách

                            OrderTableItemold.Add(item);//danh sách sau khi  đã up
                        }
                        else
                        {
                            // khi = nhau tức là tách hết sản phẩm qua đơn mới luôn nha item.Quantity = getitem.Quantity 
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
                    model.IdCustomer = null; //getBr.IdCustomer;
                    model.IdCasher = getBr.IdCasher;
                    model.IsRetailCustomer = true; //getBr.IsRetailCustomer;
                    model.Buyer = getBr.Buyer;
                    model.TypeProduct = getBr.TypeProduct;

                    List<OrderTableItem> OrderTableItemS = new List<OrderTableItem>();
                    List<OrderTableItem> OrderTableItemNewNotifyOrder = new List<OrderTableItem>();// list cho đơn mới cần báo số lượng này chưa báo bếp lần nào mà  chuyeenren từ đơn dc chọn qua
                    foreach (var item in lstitem)//DANH SÁch MỚi
                    {
                        var getItme = orderitemNguyenthuy.SingleOrDefault(x => x.IdGuid == item.idOrderItem);
                        OrderTableItem orderTableItem = new OrderTableItem();
                        orderTableItem.IdItemOrderOld = getItme.Id;
                        orderTableItem.Code = getItme.Code;
                        orderTableItem.Note = getItme.Note;
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
                        //xử lý cho đơn mới
                        var cloneitem = orderTableItem.CloneJson();
                        cloneitem.QuantityNotifyKitchen = item.QuantityNotifyKitchen;
                        if (item.QuantityNotifyKitchen == 0)//mục đich nếu đã báo bếp thì lấy số lượng từ đơn gốc là clone, còn chưa báo gì lấy số lượng từ truyền vào
                        {
                            cloneitem.Quantity = (item.Quantity == null ? 0 : item.Quantity.Value);
                        }
                        else if (item.Quantity <= (getItme.Quantity - getItme.QuantityNotifyKitchen))
                        {
                            cloneitem.QuantityNotifyKitchen = 0;
                        }
                        if (cloneitem.Quantity != cloneitem.QuantityNotifyKitchen)//có khác mới báo cho đơn mới
                        {
                            OrderTableItemNewNotifyOrder.Add(cloneitem);
                        }

                    }
                    RoomAndTable roomAndTable = null;
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
                        roomAndTable = await _roomadntableRepository.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.IdGuid == IdTable.Value && x.ComId == comid);
                        model.IdRoomAndTable = roomAndTable.Id;
                        model.IdRoomAndTableGuid = roomAndTable.IdGuid;
                        
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
                    model.RoomAndTable = roomAndTable;
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
                                //lstItemNewInDonMoi.Add(item);
                                //his.Name = $"+ {item.Quantity.ToString("0.###")} {item.Name}";
                            }

                        }
                        lsthsi.Add(his);
                    }
                    this.AddHistoryOrder(lsthsi);
                    bool isnew = false;

                    await _notifyChitkenRepository.UpdateNotifyKitchenSpitOrderAsync(getBr, OrderTableItemold, comid, model, model.OrderTableItems.ToList(), isnew, CreateDateHis);

                    //
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
                    //tạm đóng nhé tính sau, tính năng này là update các món đã chọn vào món có sẵn trên đơn cũ để k bị lặp, nhưng vậy thì lỗi bếp nhé chưa tìm dc id để update
                    //phần này là tìm các item của đơn mới để update vào nhưng hiện tại lỗi cho phần báo bếp, nên chọn sao thì cứ đẩy qua như tạo mới, có thể hiển thị nhiều dòng
                    //foreach (var x in getod.OrderTableItems)
                    //{
                    //    DetailtSpitModel getitem = null;
                    //    if (string.IsNullOrEmpty(x.Note))// đối với item của bàn mới nếu k có ghi chú thì xử lý tìm theo mã sản phẩm để update số lượng
                    //    {
                    //        // tìm cái trpong list mới có thì update vào cái danh sách hiện tại của đơn mới đó, lưu ý chỉ update vào các item k có ghi chú
                    //        getitem = lstitem.FirstOrDefault(z => z.Code == x.Code && string.IsNullOrEmpty(z.Note) && !lstnoex.Contains(z.idOrderItem.Value));
                    //    }
                    //    else //đối với item có ghi chú thì tìm theo iditem mà k tìm theo mã sản phẩm của item đó
                    //    {
                    //         getitem = lstitem.SingleOrDefault(z => z.idOrderItemInt == x.Id && !lstnoex.Contains(z.idOrderItem.Value));// tìm cái trpong list mới có thì update vào cái danh sách hiện tại của đơn mới đó, tìm theo id mới đúng vì 1 đơn có lặp lại nhiều lần 1 sản phẩm
                    //    }
                    //    if (getitem != null)
                    //    {
                    //        lstnoex.Add(getitem.idOrderItem.Value);//tìm dc thì add item đó vào list để tí lọc ra các item k update (item của list chọn ở view)
                    //        x.Quantity = x.Quantity + getitem.Quantity.Value;
                    //        x.QuantityNotifyKitchen = x.QuantityNotifyKitchen + getitem.Quantity.Value;// mục đích để tí kiểm tra và báo lại 
                    //        x.Total = Convert.ToDecimal(x.Quantity) * x.Price;
                    //    }

                    //}//update các món có trong đơn mới


                    var newlstitem = lstitem;//tìm item còn lại k có trong đơn mới, vì cái có đã update thêm số lượng vào item hiện có trong đơn mới
                   // bản cũ var newlstitem = lstitem.Where(x => !lstnoex.Contains(x.idOrderItem.Value)).ToList();//tìm item còn lại k có trong đơn mới, vì cái có đã update thêm số lượng vào item hiện có trong đơn mới
                    if (newlstitem.Count() > 0)//các item thêm mới vào trong đơn mới mà k phải là update số lượng
                    {
                        // danh sasch item đã chọn ở view của order của đơn cũ tìm để có dữ liệu mà thêm vào đơn mới, item này là newlstitem
                        var getNew = orderitemNguyenthuy.Where(x => newlstitem.Select(x => x.idOrderItem).ToArray().Contains(x.IdGuid)).ToList();// tìm trong item đơn cũ cần tách

                        foreach (var item in getNew)
                        {
                            var geti = newlstitem.SingleOrDefault(z => z.idOrderItem == item.IdGuid);// tìm theo id
                            OrderTableItem orderTableItem = new OrderTableItem();
                            orderTableItem.IdItemOrderOld = item.Id;
                            orderTableItem.Code = item.Code;
                            orderTableItem.Name = item.Name;
                            orderTableItem.Note = item.QuantityNotifyKitchen==0? item.Note:string.Empty;
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
                    getod.Amonut = getod.OrderTableItems.Sum(x => x.Total);
                    getod.Quantity = getod.OrderTableItems.Sum(x => x.Quantity);

                    await _repository.UpdateAsync(getod);
                    await _unitOfWork.SaveChangesAsync();
                    //danh sách món mà có chứa các món chưa thông báo hết cho bép, ví dụ,
                    //có 5 món mà thông báo 3, còn 2 chưa thông báo mà đã dc tách từ bàn kahsc hêm vào
                    var newlist = lstitem.Select(x => new OrderTableItem()
                    {
                        IdItemOrderOld = x.idOrderItemInt.Value,//gán id của đơn cũ đã chọn để xử lý báo bếp
                        IdProduct = x.IdProduct,
                        IdGuid = x.idOrderItem.Value,
                        Quantity = x.Quantity.Value,
                        Note = x.Note,
                    }).ToList();


                    await _notifyChitkenRepository.UpdateNotifyKitchenTachdonVaoDonDacoAsync(getBr, OrderTableItemold, comid, getod, newlist, OrderTableItemremove, CasherName, IdCasher);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                }
                return await Result<bool>.SuccessAsync(HeperConstantss.SUS006);
            }

            catch (Exception e)
            {
                _log.LogError(e.ToString());
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
                        if (x.IsVAT)
                        {
                            x.Total = x.Quantity * x.PriceNoVAT;
                            x.VATAmount = x.Total * (x.VATRate / 100);
                            x.Amount = x.Quantity * x.Price;
                        }
                        else
                        {
                            x.Total = x.Quantity * x.Price;
                            x.Amount = x.Quantity * x.Price;
                        }
                    }
                });
                get.Quantity = get.OrderTableItems.Sum(x => x.Quantity);
                get.Amonut = get.OrderTableItems.Sum(x => x.Amount);
                await _repository.UpdateAsync(get);
                await _unitOfWork.SaveChangesAsync();
                return await Result<OrderTable>.SuccessAsync(get);
            }
            return await Result<OrderTable>.FailAsync();
        }

        public IQueryable<OrderTable> GetOrderInvoiceRetail(int ComId, EnumStatusOrderTable enumStatusOrderTable, EnumTypeProduct enumTypeProduct = EnumTypeProduct.BAN_LE)
        {
            return _repository.Entities.AsNoTracking().Where(x => x.ComId == ComId && x.Status == enumStatusOrderTable && x.TypeProduct == enumTypeProduct);
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
        private  void GenagerInvoiceitem(ICollection<InvoiceItem> data, OrderInvoicePaymentSaleRetailModel model)
        {
            foreach (var x in model.Items)
            {
                var iteminvoice = new InvoiceItem()
                {
                    IdProduct = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Quantity = x.Quantity,
                    Price = x.PriceNew,
                    Unit = x.Unit,
                    Total = x.Total,
                    VATRate = (float)x.VATRate,
                    VATAmount = x.VATAmount,
                    Amonut = x.Amount,
                    EntryPrice = x.RetailPrice,
                    Discount = x.Discount,
                    DiscountAmount = x.DiscountAmount,
                };
                if (iteminvoice.VATRate.HasValue && iteminvoice.VATRate > (int)NOVAT.NOVAT)
                {
                    iteminvoice.VATRate = (float)model.VATRate;
                    iteminvoice.VATAmount = iteminvoice.Total * (Convert.ToDecimal(model.VATRate) / 100);
                    iteminvoice.Amonut = iteminvoice.VATAmount + iteminvoice.Total;
                }
                data.Add(iteminvoice);
            }
        }
        public async Task<Result<PublishInvoiceResponse>> CheckOutOrderInvoiceAsync(OrderInvoicePaymentSaleRetailModel model, EnumTypeProduct enumType = EnumTypeProduct.BAN_LE)
        {
            _unitOfWork.CreateTransaction();
            try
            {
                //------- validate xuất hóa đơn
                if (model.VATMTT)
                {
                    if (model.IdPattern==null || model.IdPattern==0)
                    {
                        return await Result<PublishInvoiceResponse>.FailAsync($"Vui lòng chọn mẫu số ký hiệu để xuất hóa đơn");
                    }
                }
                //---------check sản phẩm
                var getlstproduct = await _productrepository.GetProductbyListId(model.Items.Select(x=>x.Id).ToArray(), model.ComId);//
                var getlistnote = model.Items.Select(x => x.Id).ToArray().Except(getlstproduct.Select(x => x.Id).ToArray());// list mới với các id k có trong list query
                if (getlistnote.Count()>0)
                {
                    var getsp = model.Items.Where(x => getlistnote.ToArray().Contains(x.Id)).Select(x => x.Name);
                    return await Result<PublishInvoiceResponse>.FailAsync($"Không tìm thấy các sản phẩm {string.Join(",", getsp)}");
                }
                //----------duyệt sản phẩm update list
                foreach (var item in getlstproduct)
                {
                    model.Items.ForEach(x =>
                    {
                        if (x.Id == item.Id)
                        {
                            x.Code = item.Code;
                            x.Name = item.Name;
                            x.Unit = item.Unit;
                            x.PriceNoVAT = item.PriceNoVAT;
                            x.VATRate = item.VATRate;
                            x.IsVAT = item.IsVAT;
                            x.RetailPrice = item.RetailPrice;
                            //x.Price = item.Price;
                            //if (x.IsVAT)// nếu sp có thuế
                            //{
                            //    x.Total = x.Quantity * x.PriceNoVAT;
                            //    x.VATAmount = x.Total * (x.VATRate / 100.0M);
                            //}
                            //else if(model.VATMTT &&!x.IsVAT)//nếu hóa đơn có thuế sp k thuếthì updatelaij total và tính lại amount
                            //{
                            //    x.Total = x.Amount;
                            //    x.VATAmount = x.Total * (model.VATRate.Value / 100.0M);
                            //    x.Amount = x.Total + x.VATAmount;
                            //}
                            //else
                            //{
                            //    x.Total = x.Amount;
                            //    x.VATRate = (int)NOVAT.NOVAT;
                            //}
                        }
                    });
                }

                var inv = new Invoice();
                inv.ArrivalDate = DateTime.Now;
                var getInv = await _managerInvNorepository.UpdateInvNo(model.ComId, ENumTypeManagerInv.Invoice, false);
                inv.InvoiceCode = $"HD-{getInv.ToString("00000000")}";
                inv.ComId = model.ComId;
                inv.TypeProduct = model.EnumTypeProduct;
                inv.PurchaseDate = inv.ArrivalDate;
                inv.IdPaymentMethod = model.IdPaymentMethod;
                inv.IdOrderTable = null;
                inv.StaffName = model.Cashername;
                inv.CasherName = model.Cashername;
                inv.IdStaff = model.IdCasher;
                inv.IdCasher = model.IdCasher;
                this.GenagerInvoiceitem(inv.InvoiceItems,model);

                inv.Status = EnumStatusInvoice.DA_THANH_TOAN;
                inv.DiscountAmount = model.DiscountAmount;
                inv.Discount = (float)model.Discount;
                inv.Total = model.Total;// tong tien chưa giam, gán amount bởi vì là giá trị gốc của hóa đơn ban đầu
                inv.VATRate = (float)model.VATRate;//
                inv.VATAmount = model.VATAmount;//
                inv.Amonut = model.Amount;// tiền  cần thanh toán đoạn này là sau khi hiển thị bill khách có nhập giảm giá hay k
                inv.AmountCusPayment = model.CusSendAmount;// tieefn khasch đưa
                inv.AmountChangeCus = model.Amoutchange;// tiền thừa khách
                Customer customer = null;
                if (model.Customer != null)
                {
                    if (model.Customer.Id>0&& !string.IsNullOrEmpty(model.Customer.Code))
                    {
                        customer = await _customerRepository.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.Id == model.Customer.Id && x.Comid == model.ComId);
                        if (customer == null)
                        {
                            return await Result<PublishInvoiceResponse>.FailAsync($"Không tìm thấy khách hàng vui lòng chọn lại khách hàng");
                        }
                        inv.Buyer = customer.Buyer?.Trim();
                        inv.CusName = customer.Name?.Trim();
                        inv.Taxcode = customer.Taxcode?.Trim();
                        inv.CusCode = customer.Code?.Trim();
                        inv.PhoneNumber = customer.PhoneNumber?.Trim();
                        inv.CCCD = customer.CCCD?.Trim();
                        inv.Address = customer.Address?.Trim();
                        inv.Email = customer.Email?.Trim();
                        inv.CusBankNo = customer.CusBankNo?.Trim();
                        inv.CusBankName = customer.CusBankName?.Trim();
                        inv.IdCustomer = customer.Id;
                    }
                    else
                    {
                        inv.IsRetailCustomer = true;
                        inv.Buyer = "Khách lẻ";
                    }
                }
                else
                {
                    inv.IsRetailCustomer = true;
                    inv.Buyer = "Khách lẻ";
                }
                inv.TableNameArea = $"Mang về";
                inv.IsBringBack = true;
                //add bảng
                await _InvoiceRepository.AddAsync(inv);
                await _unitOfWork.SaveChangesAsync();
                inv.Customer = customer;

                //giwof tạo phiếu thu nhé
                RevenueExpenditure revenueExpenditure = new RevenueExpenditure()
                {
                    ComId = inv.ComId,
                    Amount = inv.Amonut,
                    IdInvoice = inv.Id,
                    ObjectRevenueExpenditure = EnumTypeObjectRevenueExpenditure.KHACHHANG,
                    IdCustomer = inv.IdCustomer,
                    Type = EnumTypeRevenueExpenditure.THU,
                    Typecategory = EnumTypeCategoryThuChi.TIENHANG,
                    Title = $"Thu {LibraryCommon.GetDisplayNameEnum(EnumTypeCategoryThuChi.TIENHANG).ToLower()}",
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
                his.Carsher = model.Cashername;
                his.InvoiceCode = inv.InvoiceCode;
                his.IdInvoice = inv.Id;
                his.Name = $"Thanh toán đơn";
                await _historyInvoiceRepository.AddAsync(his);

                //-----------------------------------//
                // update lại sản phẩm tồn kho
                var list = new List<KeyValuePair<string, decimal>>();
                foreach (var item in inv.InvoiceItems)
                {
                    if (item.TypeProductCategory != EnumTypeProductCategory.SERVICE && item.TypeProductCategory != EnumTypeProductCategory.COMBO)//lấy ra mấy csai k phải là combo và dịch vụ
                    {
                        list.Add(new KeyValuePair<string, decimal>(item.Code, item.Quantity * -1));
                    }
                    if (item.TypeProductCategory == EnumTypeProductCategory.COMBO)//nếu là combo nó sẽ có các thành phần cần lôi ra
                    {
                        //nếu là combo thì phải tìm ra các thành phần để update vào tồn kho
                        var getlistprobycombo = _comboproductrepository.Entities.AsNoTracking().Where(x => x.IdProduct == item.IdProduct).ToList();
                        if (getlistprobycombo.Count() > 0)//các sản phẩm trong combo
                        {
                            var getlstid = getlistprobycombo.Select(x => x.IdPro).ToArray();
                            var listproductcombo = await _productrepository.GetProductbyListId(getlstid, inv.ComId);//lấy các sản phẩm
                            foreach (var combo in getlistprobycombo)
                            {
                                var getprobycombo = listproductcombo.SingleOrDefault(x => x.Id == combo.IdPro);//lấy sản phẩm từu lít trên
                                // k quarn lý tồn kho,k phải dịch vụ k phải combo mới update
                                if (!getprobycombo.IsInventory && item.TypeProductCategory != EnumTypeProductCategory.SERVICE && item.TypeProductCategory != EnumTypeProductCategory.COMBO)
                                {
                                    var quantity = (combo.Quantity * -1) * item.Quantity;// lấy số lượng của sản phẩm trong combo nhân với số lượng khách đặt combo
                                    list.Add(new KeyValuePair<string, decimal>(getprobycombo.Code, quantity));
                                }
                            }
                        }
                    }
                }


                await _productrepository.UpdateQuantity(list, model.ComId);
                //end
                //--------------------------------//
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
                if (inv.IdPaymentMethod>0)
                {
                    var getpayment = await _paymentMethodRepository.GetAll(inv.ComId).Where(x => x.Id == inv.IdPaymentMethod).AsNoTracking().SingleOrDefaultAsync();
                    inv.PaymentMethod = getpayment;
                }
              
                string token = string.Empty;
                PublishInvoiceResponse publishInvoiceResponse = new PublishInvoiceResponse();

                if (model.VATMTT)
                {
                    try
                    {
                        PublishInvoiceModel modelein = new PublishInvoiceModel();
                        modelein.ComId = inv.ComId;
                        modelein.IdManagerPatternEInvoice = model.IdPattern.Value;
                        modelein.isVAT = model.VATMTT;
                        modelein.VATRate = (float)model.VATRate.Value;
                        modelein.VATAmount = model.VATAmount;
                        modelein.Amount = model.Amount;
                        modelein.TypeSupplierEInvoice = ENumSupplierEInvoice.VNPT;
                        modelein.CasherName = model.Cashername;
                        modelein.IdCarsher = model.IdCasher;
                        //modelein.lstid = ids.ToArray();
                        var publishinv = await _InvoiceRepository.PublishInvoice(inv, modelein, inv.ComId, model.IdCasher, model.Cashername);
                        if (publishinv.Succeeded)
                        {
                            publishInvoiceResponse = publishinv.Data;
                        }
                    }
                    catch (Exception e)
                    {
                        _log.LogError("Lỗi phát hành hóa đơn");
                        _log.LogError(e.ToString());
                    }
                }
                publishInvoiceResponse.Invoice = inv;
                return await Result<PublishInvoiceResponse>.SuccessAsync(publishInvoiceResponse);
            }
            catch (Exception e)
            {
                _log.LogError("Lỗi phát hành hóa đơn bán hàng");
                _log.LogError(e.ToString());
                return await Result<PublishInvoiceResponse>.FailAsync(e.Message);
            }
        }

        public async Task<Result<string>> GenHtmlPrintBep(List<NotifyOrderNewModel> model,int ComId)//hủy chế biến
        {
            if (model==null || model.Count==0)
            {
                _log.LogError("IN báo bếp lỗi");
                _log.LogError("Không có dữ liệu in");
                return await Result<string>.FailAsync("Không có dữ liệu in");
            }
            try
            {
                //string html = "<!DOCTYPE html>\r\n<html lang='vi'>\r\n<head>\r\n    <meta charset='UTF-8'>\r\n    <meta name='viewport' content='width=device-width, initial-scale=1.0'>\r\n    <meta http-equiv='X-UA-Compatible' content='ie=edge'>\r\n    <title>Vé điện tử</title>\r\n    <script type=\"text/javascript\" charset=\"UTF-8\"></script>\r\n\t\r\n\t<style>\r\n        body {\r\n           \r\n            font-family: Arial;\r\n        }\r\n\r\n        hr {\r\n            margin: 0px;\r\n            border-top: 1px solit #000;\r\n        }\r\n\r\n        .ticket {\r\n          \r\n            padding: 0mm;\r\n            margin: 0 auto;\r\n            height: auto;\r\n   width: 300mm;\r\n            background: #FFF;\r\n            transform-origin: left;\r\n        }\r\ntable { \r\n    border-collapse: collapse; \r\n}\r\ntable td,table th{\r\npadding:2px 2px 2px 0px;\r\n}\r\n        img {\r\n            max-width: inherit;\r\n            width: inherit;\r\n        }\r\n\r\n        @media print {\r\n\r\n            .hidden-print,\r\n            .hidden-print * {\r\n                display: none !important;\r\n            }\r\n\r\n            .ticket {\r\n                page-break-after: always;\r\n            }\r\n        }\r\n    </style>\r\n</head>\r\n\r\n<body>\r\n    <div class='ticket'>\r\n\t\r\n        <table style='width:100%;'>\r\n            <tr>\r\n                <td style='text-align: center;'>\r\n\t\t\t\t\t<span style='font-weight: bold;font-size: 50pt;'>{comname}</span>\r\n\t\t\t\t\t<span style='font-size: 40pt; display: block; text-align: center;margin-bottom:10px'>----------***----------</span>\r\n\t\t\t\t\t\r\n\t\t\t\t</td>\r\n            </tr>\r\n            <tr>\r\n                <td style='font-size: 18px; text-align: center; padding-top: 7px; padding-bottom: 7px;'>\r\n\t\t\t\t\t<span style='display: block; font-size: 45pt; font-weight: bold;'>THÔNG BÁO HỦY MÓN</span>\r\n\t\t\t\t\t<span style='font-size: 40pt; display: block;'>Thời gian: {ngaythangnamxuat}</span>\r\n\t\t\t\t\t<span style='font-size: 40pt; display: block;'>Nhân viên phục vụ: {staffName}</span>\r\n\t\t\t\t\t<span style='font-size: 40pt; display: block;'>Bàn: {tenbanphong}</span>\r\n                </td>\r\n            </tr>\r\n        </table>\r\n\t\r\n        \r\n\t\t<hr style=\"font-size:40pt\" />\r\n\t<table style='width:100%;margin-top:20pt;margin-bottom:10px'>\t\t\t\r\n\t    <thead>\r\n\t\t<tr  style=\"border-botom-style: dotted;border-width: 1pt\">\r\n\t\t<th style='font-size: 35pt; text-align: left;    PADDING-BOTTOM: 12PT;'>Tên hàng hóa</th>\r\n\t\t<th style='font-size: 35pt; text-align: right;    PADDING-BOTTOM: 12PT;'>Số lượng</th>\r\n\t\t</tr>\r\n\t\t</thead>\r\n\t\t<tbody>\r\n\t\t<tr>\r\n\t\t<td style=\" padding-top: 10pt;padding-bottom: 10pt;\"colspan=\"2\">\r\n\t\t\t<span style='border-top: dotted #000 4px;display: block;'></span>\r\n\t\t</td>\r\n\t\t</tr>\r\n\t\t\t<tr  style=\"border-botom-style: dotted;border-width: 1pt\">\r\n\t\t\t\t<td style='font-size: 40pt; text-align: left'>{tenhanghoa}</td>\r\n\t\t\t\t<td style='font-size: 40pt; text-align: right'>{soluong}</td>\r\n\t\t\t</tr>\r\n\r\n\t\t</tbody>\r\n\t\t<tfoot>\r\n\t\t<tr><td style=\" padding-top: 10pt;padding-bottom: 10pt;\"colspan=\"2\">\r\n\t\t\t<span style='border-top: dotted #000 4px;display: block;'></span>\r\n\t\t</td></tr>\r\n\t\t<tr style='font-size: 35pt;text-align: left;margin-top:4px;border-top-style: dotted;border-width: 0.1px;'>\r\n\t\t\t<td style='font-size: 50pt; text-align: left'>Tổng</td>\r\n\t\t\t<td style=\"text-align: right;font-size: 50pt;\">{tongsoluong}</td>\r\n\t\t</tr>\r\n\t\t\r\n\t\t</tfoot>\r\n\t</table>\r\n\t\r\n</body>\r\n</html>";
                //string trproductregex = @"<tbody>(?<xValue>(.|\n)*)<\/tbody>";
                CompanyAdminInfo company = _companyProductRepository.GetCompany(ComId);
                TemplateInvoice templateInvoice = await _templateInvoicerepository.GetTemPlate(ComId,EnumTypeTemplatePrint.IN_BA0_HUY_CHE_BIEN);
                //if (templateInvoice != null)
                //{
                if (templateInvoice == null)
                {
                    return await Result<string>.FailAsync("Hệ thống chưa cấu hình mẫu in chê biến bếp");
                }
                TemplateInvoiceParameter templateInvoiceParameter = new TemplateInvoiceParameter()
                {
                    comname = !string.IsNullOrEmpty(company.Title) ? company.Title.Trim() : company.Name,
                    ngaythangnamxuat = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    tongsoluong = model.Sum(x => x.Quantity).ToString("N0"),
                    tenbanphong = model.FirstOrDefault()?.RoomTableName,
                    staffName = model.FirstOrDefault()?.StaffName,
                };

                var newlist = new List<NotifyOrderNewModel>();
                //foreach (var item in model.GroupBy(x))
                //{

                //}
                //string tableProduct = string.Empty;
                //Regex rg = new Regex(trproductregex);
                //var match = rg.Match(html);
                //String result = match.Groups["xValue"].Value;
                //if (!string.IsNullOrEmpty(result))
                //{
                //    foreach (var item in model)
                //    {
                //        tableProduct += result.Replace("{tenhanghoa}", item.Name).Replace("{soluong}", item.Quantity.ToString("N0"));
                //    }
                //}
                //html = html.Replace(result, tableProduct);
                //string content = LibraryCommon.GetTemplate(templateInvoiceParameter, html, EnumTypeTemplate.PRINT_BEP);
                string content = PrintTemplate.PrintBaoBep(templateInvoiceParameter, model, templateInvoice.Template);
                return await Result<string>.SuccessAsync(content, "");
            }
            catch (Exception e)
            {
                _log.LogError("IN báo bếp lỗi");
                _log.LogError(e.ToString());
                return await Result<string>.FailAsync(e.Message);
            }
           
        }

        public async Task<Result<string>> AddNoteAndToppingItemOrder(int comid, Guid idOrder, Guid idOrderItem, string note)
        {
            var getorder = await _repository.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.ComId == comid && x.IdGuid == idOrder && x.Status==EnumStatusOrderTable.DANG_DAT);
            if (getorder!=null)
            {
                var getitem = await _OrderTableItemrepository.Entities.SingleOrDefaultAsync(x => x.IdOrderTable == getorder.Id && x.IdGuid == idOrderItem);
                if (getitem!=null)
                {
                    if (getitem.Quantity>getitem.QuantityNotifyKitchen)//tức là phải có đặt mới thì mới ghi chú dc, cái nào bấm thông báo rồi thì không cho
                    {
                        getitem.Note = note;
                        await  _OrderTableItemrepository.UpdateAsync(getitem);
                        await _unitOfWork.SaveChangesAsync();
                        return await Result<string>.SuccessAsync("Thêm ghi chú thành công");
                    }
                    else
                    {
                        return await Result<string>.FailAsync("Món đã thông báo bếp không thể thêm ghi chú");
                    }

                }
                return await Result<string>.FailAsync("Món không tồn tại");
            }
            return await Result<string>.FailAsync("Không tìm thấy đơn, vui lòng thử lại");
        }

        public async Task<Result<OrderTable>> CloneItemAsync(int comid, Guid idOrder, Guid idItem)
        {
            try
            {
                var getorder = await _repository.Entities.Include(x => x.OrderTableItems).SingleOrDefaultAsync(x => x.ComId == comid && x.IdGuid == idOrder && x.Status == EnumStatusOrderTable.DANG_DAT);
                if (getorder != null)
                {
                    var getitem = getorder.OrderTableItems.SingleOrDefault(x => x.IdOrderTable == getorder.Id && x.IdGuid == idItem);
                    if (getitem != null)
                    {
                        var newitem = getitem.CloneJson();
                        newitem.Id = 0;
                        newitem.IdGuid = Guid.NewGuid();
                        newitem.Quantity = 1;
                        newitem.QuantityNotifyKitchen = 0;
                        if (getitem.IsVAT)
                        {
                            newitem.Total = 1 * newitem.PriceNoVAT;
                            newitem.VATAmount = Math.Round(newitem.Total * (newitem.VATRate/100.0M), MidpointRoundingCommon.Three);
                            newitem.Amount = 1 * newitem.Price;
                        }
                        else
                        {
                            newitem.VATAmount =0;
                            newitem.Total = 1 * newitem.Price;
                            newitem.Amount = 1 * newitem.Price;
                        }
                        

                        newitem.Discount = 0;
                        newitem.Note = string.Empty;
                        newitem.DiscountAmount = 0;
                        newitem.ToppingsOrders = null;
                        getorder.OrderTableItems.Add(newitem);

                    }
                    else
                    {
                        return await Result<OrderTable>.FailAsync("Món không tồn tại hoặc đã bị xóa, không thể sao chép");
                    }
                    getorder.Quantity = getorder.OrderTableItems.Sum(x => x.Quantity);
                    getorder.Amonut = getorder.OrderTableItems.Sum(x => x.Amount);
                    await _repository.UpdateAsync(getorder);
                    await _unitOfWork.SaveChangesAsync();
                    return await Result<OrderTable>.SuccessAsync(getorder);
                }
                return await Result<OrderTable>.FailAsync("Không tìm thấy đơn, vui lòng thử lại");
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                return await Result<OrderTable>.FailAsync("Có lỗi trong quá trình sao chép đơn, vui lòng thử lại");
            }
          
        }

        public async Task<IResult> UpdateStaffAsync(int comid, Guid idOrder, string idstaff, string staffName)
        {
            var getOrder = await _repository.Entities.SingleOrDefaultAsync(x=>x.ComId == comid && x.IdGuid==idOrder);
            if (getOrder!=null)
            {
                getOrder.IdStaff = idstaff;
                getOrder.StaffName = staffName;
                await _repository.UpdateAsync(getOrder);
                await _unitOfWork.SaveChangesAsync();  
                return await Result.SuccessAsync("Cập nhật thành công");
            }
            else
            {
                return await Result.FailAsync("Không tìm thấy đơn để cập nhật");
            }
            
        }
    }
}
