using System.Threading.Tasks;
namespace Application.Interfaces.Repositories
{
    public interface ICommentProductRepository<T>
    {
        Task<bool> AddCommentAsync(T Entity);
    }
}
