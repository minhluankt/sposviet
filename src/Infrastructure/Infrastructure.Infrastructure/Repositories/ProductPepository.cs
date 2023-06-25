using Application.CacheKeys;
using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using Application.Providers;
using AspNetCoreHero.Results;
using Domain.Entities;
using Domain.Identity;
using Domain.XmlDataModel;
using Infrastructure.Infrastructure.DbContexts;
using Domain.Identity;
using IronBarCode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model;
using NStandard.Evaluators;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Utilities;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using SystemVariable;
using X.PagedList;

namespace Infrastructure.Infrastructure.Repositories
{
    public class ProductPepository : IProductPepository<Product>
    {
        private readonly IManagerInvNoRepository _managerInvNoPepository;
        private readonly IRepositoryAsync<CompanyAdminInfo> _companyrepository;

        private readonly IRepositoryAsync<UploadImgProduct> _repositoryUploadImgProduct;
        private readonly IRepositoryAsync<Document> _repositoryDocument;
        private readonly IRepositoryAsync<OptionsName> _repositoryOptionsName;
        private readonly IRepositoryAsync<StyleProduct> _repositoryStyleProduct;
        private readonly IRepositoryAsync<OptionsDetailtProduct> _repositoryOptionsDetailtProduct;
        private readonly IRepositoryAsync<Customer> _repositoryCusomer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly ILogger<ProductPepository> _log;
        private readonly IRepositoryCacheAsync<Product> _prorductcacherepository;
        private readonly ITableLinkRepository _tablelink;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserManagerRepository<ApplicationUser> _userManagerRepository;

        private UserManager<ApplicationUser> _userManager;
        private readonly IFormFileHelperRepository _fileHelper;
        //  private ApplicationDbContext _ApplicationDbContext;
        //   private IdentityContext _IdentityContext;
        private IUnitOfWork _unitOfWork { get; set; }
      

