using Application.Interfaces.Repositories;
using Domain.Entities;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class ContentPromotionProductRepository : IContentPromotionProductRepository<ContentPromotionProduct>
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IRepositoryAsync<ContentPromotionProduct> _repository;
        public ContentPromotionProductRepository(IUnitOfWork unitOfWork, IRepositoryAsync<ContentPromotionProduct> repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }
        public async Task AddAsync(ContentPromotionProduct Entity)
        {
            await _repository.AddAsync(Entity);
        }


        public async Task<bool> DeleteAsync(int id)
        {
            ContentPromotionProduct contentPromotionProduct = await _repository.GetByIdAsync(id);
            if (contentPromotionProduct != null)
            {
                await _repository.DeleteAsync(contentPromotionProduct);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}
