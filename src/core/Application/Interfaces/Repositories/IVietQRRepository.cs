using AspNetCoreHero.Results;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IVietQRRepository<T> where T : class
    {
        Task<Result> DeleteAsync(int ComId,int Id);
        Task<Result<VietQR>> AddAsync(VietQR vietQR);
        Task<Result<VietQR>> UpdateAsync(VietQR vietQR);
        Task<VietQR> GetByIdAsync(int Comid, int id);
        Task<Result<VietQR>> GetByFirstAsync(int Comid);
        Task<List<VietQR>> GetAllAsync(int Comid);
    }
}
