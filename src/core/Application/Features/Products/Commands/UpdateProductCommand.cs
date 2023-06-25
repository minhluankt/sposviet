using Application.CacheKeys;
using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.XmlDataModel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SystemVariable;

namespace Application.Features.Products.Commands
{

    public partial class UpdateProductCommand : Product, IRequest<Result<int>>
    {
        public UpdateProductCommand()
        {
            this.ExtraToppings = new List<ComponentProduct>();
            this.StyleProductModels = new List<StyleProductModel>();
            this.OptionsDetailtProductModels = new List<OptionsDetailtProductModel>();
        }
        public List<ComponentProduct> ExtraToppings { get; set; }
        public List<StyleProductModel> StyleProductModels { get; set; }
        public List<OptionsDetailtProductModel> OptionsDetailtProductModels { get; set; }
        public IFormFile ImgUpload { get; set; }
        public IList<IFormFile> Document { get; set; }
        public IList<IFormFile> albumImgUpload { get; set; }
        public int?[] idattachment { get; set; }
        public int?[] idAccessary { get; set; }
    }
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<int>>
    {
        private readonly ITableLinkRepository _tablelink;
        private readonly IMapper _IMapper;
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IDistributedCache _distributedCache;
        private readonly IContentPromotionProductRepository<ContentPromotionProduct> _contentPromotionProductRepositoryRepository;
        private readonly IRepositoryAsync<Product> _Repository;
        private readonly IRepositoryAsync<ComponentProduct> _ComboProductRepository;
        private readonly IRepositoryAsync<Document> _repositoryDocument;
        private readonly IRepositoryAsync<UploadImgProduct> _repositoryUploadImgProduct;
        private readonly IRepositoryAsync<StyleProduct> _repositoryStyleProduct;
        private readonly IRepositoryAsync<OptionsName> _repositoryOptionsName;
        private readonly IRepositoryAsync<OptionsDetailtProduct> _repositoryOptionsDetailtProduct;
        private IUnitOfWork _unitOfWork { get; set; }

        private readonly ILogger<UpdateProductHandler> _log;

        public UpdateProductHandler(IRepositoryAsync<Product> brandRepository,
            IRepositoryAsync<Document> repositoryDocument,
            IRepositoryAsync<ComponentProduct> ComboProductRepository,
            IRepositoryAsync<StyleProduct> repositoryStyleProduct,
             IRepositoryAsync<OptionsName> repositoryOptionsName,
            IRepositoryAsync<OptionsDetailtProduct> repositoryOptionsDetailtProduct,
        IContentPromotionProductRepository<ContentPromotionProduct> contentPromotionProductRepositoryRepository,
            IRepositoryAsync<UploadImgProduct> repositoryUploadImgProduct,
            ILogger<UpdateProductHandler> log, ITableLinkRepository tablelink,
            IFormFileHelperRepository fileHelper, IMapper IMapper,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _ComboProductRepository = ComboProductRepository;
            _repositoryOptionsName = repositoryOptionsName;
            _repositoryStyleProduct = repositoryStyleProduct;
            _repositoryOptionsDetailtProduct = repositoryOptionsDetailtProduct;
            _repositoryUploadImgProduct = repositoryUploadImgProduct;
            _contentPromotionProductRepositoryRepository = contentPromotionProductRepositoryRepository;
            _repositoryDocument = repositoryDocument;
            _IMapper = IMapper;
            _log = log;
            _tablelink = tablelink;
            _fileHelper = fileHelper;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            _log.LogInformation("UpdateProductCommand Handle start: " + command.Name);
            _unitOfWork.CreateTransaction();
            try
            {
                string imgold = string.Empty;
                string albumold = string.Empty;
                var brand = await _Repository.GetByIdAsync(x => x.Id == command.Id, m => m.Include(x => x.StyleProducts).ThenInclude(x => x.OptionsNames).Include(x => x.OptionsDetailtProducts).Include(x => x.ComponentProducts));

                if (brand == null)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR012);
                }
                else
                {
                    if (brand.Name != command.Name || string.IsNullOrEmpty(brand.Slug))
                    {
                        command.Slug = Common.ConvertToSlug(command.Name);
                        brand.Slug = command.Slug;
                        brand.Name = command.Name;
                    }
                    var checkcode = await _Repository.GetMulti(m => m.Slug == brand.Slug && m.ComId == command.ComId && m.Id != command.Id).FirstOrDefaultAsync();
                    if (checkcode != null)
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR014);
                    }
                    //brand = _IMapper.Map<Product>(command);

