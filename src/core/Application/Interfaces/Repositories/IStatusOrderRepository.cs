using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IStatusOrderRepository
    {
        Task AddStatusOrderAsync(StatusOrder model);
        Task<List<StatusOrder>> GetAllByOrderAsync(int idOrder);
    }
}
