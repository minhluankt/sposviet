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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Banners.Commands
{
    public partial class UpdateBannerCommand : Banner, IRequest<Result<int>>
    {
        public IFormFile Img { get; set; }
    }
    public class UpdateBannerHandler : IRequestHandler<UpdateBannerCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<Banner> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateBannerHandler(IRepositoryAsync<Banner> brandRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;

            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateBannerCommand command, CancellationToken cancellationToken)
        {
            var brand = await _Repository.GetByIdAsync(command.Id);
            if (brand == null)
            {
                return await Result<int>.FailAsync(HeperConstantss.ERR012);
            }
            else
            {
                string imgold = String.Empty;
                if (command.Img!=null && command.Img.Length>0)
                {
                     imgold = brand.Name;
                    brand.Name = _fileHelper.UploadedFile(command.Img, "", FolderUploadConstants.Banner, false);
                    brand.Slug = Common.ConvertToSlug(brand.Name.Split('.')[0]);
                    brand.Size = command.Img.Length;
                }
               
                brand.Sort = command.Sort;
                var checkcode = _Repository.Entities.Count(predicate: m => m.Slug == brand.Slug && m.Id != brand.Id);
                if (checkcode > 0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR014);
                }
                await _Repository.UpdateAsync(brand);
                await _distributedCache.RemoveAsync(BannerCacheKeys.ListKey);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                if (command.Img != null && command.Img.Length > 0)
                {
                    try
                {
                    _fileHelper.DeleteFile(imgold, FolderUploadConstants.Banner);
                }
                catch (Exception)
                {
                }
                }
                return Result<int>.Success(brand.Id);
            }
        }
    }
}
