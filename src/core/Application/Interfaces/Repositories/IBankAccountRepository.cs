using Application.Hepers;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IBankAccountRepository
    {
        Task<Result<BankAccount>> GetDefaultAsync(int ComId);
        Task<Result> DeleteAsync(int ComId, int Id);
        Task<PaginatedList<BankAccount>> GetAllAsync(EntitySearchModel model);
    }
}
