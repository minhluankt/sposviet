using Application.CacheKeys;
using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Posts.Commands
{
    public class DeletePostCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeletePostHandler : IRequestHandler<DeletePostCommand, Result<int>>
        {
            private readonly ILogger<DeletePostHandler> _log;
            private readonly ITableLinkRepository _tablelink;
            private readonly IRepositoryAsync<Post> _Repository;
            private readonly IMapper _mapper;
            private readonly IFormFileHelperRepository _fileHelper;
            private readonly IDistributedCache _distributedCache;
            private readonly IDapperRepository _dapperdb;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeletePostHandler(IRepositoryAsync<Post> brandRepository, IFormFileHelperRepository fileHelper,
                 ITableLinkRepository tablelink, ILogger<DeletePostHandler> log,
                IDapperRepository dapperdb, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _log = log;
                _tablelink = tablelink;
                _Repository = brandRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _dapperdb = dapperdb;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeletePostCommand command, CancellationToken cancellationToken)
            {
                var product = await _Repository.GetByIdAsync(command.Id);
                await _Repository.DeleteAsync(product);
                await _tablelink.DeleteAsync(TypeLinkConstants.IdTypePost, product.Id);
                await _distributedCache.RemoveAsync(PostCacheKeys.ListKey);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _log.LogInformation("DeletePostCommand:" + product.Name);
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
