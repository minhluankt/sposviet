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
    public class ParametersEmailRepository : IParametersEmailRepository
    {
        private readonly ILogger<ParametersEmailRepository> _log;
        private readonly IRepositoryAsync<ParametersEmail> _repository;
        public ParametersEmailRepository(ILogger<ParametersEmailRepository> log, IRepositoryAsync<ParametersEmail> repository)
        {
            _log = log;
            _repository = repository;
        }
        public ParametersEmail GetBykey(string key)
        {
            return _repository.Entities.Where(x => x.Key == key).SingleOrDefault();
        }
    }
}
