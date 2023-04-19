using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IUserManagerRepository<T>
    {
        bool IsInRoleAsync(T user, string rolevalue);
        Task<T> GetUserAsync(ClaimsPrincipal principal, string schema = "");
        Task<List<T>> GetAllUserAsync(ClaimsPrincipal principal);
        T GetDataUser(string userId);
    }
}
