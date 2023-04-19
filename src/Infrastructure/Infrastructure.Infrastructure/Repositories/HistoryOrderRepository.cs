using Application.Interfaces.Repositories;
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class HistoryOrderRepository : IHistoryOrderRepository<HistoryOrder>
    {
        private IUnitOfWork _unitOfWork { get; }
        private readonly IRepositoryAsync<HistoryOrder> _ordertableRepository;
        public HistoryOrderRepository(
            IRepositoryAsync<HistoryOrder> orderitemtableRepository,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ordertableRepository = orderitemtableRepository;
        }
        public async Task AddHistoryOrder(List<HistoryOrder> model)
        {
            await _ordertableRepository.AddRangeAsync(model);
        }
    }
}