                    //if (!string.IsNullOrEmpty(command.albumImg))
                    //{
                    //    albumold = brand.albumImg;
                    //    brand.albumImg = command.albumImg;
                    //}

                    brand.IsEnterInOrder = command.IsEnterInOrder;
                    brand.Title = command.Title;
                    brand.TechnicialParameter = command.TechnicialParameter;
                    brand.Promotion = command.Promotion;
                    brand.PromotionFromDate = command.PromotionFromDate;
                    brand.PromotionToDate = command.PromotionToDate;
                    brand.Packing = command.Packing;

                    if (!string.IsNullOrEmpty(command.Code))
                    {
                        if (command.Code.ToLower()!= brand.Code.ToLower())
                        {
                            var checkcodesite = await _Repository.GetMulti(m => m.Code == brand.Code && m.ComId == command.ComId && m.Id != command.Id).FirstOrDefaultAsync();
                            if (checkcodesite != null)
                            {
                                return await Result<int>.FailAsync("Mã sản phẩm đã tồn tại");
                            }
                            brand.Code = command.Code;
                        }
                    }
                    else
                    {
                        return await Result<int>.FailAsync("Vui lòng nhập mã sản phẩm");
                    }

                    brand.IdUnit = command.IdUnit;
                    brand.IdCategory = command.IdCategory;

                    if (command.isPromotion)
                    {
                        brand.isPromotion = command.isPromotion;
                        brand.Discount = command.Discount;
                        brand.PriceDiscount = command.PriceDiscount;
                        brand.ExpirationDateDiscount = command.ExpirationDateDiscount;
                    }
                    else
                    {
                        brand.isPromotion = command.isPromotion;
                        brand.Discount = 0;
                        brand.PriceDiscount = 0;
                        brand.ExpirationDateDiscount = null;
                    }

                    brand.ComId = command.ComId;
                    brand.isRunPromotion = command.isRunPromotion;
                    brand.IdPromotionRun = command.IdPromotionRun;
                    brand.DiscountRun = command.DiscountRun;
                    brand.PriceDiscountRun = command.PriceDiscountRun;

                    brand.isHotNew = command.isHotNew;
                    brand.isBestseller = command.isBestseller;
                    brand.IsOutstock = command.IsOutstock;
                    ///
                    brand.IsInventory = command.IsInventory;
                    brand.IsServiceDate = command.IsServiceDate;
                    brand.DirectSales = command.DirectSales;
                    brand.ExtraTopping = command.ExtraTopping;

                    if (command.IsInventory)
                    {
                        command.Quantity = 0;
                    }
                    brand.Quantity = command.Quantity;
                    brand.Price = command.Price;
                    //--đơn gia có thuế lưu trước thuế
                    brand.VATRate = command.IsVAT? command.VATRate: (int)NOVAT.NOVAT;
                    brand.IsVAT = command.IsVAT;
                    brand.PriceNoVAT = command.IsVAT ? command.PriceNoVAT : 0;
                    //
                    brand.RetailPrice = command.RetailPrice;
                    brand.idPrice = command.idPrice;
                    brand.IdBrand = command.IdBrand;
                
                    brand.Description = command.Description;

                    brand.Status = command.Status;
                    brand.Active = command.Active;

                    brand.seoDescription = command.seoDescription;
                    brand.seokeyword = command.seokeyword;
                    brand.seotitle = command.seotitle;
                    // phụ tùng kèm theo

