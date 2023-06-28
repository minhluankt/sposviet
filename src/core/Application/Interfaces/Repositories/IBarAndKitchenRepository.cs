using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IBarAndKitchenRepository
    {
        Task<BarAndKitchen> GetById(int ComId, int Id);
        Task<List<BarAndKitchen>> GetList(int ComId);
    }
}
