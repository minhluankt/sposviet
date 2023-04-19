using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
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

    public class GetByIdCategoryProductQuery : IRequest<Result<CategoryProduct>>
    {
        public int Id { get; set; }
        public int ComId { get; set; }
        public bool IncludeProduct { get; set; }

        public class GetCategoryByIdQueryHandler : IRequestHandler<GetByIdCategoryProductQuery, Result<CategoryProduct>>
        {
            private readonly IRepositoryAsync<CategoryProduct> _repository;
            public GetCategoryByIdQueryHandler(IRepositoryAsync<CategoryProduct> repository)
            {
                _repository = repository;
            }
            public async Task<Result<CategoryProduct>> Handle(GetByIdCategoryProductQuery query, CancellationToken cancellationToken)
            {
                CategoryProduct category = new CategoryProduct();
                if (query.IncludeProduct)
                {
                    category = await _repository.GetByIdAsync(m => m.Id == query.Id, include: m => m.Include(m => m.Products).Include(m => m.CategoryChilds));
                }
                else
                {
                    category = await _repository.GetByIdAsync(m => m.Id == query.Id, include: m => m.Include(m => m.CategoryChilds));
                }
                if (category == null)
                {
                    return await Result<CategoryProduct>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<CategoryProduct>.SuccessAsync(category);
            }
        }
    }
}
