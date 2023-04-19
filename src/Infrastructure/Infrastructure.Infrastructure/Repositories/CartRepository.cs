using Application.Constants;
using Application.DTOs.Mail;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Shared;
using Application.Providers;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using HelperLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository<Cart>
    {
        private readonly INotifyUserRepository<NotifiUser> _repositoryNotifyUser;
        private readonly IStatusOrderRepository _repositoryStatusOrder;
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IMailService _mailservice;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IParametersEmailRepository _parametersEmailRepository;

        private readonly IOrderRepository<Order> _orderRepository;
        private readonly ILogger<CartRepository> _logger;
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Cart> _repositoryCart;
        private readonly IRepositoryAsync<Product> _repositoryProduct;
        private readonly ICartDetailtRepository _repositoryCartitem;
        public CartRepository(IRepositoryAsync<Cart> repositoryCart,
            IOrderRepository<Order> orderRepository,
            ILogger<CartRepository> logger,
               IStatusOrderRepository repositoryStatusOrder,
               INotifyUserRepository<NotifiUser> repositoryNotifyUser,
            IOptions<CryptoEngine.Secrets> config,
            IServiceScopeFactory serviceScopeFactory,
             IParametersEmailRepository parametersEmailRepository,
            IMailService mailservice,
             IUnitOfWork unitOfWork, IRepositoryAsync<Product> repositoryProduct, IMapper mapper,
            ICartDetailtRepository repositoryCartitem)
        {
            _repositoryNotifyUser = repositoryNotifyUser;
            _parametersEmailRepository = parametersEmailRepository;
            _config = config;
            _mailservice = mailservice;
            _serviceScopeFactory = serviceScopeFactory;
            _orderRepository = orderRepository;
            _mapper = mapper; _repositoryStatusOrder = repositoryStatusOrder;
            _repositoryProduct = repositoryProduct;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _repositoryCartitem = repositoryCartitem;
            _repositoryCart = repositoryCart;
        }

        public async Task<ResponseModel<CartModelView>> AddOrUpdateToCartAsync(bool AddToCart, Product Product, int quantity, int idCus, int typecustomer = 0)
        {
            _unitOfWork.CreateTransaction();
            try
            {
                var checkCus = _repositoryCart.Entities.SingleOrDefault(m => m.IdCustomer == idCus);
                if (AddToCart)
                {
                    if (checkCus != null)
                    {
                        var idCartItem = 0;
                        // nếu đã có giỏ hàng
                        var checkdetailcart = _repositoryCartitem.CheckProduct(checkCus.Id, Product.Id, out idCartItem);
                        if (checkdetailcart)
                        {
                            // update nếu đã có sản phảm
                            _repositoryCartitem.UpdateItem(new CartDetailt() { Id = idCartItem, IdCart = checkCus.Id, Name = Product.Name, Quantity = quantity }, true, false);
                        }
                        else
                        {
                            // hêm 1 sản phẩm mới
                            _repositoryCartitem.AddItem(new CartDetailt() { IdCart = checkCus.Id, IdProduct = Product.Id, Name = Product.Name, Quantity = quantity, isSelected = true });
                        }
                        _unitOfWork.SaveChanges();

                        checkCus.Quantity = _repositoryCartitem.GetQuantityByCart(checkCus.Id);
                        checkCus.Amount = _repositoryCartitem.GetAmountByCart(checkCus.Id);
                        checkCus.Total = checkCus.Amount;
                        checkCus.AmountInWord = HelperLibrary.LibraryCommon.So_chu(checkCus.Amount);


                        await _repositoryCart.UpdateAsync(checkCus);
                        _unitOfWork.SaveChanges();
                        _unitOfWork.Commit();
                        var modelCartView = _mapper.Map<CartModelView>(checkCus);
                        //  modelCartView.Product = Product;
                        // checkCus.CartDetailts = _repositoryCartitem.GetListItemByCart(checkCus.Id);
                        return new ResponseModel<CartModelView>() { Data = modelCartView, Message = HeperConstantss.SUS011, isSuccess = true };
                    }
                    else
                    {
                        // truongf hopwj chua có khách hàng và lần đầu tiên

                        decimal price = Product.Price;
                        if (Product.isRunPromotion)
                        {
                            if (Product.PriceDiscountRun > 0)
                            {
                                price = Product.PriceDiscountRun;
                            }
                            else if (Product.DiscountRun > 0)
                            {
                                price = Product.Price - ((decimal)(Product.DiscountRun / 100) * Product.Price);
                            }
                        }
                        else if (Product.isPromotion && Product.ExpirationDateDiscount != null)
                        {
                            if (Product.ExpirationDateDiscount.Value >= DateTime.Now)
                            {   // tức là còn hạn mới áp dụng
                                if (Product.PriceDiscount > 0)
                                {
                                    price = Product.PriceDiscount;
                                }
                                else if (Product.Discount > 0)
                                {
                                    price = Product.Price - ((decimal)(Product.Discount / 100) * Product.Price);
                                }
                            }
                        }




                        Cart cart = new Cart();
                        cart.IdCustomer = idCus;
                        cart.Quantity = quantity;
                        cart.Total = price * quantity;
                        cart.Amount = price * quantity;
                        cart.AmountInWord = HelperLibrary.LibraryCommon.So_chu(cart.Amount);
                        await _repositoryCart.AddAsync(cart);
                        await _unitOfWork.SaveChangesAsync();

                        CartDetailt cartdetailt = new CartDetailt();
                        cartdetailt.Name = Product.Name;
                        cartdetailt.Quantity = quantity;
                        cartdetailt.isSelected = true;
                        cartdetailt.IdProduct = Product.Id;
                        cartdetailt.IdCart = cart.Id;
                        _repositoryCartitem.AddItem(cartdetailt);
                        _unitOfWork.SaveChanges();

                        _unitOfWork.Commit();

                        //var newlistcart = new List<CartDetailt>();
                        //newlistcart.Add(cartdetailt);
                        //cart.CartDetailts = newlistcart;
                        var modelCartView = _mapper.Map<CartModelView>(cart);
                        //modelCartView.Product = Product;
                        return new ResponseModel<CartModelView>() { Data = modelCartView, Message = HeperConstantss.SUS011, isSuccess = true };
                    }
                }
                else
                {
                    if (checkCus == null)
                    {
                        return new ResponseModel<CartModelView>() { Message = HeperConstantss.ERR035, isSuccess = false };
                    }
                    var idCartItem = 0;
                    var checkdetailcart = _repositoryCartitem.CheckProduct(checkCus.Id, Product.Id, out idCartItem);
                    if (checkdetailcart)
                    {
                        // update nếu đã có sản phảm
                        _repositoryCartitem.UpdateItem(new CartDetailt() { Id = idCartItem, IdCart = checkCus.Id, Name = Product.Name, Quantity = quantity }, AddToCart);
                        _unitOfWork.SaveChanges();

                        checkCus.Quantity = _repositoryCartitem.GetQuantityByCart(checkCus.Id);
                        checkCus.Amount = _repositoryCartitem.GetAmountByCart(checkCus.Id);
                        checkCus.Total = checkCus.Amount;
                        checkCus.AmountInWord = HelperLibrary.LibraryCommon.So_chu(checkCus.Amount);

                        await _repositoryCart.UpdateAsync(checkCus);

                        _unitOfWork.SaveChanges();
                        _unitOfWork.Commit();
                        var modelCartView = _mapper.Map<CartModelView>(checkCus);
                        //  modelCartView.Product = Product;
                        return new ResponseModel<CartModelView>() { Data = modelCartView, Message = HeperConstantss.SUS011, isSuccess = true };
                    }
                    else
                    {
                        return new ResponseModel<CartModelView>() { Message = HeperConstantss.ERR034, isSuccess = false };
                    }

                }
                /// trường hợp update quantiry....AddToCart false


            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(e, e.Message);
                return new ResponseModel<CartModelView>() { isSuccess = false, Message = e.Message };
            }

        }

        public IQueryable<Cart> GetCartCustomer(int id, int type = 0)
        {
            return _repositoryCart.Entities.Where(c => c.IdCustomer == id);
        }

        public async Task<int> GetQuantityCartByUserAsync(int IdCustomer)
        {
            var get = await _repositoryCart.Entities.Where(c => c.IdCustomer == IdCustomer).Include(z => z.CartDetailts).SingleOrDefaultAsync();
            if (get == null) { return 0; }
            if (get.CartDetailts.Count() == 0)
            {
                return 0;
            }
            return get.CartDetailts.Where(x => x.isSelected).Sum(x => x.Quantity);
        }

        public async Task<ResponseModel<CartModelView>> UpdateCartBySelectItemAsync(int idCus, int?[] IdItem, bool select, bool removeAll, bool checkAll)
        {
            if (!removeAll && IdItem == null)
            {
                new ResponseModel<CartModelView>() { isSuccess = false, Message = HeperConstantss.ERR012 };
            }
            _unitOfWork.CreateTransaction();
            try
            {
                int quantityAll = 0;
                decimal amountAll = 0;
                var checkCus = await _repositoryCart.Entities.SingleOrDefaultAsync(m => m.IdCustomer == idCus);
                if (checkCus == null)
                {
                    return new ResponseModel<CartModelView>() { Message = HeperConstantss.ERR034, isSuccess = false };
                }
                if (removeAll)
                {
                    _repositoryCartitem.UpdateDisableAllItem(checkCus.Id, out amountAll, out quantityAll);
                    _unitOfWork.SaveChanges();
                }
                else
                {
                    foreach (var itemCart in IdItem)
                    {
                        if (itemCart != null)
                        {
                            int quantity = 0;
                            decimal amount = 0;
                            _repositoryCartitem.UpdateSelectItem(checkCus.Id, itemCart.Value, select, out amount, out quantity);
                            _unitOfWork.SaveChanges();
                        }

                    }
                    //  _unitOfWork.SaveChanges();
                }
                checkCus.Quantity = _repositoryCartitem.GetQuantityByCart(checkCus.Id);
                checkCus.Amount = _repositoryCartitem.GetAmountByCart(checkCus.Id);
                checkCus.Total = checkCus.Amount;
                checkCus.AmountInWord = HelperLibrary.LibraryCommon.So_chu(checkCus.Amount);
                //if (select)
                //{
                //    if (checkAll || removeAll)
                //    {
                //        checkCus.Amount = amountAll;
                //        checkCus.Total = checkCus.Amount;
                //        checkCus.Quantity = quantityAll;
                //    }
                //    else
                //    {
                //        checkCus.Amount = checkCus.Amount + amountAll;
                //        checkCus.Total = checkCus.Amount;
                //        checkCus.Quantity = checkCus.Quantity + quantityAll;
                //    }

                //}
                //else
                //{
                //    if (checkAll || removeAll)
                //    {
                //        checkCus.Amount = amountAll;
                //        checkCus.Total = checkCus.Amount;
                //        checkCus.Quantity = quantityAll;
                //    }
                //    else
                //    {
                //        checkCus.Amount = checkCus.Amount - amountAll;
                //        checkCus.Total = checkCus.Amount;
                //        checkCus.Quantity = checkCus.Quantity - quantityAll;
                //    }

                //}
                _repositoryCart.Update(checkCus);
                _unitOfWork.SaveChanges();
                await _unitOfWork.CommitAsync();
                _unitOfWork.Dispose();
                var modelCartView = _mapper.Map<CartModelView>(checkCus);
                _logger.LogInformation($"UpdateCartBySelectItemAsync IdCart: {checkCus.Id}, IdItem{IdItem}");
                return new ResponseModel<CartModelView>() { Data = modelCartView, isSuccess = true, Message = HeperConstantss.SUS006 };

            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(e, e.Message);
                return new ResponseModel<CartModelView>() { isSuccess = false, Message = e.Message };
            }
        }

        public async Task<ResponseModel<CartModelView>> RemoveItemCartAsync(int IdCustomer, int IdItemCart)
        {
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                int quantityAll = 0;
                decimal amountAll = 0;
                var checkCus = await _repositoryCart.Entities.SingleOrDefaultAsync(m => m.IdCustomer == IdCustomer);
                if (checkCus == null)
                {
                    return new ResponseModel<CartModelView>() { Message = HeperConstantss.ERR034, isSuccess = false };
                }
                _repositoryCartitem.RemoveItemCart(checkCus.Id, IdItemCart, out amountAll, out quantityAll);
                await _unitOfWork.SaveChangesAsync();
                if (quantityAll == 0)
                {
                    return new ResponseModel<CartModelView>() { isSuccess = false, Message = HeperConstantss.ERR012 };
                }
                checkCus.Quantity = _repositoryCartitem.GetQuantityByCart(checkCus.Id);
                checkCus.Amount = _repositoryCartitem.GetAmountByCart(checkCus.Id);
                checkCus.Total = checkCus.Amount;
                checkCus.AmountInWord = HelperLibrary.LibraryCommon.So_chu(checkCus.Amount);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                var modelCartView = _mapper.Map<CartModelView>(checkCus);
                return new ResponseModel<CartModelView>() { Data = modelCartView, isSuccess = true, Message = HeperConstantss.SUS006 };

            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(e, e.Message);
                return new ResponseModel<CartModelView>() { isSuccess = false, Message = e.Message };
            }
        }

        public async Task<ResponseModel<OrderModelView>> CheckOutCart(Customer Customer)
        {
            _unitOfWork.CreateTransaction();
            try
            {
                var checkCus = await _repositoryCart.Entities.Include(x => x.CartDetailts.Where(x => x.isSelected && !x.isDisable)).ThenInclude(x => x.Product).AsNoTracking().SingleOrDefaultAsync(m => m.IdCustomer == Customer.Id);
                if (checkCus == null)
                {
                    return new ResponseModel<OrderModelView>() { Message = HeperConstantss.ERR034, isSuccess = false };
                }
                if (checkCus.Quantity == 0 || checkCus.CartDetailts.Count() == 0)
                {
                    return new ResponseModel<OrderModelView>() { isSuccess = false, Message = HeperConstantss.ERR036 };
                }
                Order order = new Order();
                order.Status = (int)EnumStatusOrder.AwaitingConfirmation;
                order.OrderCode = LibraryCommon.RandomString(8).ToUpper();
                //thông tin khách
                order.IdCustomer = Customer.Id;
                order.Address = Customer.Address;

                if (Customer.City != null)
                {
                    if (Customer.Ward != null)
                    {
                        order.Address += ", " + (Customer.Ward.Prefix + " " + Customer.Ward.Name);
                    }
                    if (Customer.District != null)
                    {
                        order.Address += ", " + (Customer.District.Prefix + " " + Customer.District.Name);
                    }
                    if (Customer.City != null)
                    {
                        order.Address += ", " + (Customer.City.Prefix + " " + Customer.City.Name);
                    }
                }

                order.CusName = Customer.Name;
                order.PhoneNumber = Customer.PhoneNumber;
                // order.Address = Customer.Address;
                order.Email = Customer.Email;
                order.CusCode = Customer.Code;
                // nếu nhân viên là người đặt
                order.IdPharmaceutical = 0;
                // thông tin đơn hàng
                order.Amount = checkCus.Amount;
                order.Total = checkCus.Total;
                order.VATRate = checkCus.VATRate;
                order.VATAmount = checkCus.VATAmount;
                order.AmountInWord = checkCus.AmountInWord;
                order.Quantity = checkCus.Quantity;
                //order.DiscountRate = checkCus.DiscountRate;
                //order.DiscountAmonnt = checkCus.DiscountAmonnt;

                // add item
                List<int> lstItemCart = new List<int>();
                List<OrderDetailts> orderDetailts = new List<OrderDetailts>();
                foreach (var item in checkCus.CartDetailts)
                {
                    if (item.Product != null)
                    {
                        if (!item.isDisable && item.isSelected)
                        {
                            decimal price = item.Product.Price;
                            if (item.Product.isRunPromotion)
                            {
                                if (item.Product.PriceDiscountRun > 0)
                                {
                                    price = item.Product.PriceDiscountRun;
                                }
                                else if (item.Product.DiscountRun > 0)
                                {
                                    price = item.Product.Price - ((decimal)(item.Product.DiscountRun / 100) * item.Product.Price);
                                }
                                else
                                {
                                    throw new Exception("Không có giá chiết khấu chạy khuyên mãi và phần % chiết khấu sản phẩm: " + item.Product.Name);
                                    _logger.LogError($"Không có giá chiết khấu chạy khuyên mãi và phần % chiết khấu sản phẩm: {item.Product.Name}, mã sản phẩm { item.Product.Code}");
                                }

                            }
                            else if (item.Product.isPromotion && item.Product.ExpirationDateDiscount != null)
                            {
                                if (item.Product.ExpirationDateDiscount.Value >= DateTime.Now)
                                {   // tức là còn hạn mới áp dụng
                                    if (item.Product.PriceDiscount > 0)
                                    {
                                        price = item.Product.PriceDiscount;
                                    }
                                    else if (item.Product.Discount > 0)
                                    {
                                        price = item.Product.Price - ((decimal)(item.Product.Discount / 100) * item.Product.Price);
                                    }
                                    else
                                    {
                                        throw new Exception("Không có giá chiết khấu và phần % chiết khấu sản phẩm: " + item.Product.Name);
                                        _logger.LogError($"Không có giá chiết khấu và phần % chiết khấu sản phẩm: {item.Product.Name}, mã sản phẩm { item.Product.Code}");
                                    }
                                }
                            }
                            orderDetailts.Add(new OrderDetailts()
                            {
                                IdOrder = order.Id,
                                NoteProdName = item.Product.NoteProdName,
                                ProdName = item.Product.Name,
                                ProdCode = item.Product.Code,
                                Img = item.Product.Img,
                                Packing = item.Product.Packing,
                                IsPromotion = item.Product.isPromotion,
                                IsRunPromotion = item.Product.isRunPromotion,
                                DiscountRate = item.Product.Discount,
                                Price = price,
                                Quantity = item.Quantity,
                                Amount = item.Quantity * price,
                                Total = item.Quantity * price,
                                VATAmount = 0,
                                VATRate = -1,

                            });
                            lstItemCart.Add(item.Id);
                        }
                    }
                }
                if (orderDetailts.Count() == 0)
                {
                    _logger.LogError("Thêm order lỗi k lấy dc item cart");
                    return new ResponseModel<OrderModelView>() { isSuccess = false, Message = HeperConstantss.ERR036 };

                }
                order.OrderDetailts = orderDetailts;
                _orderRepository.AddOrder(order);
                await _unitOfWork.SaveChangesAsync();

                await this.UpdateCartAfterCheckOut(checkCus.Id);//update cart = 0 vì đã đặt hết
                                                                // sau khi đặt hàng thì phải xóa item cart đã đặt
                await _repositoryCartitem.RemoveListItemCart(checkCus.Id, lstItemCart); // xóa item cart đã đặt

                await this.CheckRemoveCartInNotItem(checkCus.Id);//check để xóa cart nếu k còn item nào

                // update vào bảng status đơn hàng là chờ tiếp nhận
                //add bảng status
                await UpdateStatusOrder(order.Id, Customer.Name);

                await _unitOfWork.SaveChangesAsync();

                // thực hiện thành công xong thì mới Commit lưu tất cả
                await _unitOfWork.CommitAsync();
                //_unitOfWork.Dispose();
                OrderModelView orderModelView = new OrderModelView();
                orderModelView.Customer = Customer;
                orderModelView.Order = order;

                if (!string.IsNullOrEmpty(order.Email))
                {
                    _logger.LogInformation("Đặt hàng thành công" + order.OrderCode);
                    SendMailAfterOrder(order, Customer);
                    NotifiUser notifiUser = new NotifiUser();
                    notifiUser.OrderCode = order.OrderCode;
                    notifiUser.IdUser = order.IdCustomer;
                    notifiUser.IdUserGuid = order.Customer != null ? order.Customer.IdCodeGuid.ToString() : "";
                    notifiUser.Email = order.Email;
                    notifiUser.Title = TitleMailConstant.ban_co_don_hang_dang_xu_ly;
                    notifiUser.Content = ContentMailConstant.ban_co_don_hang_dang_xu_ly(order.OrderCode);
                    await _repositoryNotifyUser.SendNotifyAsync(notifiUser);
                    _logger.LogInformation("Đặt hàng thành công và gửi email: " + order.OrderCode);
                }
                else
                {
                    _logger.LogInformation("Đặt hàng thành công nhưng không có email để gửi" + order.OrderCode);
                }

                return new ResponseModel<OrderModelView>() { Data = orderModelView, isSuccess = true, Message = HeperConstantss.SUS012 };

            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError("Lỗi đặt đơn hàng của khách: " + Customer.Code + Customer.Name);
                _logger.LogError(e, e.Message);
                return new ResponseModel<OrderModelView>() { isSuccess = false, Message = e.Message };
            }
        }
        private async Task UpdateStatusOrder(int idOrder, string updateby)
        {
            StatusOrder statusOrder = new StatusOrder();
            statusOrder.IdOrder = idOrder;
            statusOrder.Status = (int)EnumStatusOrder.AwaitingConfirmation;
            statusOrder.IsCustomer = true;
            statusOrder.Note = GeneralMess.ConvertStatusOrder(EnumStatusOrder.AwaitingConfirmation);
            statusOrder.FullNameUpdate = updateby;
            await _repositoryStatusOrder.AddStatusOrderAsync(statusOrder);
        }
        private void SendMailAfterOrder(Order order, Customer customer)
        {
            //var getcompanyInfoAdmin = await _companyInfoAdminrepository.GetFirstAsync();
            EmailParametersModel emailParameters = new EmailParametersModel();
            emailParameters.tenkhachhang = order.CusName;
            emailParameters.diachigiaohang = order.Address;
            emailParameters.sodienthoai = order.PhoneNumber;
            emailParameters.tongtiendonhang = order.Amount.ToString("N0");
            emailParameters.soluongsanpham = order.Quantity.ToString();
            emailParameters.madonhang = order.OrderCode;
            emailParameters.ngaythangnam = $"Ngày {order.CreatedOn.Day}  Tháng {order.CreatedOn.Month} Năm {order.CreatedOn.Year} {order.CreatedOn.ToString("HH:mm:ss")}";

            string param = CryptoEngine.Encrypt("?code=" + order.OrderCode, _config.Value.Key);
            emailParameters.urldonhang = "/theo-doi-don-hang?secret=" + param;

            var getkey = _parametersEmailRepository.GetBykey(KeyTitleMail.thong_bao_dat_hang_thanh_cong);

            string content = _mailservice.GetTemplate(emailParameters, getkey.Value);
            string titleEmail = _mailservice.GetTemplateTitle(emailParameters, getkey.Title);

            MailRequest mailRequest = new MailRequest
            {
                Title = titleEmail,
                Subject = titleEmail,
                Content = content,
                EmailTo = order.Email,
                FullNameUserSend = customer.Name,
                IdTypeUserSend = (int)TypeCustomerEnum.Customer
            };
            var task = Task.Run(() =>
            {
                _mailservice.SendEmailOne(mailRequest, null, true);
            });
            //Thread thr = new Thread(new ThreadStart( () => SendMailAddStatusOrder(_mail, (OrderStatus)order.Status, model.Note)));
            //thr.Start();

        }

        private async Task UpdateCartAfterCheckOut(int idCart)
        {
            var updatecart = await _repositoryCart.GetByIdAsync(idCart);
            if (updatecart != null)
            {
                updatecart.Amount = 0;
                updatecart.Total = 0;
                updatecart.Quantity = 0;
                updatecart.AmountInWord = String.Empty;
                await _repositoryCart.UpdateAsync(updatecart);
                //await _unitOfWork.SaveChangesAsync();
            }
        }
        private async Task CheckRemoveCartInNotItem(int idCart)
        {
            try
            {
                var checkitemcart = _repositoryCartitem.GetListItemByCart(idCart);
                if (checkitemcart.Count() == 0)
                {
                    await this.RemoveCart(idCart);
                    //await  _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Xóa cart sau checkout fail");
                _logger.LogError(e, e.Message);
            }

        }

        public async Task RemoveCart(int idCart)
        {
            var remove = await _repositoryCart.GetByIdAsync(idCart);
            if (remove != null)
            {
                await _repositoryCart.DeleteAsync(remove);
                // await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
