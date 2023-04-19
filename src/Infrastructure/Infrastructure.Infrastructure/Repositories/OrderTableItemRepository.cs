using Application.Interfaces.Repositories;
using Domain.Entities;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class OrderTableItemRepository
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IRepositoryAsync<OrderTableItem> _repository;
        private readonly IRepositoryAsync<Product> _productrepository;
        public OrderTableItemRepository(IUnitOfWork unitOfWork,
            IRepositoryAsync<OrderTableItem> repository,
            IRepositoryAsync<Product> productrepository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _productrepository = productrepository;
        }
        public async Task UpdateItemAsync(Product model)
        {

        }
    }
}
