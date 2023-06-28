using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class BarAndKitchenRepository
    {
        private readonly IRepositoryAsync<BarAndKitchen> _barAndKitchenRepository;

        public BarAndKitchenRepository(IRepositoryAsync<BarAndKitchen> barAndKitchenRepository) {
            _barAndKitchenRepository = barAndKitchenRepository;
        }
        public async Task<PaginatedList<BarAndKitchen>> GetPaginatedList(int Comid, string textSearch, string sortColumn, string sortColumnDirection, int pageSize, int skip)
        {
            var datalist = _barAndKitchenRepository.Entities.AsNoTracking().Where(x => x.ComId== Comid);
         
            if (!string.IsNullOrEmpty(textSearch))
            {
                datalist = datalist.Where(m => m.Name.ToLower().Contains(textSearch.ToLower()));
            }
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                datalist = datalist.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            else
            {
                datalist = datalist.OrderByDescending(x => x.Id);
            }
        }
    }
}
