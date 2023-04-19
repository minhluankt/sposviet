using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IRevenueExpenditureRepository<T> where T : class
    {
        IQueryable<RevenueExpenditure> GetAllAsync();
        Task AddAsync(T entity);
        Task DeleteAsync(int IdInvoice, int ComId);
    }
}
