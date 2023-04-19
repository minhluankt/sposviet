using Application.Enums;
using Application.Hepers;
using AspNetCoreHero.Results;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using X.PagedList;

namespace Application.Interfaces.Repositories
{
    public interface IProductPepository<T> where T : class
    {
        IQueryable<T> Query { get; }
        List<T> GetAllIQueryableDatatable(ProductSearch textSearch, string sortColumn, string sortColumnDirection, int pageSize, int skip, out int count);
        List<ProductModelView> GetAllIQueryableDatatable2Async(ProductSearch textSearch, string sortColumn, string sortColumnDirection, int pageSize, int skip, EnumTypeProduct typeProduct, out int count);
        Task<PaginatedList<ProductResponseModel>> GetAllProductAsync(ProductApiModelSearch model);
        T GetById(int id, bool InCategoryProduct = true, bool InCustomer = true, bool InUploadImgProducts = true);
        IQueryable<T> GetAll(int comid);
        Task<T> GetByIdAsync(int comid, int id, bool AsNoTracking = false);
        Task<T> GetByCodeAsync(int comid, string code, bool AsNoTracking = false);
        void UpdateReView(int id);
        void MapBarCode(List<Product> lst);
        Task<Result<int>> UpdateBusiness(int[] lst, int Comid, bool isStop);
        Task UpdateQuantity(List<KeyValuePair<string, decimal>> lst, int Comid, EnumTypePurchaseOrder enumTypePurchaseOrder = EnumTypePurchaseOrder.NHAP_HANG);
        Task<List<T>> GetListProductCacheAsync(string text = "", bool slug = false);
        bool CheckProductbyCategoryId(int id);
        IEnumerable<T> GetProductbyCategoryId(int id);
        IEnumerable<T> GetProductbyListCategoryId(int[] id);
        Task<List<Product>> GetProductbyListId(int[] lst, int Comid);
        bool CheckProductbyListCategoryId(int[] id);
        Task<IResult<Product>> AddAsync(T model, IList<IFormFile> Document, IList<IFormFile> albumImg, IFormFile ImgUpload, EnumTypeProduct enumTypeProduct = EnumTypeProduct.THOITRANG);
        Task<IPagedList<Product>> ToPagedListAsync(IQueryable<Product> superset, int pageNumber, int pageSize, CancellationToken cancellationToken = new CancellationToken());
        IQueryable<T> Search(ProductSearch model, string keyword);

    }
}
