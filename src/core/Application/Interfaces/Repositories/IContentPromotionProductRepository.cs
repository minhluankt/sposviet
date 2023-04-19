using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IContentPromotionProductRepository<T>
    {
        Task AddAsync(T Entity);
        //Task UpdateAsync(T Entity);
        Task<bool> DeleteAsync(int id);
        //Task ClosePromotionAsync(int id);//hủy bỏ khuyến mãi

    }
}
