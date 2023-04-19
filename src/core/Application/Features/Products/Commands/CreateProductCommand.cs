using Application.CacheKeys;
using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Products.Commands
{
    public partial class CreateProductCommand : Product, IRequest<Result<int>>
    {
        public IFormFile ImgUpload { get; set; }
        public IList<IFormFile> Document { get; set; }
        public IList<IFormFile> albumImgUpload { get; set; }
        public int?[] idattachment { get; set; }
        public int?[] idAccessary { get; set; }
    }
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<int>>
    {
        private readonly IRepositoryAsync<Product> _Repository;
        private readonly IProductPepository<Product> _ProductPepository;
      
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateProductHandler(IRepositoryAsync<Product> brandRepository,
            
            IProductPepository<Product> ProductPepository,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            
            _ProductPepository = ProductPepository;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                
               
                
                if (!string.IsNullOrEmpty(request.Code))
                {
                    var checkCode = await _ProductPepository.GetByCodeAsync(request.ComId, request.Code, true);
                    if (checkCode != null)
                    {
                        return Result<int>.Fail("Mã sản phẩm đã tồn tại");
                    }
                }
                var product = _mapper.Map<Product>(request);
                if (!product.isPromotion)
                {
                    product.Discount = 0;
                    product.PriceDiscount = 0;
                    product.ExpirationDateDiscount = null;
                }
               
                var add = await _ProductPepository.AddAsync(product, request.Document, request.albumImgUpload, request.ImgUpload, request.TypeProduct);
                if (add.Succeeded)
                {
                    product = add.Data;
                }
                else
                {
                    return await Result<int>.FailAsync(add.Message);
                }
               // await _distributedCache.RemoveAsync(ProductCacheKeys.ListKey);
                //await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(product.Id);
            }
            catch (Exception e)
            {
                return await Result<int>.FailAsync(e.Message);
            }

        }
    }
}
