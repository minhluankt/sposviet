using Domain.Entities;
using Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface ICategoryProductRepository<T>
    {
        IEnumerable<T> GetListIncludeProduct(int?[] listid, bool checkActiveProduct = false, int task = 10);
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetListByIdPatternAsync(int id);
        Task<List<T>> GetListChillAllByIdAsync(int id);
        Task<int[]> GetListArrayChillAllByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<List<CategoryMenuModel>> GetListByCodeCacheAsync(string code);
        Task<Task> DeleteByIdPattern(int idPattern);
    }
    public interface ICategoryPostRepository<T>
    {
        Task<List<CategoryPost>> GetByCodeAsync(string code);
        List<CategoryPost> GetByCode(string code);
        IEnumerable<T> GetListIncludePost(int[] listid, int task = 10);
        IQueryable<T> GetListByCode(string code);
        Task<List<CategoryPost>> GetListByIdPatternAsync(int id);
        Task<List<CategoryMenuModel>> GetListByCodeCacheAsync(string code);
        Task<int[]> GetListArrayChillAllByIdAsync(int id);
        Task<Task> DeleteByIdPattern(int idPattern);
        T GetByIdTypeCategory(int idCategory);
        Task<List<CategoryPost>> GetListByIdTypeCategoryAsync(int idtype);
        Task<List<CategoryPost>> GetByIdTypeAsync(int idType); // 0 là sản phẩm 1 alf pt
        int CountbyIdTypeCategory(int idCategory);
        Task<T> GetBySlugAndTypeAsync(string slug, int? idType);
        Task<T> GetByIdAsync(int Id);
    }
}
