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

namespace Application.Features.TypeSpecification.Commands
{
    public partial class UpdateTypeSpecificationsCommand : TypeSpecifications, IRequest<Result<int>>
    {

    }
    public class UpdateTypeSpecificationsHandler : IRequestHandler<UpdateTypeSpecificationsCommand, Result<int>>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<TypeSpecifications> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateTypeSpecificationsHandler(IRepositoryAsync<TypeSpecifications> brandRepository,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateTypeSpecificationsCommand command, CancellationToken cancellationToken)
        {
            var brand = await _Repository.GetByIdAsync(command.Id);
            if (brand == null)
            {
                return await Result<int>.FailAsync(HeperConstantss.ERR012);
            }
            else
            {
                brand.Name = command.Name;
                brand.Code = !string.IsNullOrEmpty(command.Code) ? command.Code.ToUpper().Trim().Replace(" ", "") : Common.ConvertToSlugNoSpage(command.Name).ToUpper();
                if (CommonConstants.listTypeSpecification.Contains(brand.Code))
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR014);
                }
                var checkcode = _Repository.Entities.Count(predicate: m => m.Code == brand.Code && m.Id != brand.Id);
                if (checkcode > 0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR014);
                }
                await _Repository.UpdateAsync(brand);
                await _distributedCache.RemoveAsync(TypeSpecificationsCacheKeys.ListKey);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(brand.Id);
            }
        }
    }
}
