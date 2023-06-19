using AspNetCoreHero.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IDefaultFoodOrderRepository<T> where T : class
    {
        Task<IResult<Task>> UpdateFoodAsync(int[] ListId, int ComId);
        Task<IResult<Task>> UpdateQuantityFoodAsync(Guid Id, int ComId, decimal Quantity);
        Task<IResult<Task>> DeleteFoodAsync(Guid Id, int ComId);
        Task<IResult<Task>> DeleteFoodAsync(int[] LstId, int ComId);
    }
}
