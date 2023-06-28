using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
 

    public class BarAndKitchenRepository : IBarAndKitchenRepository
    {
        private readonly IRepositoryAsync<BarAndKitchen> _barAndKitchenRepository;

        public BarAndKitchenRepository(IRepositoryAsync<BarAndKitchen> barAndKitchenRepository)
        {
            _barAndKitchenRepository = barAndKitchenRepository;
        }
        public async Task<List<BarAndKitchen>> GetList(int ComId)
        {
            return await _barAndKitchenRepository.Entities.AsNoTracking().Where(x => x.ComId == ComId).OrderByDescending(x=>x.Id).ToListAsync();
        }
        public async Task<BarAndKitchen> GetById(int ComId, int Id)
        {
            return await _barAndKitchenRepository.Entities.AsNoTracking().Where(x => x.ComId == ComId && x.Id == Id).SingleOrDefaultAsync();
        }
    }
}
