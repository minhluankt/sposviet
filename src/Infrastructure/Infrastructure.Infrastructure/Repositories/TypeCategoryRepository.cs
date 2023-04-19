using Application.Enums;
using Application.Interfaces.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class TypeCategoryRepository : ITypeCategoryRepository<TypeCategory>
    {
        private readonly IRepositoryAsync<TypeCategory> _repository;
        public TypeCategoryRepository(IRepositoryAsync<TypeCategory> repository)
        {
            _repository = repository;
        }
        public TypeCategory GetByCode(string code, int? ProductType = null)
        {
            if (ProductType != null && ProductType.HasValue)
            {
                code = "phu-tung";
                if (ProductType == (int)ProductEnumcs.Procuct)
                {
                    code = "san-pham";
                }
            }
            var data = _repository.Entities.Where(m => m.Code == code).FirstOrDefault();
            return data;
        }
    }
}
