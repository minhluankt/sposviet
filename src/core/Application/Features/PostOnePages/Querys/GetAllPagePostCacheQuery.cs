using Application.CacheKeys;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PostOnePages.Querys
{
    public class GetAllPagePostCacheQuery : IRequest<Result<List<PagePost>>>
    {
        public int task { get; set; } = 20;
        public int? idCategory { get; set; }
        public bool isReview { get; set; }
    }
    public class GetListSearchPagePostQueryHandler : IRequestHandler<GetAllPagePostCacheQuery, Result<List<PagePost>>>
    {

        private readonly IRepositoryCacheAsync<PagePost> _PagePostCache;
        private readonly IRepositoryAsync<PagePost> _PagePost;
        private readonly IMapper _mapper;

        public GetListSearchPagePostQueryHandler(IRepositoryCacheAsync<PagePost> PagePostCache, IMapper mapper, IRepositoryAsync<PagePost> PagePost)
        {
            _PagePost = PagePost;
            _PagePostCache = PagePostCache;
            _mapper = mapper;
        }

        public async Task<Result<List<PagePost>>> Handle(GetAllPagePostCacheQuery request, CancellationToken cancellationToken)
        {
            var data = await _PagePostCache.GetCachedListAsync(PagePostCacheKeys.ListKey);
            //data = data.Where(m => m.Active).ToList();


            if (request.isReview)
            {
                data = data.OrderByDescending(m => m.ViewNumber).Take(request.task).ToList();
            }
            else
            {
                data = data.OrderByDescending(m => m.Id).Take(request.task).ToList();
            }


            return await Result<List<PagePost>>.SuccessAsync(data);
        }
    }
}