        private readonly IRepositoryAsync<Product> _repository;
        public ProductPepository(IRepositoryAsync<Product> repository,
            IRepositoryAsync<UploadImgProduct> repositoryUploadImgProduct,
            IRepositoryAsync<Document> repositoryDocument, IManagerInvNoRepository managerInvNoPepository,
            IRepositoryAsync<OptionsName> repositoryOptionsName,
            IRepositoryAsync<StyleProduct> repositoryStyleProduct,
            IRepositoryAsync<OptionsDetailtProduct> repositoryOptionsDetailtProduct,
            IRepositoryAsync<Customer> repositoryCusomer, IRepositoryAsync<CompanyAdminInfo> companyrepository,
            IRepositoryCacheAsync<Product> prorductcacherepository, IServiceScopeFactory serviceScopeFactory,
            IHttpContextAccessor httpContextAccessor,
         //IdentityContext IdentityContext,
         ILogger<ProductPepository> log,
         IUserManagerRepository<ApplicationUser> userManagerRepository,
         IOptions<CryptoEngine.Secrets> config,
          IFormFileHelperRepository fileHelper,
       ITableLinkRepository tablelink,
        UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _managerInvNoPepository = managerInvNoPepository;
            _repositoryOptionsName = repositoryOptionsName;
            _repositoryStyleProduct = repositoryStyleProduct;
            _repositoryOptionsDetailtProduct = repositoryOptionsDetailtProduct;
            _repositoryUploadImgProduct = repositoryUploadImgProduct;
            _repositoryDocument = repositoryDocument;
            _companyrepository = companyrepository;
            _repositoryCusomer = repositoryCusomer;
            _serviceScopeFactory = serviceScopeFactory;
            _config = config;
            _userManagerRepository = userManagerRepository;
            _prorductcacherepository = prorductcacherepository;
            _fileHelper = fileHelper;

            //  _IdentityContext = IdentityContext;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _log = log;
            _userManager = userManager;
            //_ApplicationDbContext = ApplicationDbContext;
            _repository = repository;
            _tablelink = tablelink;
        }
        public IQueryable<Product> Query => _repository.Entities;
        public List<Product> GetAllIQueryableDatatable(ProductSearch model, string sortColumn, string sortColumnDirection, int pageSize, int skip, out int recordsTotal)
        {
            //var datalist = from x in _ApplicationDbContext.Product
            //        join
            //        c in _IdentityContext.Users on x.CreatedBy equals c.Id into ps
            //        from c in ps.DefaultIfEmpty()
            //               select new ProductModelView
            //               {
            //                   Code = x.Code,
            //                   Name = x.Name,
            //                   Img = x.Name,
            //                   albumImg = x.Name,
            //                   Price = x.Price,
            //                   Quantity = x.Quantity,
            //                   CreatedOn = x.CreatedOn,
            //                   CreatedBy = c.FullName,
            //                   Id = x.Id,

            //                   Status = x.Status,
            //                   idBrands = x.idBrands,
            //                   idCountry = x.idCountry,
            //                   idCapacity = x.idCapacity,
            //                   idEngine = x.idEngine,
            //                   idForkLength = x.idForkLength,
            //                   idLoadCenter = x.idLoadCenter,
            //                   idMast = x.idLoadCenter,
            //                   idModel = x.idLoadCenter,
            //                   idProductType = x.idLoadCenter,
            //                   idSeries = x.idLoadCenter,
            //                   idYoM = x.idLoadCenter,
            //                   idTyres = x.idLoadCenter,
            //               };
            var datalist = _repository.GetAllQueryable().AsNoTracking();

           
            if (!string.IsNullOrEmpty(model.Name))
            {
                datalist = datalist.Where(m => m.Name.ToLower().Contains(model.Name.ToLower()));
                //  datalist = datalist.Where(m => m.Name.ToLower().Contains(model.TextName.ToLower()) || m.Code.ToLower().Contains(model.TextName.ToLower()));
            }
            if (!string.IsNullOrEmpty(model.IdUser))
            {
                datalist = datalist.Where(m => m.CreatedBy == model.IdUser);
            }


            if (model.idCustomer > 0)
            {
                datalist = datalist.Where(m => m.IdCustomer == model.idCustomer);
            }

            if (model.idCategory > 0)
            {
                datalist = datalist.Where(m => m.IdCategory == model.idCategory);
            }

            recordsTotal = datalist.Count();
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                datalist = datalist.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            else
            {
                datalist = datalist.OrderByDescending(x => x.Id);
            }
            if (recordsTotal == 0)
            {
                return new List<Product>();
            }
            var data = datalist.Skip(skip).Take(pageSize).ToList();
            return data;
        }
        public List<ProductModelView> GetAllIQueryableDatatable2Async(ProductSearch model, string sortColumn, string sortColumnDirection, int pageSize, int skip, EnumTypeProduct typeProduct, out int recordsTotal)
        {
            var datalist = _repository.GetAllQueryable().AsNoTracking().Where(x => x.TypeProduct == typeProduct && x.ComId == model.ComId);
            if (!string.IsNullOrEmpty(model.Name))
            {
                datalist = datalist.Where(m => m.Name.ToLower().Contains(model.Name.ToLower()));
                //  datalist = datalist.Where(m => m.Name.ToLower().Contains(model.TextName.ToLower()) || m.Code.ToLower().Contains(model.TextName.ToLower()));
            }

            if (!string.IsNullOrEmpty(model.Code))
            {
                datalist = datalist.Where(m => m.Code.ToLower().Contains(model.Code.ToLower()));
                //  datalist = datalist.Where(m => m.Name.ToLower().Contains(model.TextName.ToLower()) || m.Code.ToLower().Contains(model.TextName.ToLower()));
            }

            if (!string.IsNullOrEmpty(model.IdUser))
            {
                datalist = datalist.Where(m => m.CreatedBy == model.IdUser);
            }

            if (model.StatusProduct==EnumStatusProduct.NGUNG_HOAT_DONG)
            {
                datalist = datalist.Where(m => m.StopBusiness);
            }

            if (model.StatusProduct == EnumStatusProduct.DANG_HOAT_DONG)
            {
                datalist = datalist.Where(m => !m.StopBusiness);
            }

            if (model.isCustomer && model.idCustomer > 0)
            {
                datalist = datalist.Where(m => m.IdCustomer == model.idCustomer);
            }
            if (model.idPrice > 0)
            {
                datalist = datalist.Where(m => m.idPrice == model.idPrice);
            }

            if (model.idCategory > 0)
            {
                datalist = datalist.Where(m => m.IdCategory== model.idCategory);
            }
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                datalist = datalist.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            recordsTotal = datalist.Count();

            if (recordsTotal == 0)
            {
                return new List<ProductModelView>();
            }
            var data = datalist.OrderByDescending(m => m.Id).Skip(skip).Include(mx => mx.CategoryProduct).ThenInclude(mx => mx.CategoryChild).Take(pageSize).ToList();
            var products = data.Select(x => new ProductModelView
            {
                NameCategory = (x.CategoryProduct.CategoryChild != null ? x.CategoryProduct.CategoryChild.Name + "/" : "") + x.CategoryProduct.Name,
                Name = x.Name,
                Code = x.Code,
                Img = x.Img,
                albumImg = x.AlbumImg,
                _Quantity = x.Quantity.ToString().Replace(",", "."),
                StopBusiness = x.StopBusiness,
                Quantity = x.Quantity,
                _Price = x.Price.ToString().Replace(",", "."),
                _RetailPrice = x.RetailPrice.ToString().Replace(",","."),

                Slug = x.Slug,
                CreatedOn = x.CreatedOn,
                CreatedBy = x.CreatedBy,
                IsInventory = x.IsInventory,
                ViewNumber = x.ViewNumber,
                VATRate = x.VATRate,
                _PriceNoVAT = x.PriceNoVAT.ToString().Replace(",", "."),
                IsVAT = x.IsVAT,
                IsEnterInOrder = x.IsEnterInOrder,
                isRunPromotion = x.isRunPromotion,
                Id = x.Id,
                Status = x.Status,
                TypeProductCategory = x.TypeProductCategory,
                idCategory = x.IdCategory,
                idCustomer = x.IdCustomer,
                isCustomer = x.isCustomer,
                CusName = x.Customer != null ? x.Customer.Name : "",
                Active = x.Active,
            }).ToList();

            foreach (var item in products)
            {
                var values = "id=" + item.Id;
                var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                item.UrlParameters = secret;
                item.CreatedBy = GetCreateByAsync(item.isCustomer, item.idCustomer, item.CreatedBy).Result;
            }
            //.Include(m=>m.ApplicationUser).ToList();
            return products;
        }
        private async Task<string> GetCreateByAsync(bool isSell, int? idSell, string Userid)
        {
            if (isSell)
            {
                if (idSell != null && idSell > 0)
                {
                    var customer = await _repositoryCusomer.GetByIdAsync(idSell.Value);
                    if (customer != null)
                    {
                        return customer.Name;
                    }
                }
            }
            else
            {
                var user = _userManagerRepository.GetDataUser(Userid);
                if (user != null)
                {
                    return user.FullName;
                }
            }
            return String.Empty;

        }
        public Product GetById(int id,bool InCategoryProduct = true,bool InCustomer = true,bool InUploadImgProducts = true)
        {
          var  get =_repository.Entities.Where(m => m.Id == id);
            if (InCategoryProduct)
            {
                get = get.Include(m => m.CategoryProduct);
            }
            if (InCustomer)
            {
                get = get.Include(m => m.Customer);
            }
            if (InUploadImgProducts)
            {
                get = get.Include(m => m.UploadImgProducts);
            }
            return get.SingleOrDefault();
        }

