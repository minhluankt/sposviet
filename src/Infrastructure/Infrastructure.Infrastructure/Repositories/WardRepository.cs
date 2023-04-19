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
    public class WardRepository : IWardRepository
    {
        private readonly ILogger<WardRepository> _log;
        private readonly IRepositoryAsync<Ward> _repository;
        public WardRepository(IRepositoryAsync<Ward> repository,
            ILogger<WardRepository> log)
        {
            _log = log;
            _repository = repository;
        }
    

        public IEnumerable<Ward> GetListWardByDistrict(int idDistrict)
        {
            return _repository.Entities.Where(m => m.idDistrict == idDistrict);
        }
    }
}
