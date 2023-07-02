using Application.Hepers;
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
        Task<PaginatedList<BankAccount>> GetAllAsync(EntitySearchModel model);
    }
}
