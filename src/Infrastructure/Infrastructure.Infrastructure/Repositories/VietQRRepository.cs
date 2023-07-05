using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class VietQRRepository: IVietQRRepository<VietQR>
    {
        private IUnitOfWork _unitOfWork { get; set; }

        private readonly IRepositoryAsync<VietQR> _vietQRRepository;
        private readonly IRepositoryAsync<BankAccount> _bankAccountRepository;
        public VietQRRepository(IRepositoryAsync<VietQR> _vietQRRepository, IUnitOfWork unitOfWork, IRepositoryAsync<BankAccount> bankAccountRepository)
        {
            _unitOfWork = unitOfWork;
            this._vietQRRepository = _vietQRRepository;
            _bankAccountRepository = bankAccountRepository;
        }
        public async Task<List<VietQR>> GetAllAsync(int Comid)
        {
            return await _vietQRRepository.Entities.Where(x=>x.ComId==Comid).AsNoTracking().Include(x=>x.BankAccount).ToListAsync();
        }
        public async Task<VietQR> GetByIdAsync(int Comid,int id)
        {
            return await _vietQRRepository.Entities.Where(x=>x.ComId==Comid&&x.Id==id).AsNoTracking().Include(x=>x.BankAccount).SingleOrDefaultAsync();
        }

        public async Task<Result<VietQR>> UpdateAsync(VietQR vietQR)
        {
            var getData = await _vietQRRepository.Entities.Where(x => x.ComId == vietQR.ComId && x.Id == vietQR.Id).SingleOrDefaultAsync();
            if (getData != null)
            {
                getData.IdBankAccount= vietQR.IdBankAccount;
                getData.Template= vietQR.Template;
                getData.qrCode= vietQR.qrCode;
                await _vietQRRepository.UpdateAsync(getData);
                await _unitOfWork.SaveChangesAsync();
                if (getData.BankAccount==null)
                {
                    getData.BankAccount = await _bankAccountRepository.Entities.AsNoTracking().Where(x => x.Id == vietQR.IdBankAccount).SingleOrDefaultAsync();
                }
                return await Result<VietQR>.SuccessAsync(getData, HeperConstantss.SUS006);
            }
            return await Result<VietQR>.FailAsync(HeperConstantss.ERR012);
        }
        public async Task<Result<VietQR>> AddAsync(VietQR vietQR)
        {
            var getData = await _vietQRRepository.Entities.AsNoTracking().Where(x => x.ComId == vietQR.ComId && x.IdBankAccount == vietQR.IdBankAccount).SingleOrDefaultAsync();
            if (getData!=null)
            {
                return await Result<VietQR>.FailAsync(HeperConstantss.ERR014);
            }
            await _vietQRRepository.AddAsync(vietQR);
            await _unitOfWork.SaveChangesAsync();
            vietQR.BankAccount = await _bankAccountRepository.Entities.AsNoTracking().Where(x=>x.Id==vietQR.IdBankAccount).SingleOrDefaultAsync();
            return await Result<VietQR>.SuccessAsync(vietQR,HeperConstantss.ERR012);
        }

        public async Task<Result> DeleteAsync(int ComId, int Id)
        {
            var getData = await _vietQRRepository.Entities.Where(x => x.ComId == ComId && x.Id == Id).SingleOrDefaultAsync();
            if (getData != null)
            {
                await _vietQRRepository.DeleteAsync(getData);
                await _unitOfWork.SaveChangesAsync();
                return Result<VietQR>.Success(HeperConstantss.SUS007);
            }
            return await Result<VietQR>.FailAsync(HeperConstantss.ERR012);
        }

        public async Task<Result<VietQR>> GetByFirstAsync(int Comid)
        {
            var getData = await _vietQRRepository.Entities.AsNoTracking().Include(x => x.BankAccount).Where(x => x.ComId == Comid).FirstOrDefaultAsync();
            if (getData != null)
            {
                return Result<VietQR>.Success(getData);
            }
            return await Result<VietQR>.FailAsync(HeperConstantss.ERR012);
        }
    }
}
