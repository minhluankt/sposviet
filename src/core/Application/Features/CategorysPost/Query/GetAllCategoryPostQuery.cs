using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CategorysPost.Query
{
    public class GetAllCategoryPostQuery : IRequest<Result<List<CategoryPost>>>
    {
        public bool IncludebyCategory { get; set; }
        public bool GetPattern { get; set; }
        public GetAllCategoryPostQuery()
        {
        }
    }
    public class GetAllCategoryQueryHandler : IRequestHandler<GetAllCategoryPostQuery, Result<List<CategoryPost>>>
    {
        private readonly IRepositoryAsync<CategoryPost> _repository;
        private readonly IRepositoryAsync<TypeCategory> _repositoryTypeCategory;
        // private readonly IRepository _repositordy;
        private readonly IMapper _mapper;

        public GetAllCategoryQueryHandler(IMapper mapper, IRepositoryAsync<CategoryPost> repository, IRepositoryAsync<TypeCategory> repositoryTypeCategory)
        {
            _repositoryTypeCategory = repositoryTypeCategory;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<CategoryPost>>> Handle(GetAllCategoryPostQuery request, CancellationToken cancellationToken)
        {
            var list = new List<CategoryPost>();
            var PostList = _repository.GetAllQueryable().Where(m => m.Active).AsNoTracking();
            if (request.GetPattern)
            {
                PostList = PostList.Where(m => m.IdLevel == 0);
            }

            if (request.IncludebyCategory)
            {
                list = await PostList.Where(m => m.Active).Include(m => m.Posts).Include(m => m.CategoryChilds).ThenInclude(m => m.Posts).OrderByDescending(m => m.Sort).ToListAsync();
            }
            else
            {
                list = await PostList.Where(m => m.Active).Include(m => m.CategoryChilds).OrderByDescending(m => m.Sort).ToListAsync();
            }

            return await Result<List<CategoryPost>>.SuccessAsync(list);
        }
    }
}
