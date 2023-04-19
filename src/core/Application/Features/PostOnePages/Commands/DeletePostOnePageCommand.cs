using Application.CacheKeys;
using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Abstractions.Repository;
using AspNetCoreHero.Results;
using AutoMapper;
using Dapper;
using Domain.Entities;
using MediatR;
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
    public class DeletePostOnePageCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeletePostOnePageHandler : IRequestHandler<DeletePostOnePageCommand, Result<int>>
        {
            private readonly ILogger<DeletePostOnePageHandler> _log;
            private readonly ITableLinkRepository _tablelink;
            private readonly IRepositoryAsync<PagePost> _Repository;
            private readonly IMapper _mapper;
            private readonly IFormFileHelperRepository _fileHelper;
            private readonly IDistributedCache _distributedCache;
            private readonly IDapperRepository _dapperdb;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeletePostOnePageHandler(IRepositoryAsync<PagePost> brandRepository, IFormFileHelperRepository fileHelper,
                 ITableLinkRepository tablelink, ILogger<DeletePostOnePageHandler> log,
                IDapperRepository dapperdb, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _tablelink = tablelink;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _dapperdb = dapperdb;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeletePostOnePageCommand command, CancellationToken cancellationToken)
            {
                var product = await _Repository.GetByIdAsync(command.Id);
                await _Repository.DeleteAsync(product);
                await _tablelink.DeleteAsync(TypeLinkConstants.IdTypePagePost, product.Id);
                await _distributedCache.RemoveAsync(PagePostCacheKeys.ListKey);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                try
                {
                    _log.LogInformation("DeleteProductCommand delete Image start:" + product.Name);
                    if (!string.IsNullOrEmpty(product.Img))
                    {
                        _fileHelper.DeleteFile(product.Img, FolderUploadConstants.PagePost);
                    }
                    _log.LogInformation("DeleteProductCommand delete Image end:" + product.Name);
                }
                catch (Exception e)
                {
                    _log.LogError("DeleteProductCommand update Image error:" + product.Name + "\n" + e.ToString());
                }
                return Result<int>.Success(product.Id);
            }
        }
    }
    public class RoleClaimsModel
    {
        public string RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
