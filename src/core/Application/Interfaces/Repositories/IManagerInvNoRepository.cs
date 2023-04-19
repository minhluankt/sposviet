using Application.Enums;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IManagerInvNoRepository
    {
        Task<int> GetInvNoAsync(int ComId, ENumTypeManagerInv type = ENumTypeManagerInv.Invoice);
        Task<int> UpdateInvNo(int ComId, ENumTypeManagerInv type = ENumTypeManagerInv.Invoice, bool commit = true);
    }
}
