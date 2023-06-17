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

namespace Application.Features.Districts.Commands
{

    public partial class UpdateDistrictCommand : District, IRequest<Result<int>>
    {

    }
    public class UpdateDistrictHandler : IRequestHandler<UpdateDistrictCommand, Result<int>>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<District> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateDistrictHandler(IRepositoryAsync<District> DistrictRepository,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _Repository = DistrictRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateDistrictCommand command, CancellationToken cancellationToken)
        {
            var District = await _Repository.GetByIdAsync(command.Id);
            if (District == null)
            {
                return await Result<int>.FailAsync(HeperConstantss.ERR012);
            }
            else
            {
                District.Name = command.Name;
                District.Code = Common.ConvertToSlug(command.Name);
                // District.Slug = command.Code;
                var checkcode = _Repository.Entities.Count(predicate: m => m.Code == District.Code && m.Id != District.Id);
                if (checkcode > 0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR014);
                }
                await _Repository.UpdateAsync(District);
                await _distributedCache.RemoveAsync(DistrictCacheKeys.ListKey);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(District.Id);
            }
        }
    }
}
