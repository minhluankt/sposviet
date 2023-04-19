using Application.Interfaces.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class TypeSpecificationRepository : ITypeSpecificationRepository
    {
        private readonly IRepositoryAsync<TypeSpecifications> _repository;
        public TypeSpecificationRepository(IRepositoryAsync<TypeSpecifications> repository)
        {
            _repository = repository;
        }
        public TypeSpecifications GetByCode(string code)
        {
            return _repository.Entities.Where(m => m.Code.ToLower() == code.ToLower().Trim()).SingleOrDefault();
        }
    }
}
