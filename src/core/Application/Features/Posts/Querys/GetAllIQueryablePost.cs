using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Posts.Querys
{

    public class GetAllIQueryablePostQuery : IRequest<Result<IQueryable<Post>>>
    {
        public string text { get; set; }
        public int? idCategory { get; set; }
        public int[] lstIdcategory { get; set; }
        public bool IncludeCategory { get; set; } = true;
    }
    public class GetAllIQueryablePostHandler : IRequestHandler<GetAllIQueryablePostQuery, Result<IQueryable<Post>>>
    {

        private readonly IRepositoryCacheAsync<Post> _PostCache;
        private readonly IRepositoryAsync<Post> _Post;
        private readonly IMapper _mapper;

        public GetAllIQueryablePostHandler(IRepositoryCacheAsync<Post> PostCache, IMapper mapper, IRepositoryAsync<Post> Post)
        {
            _Post = Post;
            _PostCache = PostCache;
            _mapper = mapper;
        }

        public async Task<Result<IQueryable<Post>>> Handle(GetAllIQueryablePostQuery request, CancellationToken cancellationToken)
        {
            var data = _Post.GetAllQueryable().Where(m => m.Active);
            if (request.lstIdcategory != null && request.lstIdcategory.Length > 0)
            {
                data = data.Where(m => request.lstIdcategory.Contains(m.IdCategory));
            }
            if (request.idCategory != null)
            {
                data = data.Where(m => m.IdCategory == request.idCategory);
            }
            if (!string.IsNullOrEmpty(request.text))
            {
                data = data.Where(m => m.Name.ToLower().Contains(request.text));
            }
            if (request.IncludeCategory)
            {
                data = data.Include(x => x.CategoryPost);
            }

            return await Result<IQueryable<Post>>.SuccessAsync(data);
        }
    }
}
