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

namespace Application.Features.TypeCategorys.Commands
{

    public class DeleteTypeCategoryCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeleteTypeCategoryHandler : IRequestHandler<DeleteTypeCategoryCommand, Result<int>>
        {
            private readonly IRepositoryAsync<TypeCategory> _Repository;
            private readonly ICategoryPostRepository<CategoryPost> _TypeCategory;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteTypeCategoryHandler(ICategoryPostRepository<CategoryPost> TypeCategory, IRepositoryAsync<TypeCategory> brandRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _TypeCategory = TypeCategory;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteTypeCategoryCommand command, CancellationToken cancellationToken)
            {
                var product = await _Repository.GetByIdAsync(command.Id);
                if (product != null)
                {
                    var check = _TypeCategory.CountbyIdTypeCategory(product.Id);
                    if (check > 0)
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR016);
                    }
                    await _Repository.DeleteAsync(product);
                    await _distributedCache.RemoveAsync(TypeCategoryCacheKeys.ListKey);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success(product.Id);
                }
                return Result<int>.Fail(HeperConstantss.ERR012);
            }
        }
    }
}