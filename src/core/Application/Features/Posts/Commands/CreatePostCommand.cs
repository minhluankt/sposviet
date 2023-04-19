using Application.CacheKeys;
using Application.Constants;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Shared;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Posts.Commands
{
    public partial class CreatePostCommand : Post, IRequest<Result<int>>
    {
        public IFormFile ImgUpload { get; set; }
    }
    public class CreatePostHandler : IRequestHandler<CreatePostCommand, Result<int>>
    {
        private readonly ILogger<CreatePostHandler> _log;
        private readonly IFormFileHelperRepository _fileHelper;
        private readonly ITableLinkRepository _tablelink;
        private readonly IMailService _mailservice;
        private IFormFileHelperRepository _fileform;
        private readonly IRepositoryAsync<Post> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreatePostHandler(IRepositoryAsync<Post> brandRepository,
            IFormFileHelperRepository fileform, IMailService mailservice,
             ITableLinkRepository tablelink,
              ILogger<CreatePostHandler> log,
             IFormFileHelperRepository fileHelper,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _fileHelper = fileHelper;
            _tablelink = tablelink;
            _mailservice = mailservice;
            _log = log;
            _fileform = fileform;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            string imgfile = string.Empty;
            try
            {
                string slug = Common.ConvertToSlug(request.Name);
                var getConsultation = _Repository.Entities.Where(m => m.Slug == slug).SingleOrDefault();
                if (getConsultation != null)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR014);
                }
                var product = _mapper.Map<Post>(request);
                product.Slug = slug;

                //if (request.ImgUpload != null)
                //{
                //    product.Img = _fileHelper.UploadedFile(request.ImgUpload, "", FolderUploadConstants.Post);
                //    imgfile = product.Img;
                //}
                await _Repository.AddAsync(product);
                await _distributedCache.RemoveAsync(PostCacheKeys.ListKey);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                //await _tablelink.AddAsync(request.Slug, TypeLinkConstants.IdTypePost, TypeLinkConstants.TypePost, product.Id);
                return Result<int>.Success();
            }
            catch (Exception e)
            {
                if (!string.IsNullOrEmpty(imgfile))
                {
                    _fileHelper.DeleteFile(imgfile, FolderUploadConstants.Post);
                }
                _log.LogError(e, e.Message);
                return Result<int>.Fail(e.Message);
            }
        }
    }
}
