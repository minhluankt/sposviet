using Application.CacheKeys;
using Application.Interfaces.CacheRepositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CategorysPost.Query
{

    public class GetListChildCategoryPostByIdCacheQuery : IRequest<Result<List<CategoryPost>>>
    {
        public int IdCategory { get; set; }
    }

    public class GetListChildCategoryPostByIdQueryHandler : IRequestHandler<GetListChildCategoryPostByIdCacheQuery, Result<List<CategoryPost>>>
    {

        private readonly IRepositoryCacheAsync<CategoryPost> _CategoryCache;
        private readonly IMapper _mapper;

        public GetListChildCategoryPostByIdQueryHandler(IRepositoryCacheAsync<CategoryPost> CategoryCache, IMapper mapper)
        {
            _CategoryCache = CategoryCache;
            _mapper = mapper;
        }

        public async Task<Result<List<CategoryPost>>> Handle(GetListChildCategoryPostByIdCacheQuery request, CancellationToken cancellationToken)
        {
            List<CategoryPost> lst = new List<CategoryPost>();
            var PostList = await _CategoryCache.GetCachedListAsync(CategoryCacheKeys.ListPostKey);
            var getCate = PostList.Where(x => x.Id == request.IdCategory).SingleOrDefault();
            if (getCate != null)
            {
                List<CategoryPost> lstitem = new List<CategoryPost>();
                int iditem = 0;
                lst.Add(getCate);
                PostList.Remove(getCate);
                iditem = getCate.Id;
                var getchild = PostList.Where(x => x.IdPattern == iditem).OrderBy(x => x.Sort).ToList();
                lstitem.AddRange(getchild);
                while (lstitem.Count() > 0)
                {
                    getCate = lstitem.LastOrDefault();
                    if (getCate != null)
                    {
                        lst.Add(getCate);
                        lstitem.Remove(getCate);
                        iditem = getCate.Id;
                        getchild = PostList.Where(x => x.IdPattern == iditem).OrderBy(x => x.Sort).ToList();
                        if (getchild.Count() > 0)
                        {
                            lstitem.AddRange(getchild);
                        }
                    }

                }
            }

            return Result<List<CategoryPost>>.Success(lst);
        }
    }
}
