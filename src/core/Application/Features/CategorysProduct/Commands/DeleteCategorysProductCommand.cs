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

namespace Application.Features.CategorysProduct.Commands
{
   
    public class DeleteCategorysProductCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeleteCategoryHandler : IRequestHandler<DeleteCategorysProductCommand, Result<int>>
        {
            private readonly ITableLinkRepository _tablelink;
            private readonly IRepositoryAsync<CategoryProduct> _Repository;
            private readonly ICategoryProductRepository<CategoryProduct> _category;
            private readonly IProductPepository<Product> _product;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteCategoryHandler(ICategoryProductRepository<CategoryProduct> category,
                IProductPepository<Product> product, ITableLinkRepository tablelink,
                IRepositoryAsync<CategoryProduct> brandRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _product = product; _tablelink = tablelink;
                _category = category;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteCategorysProductCommand command, CancellationToken cancellationToken)
            {
                // return Result<int>.Fail();
                try
                {
                    var product = await _Repository.GetByIdAsync(command.Id);
                    if (product != null)
                    {

                        if (product.IdLevel == 0)
                        {
                            bool check = _product.CheckProductbyCategoryId(product.Id);
                            if (check)
                            {
                                return Result<int>.Fail(HeperConstantss.ERR014);
                            }
                            string code = product.Code;
                            //await _distributedCache.RemoveAsync(CategoryCacheKeys.GetKey(code));
                        }
                        else if (product.IdLevel == 1)
                        {
                            var ltspattern = _Repository.Entities.Where(m => m.IdPattern == product.Id).Select(m => m.Id).ToArray();
                            bool check = _product.CheckProductbyListCategoryId(ltspattern);
                            if (check)
                            {
                                return Result<int>.Fail(HeperConstantss.ERR014);
                            }
                            string code = _Repository.Entities.Where(m => m.Id == product.IdPattern).SingleOrDefault().Code;
                            //await _distributedCache.RemoveAsync(CategoryCacheKeys.GetKey(code));
                        }

                        await _Repository.DeleteAsync(product);
                        await _tablelink.DeleteAsync(TypeLinkConstants.IdTypeCategoryProduct, product.Id);
                        if (product.IdLevel == 0)
                        {
                            await _category.DeleteByIdPattern(product.Id);
                        }
                       

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        await _distributedCache.RemoveAsync(CategoryCacheKeys.ListProductKey);
                        return Result<int>.Success(product.Id);
                    }
                    return Result<int>.Fail(HeperConstantss.ERR012);
                }
                catch (Exception e)
                {

                    return Result<int>.Fail(e.Message);
                }
            }
        }
    }
}