        public bool CheckProductbyCategoryId(int id)
        {
            return _repository.Entities.Where(m => m.IdCategory == id).Any();
        }
        public IEnumerable<Product> GetProductbyCategoryId(int id)
        {
            return _repository.Entities.Where(m => m.IdCategory == id).Include(m => m.CategoryProduct)

                    .Include(m => m.Customer)

                    .Include(m => m.UploadImgProducts);
        }
        public IEnumerable<Product> GetProductbyListCategoryId(int[] lstid)
        {
            return _repository.Entities.Where(m => lstid.Contains(m.IdCategory));
        }
        public bool CheckProductbyListCategoryId(int[] listid)
        {
            return _repository.Entities.Where(m => listid.Contains(m.IdCategory)).Any();
        }
        public async Task<IResult<Product>> AddAsync(Product product, IList<IFormFile> Document, IList<IFormFile> albumImg, IFormFile ImgUpload, EnumTypeProduct enumTypeProduct = EnumTypeProduct.THOITRANG)
        {
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(product.Code))
                {
                    int prono = await _managerInvNoPepository.UpdateInvNo(product.ComId, ENumTypeManagerInv.Product, false);
                    product.Code = GetNoProduct.get_ma_sp(prono);
                }
                product.Slug = Common.ConvertToSlug(product.Name +"-"+ product.Code + product.ComId);
                var listStylePro = product.StyleProducts.ToList();
                var listTableStylePro = product.OptionsDetailtProducts.ToList();
                // hình ảnh đại diện
                //product.Img = _fileHelper.UploadedFile(ImgUpload, "", FolderUploadConstants.Product);
                if (enumTypeProduct == EnumTypeProduct.AMTHUC || enumTypeProduct == EnumTypeProduct.BAN_LE || enumTypeProduct == EnumTypeProduct.TAPHOA_SIEUTHI|| enumTypeProduct == EnumTypeProduct.THOITRANG)
                {
                    if (ImgUpload!=null && ImgUpload.Length>0)
                    {
                        product.Img = $"{SystemVariableHelper.FolderUpload}{FolderUploadConstants.Product}/{product.ComId}/" + _fileHelper.UploadedFile(ImgUpload, "", $"{FolderUploadConstants.Product}/{product.ComId}");
                    }
                    
                }
                product.StyleProducts = null;
                product.OptionsDetailtProducts = null;
                product.CreatedBy = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
                product.CreatedOn = DateTime.Now;

              
                var fidn = _repository.Entities.AsNoTracking().Where(m => m.Slug == product.Slug && m.ComId == product.ComId).SingleOrDefault();
                if (fidn != null)
                {
                    return Result<Product>.Fail(GeneralMess.ConvertStatusToString(HeperConstantss.ERR018));
                }

