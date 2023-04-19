using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class ManagerPatternEInvoiceRepository : IManagerPatternEInvoiceRepository<ManagerPatternEInvoice>
    {
        private readonly IRepositoryAsync<ManagerPatternEInvoice> _repository;
        private readonly IRepositoryAsync<EInvoice> _repositoryEInvoice;

        public IQueryable<ManagerPatternEInvoice> Entities => _repository.Entities;

        public ManagerPatternEInvoiceRepository(IRepositoryAsync<ManagerPatternEInvoice> repository, IRepositoryAsync<EInvoice> repositoryEInvoice)
        {
            _repositoryEInvoice= repositoryEInvoice;
            _repository = repository;
        }
        public async Task AddAsync(ManagerPatternEInvoice Entity)
        {
            await _repository.AddAsync(Entity);
        }

        public IQueryable<ManagerPatternEInvoice> GetAllAsync()
        {
            return _repository.GetAllQueryable().AsNoTracking();
        }
        public async Task<IResult> DeleteAsync(int Id, int Comid, ENumSupplierEInvoice TypeSupplierEInvoice)
        {
            var checkinvoice = _repositoryEInvoice.Entities.Where(x=>x.IdManagerPatternEInvoice==Id).AsNoTracking().FirstOrDefault();
            if (checkinvoice!=null)
            {
                return await Result.FailAsync("Bạn không thể xóa mẫu số ký hiệu này vì đã phát hành hóa đơn điện tử cho mẫu số này rồi!");
            }
            var getdata = await _repository.Entities.SingleOrDefaultAsync(x => x.Id == Id && x.ComId == Comid&&x.TypeSupplierEInvoice== TypeSupplierEInvoice);
            if (getdata != null)
            {
                await _repository.DeleteAsync(getdata);
                return await Result.SuccessAsync();
            }
            return await Result.FailAsync();
        }

        public async Task<IResult> UpdateAsync(ManagerPatternEInvoice Entity)
        {
            var getdata = await _repository.GetByIdAsync(Entity.Id);
            if (getdata != null)
            {
                if (Entity.Selected != getdata.Selected && !getdata.Selected)
                {
                    var getall = _repository.Entities.Where(x => x.ComId == Entity.ComId && x.Id != Entity.Id).ToList();
                    if (getall.Count() > 0)
                    {
                        getall.ForEach(x => x.Selected = false);
                        await _repository.UpdateRangeAsync(getall);
                       
                    }
                }


                getdata.Pattern = Entity.Pattern;
                getdata.Serial = Entity.Serial;
                getdata.Active = Entity.Active;
                getdata.Selected = Entity.Selected;
                await _repository.UpdateAsync(getdata);

                return await Result.SuccessAsync();
            }
            return await Result.FailAsync();
        }

        public async Task<ManagerPatternEInvoice> GetbykeyAsync(string key)
        {
            return await _repository.Entities.SingleOrDefaultAsync(x=>x.VFkey == key);
        }

        public async Task<ManagerPatternEInvoice> GetbyIdAsync(int id,bool AsNoTracking =false)
        {
            if (AsNoTracking)
            {
                return await _repository.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            }
            return await _repository.Entities.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateRangeAsync(List<ManagerPatternEInvoice> Entity)
        {
            await _repository.UpdateRangeAsync(Entity);
        }
    }
}
