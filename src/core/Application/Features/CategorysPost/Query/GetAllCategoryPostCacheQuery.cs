using Application.CacheKeys;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CategorysPost.Query
{
   
    public class GetAllCategoryPostCacheQuery : IRequest<Result<List<CategoryPost>>>
    {
        public GetAllCategoryPostCacheQuery()
        {
        }
    }

    public class GetAllCategoryCachedQueryHandler : IRequestHandler<GetAllCategoryPostCacheQuery, Result<List<CategoryPost>>>
    {
       
        private readonly IRepositoryCacheAsync<CategoryPost> _CategoryCache;
        private readonly IMapper _mapper;

        public GetAllCategoryCachedQueryHandler(IRepositoryCacheAsync<CategoryPost> CategoryCache, IMapper mapper)
        {
            _CategoryCache = CategoryCache;
            _mapper = mapper;
        }

        public async Task<Result<List<CategoryPost>>> Handle(GetAllCategoryPostCacheQuery request, CancellationToken cancellationToken)
        {
            var PostList = await _CategoryCache.GetCachedListAsync(CategoryCacheKeys.ListPostKey);
            return Result<List<CategoryPost>>.Success(PostList);
        }
    }
}
