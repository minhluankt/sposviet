using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class SuppliersRepository : ISuppliersRepository
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly ILogger<SuppliersRepository> _log;
        private readonly IRepositoryAsync<Suppliers> _repository;
        public SuppliersRepository(IRepositoryAsync<Suppliers> repository, IUnitOfWork unitOfWork,
            ILogger<SuppliersRepository> log)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repository = repository;
        }
        public IQueryable<Suppliers> GetAll(int ComId)
        {
            return _repository.Entities.Where(x => x.ComId == ComId);
        }
        public async Task<Suppliers> GetById(int ComId,int Id)
        {
            return await _repository.Entities.Where(x => x.ComId == ComId && x.Id==Id).Include(x=>x.PurchaseOrders).SingleOrDefaultAsync();
        }
        public async Task<Suppliers> UpdateCongNo(int ComId, int Id,decimal Amount)
        {
            var get = await  _repository.Entities.SingleOrDefaultAsync(x => x.ComId == ComId && x.Id== Id);
            if (get != null)
            {
                get.Amount += Amount;
                await _repository.UpdateAsync(get);
            }
            return get;
        }
    }
}
