using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class StatusOrderRepository: IStatusOrderRepository
    {
        //private readonly IOrderRepository<Order> _orderRepository;
        private readonly ILogger<StatusOrderRepository> _logger;
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IRepositoryAsync<StatusOrder> _repositoryStatusOrder;
        public StatusOrderRepository(IRepositoryAsync<StatusOrder> repositoryCart,
        
            ILogger<StatusOrderRepository> logger,
             IUnitOfWork unitOfWork, IRepositoryAsync<Product> repositoryProduct)
        {
           // _orderRepository = orderRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _repositoryStatusOrder = repositoryCart;
        }

        public async Task AddStatusOrderAsync(StatusOrder model)
        {
            model.CreateDate=DateTime.Now;
             await _repositoryStatusOrder.AddAsync(model);
        }

        public async Task<List<StatusOrder>> GetAllByOrderAsync(int idOrder)
        {
           return await _repositoryStatusOrder.Entities.Where(m => m.IdOrder == idOrder).AsNoTracking().ToListAsync(); 
        }
    }
}
