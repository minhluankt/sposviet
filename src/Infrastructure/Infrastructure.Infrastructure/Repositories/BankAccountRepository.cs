using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class BankAccountRepository: IBankAccountRepository
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IRepositoryAsync<BankAccount> _repository;
        private readonly IMapper _mapper;

        public BankAccountRepository(
            IRepositoryAsync<BankAccount> repository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<PaginatedList<BankAccount>> GetAllAsync(EntitySearchModel model)
        {
            var iquery = _repository.Entities.Where(x => x.ComId == model.Comid).AsNoTracking();
            if (!string.IsNullOrEmpty(model.Name))
            {
                iquery = iquery.Where(x => x.BankNumber.ToLower().Contains(model.Name.ToLower()) || x.AccountName.ToLower().Contains(model.Name.ToLower()));
            }

            if (string.IsNullOrEmpty(model.sortOn))
            {
                model.sortDirection = "DESC";
                model.sortOn = "Id";
            }
            else
            {
                model.sortDirection = model.sortDirection.ToString();
                model.sortOn = model.sortOn.ToString();
            }

            return await PaginatedList<BankAccount>.ToPagedListAsync(iquery, model.PageNumber, model.PageSize, model.sortOn, model.sortDirection);
        }
        public async Task<Result> DeleteAsync(int ComId,int Id)
        {
            try
            {
                var product = await _repository.Entities.Include(x=>x.VietQR).SingleOrDefaultAsync(x=>x.ComId==ComId && x.Id==Id);
                if (product != null)
                {
                    if (product.VietQR!=null)
                    {
                        return Result<int>.Fail("Tài khoản đã được sử dụng kích hoạt cho VietQR không thể xóa!");
                    }
                    await _repository.DeleteAsync(product);
                    await _unitOfWork.SaveChangesAsync();
                    return Result<int>.Success(product.Id);
                }
                return Result<int>.Fail(HeperConstantss.ERR012);
            }
            catch (System.Exception e)
            {
                return Result<int>.Fail(e.Message);
            }
        }

        public async Task<Result<BankAccount>> GetDefaultAsync(int ComId)
        {
            var product = await _repository.Entities.FirstOrDefaultAsync(x => x.ComId == ComId&&x.IsSetDefault);
            if (product==null)
            {
                return await Result<BankAccount>.FailAsync(HeperConstantss.ERR012);
            }
            return await Result<BankAccount>.SuccessAsync(product);
        }
    }
}
