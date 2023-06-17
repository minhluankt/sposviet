using Application.CacheKeys;
using Application.Extensions;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Application.Features.DefaultFoodOrders.Query
{

    public class GetPaginatedDefaultFoodOrderQuery : IRequest<Result<PaginatedResult<DefaultFoodOrderModel>>>
    {
        public string Name { get; set; }
        public int? IdCategory { get; set; }
        public int ComId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public GetPaginatedDefaultFoodOrderQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllDefaultFoodOrderdQueryHandler : IRequestHandler<GetPaginatedDefaultFoodOrderQuery, Result<PaginatedResult<DefaultFoodOrderModel>>>
    {

        private readonly IRepositoryAsync<DefaultFoodOrder> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllDefaultFoodOrderdQueryHandler(IRepositoryAsync<DefaultFoodOrder> repositoryAsync,IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedResult<DefaultFoodOrderModel>>> Handle(GetPaginatedDefaultFoodOrderQuery request, CancellationToken cancellationToken)
        {
            var productList =  _repositoryAsync.Entities.AsNoTracking()
                .Include(x=>x.Product)
                .ThenInclude(x=>x.CategoryProduct)
                .Where(x => x.ComId == request.ComId)
                .Select(x=> new DefaultFoodOrderModel()
                {
                    Id=x.Id,
                    IdItem=x.IdItem,
                    IdCategory = x.Product.CategoryProduct.Id,
                    CategoryName = x.Product.CategoryProduct.Name,
                    Price = x.Product.Price,
                    ProName = x.Product.Name,
                    Quantity = x.Quantity,
                    Unit = x.Product.Unit,
                });
            if (!string.IsNullOrEmpty(request.Name))
            {
                productList = productList.Where(x=>x.ProName.ToLower().Contains(request.Name.ToLower()));
            }  
            if (request.IdCategory!=null)
            {
                productList = productList.Where(x=>x.IdCategory== request.IdCategory);
            }
            var list = await productList.ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return await Result<PaginatedResult<DefaultFoodOrderModel>>.SuccessAsync(list);
        }
    }
}
