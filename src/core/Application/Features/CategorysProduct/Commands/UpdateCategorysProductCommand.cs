using Application.CacheKeys;
using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.CategorysProduct.Commands
{

    public partial class UpdateCategorysProductCommand : CategoryProduct, IRequest<Result<int>>
    {

    }
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategorysProductCommand, Result<int>>
    {
        private readonly ITableLinkRepository _tablelink;
        private readonly ILogger<UpdateCategorysProductCommand> _log;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<CategoryProduct> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateCategoryHandler(IRepositoryAsync<CategoryProduct> brandRepository, ILogger<UpdateCategorysProductCommand> log,
              ITableLinkRepository tablelink,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _tablelink = tablelink;
            _log = log;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateCategorysProductCommand command, CancellationToken cancellationToken)
        {
            await _unitOfWork.CreateTransactionAsync();
            try
            {

                var brand = await _Repository.GetByIdAsync(command.Id);
                if (brand == null)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR012);
                }
                else
                {
                    if (brand.IdPattern != command.IdPattern && command.IdPattern > 0)
                    {
                        var getlevel = _Repository.Entities.Where(m => m.Id == command.IdPattern).SingleOrDefault();
                        if (getlevel != null)
                        {
                            brand.IdLevel = getlevel.IdLevel + 1;
                        }
                        if (brand.IdLevel > 2)
                        {
                            return await Result<int>.FailAsync(HeperConstantss.ERR008);
                        }
                    }
                    else if (command.IdPattern ==null)
                    {
                        brand.IdLevel = 0;
                    }
                    if (command.Name != brand.Name)
                    {
                        brand.Name = command.Name;
                        brand.Slug = Common.ConvertToSlug(command.Name);
                        brand.Code = Common.ConvertToSlug(command.Name);
                    }

                    var checkcode = _Repository.Entities.Count(predicate: m => m.Code == brand.Code && m.ComId == brand.ComId && m.Id != brand.Id);
                    if (checkcode > 0)
                    {
                        _log.LogError($"{HeperConstantss.ERR014} {brand.Name} {brand.Id}");
                        return await Result<int>.FailAsync(HeperConstantss.ERR014);
                    }

                    //if (brand.IdLevel == 0 && (command.IdPattern != brand.IdPattern))
                    //{
                    //    var collection = _Repository.Entities.Where(m => m.IdPattern == brand.Id);
                    //    if (collection.Count() > 0)
                    //    {
                    //        foreach (var item in collection)
                    //        {
                    //            item.IdPattern = command.IdPattern;
                    //        }
                    //        await _Repository.UpdateRangeAsync(collection);
                    //    }
                    //    await _distributedCache.RemoveAsync(CategoryCacheKeys.GetKey(brand.Code));
                    //}

                    brand.Sort = command.Sort;
                    brand.Icon = command.Icon;
                    brand.IdPattern = command.IdPattern;


                    await _Repository.UpdateAsync(brand);


                    await _distributedCache.RemoveAsync(CategoryCacheKeys.ListProductKey);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    var checktablelink = await _tablelink.GetBySlug(brand.Slug);

                    if (checktablelink != null)
                    {
                        if (checktablelink.parentId != brand.Id)
                        {
                            var getpattern = _Repository.Entities.Where(m => m.Id == brand.IdPattern).SingleOrDefault();
                            string slugnew = Common.ConvertToSlug($"{getpattern.Slug}-{brand.Slug}");
                            brand.Code = slugnew;
                            var updateslug = await _Repository.GetByIdAsync(brand.Id);
                            updateslug.Code = slugnew;
                            updateslug.Slug = slugnew;
                            await _Repository.UpdateAsync(updateslug);
                            await _unitOfWork.SaveChangesAsync(cancellationToken);

                        }
                    }
                    await _tablelink.UpdateAsync(brand.Code, TypeLinkConstants.IdTypeCategoryProduct, brand.Id, brand.ComId);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    return Result<int>.Success(brand.Id);
                }
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                _log.LogError(e, e.Message);
                return await Result<int>.FailAsync(e.Message);
            }
        }
    }
}
