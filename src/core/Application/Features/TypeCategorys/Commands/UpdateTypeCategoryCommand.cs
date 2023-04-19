﻿using Application.CacheKeys;
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

namespace Application.Features.TypeCategorys.Commands
{

    public partial class UpdateTypeCategoryCommand : TypeCategory, IRequest<Result<int>>
    {

    }
    public class UpdateTypeCategoryHandler : IRequestHandler<UpdateTypeCategoryCommand, Result<int>>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<TypeCategory> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateTypeCategoryHandler(IRepositoryAsync<TypeCategory> brandRepository,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateTypeCategoryCommand command, CancellationToken cancellationToken)
        {
            var brand = await _Repository.GetByIdAsync(command.Id);
            if (brand == null)
            {
                return await Result<int>.FailAsync(HeperConstantss.ERR012);
            }
            else
            {
                brand.Name = command.Name;
                brand.Code = Common.ConvertToSlug(command.Name);
                var checkcode = _Repository.Entities.Count(predicate: m => m.Code == brand.Code && m.Id!=brand.Id);
                if (checkcode > 0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR014);
                }
                await _Repository.UpdateAsync(brand);
                await _distributedCache.RemoveAsync(TypeCategoryCacheKeys.ListKey);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(brand.Id);
            }
        }
    }
}
