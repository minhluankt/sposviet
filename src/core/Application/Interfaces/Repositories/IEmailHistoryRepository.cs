using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IEmailHistoryRepository<T> where T: class 
    {
        Task<T> AddAsync(T model);
        T Add(T model);
        Task<T> GetByIdAsync(int id);
        Task<bool> UpdateStatusAsync(int id, bool sendAgain = false);
        bool UpdateStatus(int id,bool sendAgain=false);
    }
}
