using Application.Constants;
using Application.DTOs.Mail;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Shared;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{

    public class OrderRepository : IOrderRepository<Order>
    {
        private readonly INotifyUserRepository<NotifiUser> _repositoryNotifyUser;
        private readonly IMailService _mailservice;
        private readonly IParametersEmailRepository _parametersEmailRepository;
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly ILogger<OrderRepository> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepositoryAsync<Order> _repositoryOrder;
        private readonly IStatusOrderRepository _repositoryStatusOrder;
        public OrderRepository(IRepositoryAsync<Order> repositoryCart, IMailService mailservice,
                 ILogger<OrderRepository> logger, IOptions<CryptoEngine.Secrets> config,
                     IParametersEmailRepository parametersEmailRepository,
                        INotifyUserRepository<NotifiUser> repositoryNotifyUser,
            IStatusOrderRepository repositoryStatusOrder, IUnitOfWork unitOfWork)
        {
            _repositoryNotifyUser = repositoryNotifyUser;
            _mailservice = mailservice;
            _parametersEmailRepository = parametersEmailRepository;
            _config = config;
            _logger = logger;
            _repositoryStatusOrder = repositoryStatusOrder;
            _repositoryOrder = repositoryCart;
            _unitOfWork = unitOfWork;
        }

        public void AddOrder(Order order) // test lại chi tiết cart và luu thêm bảng đơn vijg giao hàng, hình thức thanh toán khi đặt, lưu vào order có chi tiết
        {
            _repositoryOrder.Add(order);
        }

        public async Task<ResponseModel<StatusOrder>> CancelByIdAsync(int id, string note, string updateby, bool isCustomer = true)
        {
            var get = await _repositoryOrder.GetByIdAsync(id);
            if (get != null)
            {
                if (isCustomer && (get.Status == (int)EnumStatusOrder.AwaitingConfirmation))
                {
                    //nếu là khách hàng chỉ hủy đơn khi mới tạo chưa xác nhận
                    return await this.UpdateStatusAsync(id, EnumStatusOrder.Cancel, note, updateby, isCustomer);

                }
                else if (get.Status == (int)EnumStatusOrder.AwaitingConfirmation || get.Status == (int)EnumStatusOrder.Processing)
                {
                    //nếu là admin thì đơn chưa xác nhận hoặc đang xử lý mới được hủy, giao hàng thì k dc hủy

                    return await this.UpdateStatusAsync(id, EnumStatusOrder.Cancel, note, updateby, isCustomer);
                    // return new ResponseModel<string> { isSuccess = true, Message = HeperConstantss.SUS013 };

                }

                string mess = $"{GeneralMess.ConvertStatusToString(HeperConstantss.ERR038)} , đơn hàng {GeneralMess.ConvertStatusOrder((EnumStatusOrder)get.Status).ToLower()}";
                return new ResponseModel<StatusOrder> { isSuccess = false, Message = mess };

            }
            return new ResponseModel<StatusOrder> { isSuccess = false, Message = HeperConstantss.ERR012 };
        }

        public async Task<ResponseModel<string>> DeleteByIdAsync(int id)
        {
            var get = await _repositoryOrder.GetByIdAsync(id);
            if (get != null)
            {
                await _repositoryOrder.DeleteAsync(get);
                await _unitOfWork.SaveChangesAsync();
                return new ResponseModel<string> { isSuccess = true, Message = HeperConstantss.SUS007 };
            }
            return new ResponseModel<string> { isSuccess = false, Message = HeperConstantss.ERR012 };
        }

        public IQueryable<Order> GetAllOrder()
        {
            return _repositoryOrder.GetAllQueryable().AsNoTracking();
        }
        public async Task<PaginatedList<Order>> GetAllOrderAsync(OrderViewModel model)
        {
            var iquery = _repositoryOrder.GetAllQueryable();
            if (!string.IsNullOrEmpty(model.sortOn))
            {
                model.sortDirection = "DESC";
                model.sortOn = "Id";
            }
            return await PaginatedList<Order>.ToPagedListAsync(iquery, model.PageNumber, model.PageSize, model.sortOn, model.sortDirection);

            //return  _repositoryOrder.GetAllQueryable().Skip((model.PageNumber - 1) * model.PageSize)
            //                .Take(model.PageSize)
            //                .ToList();
        }

        public async Task<Order> GetByIdOrderAndCustomerAsync(string codeOrder, int idCustomer)
        {
            return await _repositoryOrder.Entities.Where(c => c.IdCustomer == idCustomer && c.OrderCode == codeOrder).Include(x => x.OrderDetailts).SingleOrDefaultAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _repositoryOrder.GetByIdAsync(x => x.Id == id, x => x.Include(x => x.OrderDetailts).Include(x => x.Customer));
        }

        public IQueryable<Order> GetOrderCustomer(int id, int type = 0)
        {
            return _repositoryOrder.Entities.Where(c => c.IdCustomer == id);
        }

        public async Task<ResponseModel<StatusOrder>> UpdateStatusAsync(int idOrder, EnumStatusOrder status, string note, string updateby, bool isCustomer = false)
        {
            _logger.LogInformation("UpdateStatusAsync");
            _unitOfWork.CreateTransaction();
            try
            {
                var getorder = _repositoryOrder.Entities.Where(x => x.Id == idOrder).Include(x => x.Customer).SingleOrDefault();
                if (getorder != null)
                {
                    if (getorder.Status > (int)status)
                    {
                        return new ResponseModel<StatusOrder>() { isSuccess = false, Message = HeperConstantss.ERR039 };
                    }
                    if (getorder.Status == (int)EnumStatusOrder.Cancel)
                    {
                        return new ResponseModel<StatusOrder>() { isSuccess = false, Message = HeperConstantss.ERR040 };
                    }
                    getorder.Status = (int)status;
                    _repositoryOrder.Update(getorder);
                    //add bảng status
                    StatusOrder statusOrder = new StatusOrder();
                    statusOrder.IdOrder = getorder.Id;
                    statusOrder.Status = (int)status;
                    statusOrder.IsCustomer = isCustomer;
                    statusOrder.Note = note;
                    statusOrder.FullNameUpdate = updateby;
                    await _repositoryStatusOrder.AddStatusOrderAsync(statusOrder);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();

                    if (!string.IsNullOrEmpty(getorder.Email))
                    {

                        _logger.LogInformation("Cập nhật trạng thái và gửi email: " + getorder.OrderCode);
                        SendMailAfterUpdateOrder(status, note, getorder, updateby);
                        NotifiUser notifiUser = new NotifiUser();
                        switch (status)
                        {
                            case EnumStatusOrder.Cancel:
                                notifiUser.Title = TitleMailConstant.ban_da_huy_bo_don_hang;
                                notifiUser.Content = ContentMailConstant.ban_da_huy_bo_don_hang(getorder.OrderCode, note, updateby);
                                break;
                            case EnumStatusOrder.Shipping:
                                notifiUser.Title = TitleMailConstant.ban_co_don_hang_dang_van_chuyen;
                                notifiUser.Content = ContentMailConstant.ban_co_don_hang_dang_van_chuyen(getorder.OrderCode, note);
                                break;

                            case EnumStatusOrder.AwaitingConfirmation:
                                notifiUser.Title = TitleMailConstant.ban_co_don_hang_cho_xac_nhan;
                                notifiUser.Content = ContentMailConstant.ban_co_don_hang_cho_xac_nhan(getorder.OrderCode);

                                break;
                            case EnumStatusOrder.Processing:
                                notifiUser.Title = TitleMailConstant.ban_co_don_hang_dang_xu_ly;
                                notifiUser.Content = ContentMailConstant.ban_co_don_hang_dang_xu_ly(getorder.OrderCode, note);

                                break;
                            case EnumStatusOrder.Delivered:
                                notifiUser.Title = TitleMailConstant.ban_co_don_hang_giao_thanh_cong;
                                notifiUser.Content = ContentMailConstant.ban_co_don_hang_giao_thanh_cong(getorder.OrderCode);

                                break;
                            default:

                                break;
                        }

                        notifiUser.OrderCode = getorder.OrderCode;
                        notifiUser.IdUser = getorder.IdCustomer;
                        notifiUser.IdUserGuid = getorder.Customer != null ? getorder.Customer.IdCodeGuid.ToString() : "";
                        notifiUser.Email = getorder.Email;
                        if (!string.IsNullOrEmpty(notifiUser.Title))
                        {
                            await _repositoryNotifyUser.SendNotifyAsync(notifiUser);
                        }
                    }
                    return new ResponseModel<StatusOrder>() { Data = statusOrder, isSuccess = true, Message = HeperConstantss.SUS006 };
                }
                return new ResponseModel<StatusOrder>() { isSuccess = false, Message = HeperConstantss.ERR012 };
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(e, e.Message);
                return new ResponseModel<StatusOrder>() { isSuccess = false, Message = e.Message };
            }
        }
        private void SendMailAfterUpdateOrder(EnumStatusOrder status, string note, Order order, string name)
        {
            //var getcompanyInfoAdmin = await _companyInfoAdminrepository.GetFirstAsync();
            EmailParametersModel emailParameters = new EmailParametersModel();
            emailParameters.noidungcapnhatdonhang = note;//nội dung cập nhập
            emailParameters.tenkhachhang = order.CusName;
            emailParameters.diachigiaohang = order.Address;
            emailParameters.sodienthoai = order.PhoneNumber;
            emailParameters.tongtiendonhang = order.Amount.ToString("N0");
            emailParameters.soluongsanpham = order.Quantity.ToString();
            emailParameters.madonhang = order.OrderCode;
            emailParameters.ngaythangnam = $"Ngày {order.CreatedOn.Day}  Tháng {order.CreatedOn.Month} Năm {order.CreatedOn.Year} {order.CreatedOn.ToString("HH:mm:ss")}";

            string param = CryptoEngine.Encrypt("?code=" + order.OrderCode, _config.Value.Key);
            emailParameters.urldonhang = "/theo-doi-don-hang?secret=" + param;

            emailParameters.trangthaidonhang = GeneralMess.ConvertStatusOrder(status);

            ParametersEmail getkey = null;
            string content = string.Empty;
            string titleEmail = string.Empty;
            if (status == EnumStatusOrder.Cancel)
            {
                // nếu đơn hủy thì chạy cái này
                getkey = _parametersEmailRepository.GetBykey(KeyTitleMail.thong_bao_huy_don_hang);
            }
            else
            {
                getkey = _parametersEmailRepository.GetBykey(KeyTitleMail.thong_bao_cap_nhat_trang_thai_don_hang);
            }
            if (getkey != null)
            {
                content = _mailservice.GetTemplate(emailParameters, getkey.Value);
                titleEmail = _mailservice.GetTemplateTitle(emailParameters, getkey.Title);

                MailRequest mailRequest = new MailRequest
                {
                    Title = titleEmail,
                    Subject = titleEmail,
                    Content = content,
                    EmailTo = order.Email,
                    FullNameUserSend = name,
                    IdTypeUserSend = (int)TypeCustomerEnum.Admin
                };
                var task = Task.Run(() =>
                {
                    _mailservice.SendEmailOne(mailRequest, null, true);
                });

            }
            else
            {
                _logger.LogError("Cập nhật đơn hàng gửi email nhưng k tìm thấy nội dung email để gửi" + order.OrderCode);
            }

        }
    }
}