                    // upload hình ảnh
                    if (!string.IsNullOrEmpty(command.Img))
                    {
                        brand.Img = command.Img;
                    }
                    else if (command.ImgUpload != null)
                    {
                        imgold = brand.Img;
                        // brand.Img = _fileHelper.UploadedFile(command.ImgUpload, "", FolderUploadConstants.Product);
                        brand.Img = $"{SystemVariableHelper.FolderUpload}{FolderUploadConstants.Product}/{command.ComId}/" + _fileHelper.UploadedFile(command.ImgUpload, "", $"{FolderUploadConstants.Product}/{brand.ComId}");
                    }
                    if (command.albumImgUpload != null && command.albumImgUpload.Count() > 0)
                    {
                        List<UploadImgProduct> UploadImgProducts = new List<UploadImgProduct>();
                        foreach (var item in command.albumImgUpload)
                        {
                            string filename = _fileHelper.UploadedFile(item, "", FolderUploadConstants.Product);
                            UploadImgProduct forms = new()
                            {
                                FileName = filename,
                                IdProduct = brand.Id,
                                Size = item.Length
                            };
                            UploadImgProducts.Add(forms);
                        }
                        await _repositoryUploadImgProduct.AddRangeAsync(UploadImgProducts);
                        // collection.albumImg = _fileHelper.UploadedListFile(collection.albumImgUpload, FolderUploadConstants.Product);
                    }
                    // xử lý combo có thành phần hoặc có món thêm
                    if (command.ComponentProducts.Count()>0 || command.ExtraToppings.Count()>0)
                    {
                        List<ComponentProduct> lstcomponent = brand.ComponentProducts.Where(x=>x.TypeComponentProduct==EnumtypeComponentProduct.COMPONENT).ToList();
                        List<ComponentProduct> lstextratopping = brand.ComponentProducts.Where(x => x.TypeComponentProduct == EnumtypeComponentProduct.EXTRA_TOPPING).ToList();

                        List<ComponentProduct> Removelst = new List<ComponentProduct>();
                        //------------------xóa các cái khác loại-------------------------//
                        var getnottype = brand.ComponentProducts.Where(x=>x.TypeComponentProduct!= EnumtypeComponentProduct.COMPONENT&&x.TypeComponentProduct!= EnumtypeComponentProduct.EXTRA_TOPPING);
                        Removelst.AddRange(getnottype.ToList());
                        //--------------------------------------------//
                        //xử lý cho thành phần
                        foreach (var item in lstcomponent)
                        {
                            var getid = command.ComponentProducts.SingleOrDefault(x=>x.Id == item.Id);//tìm trong cái update vào
                            if (getid!=null)
                            {
                                item.Quantity = getid.Quantity;
                            }
                            else // k có trong cái mới tức là bị xóa đi
                            {
                                Removelst.Add(item);
                            }
                        }
                        //tìm trong cái mới nhưng cái k có trong db tức là thêm mới:
                        var getlstadd = command.ComponentProducts.Where(x => !lstcomponent.Select(z => z.Id).ToArray().Contains(x.Id)).ToList();
                        if (getlstadd.Count() > 0)
                        {
                            getlstadd.ForEach(x => {
                                //x.IdProduct = brand.Id;
                                brand.ComponentProducts.Add(x);
                            });
                        }
                        //--------------------------------------------//
                        // xử lý cho món thêm tìm cái k có và cái có
                        var getnoexit = lstextratopping.Where(x=> !command.ExtraToppings.Select(x=>x.Id).ToArray().Contains(x.Id)).ToList();//phủ định là tìm trong cũ những cái k có trong mới là remove
                        if (getnoexit.Count()>0)
                        {
                            Removelst.AddRange(getnoexit);//xoa cái k có
                        }
                        //tìm trong cái mới nhưng cái k có trong db tức là thêm mới:
                        var getlstaddextra = command.ExtraToppings.Where(x => !lstextratopping.Select(z => z.Id).ToArray().Contains(x.Id)).ToList();
                        if (getlstaddextra.Count() > 0)
                        {
                            getlstaddextra.ForEach(x => {
                                brand.ComponentProducts.Add(x);
                            });
                        }
                        //end
                        //---------------------------xử lý cho update số lượng lại từ thành phần vào db-----------------//
                        foreach (var item in command.ComponentProducts)
                        {
                            var itemToChange = lstcomponent.FirstOrDefault(d => d.Id == item.Id);
                            if (itemToChange != null)
                            {
                                item.Quantity = itemToChange.Quantity;
                            }
                        }

                        //---------------------xóa các bản ghi cần xóa-----------------------//

                        if (Removelst.Count()>0)
                        {
                            //foreach (var item in command.ComponentProducts)
                            //{
                            //    var itemToChange = Removelst.FirstOrDefault(d => d.Id == item.Id);
                            //    if (itemToChange != null)
                            //    {
                            //        command.ComponentProducts.Remove(item);
                            //    }
                            //}

                            await  _ComboProductRepository.DeleteRangeAsync(Removelst);
                        }
                        //--------------------------------------------//
                    }

