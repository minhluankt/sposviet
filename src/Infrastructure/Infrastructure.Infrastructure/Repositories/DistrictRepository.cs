using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class DistrictRepository : IDistrictRepository
    {
        private readonly ILogger<DistrictRepository> _log;
        private readonly IRepositoryAsync<District> _repository;
        public DistrictRepository(IRepositoryAsync<District> repository,
            ILogger<DistrictRepository> log)
        {
            _log = log;
            _repository = repository;
        }
        public IEnumerable<District> GetListDistrict(int idCity)
        {
            return _repository.Entities.Where(m => m.idCity == idCity);
        }
    }
}
