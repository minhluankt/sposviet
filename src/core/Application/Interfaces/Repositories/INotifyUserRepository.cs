using Application.Hepers;
using Domain.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface INotifyUserRepository<T> where T : class
    {
        IQueryable<T> GetAll(NotifyUserModel model);//
        Task<T> GetByIdAsync(int id);//
        Task<bool> DeleteByIdAsync(int id, int iduser);//
        Task AddAsync(T entity);//
        int CountNotifyNoReviewAsync(int iduser);//
        Task SendNotifyAsync(T entity);//
        Task<bool> UpdateReviewAsync(int id);//
        Task<PaginatedList<T>> GetAllPaginatedListAsync(NotifyUserModel model);//
    }
}
