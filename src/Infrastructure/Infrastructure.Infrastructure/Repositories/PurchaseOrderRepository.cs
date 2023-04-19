using Application.Interfaces.Repositories;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model;
using Spire.Doc.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Application.Enums;
using AspNetCoreHero.Results;
using Application.Constants;
using NStandard;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Infrastructure.Repositories
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository<PurchaseOrder>
    {
        private readonly IRevenueExpenditureRepository<RevenueExpenditure> _revenueExpenditureRepository;
        private readonly ILogger<PurchaseOrderRepository> _logger;
        private readonly ISuppliersRepository _SuppliersRepository;
        private readonly IManagerInvNoRepository _managerInvNoRepository;
        private IOptions<CryptoEngine.Secrets> _config;
        private IUnitOfWork _unitOfWork;
        private readonly IProductPepository<Product> _Productrepository;
        private readonly IRepositoryAsync<PurchaseOrder> _repository;
        public PurchaseOrderRepository(IRepositoryAsync<PurchaseOrder> repository, IProductPepository<Product> Productrepository,
            ILogger<PurchaseOrderRepository> logger,
            IRevenueExpenditureRepository<RevenueExpenditure> revenueExpenditureRepository,
            IOptions<CryptoEngine.Secrets> config, IManagerInvNoRepository managerInvNoRepository, ISuppliersRepository SuppliersRepository,
            IUnitOfWork unitOfWork)
        {
            _revenueExpenditureRepository = revenueExpenditureRepository;
            _SuppliersRepository = SuppliersRepository;
            _Productrepository = Productrepository;
            _managerInvNoRepository = managerInvNoRepository;
            _config = config;
            _unitOfWork = unitOfWork;
            _repository = repository;
        }
        public MediatRResponseModel<List<PurchaseOrderModel>> GetAll(int ComId, EnumTypePurchaseOrder Type ,string code,int skip,int  pageSize, string sortColumn, string sortColumnDirection)
        {
            var datalist = _repository.GetAllQueryable().AsNoTracking().Where(x => x.Comid == ComId && x.Type== Type);

            if (!string.IsNullOrEmpty(code))
            {
                datalist = datalist.Where(m => m.PurchaseNo.Contains(code));
            }
            int recordsTotal = datalist.Count();
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                datalist = datalist.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            else
            {
                datalist = datalist.OrderByDescending(x => x.Id);
            }
            var data = datalist.Include(x=>x.Suppliers).Skip(skip).Take(pageSize).ToList();
            var products = data.Select(x => new PurchaseOrderModel
            {
              
                Id = x.Id,
                Total = x.Total,
                PurchaseNo = x.PurchaseNo,
               SuppliersCode = x.Suppliers?.CodeSupplier,
               SuppliersName = x.Suppliers?.Name,
                Quantity = x.Quantity,
                DiscountAmount = x.DisCountAmount,
               Amount = x.Amount,
                DebtAmount = x.DebtAmount*-1,
               AmountSuppliers = x.AmountSuppliers,
                Status = x.Status,
               Date = x.CreateDate.ToString("dd/MM/yyyy HH:mm")
            }).ToList();

            foreach (var item in products)
            {
                var values = "id=" + item.Id;
                var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                item.UrlParameters = secret;
               // item.CreatedBy = GetCreateByAsync(item.isCustomer, item.idCustomer, item.CreatedBy).Result;
            }
            var model = new MediatRResponseModel<List<PurchaseOrderModel>>()
            {
                Data = products,
                Count = recordsTotal,
                isSuccess = true
            };
            return model;
        }

        public async Task<Result<bool>> AddAsync(PurchaseOrder entity)
        {
            await _unitOfWork.CreateTransactionAsync();
            try
            {
              
                if (entity.Type == EnumTypePurchaseOrder.NHAP_HANG)
                {
                    var getinv = await _managerInvNoRepository.UpdateInvNo(entity.Comid, ENumTypeManagerInv.ImportGoods, false);
                    entity.PurchaseNo = GetNumberNhapTraHang.ma_nhap_hang(getinv);
                }
                else if (entity.Type == EnumTypePurchaseOrder.TRA_HANG_NHAP)
                {
                    var getinv = await _managerInvNoRepository.UpdateInvNo(entity.Comid, ENumTypeManagerInv.PurchaseReturns, false);
                    entity.PurchaseNo = GetNumberNhapTraHang.ma_tra_hang(getinv);
                }
                else
                {
                    return await Result<bool>.FailAsync("Không đúng kiểu");
                }
                await _repository.AddAsync(entity);
                //lọc mã sp và sl ra
                var list = new List<KeyValuePair<string, decimal>>();
                foreach (var item in entity.ItemPurchaseOrders)
                {
                    list.Add(new KeyValuePair<string, decimal>(item.Code, item.Quantity));
                }

                await _Productrepository.UpdateQuantity(list, entity.Comid, entity.Type);//update vào danh sách sản phẩm tòon kho

                Suppliers suppler = null;
                if (entity.Type == EnumTypePurchaseOrder.NHAP_HANG)
                {
                    if (entity.DebtAmount != 0 && entity.IdSuppliers != null)
                    {
                        suppler =  await _SuppliersRepository.UpdateCongNo(entity.Comid, entity.IdSuppliers.Value, entity.DebtAmount);
                    }
                }
                else if (entity.Type == EnumTypePurchaseOrder.TRA_HANG_NHAP)
                {
                    if (entity.DebtAmount != 0 && entity.IdSuppliers != null)
                    {
                        //nếu tính vào công nợ thì phải lấy nợ NCC - đi số này,vì họ k đưa tiền mặt mà trừ vào nợ hiện tại
                        suppler = await _SuppliersRepository.UpdateCongNo(entity.Comid, entity.IdSuppliers.Value, (entity.DebtAmount>0?entity.DebtAmount*-1:entity.DebtAmount));
                    }
                }
                await _unitOfWork.SaveChangesAsync();

                //đoạn này tạp phiếu chi
                if (entity.AmountSuppliers > 0 && entity.Type == EnumTypePurchaseOrder.NHAP_HANG)//tức là có trả nhà cung cấp
                    {
                   
                    //giwof tạo phiếu thu nhé
                    RevenueExpenditure revenueExpenditure = new RevenueExpenditure()
                    {
                        ComId = entity.Comid,
                        Amount = entity.AmountSuppliers,
                        IdPurchaseOrder = entity.Id,
                        ObjectRevenueExpenditure = suppler!=null? EnumTypeObjectRevenueExpenditure.DOITAC: EnumTypeObjectRevenueExpenditure.DOITUONGKHAC,
                        IdCustomer = entity.IdSuppliers,
                        Type = EnumTypeRevenueExpenditure.CHI,
                        Typecategory = EnumTypeCategoryThuChi.TIENHANG,
                        Title = $"Chi tiền nhập hàng",
                        Date = entity.CreatedOn,
                        Code = $"PC{entity.PurchaseNo}",
                        CustomerName = suppler != null ? suppler.Name : "",
                        Status = EnumStatusRevenueExpenditure.HOANTHANH
                    };
                    await _revenueExpenditureRepository.AddAsync(revenueExpenditure);
                }

                //
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return await Result<bool>.SuccessAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("Trả nhập hàng lỗi: "+e.ToString());
                await _unitOfWork.RollbackAsync();
                return await Result<bool>.FailAsync(e.Message);
            }
           
        }
        public async Task<PurchaseOrder> GetByCodeAsync(string code)
        {
           return await _repository.Entities.SingleOrDefaultAsync(x => x.Code == code);
        }
        public async Task<PurchaseOrder> GetByIdAsync(int Id)
        {
            return await _repository.Entities.SingleOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<Result<bool>> UpdateAsync(PurchaseOrder entity)
        {
            var getudpate = await _repository.Entities.SingleOrDefaultAsync(x=>x.Comid==entity.Comid&& x.Id==entity.Id && x.Type==entity.Type);
            int? oldsupler = getudpate.IdSuppliers;
            int? IdSuppliersnew = entity.IdSuppliers;
            if (getudpate!=null)
            {
                getudpate.IdPayment = entity.IdPayment;
                getudpate.Note = entity.Note;
                getudpate.CreateDate = entity.CreateDate;
                if (string.IsNullOrEmpty(getudpate.PurchaseOrderCode))
                {
                    if (oldsupler != entity.IdSuppliers)//nếu có sự thay đổi
                    {
                        decimal amount = 0;
                        if (oldsupler==null&& entity.IdSuppliers>0 && getudpate.DebtAmount != 0)//th mới 
                        {
                            //tính vào công nợ giảm tiền đi
                            
                                //nếu tính vào công nợ thì phải lấy nợ NCC - đi số này,vì họ k đưa tiền mặt mà trừ vào nợ hiện tại,tức là nợ giảm đi
                                amount = (getudpate.DebtAmount > 0 ? getudpate.DebtAmount * -1 : getudpate.DebtAmount);
                                await _SuppliersRepository.UpdateCongNo(getudpate.Comid, entity.IdSuppliers.Value, amount);
                            
                        }
                        else if (oldsupler != null && entity.IdSuppliers == null && getudpate.DebtAmount != 0)//tức là gỡ và updaye lại thèn cũ trước đó
                        {
                            amount = (getudpate.DebtAmount < 0 ? getudpate.DebtAmount * -1 : getudpate.DebtAmount);
                            await _SuppliersRepository.UpdateCongNo(getudpate.Comid, oldsupler.Value, amount);
                        }
                        else if (oldsupler != null && entity.IdSuppliers != null && getudpate.DebtAmount != 0)
                        {
                            //tính cho thèn cũ trước, gỡ công nợ nó ra lại
                            amount = (getudpate.DebtAmount < 0 ? getudpate.DebtAmount * -1 : getudpate.DebtAmount);
                            await _SuppliersRepository.UpdateCongNo(getudpate.Comid, oldsupler.Value, amount);

                            //tính cho hèn sau trừ công nợ nó ra giảm tiền
                            amount = (getudpate.DebtAmount > 0 ? getudpate.DebtAmount * -1 : getudpate.DebtAmount);
                            await _SuppliersRepository.UpdateCongNo(getudpate.Comid, entity.IdSuppliers.Value, amount);


                        }
                    }
                    getudpate.IdSuppliers = entity.IdSuppliers;// trả đơn k phải từ đơn nhập ớidc
                }
                await _unitOfWork.SaveChangesAsync();
                return await Result<bool>.SuccessAsync();
            }
            return await Result<bool>.FailAsync(HeperConstantss.ERR012);
        }
    }
}
