using Application.CacheKeys;
using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Abstractions.Repository;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PromotionRuns.Commands
{

    public partial class UpdatePromotionRunCommand : PromotionRun, IRequest<Result<int>>
    {
    }
    public class UpdatePromotionRunHandler : IRequestHandler<UpdatePromotionRunCommand, Result<int>>
    {
        private readonly IPromotionRunRepository _rromotionRunRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<PromotionRun> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdatePromotionRunHandler(IRepositoryAsync<PromotionRun> PromotionRunRepository, IPromotionRunRepository rromotionRunRepository,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _rromotionRunRepository = rromotionRunRepository;
            _Repository = PromotionRunRepository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdatePromotionRunCommand command, CancellationToken cancellationToken)
        {
            var PromotionRun = await _Repository.GetByIdAsync(command.Id);
            if (PromotionRun == null)
            {
                return await Result<int>.FailAsync(HeperConstantss.ERR012);
            }
            else
            {
                //check nếu có sự kiện nào dg chạy thì k cho chạy cái nữa
                var check = _Repository.GetAllQueryable().Where(x => x.IsActive && x.Status == (int)StatusPromotionRun.Processing && x.Id!= PromotionRun.Id).FirstOrDefault();
                if (check!=null)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR041);
                }


                if (PromotionRun.IsActive == true && command.IsActive == true)
                {
                    if (PromotionRun.StartDate!= command.StartDate || command.EndDate!= PromotionRun.EndDate)
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR032);
                    }
                }

                PromotionRun.Name = command.Name;
                PromotionRun.StartDate = command.StartDate;
                PromotionRun.EndDate = command.EndDate;
                if (!PromotionRun.IsActive && command.IsActive)
                {
                    if (command.EndDate <= DateTime.Now.AddMinutes(10))
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR031);
                    }

                    PromotionRun.IsActive = command.IsActive;
                    if (command.StartDate<=DateTime.Now)
                    {
                        PromotionRun.Status = (int)StatusPromotionRun.Processing;
                    }
                    else
                    {
                        PromotionRun.Status = (int)StatusPromotionRun.Upcoming;
                    }

                    
                        if (command.StartDate > DateTime.Now)
                        {
                            var idjobstart = BackgroundJob.Schedule(
                            () => _rromotionRunRepository.CheckUpdateStatus(PromotionRun.Id, (int)StatusPromotionRun.Processing),
                             command.StartDate);
                            PromotionRun.JobStart = idjobstart;
                        }
                        //tự động update dsau khi kết thúc sự kiện
                        var idjob = BackgroundJob.Schedule(
                          () => _rromotionRunRepository.CheckUpdateStatus(PromotionRun.Id, (int)StatusPromotionRun.Done),
                         command.EndDate);
                        PromotionRun.JobEnd = idjob;
                   

                }
                else if(command.IsCancelEvent)
                {
                    PromotionRun.IsCancelEvent = command.IsCancelEvent;
                    PromotionRun.Status = (int)StatusPromotionRun.Cancel;
                    var time = command.EndDate.Subtract(command.StartDate);
                    PromotionRun.TimeRemain = time.TotalSeconds;

                    if (!string.IsNullOrEmpty(PromotionRun.JobStart))
                    {
                        RecurringJob.RemoveIfExists(PromotionRun.JobStart);
                    }
                    if (!string.IsNullOrEmpty(PromotionRun.JobEnd))
                    {
                        RecurringJob.RemoveIfExists(PromotionRun.JobEnd);
                    }
                    _rromotionRunRepository.CheckUpdateStatus(PromotionRun.Id, (int)StatusPromotionRun.Cancel);
                }

                PromotionRun.Slug = Common.ConvertToSlug(command.Name);
                var checkcode = _Repository.Entities.Count(predicate: m => m.Slug == PromotionRun.Slug && m.Id!=PromotionRun.Id);
                if (checkcode > 0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR014);
                }
                await _Repository.UpdateAsync(PromotionRun);
                await _distributedCache.RemoveAsync(PromotionRunCacheKeys.ListKey);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(PromotionRun.Id);
            }
        }
    }
}
