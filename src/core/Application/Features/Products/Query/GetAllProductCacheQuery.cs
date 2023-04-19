using Application.Enums;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Products.Query
{
    public class GetAllProductCacheQuery : DatatableModel, IRequest<Result<List<ProductModelView>>>
    {
        public ProductSearch Product { get; set; }
        public EnumTypeProduct TypeProduct { get; set; } = EnumTypeProduct.THOITRANG;
        public GetAllProductCacheQuery()
        {
        }
    }

    public class GetAllProductCachedQueryHandler : IRequestHandler<GetAllProductCacheQuery, Result<List<ProductModelView>>>
    {
        private readonly ICategoryProductRepository<CategoryProduct> _Repositorycate;
        private readonly IRepositoryCacheAsync<Product> _ProductCache;
        private readonly IProductPepository<Product> _Product;
        private readonly IMapper _mapper;

        public GetAllProductCachedQueryHandler(IRepositoryCacheAsync<Product> ProductCache,
            ICategoryProductRepository<CategoryProduct> Repositorycate,
            IMapper mapper, IProductPepository<Product> Product)
        {
            _Repositorycate = Repositorycate;
            _Product = Product;
            _ProductCache = ProductCache;
            _mapper = mapper;
        }

        public async Task<Result<List<ProductModelView>>> Handle(GetAllProductCacheQuery request, CancellationToken cancellationToken)
        {
            int count = 0;
            List<ProductModelView> productList = new List<ProductModelView>();
            if (request.Product.idCategory > 0)
            {
                request.Product.lstidCategory = (_Repositorycate.GetListArrayChillAllByIdAsync(request.Product.idCategory).Result);
            }
            productList = _Product.GetAllIQueryableDatatable2Async(request.Product, request.sortColumn, request.sortColumnDirection, request.pageSize, request.skip, request.TypeProduct, out count);
          
            request.recordsTotal = count;
            return await Result<List<ProductModelView>>.SuccessAsync(productList, count.ToString());
        }
    }
}
