using Application.CacheKeys;
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

namespace Application.Features.TypeSpecification.Commands
{
    public partial class CreateTypeSpecificationsCommand : TypeSpecifications, IRequest<Result<int>>
    {

    }
    public class CreateTypeSpecificationsHandler : IRequestHandler<CreateTypeSpecificationsCommand, Result<int>>
    {
        private readonly IRepositoryAsync<TypeSpecifications> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateTypeSpecificationsHandler(IRepositoryAsync<TypeSpecifications> brandRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateTypeSpecificationsCommand request, CancellationToken cancellationToken)
        {
            request.Name = request.Name.Trim();
            request.Code = !string.IsNullOrEmpty(request.Code) ? request.Code.ToUpper().Trim().Replace(" ", "") : Common.ConvertToSlugNoSpage(request.Name).ToUpper();
            var fidn = _Repository.Entities.Where(m => m.Code == request.Code).SingleOrDefault();
            if (fidn != null)
            {
                return await Result<int>.FailAsync("Tên đã tồn tại!");
            }
            var product = _mapper.Map<TypeSpecifications>(request);
            await _Repository.AddAsync(product);
            await _distributedCache.RemoveAsync(TypeSpecificationsCacheKeys.ListKey);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(product.Id);
        }
    }
}
