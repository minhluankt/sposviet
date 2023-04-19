
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

    public class GetAllCategoryIncludePostQuery : IRequest<Result<IEnumerable<CategoryPost>>>
    {
        public int[] lstIdCategory { get; set; }
        public int  taskPost { get; set; }
        public GetAllCategoryIncludePostQuery()
        {
           
        }
    }

    public class GetAllCategoryIncludePostdQueryHandler : IRequestHandler<GetAllCategoryIncludePostQuery, Result<IEnumerable<CategoryPost>>>
    {

        private readonly ICategoryPostRepository<CategoryPost> _CategoryIncludePost;
        private readonly IMapper _mapper;

        public GetAllCategoryIncludePostdQueryHandler(ICategoryPostRepository<CategoryPost> CategoryIncludePost, IMapper mapper)
        {
            _CategoryIncludePost = CategoryIncludePost;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<CategoryPost>>> Handle(GetAllCategoryIncludePostQuery request, CancellationToken cancellationToken)
        {
            var PostList =  _CategoryIncludePost.GetListIncludePost(request.lstIdCategory, request.taskPost);
            return await Result<IEnumerable<CategoryPost>>.SuccessAsync(PostList);
        }
    }
}
