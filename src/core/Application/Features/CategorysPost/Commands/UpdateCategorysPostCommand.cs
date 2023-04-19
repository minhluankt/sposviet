using Application.CacheKeys;
using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.CategorysPost.Commands
{

    public partial class UpdateCategorysPostCommand : CategoryPost, IRequest<Result<int>>
    {

    }
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategorysPostCommand, Result<int>>
    {
        private readonly ITableLinkRepository _tablelink;
        private readonly ILogger<UpdateCategorysPostCommand> _log;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<CategoryPost> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateCategoryHandler(IRepositoryAsync<CategoryPost> brandRepository, ILogger<UpdateCategorysPostCommand> log,
              ITableLinkRepository tablelink,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _tablelink = tablelink;
            _log = log;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateCategorysPostCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var brand = await _Repository.GetByIdAsync(command.Id);
                if (brand == null)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR012);
                }
                else
                {

                    brand.Name = command.Name;
                    brand.Code = Common.ConvertToSlug(command.Name);
                    brand.Slug = Common.ConvertToSlug(command.Name);
                    var checkcode = _Repository.Entities.Count(predicate: m => m.Code == brand.Code && m.Id != brand.Id);
                    if (checkcode > 0)
                    {
                        _log.LogError($"{HeperConstantss.ERR014} {brand.Name} {brand.Id}");
                        return await Result<int>.FailAsync(HeperConstantss.ERR014);
                    }

                    if (brand.IdLevel == 0 && (command.IdPattern != brand.IdPattern))
                    {
                        var collection = _Repository.Entities.Where(m => m.IdPattern == brand.Id);
                        if (collection.Count() > 0)
                        {
                            foreach (var item in collection)
                            {
                                item.IdPattern = command.IdPattern;
                            }
                            await _Repository.UpdateRangeAsync(collection);
                        }
                        //await _distributedCache.RemoveAsync(CategoryCacheKeys.GetKey(brand.Code));
                    }

                    brand.Sort = command.Sort;
                    brand.Url = command.Url;
                    brand.IdPattern = command.IdPattern;

                    await _Repository.UpdateAsync(brand);
                    //await _tablelink.UpdateAsync(brand.Code, TypeLinkConstants.IdTypeCategoryPost, brand.Id);

                    await _distributedCache.RemoveAsync(CategoryCacheKeys.ListPostKey);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success(brand.Id);
                }
            }
            catch (Exception e)
            {
                _log.LogError(e, e.Message);
                return await Result<int>.FailAsync(e.Message);
            }
        }
    }
}
