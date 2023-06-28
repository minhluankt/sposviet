using Application.Hepers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Domain.ViewModel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using Application.Providers;
using Microsoft.Extensions.Options;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Application.Constants;
using AspNetCoreHero.Results;
using Microsoft.Extensions.Logging;
using Infrastructure.Infrastructure.Migrations;

namespace Infrastructure.Infrastructure.Repositories
{
    public class ProductInBarAndKitchenRepository : IProductInBarAndKitchenRepository
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private UserManager<ApplicationUser> _userManager;
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IRepositoryAsync<BarAndKitchen> _barAndKitchenRepository;
        private readonly IRepositoryAsync<ProductInBarAndKitchen> _productInBarAndKitchenRepository;
        private readonly IRepositoryAsync<Product> _productepository;
        private readonly IRepositoryAsync<CategoryProduct> _categoryPostrepository;
        private readonly ILogger<ProductInBarAndKitchenRepository> _log;

        public ProductInBarAndKitchenRepository(IRepositoryAsync<ProductInBarAndKitchen> productInBarAndKitchenRepository,
            IOptions<CryptoEngine.Secrets> config, UserManager<ApplicationUser> _userManager, IUnitOfWork _unitOfWork,
            ILogger<ProductInBarAndKitchenRepository> _log, IRepositoryAsync<BarAndKitchen> _barAndKitchenRepository, IRepositoryAsync<CategoryProduct> _categoryPostrepository,
            IRepositoryAsync<Product> productepository)
        {
            _productInBarAndKitchenRepository = productInBarAndKitchenRepository;
            _config = config;
            this._userManager = _userManager;
            this._unitOfWork = _unitOfWork;
            this._log = _log;
            this._barAndKitchenRepository = _barAndKitchenRepository;
            this._categoryPostrepository = _categoryPostrepository;
            _productepository = productepository;
        }
        public async Task<PaginatedList<ProductInBarAndKitchenModel>> GetPaginatedList(int? IdCategory, int IdBarAndKitchen, string textSearch, string sortColumn, string sortColumnDirection, int Currentpage, int pageSize, int skip)
        {
            var datalist = _productInBarAndKitchenRepository.Entities.AsNoTracking().Where(x => x.IdBarAndKitchen == IdBarAndKitchen)
                .Join(_productepository.Entities, p => p.IdProduct, e => e.Id, (barAndKitchen, product) => new ProductInBarAndKitchenModel()
                {
                    Id = barAndKitchen.Id,
                    IdProduct = barAndKitchen.IdProduct,
                    IdBarAndKitchen = barAndKitchen.IdBarAndKitchen,
                    IdCategoryProduct = product.IdCategory,
                    ProCode = product.Code,
                    ProName = product.Name,
                    Price = product.Price,
                    Img = product.Img,
                    CreateDate = barAndKitchen.CreatedOn,
                    CreateBy = barAndKitchen.CreatedBy,
                });

            if (!string.IsNullOrEmpty(textSearch))
            {
                datalist = datalist.Where(m => m.ProName.ToLower().Contains(textSearch.ToLower()));
            } 
            if (IdCategory!=0 &&IdCategory!=null)
            {
                datalist = datalist.Where(m => m.IdCategoryProduct==IdCategory);
            }
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                datalist = datalist.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            else
            {
                datalist = datalist.OrderByDescending(x => x.Id);
            }
            var data = await PaginatedList<ProductInBarAndKitchenModel>.ToPagedListAsync(datalist, Currentpage, pageSize);
            var arrid = data.Items.Select(x=>x.IdCategoryProduct).Distinct().ToArray();
            var getallcategory = await _categoryPostrepository.Entities.AsNoTracking().Where(x => arrid.Contains(x.Id)).ToListAsync();
            data.Items.ForEach( x =>
            {
                var values = "id=" + x.Id;
                var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                x.secret = secret;
                x.CategoryName = getallcategory.FirstOrDefault(z=>z.Id==x.IdCategoryProduct)?.Name;
                x.CreateBy =  _userManager.FindByIdAsync(x.CreateBy).Result.FullName;
            });
            return data;
        }
        public async Task<IResult<Task>> UpdateFoodInBarKitChenAsync(int[] ListId, int IdBarAndKitchen, int ComId)
        {
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                if (ListId != null && ListId.Count() > 0)
                {
                    var getdatadelete = await _productInBarAndKitchenRepository.Entities.AsNoTracking().Where(x => x.IdBarAndKitchen == IdBarAndKitchen).ToListAsync();//check xem hienejt aij cos k
                    if (getdatadelete.Count() == 0)
                    {
                        var getpro = await _productepository.Entities.AsNoTracking().Where(x => x.ComId == ComId && ListId.Contains(x.Id)).Select(x => new
                        ProductInBarAndKitchen()
                        {
                            IdBarAndKitchen = IdBarAndKitchen,
                            IdProduct = x.Id,
                        }
                        ).ToListAsync();
                        if (getpro.Count() == 0)
                        {
                            return await Result<Task>.FailAsync(HeperConstantss.ERR012);
                        }
                        await _productInBarAndKitchenRepository.AddRangeAsync(getpro);
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitAsync();
                        return await Result<Task>.SuccessAsync(HeperConstantss.SUS006);
                    }
                    else
                    {
                        //laasy ra các sản phẩm k có trong database xóa đi
                        bool isUpdate = false;
                        var delete = getdatadelete.Where(x => !ListId.Contains(x.IdProduct)).ToList();
                        if (delete.Count() > 0)
                        {
                            isUpdate = true;
                            await _productInBarAndKitchenRepository.DeleteRangeAsync(delete);
                        }
                        //chuyển thành array từ list gốc đã tìm dc
                        var getidproall = getdatadelete.Select(x => x.IdProduct).ToArray();
                        //lấy ra các id mới cần thêm
                        var getIdNew = ListId.Where(p => !getidproall.Any(p2 => p2 == p));
                        //var result2 = ListId.Where(p => getIdNew.All(p2 => p2 != p));//câu nào cũng đúng
                        if (getIdNew.Count() > 0)
                        {
                            //new list
                            var getpro = await _productepository.Entities.AsNoTracking().Where(x => x.ComId == ComId && getIdNew.Contains(x.Id)).Select(x => new
                            ProductInBarAndKitchen()
                            {
                                IdBarAndKitchen = IdBarAndKitchen,
                                IdProduct = x.Id,
                            }
                            ).ToListAsync();
                            if (getpro.Count() == 0)
                            {
                                return await Result<Task>.FailAsync(HeperConstantss.ERR012);
                            }
                            isUpdate = true;
                            await _productInBarAndKitchenRepository.AddRangeAsync(getpro);
                        }
                        if (isUpdate)
                        {
                            await _unitOfWork.SaveChangesAsync();
                            await _unitOfWork.CommitAsync();
                            return await Result<Task>.SuccessAsync(HeperConstantss.SUS006);
                        }
                    }
                    return await Result<Task>.SuccessAsync(HeperConstantss.ERR012);
                }
                return await Result<Task>.FailAsync(HeperConstantss.ERR012);
            }
            catch (Exception e)
            {
                _log.LogError(e.ToString());
                return await Result<Task>.FailAsync(e.Message);
            }

        }
        public async Task<IResult<Task>> DeleteFoodAsync(int Id, int IdBarAndKitchen, int ComId)
        {
            var checkbar = await _barAndKitchenRepository.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.Id == IdBarAndKitchen && x.ComId == ComId);
            if (checkbar == null)
            {
                return await Result<Task>.FailAsync(HeperConstantss.ERR012);
            }
            var getid = await _productInBarAndKitchenRepository.Entities.SingleOrDefaultAsync(x => x.IdBarAndKitchen == IdBarAndKitchen && x.Id == Id);
            if (getid == null)
            {
                return await Result<Task>.FailAsync(HeperConstantss.ERR012);
            }
            await _productInBarAndKitchenRepository.DeleteAsync(getid);
            await _unitOfWork.SaveChangesAsync();
            return await Result<Task>.SuccessAsync(HeperConstantss.SUS006);
        }
        public async Task<IResult<Task>> DeleteFoodAsync(int[] LstId, int IdBarAndKitchen, int ComId)
        {
            var checkbar = await _barAndKitchenRepository.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.Id == IdBarAndKitchen && x.ComId == ComId);
            if (checkbar == null)
            {
                return await Result<Task>.FailAsync(HeperConstantss.ERR012);
            }
            var getid = _productInBarAndKitchenRepository.Entities.Where(x => x.IdBarAndKitchen == IdBarAndKitchen && LstId.Contains(x.Id));
            if (getid.Count() == 0)
            {
                return await Result<Task>.FailAsync(HeperConstantss.ERR012);
            }
            await _productInBarAndKitchenRepository.DeleteRangeAsync(getid);
            await _unitOfWork.SaveChangesAsync();
            return await Result<Task>.SuccessAsync(HeperConstantss.SUS006);
        }
    }
}
