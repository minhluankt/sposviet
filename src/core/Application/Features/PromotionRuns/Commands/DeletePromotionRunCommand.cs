using Application.CacheKeys;
using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Abstractions.Repository;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Hangfire;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.PromotionRuns.Commands
{

    public class DeletePromotionRunCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public class DeletePromotionRunHandler : IRequestHandler<DeletePromotionRunCommand, Result<int>>
        {
            private readonly IRepositoryAsync<PromotionRun> _Repository;
            private readonly IRepositoryAsync<Product> _Product;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeletePromotionRunHandler(IRepositoryAsync<Product> Product,
                IRepositoryAsync<PromotionRun> PromotionRunRepository, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _Product = Product;
                _Repository = PromotionRunRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeletePromotionRunCommand command, CancellationToken cancellationToken)
            {
                var PromotionRun = await _Repository.GetByIdAsync(command.Id);
                if (PromotionRun != null)
                {
                    if (PromotionRun.Status!= (int)StatusPromotionRun.Done && PromotionRun.Status != (int)StatusPromotionRun.Cancel)
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR017);
                    }
                    var check = _Product.GetAll(m => m.IdPromotionRun == PromotionRun.Id).Count();
                    if (check > 0)
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR016);
                    }
                    await _Repository.DeleteAsync(PromotionRun);
                    await _distributedCache.RemoveAsync(PromotionRunCacheKeys.ListKey);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    if (!string.IsNullOrEmpty(PromotionRun.JobStart))
                    {
                        RecurringJob.RemoveIfExists(PromotionRun.JobStart);
                    }
                    if (!string.IsNullOrEmpty(PromotionRun.JobEnd))
                    {
                        RecurringJob.RemoveIfExists(PromotionRun.JobEnd);
                    }
                    return Result<int>.Success();
                }
                return Result<int>.Fail(HeperConstantss.ERR012);
            }
        }
    }
}

