using Application.CacheKeys;
using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Abstractions.Repository;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.PostOnePages.Commands
{

    public partial class UpdatePostOnePageCommand : PagePost, IRequest<Result<int>>
    {
        public IFormFile ImgUpload { get; set; }
    }
    public class UpdatePostOnePageHandler : IRequestHandler<UpdatePostOnePageCommand, Result<int>>
    {
        private readonly ITableLinkRepository _tablelink;
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly ILogger<UpdatePostOnePageCommand> _log;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<PagePost> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdatePostOnePageHandler(IRepositoryAsync<PagePost> brandRepository, ILogger<UpdatePostOnePageCommand> log,
              ITableLinkRepository tablelink, IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _tablelink = tablelink;
            _log = log;
            _fileHelper = fileHelper;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdatePostOnePageCommand command, CancellationToken cancellationToken)
        {
            try
            {
                string imgold = string.Empty;
                var brand = await _Repository.GetByIdAsync(command.Id);
                if (brand == null)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR012);
                }
                else
                {   // upload hình ảnh
                    if (command.ImgUpload != null)
                    {
                        imgold = brand.Img;
                        brand.Img = _fileHelper.UploadedFile(command.ImgUpload, "", FolderUploadConstants.PagePost);
                    }

                    brand.Name = command.Name;
                    brand.Title = command.Title;
                    brand.Decription = command.Decription;
                    brand.seoDescription = command.seoDescription;
                    brand.seokeyword = command.seokeyword;
                    brand.seotitle = command.seotitle;
                    brand.Active = command.Active;
                    brand.Slug = Common.ConvertToSlug(command.Name);
                    var checkcode = _Repository.Entities.Count(predicate: m => m.Slug == brand.Slug && m.Id != brand.Id);
                    if (checkcode > 0)
                    {
                        _log.LogError($"{HeperConstantss.ERR014} {brand.Name} {brand.Id}");
                        return await Result<int>.FailAsync(HeperConstantss.ERR014);
                    }


                    await _Repository.UpdateAsync(brand);
                    //await _tablelink.UpdateAsync(brand.Slug, TypeLinkConstants.IdTypePagePost, brand.Id);
                    await _distributedCache.RemoveAsync(PagePostCacheKeys.ListKey);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    _log.LogInformation("UpdatePostOnePageCommand update end: " + command.Name);
                    try
                    {
                        _log.LogInformation("UpdatePostOnePageCommand update Image start:" + command.Name);
                        if (!string.IsNullOrEmpty(imgold))
                        {
                            _fileHelper.DeleteFile(imgold, FolderUploadConstants.PagePost);
                        }

                        _log.LogInformation("UpdatePostOnePageCommand update Image end:" + command.Name);
                    }
                    catch (Exception e)
                    {
                        _log.LogError("UpdatePostOnePageCommand update Image error:" + command.Name + "\n" + e.ToString());
                    }
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
