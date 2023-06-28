using Application.Hepers;
using AspNetCoreHero.Results;
using Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IProductInBarAndKitchenRepository
    {
        Task<IResult<Task>> DeleteFoodAsync(int Id, int IdBarAndKitchen, int ComId);
        Task<IResult<Task>> DeleteFoodAsync(int[] LstId, int IdBarAndKitchen, int ComId);
        Task<PaginatedList<ProductInBarAndKitchenModel>> GetPaginatedList(int? IdCategory, int IdBarAndKitchen, string textSearch, string sortColumn, string sortColumnDirection, int Currentpage, int pageSize, int skip);
        Task<IResult<Task>> UpdateFoodInBarKitChenAsync(int[] ListId, int IdBarAndKitchen, int ComId);
    }
}
