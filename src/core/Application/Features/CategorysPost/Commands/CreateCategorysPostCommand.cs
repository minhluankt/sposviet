using Application.CacheKeys;
using Application.Constants;
using Application.Hepers;
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
    public partial class CreateCategorysPostCommand : CategoryPost, IRequest<Result<int>>
    {
    }
    public class CreateCategoryHandler : IRequestHandler<CreateCategorysPostCommand, Result<int>>
    {
        private readonly ITableLinkRepository _tablelink;
        private readonly IRepositoryAsync<CategoryPost> _Repository;
     
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateCategoryHandler(IRepositoryAsync<CategoryPost> brandRepository,
            ITableLinkRepository tablelink, 
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
           
            _tablelink = tablelink;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateCategorysPostCommand request, CancellationToken cancellationToken)
        {
            request.Code = Common.ConvertToSlug(request.Name);
            request.Slug = Common.ConvertToSlug(request.Name);
           
            var fidn = _Repository.Entities.Where(m => m.Code == request.Code && m.IdPattern == request.IdPattern).SingleOrDefault();
            if (fidn != null)
            {
                return await Result<int>.FailAsync("Tên đã tồn tại!");
            }
            var Post = _mapper.Map<CategoryPost>(request);
            await _Repository.AddAsync(Post);
            //if (request.IdPattern > 0)
            //{
            //    string code = _Repository.Entities.Where(m => m.Id == request.IdPattern).SingleOrDefault().Code;
            //    await _distributedCache.RemoveAsync(CategoryCacheKeys.GetKey(code));
            //}

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _distributedCache.RemoveAsync(CategoryCacheKeys.ListPostKey);
            await _tablelink.AddAsync(request.Code, TypeLinkConstants.IdTypeCategoryPost, TypeLinkConstants.TypeCategoryPost, Post.Id, 0);
            return Result<int>.Success(Post.Id);
        }
    }
}