                    //end
                    if (command.StyleProductModels.Count() > 0) // lưu các style như kích thước, size...
                    {
                        var lstStyexit = brand.StyleProducts.Select(x => x.IdStyleOptionsProduct).ToArray();// cái hiện tại
                        var _lstNewStyle = command.StyleProductModels.Where(x => !lstStyexit.Contains(x.IdStyleOptionsProduct)).ToList();
                        var _lstRemoveStyle = brand.StyleProducts.Where(x => !command.StyleProductModels.Select(x => x.IdStyleOptionsProduct).ToArray().Contains(x.IdStyleOptionsProduct)).ToList();// lấy cáicũ k có trong cái danh sách mới
                        if (_lstNewStyle.Count() > 0)
                        {
                            // xử lý với cái thêm mới
                            List<StyleProduct> styleProduct = _IMapper.Map<List<StyleProduct>>(_lstNewStyle);
                            styleProduct.ForEach(x => x.IdProduct = brand.Id);
                            await _repositoryStyleProduct.AddRangeAsync(styleProduct);
                        }
                        if (_lstRemoveStyle.Count() > 0)
                        {
                            // xử lý với cái remove đi
                            await _repositoryStyleProduct.DeleteRangeAsync(_lstRemoveStyle);
                        }
                        var _lstCurentStyle = brand.StyleProducts.Where(x => command.StyleProductModels.Select(x => x.IdStyleOptionsProduct).ToArray().Contains(x.IdStyleOptionsProduct)).ToList();// lấy cái cũ có trong danh sách mới
                        if (_lstCurentStyle.Count() > 0)//phải khác nhau ms đi tiếp, tức lafc cũ và ms khác nhau
                        {
                            var _lstOptionRemove = new List<OptionsName>();
                            foreach (var item in _lstCurentStyle)
                            {
                                var lstNameCurent = item.OptionsNames;

                                // lấy danh sách các item có trong db từ danh sách truyefn vào để update id
                                var getUpdateOption = command.StyleProductModels.Where(x => x.IdStyleOptionsProduct == item.IdStyleOptionsProduct).SingleOrDefault();
                                if (getUpdateOption != null)
                                {
                                    var arrCompairDb = item.OptionsNames.Select(x => x.Id).ToArray();// từ db
                                    var arrCompairModel = getUpdateOption.OptionsNames.ToList();// từ model truyền vào
                                    arrCompairModel.ForEach(x =>
                                     {
                                         if (!x.Id.HasValue)
                                         {
                                             x.Id = 0;
                                         }
                                     });

                                    if (!Enumerable.SequenceEqual(arrCompairDb, arrCompairModel.Select(x => x.Id.Value).ToArray()))
                                    {
                                        var lstOption = getUpdateOption.OptionsNames.ToList();
                                        foreach (var _item in lstOption)
                                        {
                                            var getVal = lstNameCurent.FirstOrDefault();
                                            _item.IdStyleProduct = getVal.IdStyleProduct;
                                            // var getVal = lstNameCurent.Where(z => z.Id == _item.Id).SingleOrDefault();
                                            //if (getVal != null)
                                            //{
                                            //    _item.Id = getVal.Id;
                                            //}
                                        }
                                        item.OptionsNames = _IMapper.Map<List<OptionsName>>(lstOption);// gán lại cái list
                                        var _arrRemove = lstNameCurent.Where(x => !getUpdateOption.OptionsNames.Select(x => x.Id).ToArray().Contains(x.Id)).ToList();
                                        _lstOptionRemove.AddRange(_arrRemove);
                                    }
                                }
                            }
                            if (_lstOptionRemove.Count() > 0)
                            {
                                await _repositoryOptionsName.DeleteRangeAsync(_lstOptionRemove);
                                // await _unitOfWork.SaveChangesAsync(cancellationToken);
                            }
                            await _repositoryStyleProduct.UpdateRangeAsync(_lstCurentStyle);
                        }

                        if (command.OptionsDetailtProductModels.Count() > 0)
                        {
                            var _filterNew = command.OptionsDetailtProductModels.Where(x => x.ClassName.Contains(ParametersClassStyleProduct.NEW)).ToList();
                            var _filterRemove = command.OptionsDetailtProductModels.Where(x => x.ClassName.Contains(ParametersClassStyleProduct.REMOVE)).ToList();

                            if (_filterNew.Count() > 0)
                            {
                                var lstNew = _IMapper.Map<List<OptionsDetailtProduct>>(_filterNew);
                                lstNew.ForEach(x => x.IdProduct = brand.Id);
                                await _repositoryOptionsDetailtProduct.AddRangeAsync(lstNew);//lưu cái mới
                            }
                            if (_filterRemove.Count() > 0)
                            {
                                var lstRemove = brand.OptionsDetailtProducts.Where(x => _filterRemove.Select(x => x.Id).ToArray().Contains(x.Id)).ToList();
                                if (lstRemove.Count() > 0)
                                {
                                    await _repositoryOptionsDetailtProduct.DeleteRangeAsync(lstRemove);//xóa
                                }
                            }
                            // update các item đã có

                            List<OptionsDetailtProductModel> itemCurent = command.OptionsDetailtProductModels.Where(x => !x.ClassName.Contains(ParametersClassStyleProduct.NEW) && !x.ClassName.Contains(ParametersClassStyleProduct.REMOVE)).OrderByDescending(x => x.Id).ToList();
                            var _getOpDb = brand.OptionsDetailtProducts.Where(x => itemCurent.Select(x => x.Id).ToArray().Contains(x.Id)).OrderByDescending(x => x.Id).ToList();

                            // var lstcurent = _IMapper.Map<List<OptionsDetailtProduct>>(itemCurent);
                            // lstcurent.ForEach(x => x.IdProduct = brand.Id);

                            foreach (var item in _getOpDb)
                            {
                                var forcus = itemCurent.SingleOrDefault(x => x.Id == item.Id);
                                item.Quantity = forcus.Quantity;
                                item.Price = forcus.Price;
                                item.SKU = forcus.SKU;
                                item.Img = forcus.Img;
                            }
                            await _repositoryOptionsDetailtProduct.UpdateRangeAsync(_getOpDb);
                        }
                    }

                    await _Repository.UpdateAsync(brand);
                    await _tablelink.UpdateAsync(brand.Slug, TypeLinkConstants.IdTypeProduct, brand.Id, brand.ComId);
                    await _distributedCache.RemoveAsync(ProductCacheKeys.ListKey);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    await _unitOfWork.CommitAsync();
                    _log.LogInformation("UpdateProductCommand update end: " + command.Name);
                    try
                    {
                        _log.LogInformation("UpdateProductCommand update Image start:" + command.Name);
                        if (!string.IsNullOrEmpty(imgold))
                        {
                            _fileHelper.DeleteFile(imgold, $"{FolderUploadConstants.Product}/{brand.ComId}");
                        }

                        _log.LogInformation("UpdateProductCommand update Image end:" + command.Name);
                    }
                    catch (Exception e)
                    {
                        _log.LogError("UpdateProductCommand update Image error:" + command.Name + "\n" + e.ToString());
                    }

                    return Result<int>.Success(brand.Id);
                }
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                _log.LogInformation("UpdateProductCommand err Exception: " + e.ToString());
                return await Result<int>.FailAsync(e.Message);
            }
        }
    }
}