                _repository.Add(product);
                await _unitOfWork.SaveChangesAsync(new CancellationToken());
                if (listStylePro.Count() > 0)
                {
                    // lưu style
                    listStylePro.ForEach(x => x.IdProduct = product.Id);
                    await _repositoryStyleProduct.AddRangeAsync(listStylePro);
                    await _unitOfWork.SaveChangesAsync(new CancellationToken());

                    var _optioname1 = new List<OptionsName>();
                    var _optioname2 = new List<OptionsName>();
                    var _nlistStylePro = listStylePro.OrderBy(x => x.Sort);
                    for (int i = 0; i < listStylePro.Count(); i++)
                    {
                        foreach (var item in listStylePro[i].OptionsNames.OrderBy(x => x.Sort))
                        {
                            if (i == 0)
                            {
                                _optioname1.Add(item);
                            }
                            else if (i == 1)
                            {
                                _optioname2.Add(item);
                            }
                        }
                    }
                    var _OptionsDetailtProduct = new List<OptionsDetailtProduct>();
                    foreach (var item in _optioname1)
                    {
                        if (_optioname2.Count() > 0)
                        {
                            foreach (var item2 in _optioname2)
                            {
                                _OptionsDetailtProduct.Add(new OptionsDetailtProduct() { IdOptionsName = $"{item.Id}_{item2.Id}", Name = $"{item.Name}/{item2.Name}" });
                            }
                        }
                        else
                        {
                            _OptionsDetailtProduct.Add(new OptionsDetailtProduct() { IdOptionsName = $"{item.Id}", Name = $"{item.Name}" });
                        }
                    }


                    listTableStylePro.ForEach(x => x.IdProduct = product.Id);
                    listTableStylePro.ForEach(x => { if (_OptionsDetailtProduct.FirstOrDefault(z => z.Name == x.Name) != null) { x.IdOptionsName = _OptionsDetailtProduct.FirstOrDefault(z => z.Name == x.Name).IdOptionsName; } });

                    await _repositoryOptionsDetailtProduct.AddRangeAsync(listTableStylePro);

                }
                await _tablelink.AddAsync(product.Slug, TypeLinkConstants.IdTypeProduct, TypeLinkConstants.TypeProduct, product.Id, product.ComId);
                await _unitOfWork.SaveChangesAsync(new CancellationToken());



