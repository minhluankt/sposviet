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

namespace Application.Features.CategorysProduct.Commands
{
    public partial class CreateCategorysProductCommand : CategoryProduct, IRequest<Result<int>>
    {
    }
    public class CreateCategoryHandler : IRequestHandler<CreateCategorysProductCommand, Result<int>>
    {
        private readonly ITableLinkRepository _tablelink;
        private readonly IRepositoryAsync<CategoryProduct> _Repository;
     
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateCategoryHandler(IRepositoryAsync<CategoryProduct> brandRepository,
            ITableLinkRepository tablelink, 
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
           
            _tablelink = tablelink;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateCategorysProductCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.CreateTransactionAsync();
            try
            {
                if (request.IsPos)
                {
                    request.Code = Common.ConvertToSlug($"{request.Name}-{request.ComId}");
                    request.Slug = Common.ConvertToSlug($"{request.Name}-{request.ComId}");
                }
                else
                {
                    request.Code = Common.ConvertToSlug(request.Name);
                    request.Slug = Common.ConvertToSlug(request.Name);
                }
               

                var fidn = _Repository.Entities.Where(m => m.Code == request.Code &&m.ComId == request.ComId && m.IdPattern == request.IdPattern).SingleOrDefault();
                if (fidn != null)
                {
                    return await Result<int>.FailAsync("Tên đã tồn tại!");
                }
                var product = _mapper.Map<CategoryProduct>(request);
                await _Repository.AddAsync(product);
                //if (request.IdPattern > 0)
                //{
                //    string code = _Repository.Entities.Where(m => m.Id == request.IdPattern).SingleOrDefault().Code;
                //    await _distributedCache.RemoveAsync(CategoryCacheKeys.GetKey(code));
                //}

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _distributedCache.RemoveAsync(CategoryCacheKeys.ListProductKey);
                var checktablelink = await _tablelink.GetBySlug(request.Code);
                if (checktablelink != null)
                {
                    if (checktablelink.parentId != product.Id)
                    {
                        var getpattern = _Repository.Entities.Where(m => m.Id == request.IdPattern).SingleOrDefault();
                        string slugnew = Common.ConvertToSlug($"{getpattern.Slug}-{request.Slug}");
                        request.Code = slugnew;
                        var updateslug = await _Repository.GetByIdAsync(product.Id);
                        updateslug.Code = slugnew;
                        updateslug.Slug = slugnew;
                        await _Repository.UpdateAsync(updateslug);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                    }
                }
                await _tablelink.AddAsync(request.Code, TypeLinkConstants.IdTypeCategoryProduct, TypeLinkConstants.TypeCategoryProduct, product.Id, product.ComId);
                await _unitOfWork.CommitAsync();
                

                return Result<int>.Success(product.Id);
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(e.Message,e);
            }
           
        }
    }
}
