using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class RevenueExpenditureRepository : IRevenueExpenditureRepository<RevenueExpenditure>
    {   private readonly IManagerInvNoRepository _managerInvNorepository;
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IRepositoryAsync<RevenueExpenditure> _repository;
        public RevenueExpenditureRepository(IManagerInvNoRepository managerInvNorepository, IRepositoryAsync<RevenueExpenditure> repository)
        {
            _managerInvNorepository = managerInvNorepository;
            _repository = repository;
        }
         public async Task AddAsync(RevenueExpenditure entity)
        {
          await  _repository.AddAsync(entity); 
        }

        public IQueryable<RevenueExpenditure> GetAllAsync()
        {
            return  _repository.GetAllQueryable();
        }

        public async Task DeleteAsync(int IdInvoice, int ComId)
        {
            var get = await  _repository.Entities.SingleOrDefaultAsync(x=>x.IdInvoice== IdInvoice&&x.ComId==ComId);
            if (get!=null)
            {
                await _repository.DeleteAsync(get);
            }
           
        }
        public async Task CancelAsync(int IdInvoice, int ComId)
        {
            var get = await  _repository.Entities.SingleOrDefaultAsync(x=>x.IdInvoice== IdInvoice&&x.ComId==ComId);
            if (get != null)
            {
                get.Status = Application.Enums.EnumStatusRevenueExpenditure.HUYBO;
                await _repository.UpdateAsync(get);
            }
        } 
        public async Task RestoreAsync(int IdInvoice, int ComId)
        {
            var get = await  _repository.Entities.SingleOrDefaultAsync(x=>x.IdInvoice== IdInvoice&&x.ComId==ComId);
            if (get != null)
            {
                get.Status = Application.Enums.EnumStatusRevenueExpenditure.HOANTHANH;
                await _repository.UpdateAsync(get);
            }
        }
    }
}
