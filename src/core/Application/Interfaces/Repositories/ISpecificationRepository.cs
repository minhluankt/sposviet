using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface ISpecificationRepository
    {
        IEnumerable<Specifications> GetByIdType(int idTypeSpecifications, int? idbrand = null);
        Task<Specifications> GetBySlug(string slug);
        Task<Specifications> GetByIdAsync(int id);
    }
}
