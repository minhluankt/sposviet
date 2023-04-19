using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Infrastructure.Migrations;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class DetailtKitchenRepository : IDetailtKitchenRepository<DetailtKitchen>
    {
        private IUnitOfWork _unitOfWork { get; }
        private readonly IRepositoryAsync<DetailtKitchen> _ordertableRepository;
        public DetailtKitchenRepository(
            IRepositoryAsync<DetailtKitchen> orderitemtableRepository,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ordertableRepository = orderitemtableRepository;
        }
        public void Add(DetailtKitchen entity)
        {
            _ordertableRepository.Add(entity);
        }

        public async Task Remove(int idKitchen)
        {
            await _ordertableRepository.DeleteRangeAsync(_ordertableRepository.Entities.Where(x => x.IdKitchen == idKitchen));
        }

        public async Task AddRangeAsync(List<DetailtKitchen> entity)
        {
            await _ordertableRepository.AddRangeAsync(entity);
        }

        public async Task UpdateRangeAsync(List<DetailtKitchen> kitchens)
        {
            await _ordertableRepository.UpdateRangeAsync(kitchens);
        }

        public async Task UpdateRangeAsync(List<Kitchen> kitchens)
        {
            var getitem = _ordertableRepository.Entities.Where(x => kitchens.Select(z => z.Id).Contains(x.IdKitchen));
            await getitem.ForEachAsync(x => x.IsRemove = true);
            await _ordertableRepository.UpdateRangeAsync(getitem);
        }

        public async Task RemoveRangeAsync(List<int> kitchens)
        {
            await _ordertableRepository.DeleteRangeAsync(_ordertableRepository.Entities.Where(x => kitchens.Contains(x.IdKitchen)));
        }
    }
}
