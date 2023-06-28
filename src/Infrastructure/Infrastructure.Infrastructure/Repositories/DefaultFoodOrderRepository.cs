using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Hangfire.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class DefaultFoodOrderRepository : IDefaultFoodOrderRepository<DefaultFoodOrder>
    {
        private readonly ILogger<ProductInBarAndKitchenRepository> _log;
        private readonly IRepositoryAsync<Product> _productrepository;
        private readonly IRepositoryAsync<DefaultFoodOrder> _repository;
        private IUnitOfWork _unitOfWork { get; set; }
        public DefaultFoodOrderRepository(IUnitOfWork unitOfWork,
            ILogger<ProductInBarAndKitchenRepository> _log,
            IRepositoryAsync<Product> productrepository, IRepositoryAsync<DefaultFoodOrder> repository)
        {
            _unitOfWork = unitOfWork;
            this._log = _log;
            _productrepository = productrepository;
            _repository = repository;
        }

        public async Task<IResult<Task>> UpdateFoodAsync(int[] ListId, int ComId)
        {
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                if (ListId != null && ListId.Count() > 0)
                {
                    var getdatadelete = await _repository.Entities.AsNoTracking().Where(x => x.ComId==ComId).ToListAsync();//check xem hienejt aij cos k
                    if (getdatadelete.Count()==0)
                    {
                        var getpro = await _productrepository.Entities.AsNoTracking().Where(x=>x.ComId==ComId&& ListId.Contains(x.Id)).Select(x=> new
                        DefaultFoodOrder()
                        {
                            IdItem =Guid.NewGuid(),
                            ComId= ComId,
                            IdProduct=x.Id,
                            Quantity = 1,
                        }
                        ).ToListAsync();
                        if (getpro.Count() == 0)
                        {
                            return await Result<Task>.FailAsync(HeperConstantss.ERR012);
                        }
                        await _repository.AddRangeAsync(getpro);
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitAsync();
                        return await Result<Task>.SuccessAsync(HeperConstantss.SUS006);
                    }
                    else
                    {
                        //laasy ra các sản phẩm k có trong database xóa đi
                        bool isUpdate =false;
                        var delete = getdatadelete.Where(x => !ListId.Contains(x.IdProduct)).ToList();
                        if (delete.Count() > 0)
                        {
                            isUpdate = true;
                            await _repository.DeleteRangeAsync(delete);
                        }
                        var  getidproall= getdatadelete.Select(x => x.IdProduct).ToArray();
                        //lấy ra các id mới
                        var getIdNew = ListId.Where(p => !getidproall.Any(p2 => p2 == p));
                        //var result2 = ListId.Where(p => getIdNew.All(p2 => p2 != p));//câu nào cũng đúng
                        if (getIdNew.Count()>0)
                        {
                            //new list
                            var getpro = await _productrepository.Entities.AsNoTracking().Where(x => x.ComId == ComId && getIdNew.Contains(x.Id)).Select(x => new
                            DefaultFoodOrder()
                                {
                                    IdItem = Guid.NewGuid(),
                                    ComId = ComId,
                                    IdProduct = x.Id,
                                    Quantity = 1,
                                }
                            ).ToListAsync();
                            if (getpro.Count() == 0)
                            {
                                return await Result<Task>.FailAsync(HeperConstantss.ERR012);
                            }
                            isUpdate = true;
                            await _repository.AddRangeAsync(getpro);
                        }
                        if (isUpdate)
                        {
                            await _unitOfWork.SaveChangesAsync();
                            await _unitOfWork.CommitAsync();
                            return await Result<Task>.SuccessAsync(HeperConstantss.SUS006);
                        }
                    }
                    return await Result<Task>.SuccessAsync(HeperConstantss.ERR012);
                }
                return await Result<Task>.FailAsync(HeperConstantss.ERR012);
            }
            catch (Exception e)
            {
                _log.LogError(e.ToString());
                return await Result<Task>.FailAsync(e.Message);
            }
          
        }

        public async Task<IResult<Task>> UpdateQuantityFoodAsync(Guid Id, int ComId,decimal Quantity)
        {
            if (Quantity<=0)
            {
                return await Result<Task>.FailAsync(HeperConstantss.ERR000);
            }
            var getid = await _repository.Entities.SingleOrDefaultAsync(x=>x.ComId == ComId&&x.IdItem==Id);
            if (getid == null)
            {
                return await Result<Task>.FailAsync(HeperConstantss.ERR012);
            }
            getid.Quantity = Quantity;
            await _repository.UpdateAsync(getid);
            await _unitOfWork.SaveChangesAsync();
            return await Result<Task>.SuccessAsync(HeperConstantss.SUS006);
        }

        public async Task<IResult<Task>> DeleteFoodAsync(Guid Id, int ComId)
        {
            var getid = await _repository.Entities.SingleOrDefaultAsync(x => x.ComId == ComId && x.IdItem == Id);
            if (getid == null)
            {
                return await Result<Task>.FailAsync(HeperConstantss.ERR012);
            }
            await _repository.DeleteAsync(getid);
            await _unitOfWork.SaveChangesAsync();
            return await Result<Task>.SuccessAsync(HeperConstantss.SUS006);
        } 
        public async Task<IResult<Task>> DeleteFoodAsync(int[] LstId, int ComId)
        {
            var getid =  _repository.Entities.Where(x => x.ComId == ComId && LstId.Contains(x.Id));
            if (getid.Count()==0)
            {
                return await Result<Task>.FailAsync(HeperConstantss.ERR012);
            }
            await _repository.DeleteRangeAsync(getid);
            await _unitOfWork.SaveChangesAsync();
            return await Result<Task>.SuccessAsync(HeperConstantss.SUS006);
        }
    }
}
