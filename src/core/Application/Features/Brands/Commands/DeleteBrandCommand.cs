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


namespace Application.Features.Brands.Commands
{

    public class DeleteBrandCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeleteBrandHandler : IRequestHandler<DeleteBrandCommand, Result<int>>
        {
            private readonly IRepositoryAsync<Brand> _Repository;
            private readonly IRepositoryAsync<Product> _Product;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteBrandHandler(IRepositoryAsync<Product> Product,
                IRepositoryAsync<Brand> brandRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _Product = Product;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteBrandCommand command, CancellationToken cancellationToken)
            {
                var brand = await _Repository.GetByIdAsync(command.Id);
                if (brand != null)
                {
                    var check = _Product.GetAll(m => m.IdBrand == brand.Id).Count();
                    if (check > 0)
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR016);
                    }
                    await _Repository.DeleteAsync(brand);
                    await _distributedCache.RemoveAsync(BrandCacheKeys.ListKey);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success();
                }
                return Result<int>.Fail(HeperConstantss.ERR012);
            }
        }
    }
}

