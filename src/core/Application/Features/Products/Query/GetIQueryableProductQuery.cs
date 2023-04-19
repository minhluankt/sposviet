using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Products.Query
{

    public class GetIQueryableProductQuery : IRequest<Result<IQueryable<Product>>>
    {
        public ProductSearch ProductSearch { get; set; }
        public int? take { get; set; }
        public int?[] gia { get; set; }
        public string orderName { get; set; }
        public string orderType { get; set; }
        public bool searchCustome { get; set; }
        public int idCategory { get; set; }
  
        public int Task { get; set; }

      
    }
    public class GetListProductCustomQueryHandler : IRequestHandler<GetIQueryableProductQuery, Result<IQueryable<Product>>>
    {

        private readonly IRepositoryAsync<Product> _repository;
        private readonly IProductPepository<Product> _Product;
        private readonly IMapper _mapper;

        public GetListProductCustomQueryHandler(IRepositoryAsync<Product> ProductCache, IMapper mapper, IProductPepository<Product> Product)
        {
            _Product = Product;
            _repository = ProductCache;
            _mapper = mapper;
        }

        public async Task<Result<IQueryable<Product>>> Handle(GetIQueryableProductQuery request, CancellationToken cancellationToken)
        {
            var modelsearchcus = request.ProductSearch;
            if (!string.IsNullOrEmpty(modelsearchcus.sortby) && modelsearchcus.sortby.Contains("-"))
            {
                modelsearchcus.orderName = modelsearchcus.sortby.Split('-')[0].ToLower();
                modelsearchcus.orderType = modelsearchcus.sortby.Split('-')[1].ToLower();
            }
            var getallBDS = _Product.Search(modelsearchcus, modelsearchcus.keyword);
            return await Result<IQueryable<Product>>.SuccessAsync(getallBDS);
        }
    }
}
