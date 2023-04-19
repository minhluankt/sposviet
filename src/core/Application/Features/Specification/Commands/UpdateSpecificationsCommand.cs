using Application.CacheKeys;
using Application.Constants;
using Application.Enums;
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

namespace Application.Features.Specificationss.Commands
{

    public partial class UpdateSpecificationsCommand : Specifications, IRequest<Result<int>>
    {

    }
    public class UpdateSpecificationsHandler : IRequestHandler<UpdateSpecificationsCommand, Result<int>>
    {
        private readonly IRepositoryAsync<TypeSpecifications> _RepositoryCategory;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<Specifications> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateSpecificationsHandler(IRepositoryAsync<Specifications> brandRepository, IRepositoryAsync<TypeSpecifications> RepositoryCategory,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _RepositoryCategory = RepositoryCategory;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateSpecificationsCommand command, CancellationToken cancellationToken)
        {



            var brand = await _Repository.GetByIdAsync(command.Id);
            if (brand == null)
            {
                return await Result<int>.FailAsync(HeperConstantss.ERR012);
            }
            else
            {
               // var type = await _RepositoryCategory.GetByIdAsync(command.idTypeSpecifications);
               
                    brand.Slug = Common.ConvertToSlugCore(command.Name);
                

                brand.Name = command.Name;
                brand.From = command.From;
                brand.To = command.To;
                brand.Sort = command.Sort;
                brand.Code = Common.ConvertToSlug(command.Name);
                var checkcode = _Repository.Entities.Count(predicate: m => m.Code == brand.Code && m.idTypeSpecifications == command.idTypeSpecifications && m.Id != brand.Id);
                if (checkcode > 0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR014);
                }
                await _Repository.UpdateAsync(brand);
                await _distributedCache.RemoveAsync(SpecificationsCacheKeys.ListKey);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(brand.Id);
            }
        }
    }
}
