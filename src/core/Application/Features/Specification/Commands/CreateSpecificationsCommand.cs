using Application.CacheKeys;
using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
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

namespace Application.Features.Specification.Commands
{
    public partial class CreateSpecificationsCommand : Specifications, IRequest<Result<int>>
    {

    }
    public class CreateSpecificationsHandler : IRequestHandler<CreateSpecificationsCommand, Result<int>>
    {
        private readonly IRepositoryAsync<TypeSpecifications> _RepositoryCategory;
        private readonly IRepositoryAsync<Specifications> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateSpecificationsHandler(IRepositoryAsync<Specifications> brandRepository,
             IRepositoryAsync<TypeSpecifications> RepositoryCategory,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _RepositoryCategory = RepositoryCategory;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateSpecificationsCommand request, CancellationToken cancellationToken)
        {
            request.Code = Common.ConvertToSlug(request.Name);

            request.Slug = Common.ConvertToSlug(request.Name);
            


            var fidn = _Repository.Entities.Where(m => m.Code == request.Code && m.idTypeSpecifications == request.idTypeSpecifications).SingleOrDefault();
            if (fidn != null)
            {
                return await Result<int>.FailAsync("Tên đã tồn tại!");
            }
            var product = _mapper.Map<Specifications>(request);
            await _Repository.AddAsync(product);
            await _distributedCache.RemoveAsync(SpecificationsCacheKeys.ListKey);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(product.Id);
        }
    }
}
