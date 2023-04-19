using Application.Constants;
using Application.Extensions;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CategorysProduct.Query
{
    public class GetAllCategoryProductQuery : IRequest<Result<List<CategoryProduct>>>
    {
        public bool IncludebyCategory { get; set; }
        public bool GetPattern { get; set; }
        public int? ComId { get; set; }
        public GetAllCategoryProductQuery()
        {
        }
    }
    public class GetAllCategoryQueryHandler : IRequestHandler<GetAllCategoryProductQuery, Result<List<CategoryProduct>>>
    {
        private readonly IRepositoryAsync<CategoryProduct> _repository;
        private readonly IRepositoryAsync<TypeCategory> _repositoryTypeCategory;
       // private readonly IRepository _repositordy;
        private readonly IMapper _mapper;

        public GetAllCategoryQueryHandler(IMapper mapper, IRepositoryAsync<CategoryProduct> repository, IRepositoryAsync<TypeCategory> repositoryTypeCategory)
        {
            _repositoryTypeCategory = repositoryTypeCategory;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<CategoryProduct>>> Handle(GetAllCategoryProductQuery request, CancellationToken cancellationToken)
        {
            var list = new List<CategoryProduct>();
            var productList =  _repository.GetAllQueryable().AsNoTracking();
            if (request.GetPattern)
            {
                    productList = productList.Where(m => m.IdLevel == 0);
            }
            if (request.ComId !=null)
            {
                    productList = productList.Where(m => m.ComId == request.ComId);
            }
          
            if (request.IncludebyCategory)
            {
                list = await productList.Where(m => m.Active).Include(m => m.Products).Include(m => m.CategoryChilds).ThenInclude(m => m.Products).OrderBy(m => m.Sort).ToListAsync();
            }
            else
            {
                list = await productList.Where(m => m.Active).Include(m => m.CategoryChild).Include(m => m.CategoryChilds).OrderBy(m => m.Sort).ToListAsync();
            }
            
            return await Result<List<CategoryProduct>>.SuccessAsync(list);
        }
    }
}
