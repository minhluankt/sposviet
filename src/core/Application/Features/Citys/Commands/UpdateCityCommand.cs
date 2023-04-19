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

namespace Application.Features.Citys.Commands
{
    public partial class UpdateCityCommand : City, IRequest<Result<int>>
    {

    }
    public class UpdateCityHandler : IRequestHandler<UpdateCityCommand, Result<int>>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<City> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateCityHandler(IRepositoryAsync<City> brandRepository,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateCityCommand command, CancellationToken cancellationToken)
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
              //  brand.Slug = brand.Code;
                var checkcode = _Repository.Entities.Count(predicate: m => m.Code == brand.Code && m.Id != brand.Id);
                if (checkcode > 0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR014);
                }
                await _Repository.UpdateAsync(brand);
                await _distributedCache.RemoveAsync(CityCacheKeys.ListKey);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(brand.Id);
            }
        }
    }
}