                // thêm các tài liệu
                if (Document.Count() > 0)
                {
                    List<Document> Documents = new List<Document>();
                    foreach (var item in Document)
                    {
                        string filename = _fileHelper.UploadedFile(item, "", $"{FolderUploadConstants.ProductDocument}/{product.ComId}");
                        Document forms = new()
                        {
                            Name = filename,
                            IdProduct = product.Id,
                            Size = item.Length
                        };
                        Documents.Add(forms);
                    }
                    await _repositoryDocument.AddRangeAsync(Documents);
                    await _unitOfWork.SaveChangesAsync(new CancellationToken());
                }
                //thêm các img upload
                if (albumImg != null && albumImg.Count() > 0)
                {
                    List<UploadImgProduct> UploadImgProducts = new List<UploadImgProduct>();
                    foreach (var item in albumImg)
                    {
                        string filename = _fileHelper.UploadedFile(item, "", $"{FolderUploadConstants.Product}/{product.ComId}");
                        UploadImgProduct forms = new()
                        {
                            FileName = filename,
                            IdProduct = product.Id,
                            Size = item.Length
                        };
                        UploadImgProducts.Add(forms);
                    }
                    await _repositoryUploadImgProduct.AddRangeAsync(UploadImgProducts);
                    await _unitOfWork.SaveChangesAsync(new CancellationToken());
                    // collection.albumImg = _fileHelper.UploadedListFile(collection.albumImgUpload, FolderUploadConstants.Product);
                }

