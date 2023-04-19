using AspNetCoreHero.Results;
using Application.CacheKeys;
using Application.Interfaces.CacheRepositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Application.Enums;
using Application.Interfaces.Repositories;
using Model;
using Microsoft.EntityFrameworkCore;
using SystemVariable;

namespace Application.Features.Products.Query
{
    public class QueryProductAutoComplete : IRequest<Result<List<AutocompleteProductPosModel>>>
    {
        public EnumTypeProductCategory enumTypeProductCategory { get; set; } = EnumTypeProductCategory.PRODUCT;
        public int ComId { get; set; }
        public EnumTypeProduct typeProduct { get; set; } = EnumTypeProduct.NONE;
        public int? idCity { get; set; }
        public string text { get; set; }
        public string code { get; set; }
        public bool isRunPromotion { get; set; }//sản phẩm sell  chạy ct theo giờ
        public bool isPromotion { get; set; }//sản phẩm sell k chạy ct
        public bool iSsell { get; set; }//tức là tại giao diện bán hàng 
        public int IdPromotionRun { get; set; }
        public int Take { get; set; } = 15;
        public int Idcate { get; set; }
        public bool IncludeCategory { get; set; }
        public bool CheckExpirationDateDiscount { get; set; }
    }
    public class QueryAutoCompleteHandler : IRequestHandler<QueryProductAutoComplete, Result<List<AutocompleteProductPosModel>>>
    {

        private readonly IRepositoryCacheAsync<Product> _ProductCache;
        private readonly IProductPepository<Product> _Product;
        private readonly IMapper _mapper;

        public QueryAutoCompleteHandler(IRepositoryCacheAsync<Product> ProductCache, IMapper mapper, IProductPepository<Product> Product)
        {
            _Product = Product;
            _ProductCache = ProductCache;
            _mapper = mapper;
        }

        public async Task<Result<List<AutocompleteProductPosModel>>> Handle(QueryProductAutoComplete request, CancellationToken cancellationToken)
        {
            IQueryable<Product> productList = _Product.Query.AsNoTracking().Where(x => !x.StopBusiness);
            if (request.iSsell)
            {
                productList = productList.Where(x => x.DirectSales);
            }
            if (request.typeProduct != EnumTypeProduct.NONE)
            {
                productList = productList.Where(x => x.TypeProduct == request.typeProduct);
            }
            if (request.enumTypeProductCategory == EnumTypeProductCategory.COMBO)
            {
                productList = productList.Where(x => x.TypeProductCategory != EnumTypeProductCategory.COMBO);
            }
            else if (request.enumTypeProductCategory == EnumTypeProductCategory.SERVICE)
            {
                productList = productList.Where(x => x.TypeProductCategory == EnumTypeProductCategory.COOKING);
            }
            if (request.ComId > 0)
            {
                productList = productList.Where(x => x.ComId == request.ComId);
            }
            if (!string.IsNullOrEmpty(request.text))
            {
                if (!string.IsNullOrEmpty(request.code))
                {
                    productList = productList.Where(m => m.Name.ToLower().Contains(request.text.ToLower()) || m.Code.ToLower().Contains(request.code.ToLower()));
                }
                else
                {
                    productList = productList.Where(m => m.Name.ToLower().Contains(request.text.ToLower()));
                }
            }

            if (request.isRunPromotion)
            {
                productList = productList.Where(m => m.isRunPromotion);
            }
            if (request.isPromotion)
            {
                productList = productList.Where(m => m.isPromotion && !m.isRunPromotion);
            }
            if (request.IdPromotionRun > 0)
            {
                productList = productList.Where(m => m.IdPromotionRun == request.IdPromotionRun);
            }
            if (request.CheckExpirationDateDiscount)
            {
                productList = productList.Where(m => m.ExpirationDateDiscount >= System.DateTime.Now);
            }
            if (request.IncludeCategory)
            {
                productList = productList.Include(s => s.CategoryProduct);
            }
            var listkq = productList.OrderBy(x => x.Name).Take(request.Take).Select(x => new AutocompleteProductPosModel
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                typeProductCategory = (int)request.enumTypeProductCategory,
                Img = !string.IsNullOrEmpty(x.Img) ? x.Img : SystemVariableHelper.UrlImgPos,
                Price = x.Price.ToString("F3").Replace(",", "."),
                RetailPrice = x.RetailPrice.ToString("F3").Replace(",", "."),
                Quantity = x.Quantity.ToString("F3").Replace(",","."),
                Length = x.Name.Length,
                IsInventory = x.IsInventory// cho phép không quản lý tồn kho
            }).ToList();
            return await Result<List<AutocompleteProductPosModel>>.SuccessAsync(listkq);
        }
    }
}
