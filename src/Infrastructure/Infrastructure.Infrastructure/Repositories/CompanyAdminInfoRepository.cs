using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class CompanyAdminInfoRepository : ICompanyAdminInfoRepository
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IRepositoryAsync<CompanyAdminInfo> _repositoryAsync;
        public CompanyAdminInfoRepository(IRepositoryAsync<CompanyAdminInfo> repositoryAsync,
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
            _repositoryAsync = repositoryAsync;
        }
        public CompanyAdminInfo GetCompany(int Comid)
        {
            return _repositoryAsync.Entities.AsNoTracking().Where(x => x.Id == Comid).SingleOrDefault();
        }

        public async Task UpdateUserNameCompany(int id, string UserName)
        {
            var get = await _repositoryAsync.Entities.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (get != null)
            {
                get.AccountName = UserName;
                await _repositoryAsync.UpdateAsync(get);
                await _unitOfWork.SaveChangesAsync();
            }

        }
    }
}
