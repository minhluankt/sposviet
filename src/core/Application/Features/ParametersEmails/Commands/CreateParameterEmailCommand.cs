using Application.CacheKeys;
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

namespace Application.Features.ParametersEmails.Commands
{
    public partial class CreateParametersEmailCommand : IRequest<Result<int>>
    {
        public string Key { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
    }
    public class CreateParametersEmailHandler : IRequestHandler<CreateParametersEmailCommand, Result<int>>
    {
        private readonly IRepositoryAsync<ParametersEmail> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateParametersEmailHandler(IRepositoryAsync<ParametersEmail> brandRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateParametersEmailCommand request, CancellationToken cancellationToken)
        {
            var fidn =  _Repository.Entities.Where(m=>m.Key == request.Key).SingleOrDefault();
            if (fidn!=null)
            {
                return await Result<int>.FailAsync("Mã quyền đã tồn tại!");
            }
            var product = _mapper.Map<ParametersEmail>(request);
            await _Repository.AddAsync(product);
            await _distributedCache.RemoveAsync(ParametersEmailCacheKeys.ListKey);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(product.Id);
        }
    }
}
