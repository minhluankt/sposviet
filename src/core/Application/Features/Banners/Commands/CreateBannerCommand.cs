using Application.CacheKeys;
using Application.Constants;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Banners.Commands
{
    public partial class CreateBannerCommand : Banner, IRequest<Result<int>>
    {
        public IFormFile Img { get; set; }
    }
    public class CreateBannerHandler : IRequestHandler<CreateBannerCommand, Result<int>>
    {
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly IRepositoryAsync<Banner> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateBannerHandler(IRepositoryAsync<Banner> brandRepository,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateBannerCommand request, CancellationToken cancellationToken)
        {
            request.Name = _fileHelper.UploadedFile(request.Img, "", FolderUploadConstants.Banner, false);
            request.Slug = Common.ConvertToSlug(request.Name.Split('.')[0]);
            request.Size = request.Img.Length;
            request.Active = true;
            var fidn = _Repository.Entities.Where(m => m.Slug == request.Slug).SingleOrDefault();
            if (fidn != null)
            {
                return await Result<int>.FailAsync("Tên đã tồn tại!");
            }
            var product = _mapper.Map<Banner>(request);
            await _Repository.AddAsync(product);
            await _distributedCache.RemoveAsync(BannerCacheKeys.ListKey);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result<int>.Success(product.Id);
        }
    }
}
