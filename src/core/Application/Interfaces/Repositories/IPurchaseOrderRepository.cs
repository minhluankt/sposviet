using Application.Enums;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.ViewModel;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IPurchaseOrderRepository<T>
    {
        Task<PurchaseOrder> GetByIdAsync(int Id);
        Task<PurchaseOrder> GetByCodeAsync(string code);
        Task<Result<bool>> AddAsync(T entity);
        Task<Result<bool>> UpdateAsync(T entity);
        MediatRResponseModel<List<PurchaseOrderModel>> GetAll(int ComId, EnumTypePurchaseOrder Type , string code, int skip, int pageSize,string sortColumn,string sortColumnDirection);
    }
}
