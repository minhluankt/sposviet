using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Comments.Commands
{
    public partial class CreateCommentProductCommand : CommentModel, IRequest<Result<int>>
    {

    }
    public class CreateCommentProductHandler : IRequestHandler<CreateCommentProductCommand, Result<int>>
    {
        private readonly ILogger<CreateCommentProductCommand> _log;
        private readonly ICommentProductRepository<Comment> _repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateCommentProductHandler(ICommentProductRepository<Comment> repository,
            ILogger<CreateCommentProductCommand> log,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
            _log = log;
        }

        public async Task<Result<int>> Handle(CreateCommentProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _log.LogInformation("CreateCommentProductCommand  start " + request.IdProduct);

                //var product = _mapper.Map<Comment>(request);
                Comment comment = new Comment();
                comment.IdProduct = request.IdProduct;
                comment.IdPattern = request.IdPattern;
                comment.IdCustomer = request.IdCustomer;
                comment.FullName = request.CusName;
                comment.Email = request.CusEmail;
                comment.PhoneNumber = request.CusPhone;
                comment.Content = request.Comment;
                comment.DeviceName = request.DeviceName;
                comment.DeviceType = request.DeviceType;
                comment.Browser = request.Browser;
                comment.OS = request.OS;
                await _repository.AddCommentAsync(comment);
                //await _distributedCache.RemoveAsync(CompanyAdminInfoCacheKeys.ListKey);
                _log.LogInformation("CreateCommentProductCommand  end " + request.IdProduct);
                return await Result<int>.SuccessAsync();
            }
            catch (Exception ex)
            {
                _log.LogError("CreateCommentProductCommand Create " + request.IdProduct + "\n" + ex.ToString());
                return await Result<int>.FailAsync(ex.Message);
            }

        }
    }
}
