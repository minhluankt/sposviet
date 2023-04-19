using Ardalis.Specification;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{

    public partial interface IRepositoryAsync<T> where T : class
    {
        IQueryable<T> Entities { get; }
        T GetById(int id);
        T GetFirstAsNoTracking();
        Task<T> GetFirstAsNoTrackingAsync();
        Task<T> GetFirstAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        Task<T> GetByIdAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IIncludableQueryable<List<T>, object>> include = null);
        Task<T> GetByIdAsync(string id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetPagedReponseAsync(int pageNumber, int pageSize);
        Task<T> AddAsync(T entity);
        T Add(T entity);
        Task<Task> AddRangeAsync(IEnumerable<T> entity);
        Task UpdateAsync(T entity);
        void Update(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entity);
        void UpdateRange(IEnumerable<T> entity);
        void Delete(T entity);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entity);
        IEnumerable<T> GetAll(Expression<Func<T, bool>> expression);
        IEnumerable<T> GetAllEnumerable();
        IQueryable<T> GetAllQueryable();

        Task<T> SingleByExpressionAsync(Expression<Func<T, bool>> expression);
        T SingleByExpression(Expression<Func<T, bool>> expression);
        IQueryable<T> GetMultiPaging(Expression<Func<T, bool>> predicate, out int total, int index = 0, int size = 20);
        IQueryable<T> GetMultiListInclude(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<List<T>, object>> include = null);
        IQueryable<T> GetMulti(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        IEnumerable<T> FindWithSpecificationPattern(ISpecification<T> specification = null);
        IQueryable<T> GetListInclude(Func<IQueryable<T>, IIncludableQueryable<T, object>> include);
        IQueryable<T> GetListInclude(Func<IQueryable<T>, IIncludableQueryable<List<T>, object>> include = null);
    }
}
