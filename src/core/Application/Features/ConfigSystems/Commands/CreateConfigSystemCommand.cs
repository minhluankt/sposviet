using Application.CacheKeys;
using Application.Hepers;
using Application.Interfaces.Repositories;
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

namespace Application.Features.ConfigSystems.Commands
{
    public partial class CreateConfigSystemCommand : ConfigSystem, IRequest<Result<int>>
    {
        public int?[] listCategoryproduct { get; set; }
        //  public int?[] listCategoryaccessary { get; set; }
    }
    public class CreateConfigSystemHandler : IRequestHandler<CreateConfigSystemCommand, Result<int>>
    {
        private readonly IRepositoryAsync<ConfigSystem> _Repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreateConfigSystemHandler(IRepositoryAsync<ConfigSystem> brandRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreateConfigSystemCommand request, CancellationToken cancellationToken)
        {
            var fidn = await _Repository.GetFirstAsync();
            if (fidn != null)
            {
                return await Result<int>.FailAsync("Tên đã tồn tại!");
            }
           // fidn.lstIdAndNameCategoryShowInHome = Common.ConverObjectToJsonString(request.lstIdAndNameCategoryShowInHome);
            // request.ListIdCategoryAccessary = Common.ConverObjectToJsonString(request.listCategoryaccessary);
            var product = _mapper.Map<ConfigSystem>(request);
            await _Repository.AddAsync(product);
            await _distributedCache.RemoveAsync(ConfigSystemCacheKeys.key);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(product.Id);
        }
    }
}
