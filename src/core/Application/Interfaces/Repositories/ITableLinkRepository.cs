using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface ITableLinkRepository
    {
        Task AddAsync(string slug, int tableId, string type, int parentid, int comid, bool commit = true);
        Task UpdateAsync(string slug, int tableId, int parentid, int comid, string type = "");
        Task DeleteAsync(int tableId, int parentid);
        Task<TableLink> GetBySlug(string slug);
    }
}
