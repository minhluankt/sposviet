using Application.Enums;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IReSearchRepository
    {
        void Add(ReSearch reSearch);
        Task<List<ReSearch>> SearchAsync(string text, ProductEnumcs ProductType = ProductEnumcs.Procuct);
        Task<List<ReSearch>> GetHistoriAsync(ProductEnumcs ProductType = ProductEnumcs.Procuct, int? take = null, bool showhistorylayout = false);
    }
}
