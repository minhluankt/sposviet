using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Posts.Querys
{

    public class GetByIdPostQuery : IRequest<Result<PostModel>>
    {
        public bool IsView { get; set; }
        public int Id { get; set; }

        public class GetPostByIdQueryHandler : IRequestHandler<GetByIdPostQuery, Result<PostModel>>
        {
            private readonly IMapper _mapper;
            private readonly ICategoryPostRepository<CategoryPost> _categoryPostRepository;
            private readonly IRepositoryAsync<Post> _repository;
            private readonly IPostRepository<Post> _postrepository;
            public GetPostByIdQueryHandler(IRepositoryAsync<Post> repository, IMapper mapper,
                        ICategoryPostRepository<CategoryPost> categoryPostRepository,
                IPostRepository<Post> postrepository)
            {
                _mapper = mapper;
                _categoryPostRepository = categoryPostRepository;
                _postrepository = postrepository;
                _repository = repository;
            }
            public async Task<Result<PostModel>> Handle(GetByIdPostQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(x => x.Id == query.Id, m => m.Include(x => x.CategoryPost));
                if (product == null)
                {
                    return await Result<PostModel>.FailAsync(HeperConstantss.ERR012);
                }
                var model = _mapper.Map<PostModel>(product);
                var getcategory = await _categoryPostRepository.GetByIdAsync(product.IdCategory);
                List<string> lst = new List<string>();
                if (getcategory != null)
                {
                    lst.Add(getcategory.Name);
                }
                int? idPat = getcategory != null ? getcategory.IdPattern : 0;
                while (idPat > 0)
                {
                    getcategory = await _categoryPostRepository.GetByIdAsync(idPat.Value);
                    if (getcategory != null)
                    {
                        lst.Add(getcategory.Name);
                        idPat = getcategory != null ? getcategory.IdPattern : 0;
                    }
                    else
                    {
                        idPat = null;
                    }

                };
                if (lst.Count() > 0)
                {
                    // string[] arrycate = model.category.Split('/');
                    string[] arrycate = lst.ToArray();
                    // arrycate = arrycate.SkipLast(1).ToArray();
                    Array.Reverse(arrycate);
                    string name = String.Join("/", arrycate);
                    model.category = name;
                }


                if (query.IsView)
                {
                    var task = Task.Run(() =>
                    {
                        _postrepository.UpdateReView(product.Id);
                    });
                }
                return await Result<PostModel>.SuccessAsync(model);
            }
        }
    }
}
