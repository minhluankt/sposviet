using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{

    public class ManagerIdCustomerRepository : IManagerIdCustomerRepository
    {
        private IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryProductRepository> _log;
        private readonly IRepositoryAsync<ManagerIdCustomer> _repository;
        public ManagerIdCustomerRepository(IRepositoryAsync<ManagerIdCustomer> repository,
            IUnitOfWork unitOfWork,
            ILogger<CategoryProductRepository> log)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _log = log;
        }
        public async Task<ManagerIdCustomer> GetByComIdAsync(int comid) => await _repository.Entities.Where(x => x.ComId == comid).SingleOrDefaultAsync();
        public async Task<int> UpdateIdAsync(int comid)
        {
            int id = 0;
            var get = await this.GetByComIdAsync(comid);
            if (get == null)
            {
                id = 1;
                _repository.Add(new ManagerIdCustomer() { ComId = comid, CusNo = id });
                await _unitOfWork.SaveChangesAsync();
                return id;
            }
            else
            {
                id = get.CusNo + 1;
                get.CusNo = id;
                await _unitOfWork.SaveChangesAsync();
                return id;
            }
        }
    }
}
