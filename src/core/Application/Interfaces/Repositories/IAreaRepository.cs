using Application.Hepers;
using Domain.ViewModel;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
     public interface IAreaRepository
    {
        Task<PaginatedList<AreasModel>> GetAllAsync(EntitySearchModel model);
    }
}
