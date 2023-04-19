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

namespace Application.Features.CategorysPost.Query
{

    public class GetByIdCategoryPostQuery : IRequest<Result<CategoryPost>>
    {
        public int Id { get; set; }

        public class GetCategoryByIdQueryHandler : IRequestHandler<GetByIdCategoryPostQuery, Result<CategoryPost>>
        {
            private readonly IRepositoryAsync<CategoryPost> _repository;
            public GetCategoryByIdQueryHandler(IRepositoryAsync<CategoryPost> repository)
            {
                _repository = repository;
            }
            public async Task<Result<CategoryPost>> Handle(GetByIdCategoryPostQuery query, CancellationToken cancellationToken)
            {
                var Post = await _repository.GetByIdAsync(m=>m.Id== query.Id,include: m=>m.Include(m => m.CategoryChilds));
                if (Post==null)
                {
                    return await Result<CategoryPost>.FailAsync(HeperConstantss.ERR012);
                }
                return await Result<CategoryPost>.SuccessAsync(Post);
            }
        }
    }
}
