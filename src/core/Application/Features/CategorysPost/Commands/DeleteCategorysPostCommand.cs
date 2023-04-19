using Application.CacheKeys;
using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Abstractions.Repository;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CategorysPost.Commands
{
   
    public class DeleteCategorysPostCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeleteCategoryHandler : IRequestHandler<DeleteCategorysPostCommand, Result<int>>
        {
            private readonly ITableLinkRepository _tablelink;
            private readonly IRepositoryAsync<CategoryPost> _Repository;
            private readonly ICategoryPostRepository<CategoryPost> _category;
            private readonly IPostRepository<Post> _Post;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteCategoryHandler(ICategoryPostRepository<CategoryPost> category,
                IPostRepository<Post> Post, ITableLinkRepository tablelink,
                IRepositoryAsync<CategoryPost> brandRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _Post = Post; _tablelink = tablelink;
                _category = category;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteCategorysPostCommand command, CancellationToken cancellationToken)
            {
                // return Result<int>.Fail();
                try
                {
                    var Post = await _Repository.GetByIdAsync(command.Id);
                    if (Post != null)
                    {

                        if (Post.IdLevel == 0)
                        {
                            bool check = _Post.CheckPostbyCategoryId(Post.Id);
                            if (check)
                            {
                                return Result<int>.Fail(HeperConstantss.ERR014);
                            }
                            string code = Post.Code;
                            //await _distributedCache.RemoveAsync(CategoryCacheKeys.GetKey(code));
                        }
                        else if (Post.IdLevel == 1)
                        {
                            var ltspattern = _Repository.Entities.Where(m => m.IdPattern == Post.Id).Select(m => m.Id).ToArray();
                            bool check = _Post.CheckPostbyListCategoryId(ltspattern);
                            if (check)
                            {
                                return Result<int>.Fail(HeperConstantss.ERR014);
                            }
                            string code = _Repository.Entities.Where(m => m.Id == Post.IdPattern).SingleOrDefault().Code;
                            //await _distributedCache.RemoveAsync(CategoryCacheKeys.GetKey(code));
                        }

                        await _Repository.DeleteAsync(Post);
                        await _tablelink.DeleteAsync(TypeLinkConstants.IdTypeCategoryPost, Post.Id);
                        if (Post.IdLevel == 0)
                        {
                            await _category.DeleteByIdPattern(Post.Id);
                        }
                       

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        await _distributedCache.RemoveAsync(CategoryCacheKeys.ListPostKey);
                        return Result<int>.Success(Post.Id);
                    }
                    return Result<int>.Fail(HeperConstantss.ERR012);
                }
                catch (Exception e)
                {

                    return Result<int>.Fail(e.Message);
                }
            }
        }
    }
}
