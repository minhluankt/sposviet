using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class SpecificationRepository : ISpecificationRepository
    {
        private ApplicationDbContext _ApplicationDbContext;
        private readonly IRepositoryAsync<Specifications> _repository;
        public SpecificationRepository(IRepositoryAsync<Specifications> repository, ApplicationDbContext ApplicationDbContext)
        {
            _ApplicationDbContext = ApplicationDbContext;
            _repository = repository;
        }
        public IEnumerable<Specifications> GetByIdType(int idTypeSpecifications, int? idbrand = null)
        {
            var data = _repository.Entities.Where(m => m.idTypeSpecifications == idTypeSpecifications);

            return data.AsNoTracking();
        }

        public async Task<Specifications> GetBySlug(string slug)
        {
            return await _repository.Entities.Where(m => m.Slug == slug).SingleOrDefaultAsync();
        }

        public async Task<Specifications> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
    }
}
