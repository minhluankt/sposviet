using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface ICompanyAdminInfoRepository
    {
        CompanyAdminInfo GetCompany(int Comid);
        Task UpdateUserNameCompany(int id, string UserName);
    }
}