                await _unitOfWork.CommitAsync();
            }
            catch (Exception e)
            {
                _log.LogError(e, e.Message);
                _unitOfWork.Rollback();
                // thêm các tài liệu
                if (Document.Count() > 0)
                {
                    foreach (var item in Document)
                    {
                        _fileHelper.UploadedFile(item, "", FolderUploadConstants.ProductDocument);
                    }
                }
                //thêm các img upload
                if (albumImg != null && albumImg.Count() > 0)
                {
                    foreach (var item in albumImg)
                    {
                        _fileHelper.DeleteFile(item.FileName, FolderUploadConstants.Product);
                    }
                    // collection.albumImg = _fileHelper.UploadedListFile(collection.albumImgUpload, FolderUploadConstants.Product);
                }
                if (ImgUpload != null)
                {
                    _fileHelper.DeleteFile(ImgUpload.FileName, FolderUploadConstants.Product);
                }
                throw new ArgumentException(e.Message, e);
            }
            return Result<Product>.Success(product);
        }

        public async Task<List<Product>> GetListProductCacheAsync(string text = "", bool slug = false)
        {
            var products = await _prorductcacherepository.GetCachedListAsync(ProductCacheKeys.ListKey);
            if (!string.IsNullOrEmpty(text))
            {
                if (!slug)
                {
                    text = Common.ConvertToSlug(text);
                }
                products = products.Where(m => m.Slug.Contains(text)).OrderByDescending(m => m.Name).ToList();

            }
            return products;

        }

        public IQueryable<Product> Search(ProductSearch model, string keyword)
        {
            var getall = _repository.Entities;
            if (model.ComId > 0)
            {
                getall = getall.Where(x => x.ComId == model.ComId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                string slug = Common.ConvertToSlug(keyword.Trim());
                getall = getall.Where(m => m.Slug.Contains(slug));
            }

            if (model.lstidCategory != null)
            {
                getall = getall.Where(m => model.lstidCategory.Contains(m.IdCategory));
            } if (model.lstidProduct != null)
            {
                getall = getall.Where(m => model.lstidProduct.Contains(m.Id));
            }
            if (model.idPrice > 0)
            {
                getall = getall.Where(m => m.idPrice == model.idPrice);
            }
            if (model.isPromotion)
            {
                getall = getall.Where(m => m.isPromotion == model.isPromotion && !m.isRunPromotion);
            }
            switch (model.orderName)
            {
                case "name":
                    if (model.orderType == "asc")
                    {
                        getall = getall.OrderBy(m => m.Name);
                    }
                    else
                    {
                        getall = getall.OrderByDescending(m => m.Name);
                    }
                    break;
                case "price":
                    if (model.orderType == "asc")
                    {
                        getall = getall.OrderBy(m => m.Price);
                    }
                    else
                    {
                        getall = getall.OrderByDescending(m => m.Price);
                    }
                    break;
                case "id":
                    getall = getall.OrderByDescending(m => m.Id);
                    break;
                default:
                    getall = getall.OrderByDescending(m => m.Id);
                    break;
            }

            return getall;
        }
        public async Task<IPagedList<Product>> ToPagedListAsync(IQueryable<Product> superset, int pageNumber, int pageSize, CancellationToken cancellationToken = new CancellationToken())
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException($"pageNumber = {pageNumber}. PageNumber cannot be below 1.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException($"pageSize = {pageSize}. PageSize cannot be less than 1.");
            }

            var subset = new List<Product>();
            var totalCount = 0;

            if (superset != null)
            {
                totalCount = superset.Count();
                if (totalCount > 0)
                {
                    subset.AddRange(
                        (pageNumber == 1)
                            ? await superset.Skip(0).Take(pageSize).Include(m => m.UploadImgProducts).ToListAsync(cancellationToken).ConfigureAwait(false)
                            : await superset.Skip(((pageNumber - 1) * pageSize)).Take(pageSize).Include(m => m.UploadImgProducts).ToListAsync(cancellationToken).ConfigureAwait(false)
                    );
                }
            }

            return new StaticPagedList<Product>(subset, pageNumber, pageSize, totalCount);
        }

        public void UpdateReView(int id)
        {
            _log.LogInformation("Update lượt virew sản phẩm");
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    var get = db.Product.Find(id);
                    if (get != null)
                    {
                        get.ViewNumber = get.ViewNumber + 1;
                        db.Product.Update(get);
                        db.SaveChanges();
                    }
                    db.Dispose();
                }
            }
            catch (Exception e)
            {
                _log.LogError(e, e.Message);
            }
        }

        public async Task<PaginatedList<ProductResponseModel>> GetAllProductAsync(ProductApiModelSearch model)
        {
            var iquery = _repository.Entities.Where(x=>x.ComId==model.Comid).AsNoTracking();
            if (!string.IsNullOrEmpty(model.keyword))
            {
                iquery = iquery.Where(x => x.Name.ToLower().Contains(model.keyword.ToLower()));
            }
            if (!string.IsNullOrEmpty(model.Code))
            {
                iquery = iquery.Where(x => x.Code.ToLower().Contains(model.Code.ToLower()));
            }
            if (model.idCategory > 0)
            {
                iquery = iquery.Where(x => x.IdCategory == model.idCategory);
            }
            if (model.isPromotion)
            {
                iquery = iquery.Where(x => x.isPromotion == true && !x.isRunPromotion && x.ExpirationDateDiscount >= DateTime.Now);
            }
            if (model.isPromotionRun)
            {
                iquery = iquery.Where(x => x.isRunPromotion);
            }
            if (string.IsNullOrEmpty(model.sortOn))
            {
                model.sortDirection = "DESC";
                model.sortOn = "Id";
            }
            else
            {
                model.sortDirection = model.sortDirection.ToString();
                model.sortOn = model.sortOn.ToString();
            }
            string urldomain = string.Empty;
            var company = await _companyrepository.Entities.AsNoTracking().SingleOrDefaultAsync(x => x.Id == model.Comid);
            if (company!=null)
            {
                urldomain = company.Website + "/";
            }
            var data = iquery.Select(x => new ProductResponseModel()
            {
                Id = x.Id,
                Name = x.Name,
                Img = urldomain + x.Img,
                isPromotionRun = model.isPromotionRun,
                isPromotion = model.isPromotion,
                PriceSell = model.isPromotion ? (x.PriceDiscount > 0 ? x.PriceDiscount : x.Price - ((decimal)x.Discount / 100 * x.Price)) : 0,
                PriceSellRun = model.isPromotionRun ? (x.PriceDiscount > 0 ? x.PriceDiscount : x.Price - ((decimal)x.Discount / 100 * x.Price)) : 0,
                Price = x.Price
            });
            return await PaginatedList<ProductResponseModel>.ToPagedListAsync(data, model.PageNumber, model.PageSize, model.sortOn, model.sortDirection);
        }

        public async Task UpdateQuantity(List<KeyValuePair<string, decimal>> lst, int Comid, EnumTypePurchaseOrder enumTypePurchaseOrder=EnumTypePurchaseOrder.NHAP_HANG)
        {
            var lstcode = lst.Select(x => x.Key).ToArray();
            var getProcude = _repository.Entities.Where(x => x.ComId == Comid && lstcode.Contains(x.Code)).ToList();
            getProcude.ForEach(x =>
            {
                if (!x.IsInventory && x.TypeProductCategory != EnumTypeProductCategory.SERVICE && x.TypeProductCategory != EnumTypeProductCategory.COMBO)//k phải là sản phẩm k quản lý tồn kho mới update
                {
                    var getCode = lst.SingleOrDefault(z => z.Key == x.Code);
                    if (!string.IsNullOrEmpty(getCode.Key))
                    {
                        if (enumTypePurchaseOrder == EnumTypePurchaseOrder.NHAP_HANG || enumTypePurchaseOrder == EnumTypePurchaseOrder.TRA_HANG_DON)
                        {
                            x.Quantity = x.Quantity + getCode.Value;
                        }
                        else//dạng trả hàng nhập thì trừ đi vì trả đi cho nhà cung cáấp
                        {
                            x.Quantity = x.Quantity - getCode.Value;

                        }
                    }
                }
            });
            if (getProcude.Count()>0)
            {
                await _repository.UpdateRangeAsync(getProcude);
            }
        }

        public async Task<Product> GetByIdAsync(int comid,int id,bool AsNoTracking = false)
        {
            if (AsNoTracking)
            {
                return await _repository.Entities.AsNoTracking().Include(x=>x.UnitType).SingleOrDefaultAsync(x => x.Id == id && x.ComId== comid);
            }
            return await _repository.Entities.Include(x => x.UnitType).SingleOrDefaultAsync(x => x.Id == id && x.ComId == comid);
        }

        public async Task<Product> GetByCodeAsync(int comid, string code, bool AsNoTracking = false)
        {
            if (AsNoTracking)
            {
                return await _repository.Entities.AsNoTracking().Include(x => x.UnitType).SingleOrDefaultAsync(x => x.Code == code && x.ComId == comid);
            }
            return await _repository.Entities.Include(x => x.UnitType).SingleOrDefaultAsync(x => x.Code == code && x.ComId == comid);
        }

        public async Task<Result<int>> UpdateBusiness(int[] lst,int Comid, bool isStop)
        {
            var gets = await _repository.Entities.Where(x => lst.Contains(x.Id) && x.ComId== Comid).ToListAsync();
            if (isStop)
            {
                gets.ForEach(x => x.StopBusiness = true);
            }
            else
            {
                gets.ForEach(x => x.StopBusiness = false);
            }
           
            await _repository.UpdateRangeAsync(gets);
            await _unitOfWork.SaveChangesAsync();
            if (gets.Count()>0)
            {
                return await Result<int>.SuccessAsync(gets.Count());
            }
            return await Result<int>.FailAsync();
        }

        public void MapBarCode(List<Product> lst)
        {
            foreach (var item in lst)
            {
                var myBarcode = BarcodeWriter.CreateBarcode(item.Code, BarcodeWriterEncoding.Code128);
                myBarcode.ResizeTo(100, 20);
                myBarcode.SaveAsImage("EAN8.jpeg");
                string base64String = Convert.ToBase64String(myBarcode.ToPngBinaryData(), 0, myBarcode.ToPngBinaryData().Length);
                item.base64 = "data:image/jpeg;base64," + base64String;
            }
            
        }
        public async Task<List<Product>> GetProductbyListId(int[] lst, int Comid)
        {
            return await _repository.Entities.Where(x => lst.Contains(x.Id) && x.ComId == Comid).AsNoTracking().ToListAsync();
        }

        public IQueryable<Product> GetAll(int comid)
        {
            return  _repository.Entities.Where(x =>x.ComId == comid);
        }
    }
}
