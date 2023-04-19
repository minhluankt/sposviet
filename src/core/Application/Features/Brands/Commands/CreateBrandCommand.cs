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

namespace Application.Features.Brands.Commands
{
    public partial class CreateBrandCommand : Brand,  IRequest<Result<int>>
    {

    }
    public class CreateBrandHandler : IRequestHandler<CreateBrandCommand, Result<int>>
    {
        private readonly IRepositoryAsync<Brand> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateBrandHandler(IRepositoryAsync<Brand> brandRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Code))
            {
                request.Code = Common.ConvertToSlug(request.Name);
            }
           
            var fidn = _Repository.Entities.Where(m => m.Code == request.Code).SingleOrDefault();
            if (fidn != null)
            {
                return await Result<int>.FailAsync("Tên đã tồn tại!");
            }
            var product = _mapper.Map<Brand>(request);
            await _Repository.AddAsync(product);
            await _distributedCache.RemoveAsync(BrandCacheKeys.ListKey);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(product.Id);
        }
    }
}
