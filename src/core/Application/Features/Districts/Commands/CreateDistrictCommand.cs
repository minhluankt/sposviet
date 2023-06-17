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

namespace Application.Features.Districts.Commands
{
    public partial class CreateDistrictCommand : District, IRequest<Result<int>>
    {

    }
    public class CreateDistrictHandler : IRequestHandler<CreateDistrictCommand, Result<int>>
    {
        private readonly IRepositoryAsync<District> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateDistrictHandler(IRepositoryAsync<District> DistrictRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = DistrictRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateDistrictCommand request, CancellationToken cancellationToken)
        {
            request.Code = Common.ConvertToSlug(request.Name);
            //  request.Slug = request.Code;
            var fidn = _Repository.Entities.Where(m => m.Code == request.Code).SingleOrDefault();
            if (fidn != null)
            {
                return await Result<int>.FailAsync("Tên đã tồn tại!");
            }
            var product = _mapper.Map<District>(request);
            await _Repository.AddAsync(product);
            await _distributedCache.RemoveAsync(DistrictCacheKeys.ListKey);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(product.Id);
        }
    }
}
