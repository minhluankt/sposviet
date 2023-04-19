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

namespace Application.Features.Posts.Querys
{
    public class  GetListSearchPostCachedQuery : IRequest<Result<List<Post>>>
    {
        public int task { get; set; } = 20;
        public int? idCategory { get; set; }
        public bool isReview { get; set; }
    }
    public class  GetListSearchPostQueryHandler : IRequestHandler< GetListSearchPostCachedQuery, Result<List<Post>>>
    {

        private readonly IRepositoryCacheAsync<Post> _PostCache;
        private readonly IRepositoryAsync<Post> _Post;
        private readonly IMapper _mapper;

        public  GetListSearchPostQueryHandler(IRepositoryCacheAsync<Post> PostCache, IMapper mapper, IRepositoryAsync<Post> Post)
        {
            _Post = Post;
            _PostCache = PostCache;
            _mapper = mapper;
        }

        public async Task<Result<List<Post>>> Handle( GetListSearchPostCachedQuery request, CancellationToken cancellationToken)
        {
            var data = await _PostCache.GetCachedListAsync(PostCacheKeys.ListKey);
            data = data.Where(m => m.Active).ToList();

            if (request.idCategory != null)
            {
                data = data.Where(m => m.IdCategory == request.idCategory).ToList();
            }


            if (request.isReview)
            {
                data = data.OrderByDescending(m => m.ViewNumber).Take(request.task).ToList();
            }
            else
            {
                data = data.OrderByDescending(m => m.Id).Take(request.task).ToList();
            }


            return await Result<List<Post>>.SuccessAsync(data);
        }
    }
}
