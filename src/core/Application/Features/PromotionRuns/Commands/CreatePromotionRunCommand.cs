using Application.CacheKeys;
using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
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
    public partial class CreatePromotionRunCommand : PromotionRun,  IRequest<Result<int>>
    {
        public string JsonProduct { get; set; }
        public bool IsCloneEvent { get; set; }
    }
    public class CreatePromotionRunHandler : IRequestHandler<CreatePromotionRunCommand, Result<int>>
    {
        private readonly IRepositoryAsync<PromotionRun> _Repository;
        private readonly IRepositoryAsync<Product> _RepositoryProduct;
        private readonly IPromotionRunRepository _rromotionRunRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }

        public CreatePromotionRunHandler(IRepositoryAsync<PromotionRun> PromotionRunRepository,
            IRepositoryAsync<Product> RepositoryProduct,
            IPromotionRunRepository rromotionRunRepository,
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _rromotionRunRepository = rromotionRunRepository;
            _RepositoryProduct = RepositoryProduct;
            _Repository = PromotionRunRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(CreatePromotionRunCommand request, CancellationToken cancellationToken)
        {
            _unitOfWork.CreateTransaction();
            try
            {
                request.Slug = Common.ConvertToSlug(request.Name);

                var fidn = _Repository.Entities.Where(m => m.Slug == request.Slug).SingleOrDefault();
                if (fidn != null)
                {
                    return await Result<int>.FailAsync("Tên đã tồn tại!");
                }
                var product = _mapper.Map<PromotionRun>(request);
                if (product.IsActive)
                {
                    // check nếu có cái dg chạy thì k cho chạy cái này
                    var check = _Repository.GetAllQueryable().Where(x => x.IsActive && x.Status == (int)StatusPromotionRun.Processing).FirstOrDefault();
                    if (check != null)
                    {
                        return await Result<int>.FailAsync(HeperConstantss.ERR041);
                    }
                    if (request.StartDate <= DateTime.Now)
                    {
                        product.Status = (int)StatusPromotionRun.Processing;
                      //  var jobId = BackgroundJob.Enqueue(
                      //() => Console.WriteLine("Delayed!"));
                      
                    }
                    else
                    {
                        product.Status = (int)StatusPromotionRun.Upcoming;
                    }
               

                }
                product.Id = 0;
                await _Repository.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();

                if (request.IsActive)
                {
                    if (request.StartDate > DateTime.Now)
                    {
                        var idjobstart = BackgroundJob.Schedule(
                        () => _rromotionRunRepository.CheckUpdateStatus(product.Id, (int)StatusPromotionRun.Processing),
                         request.StartDate);
                        product.JobStart = idjobstart;
                    }
                    //tự động update dsau khi kết thúc sự kiện
                    var idjob = BackgroundJob.Schedule(
                      () => _rromotionRunRepository.CheckUpdateStatus(product.Id, (int)StatusPromotionRun.Done),
                     request.EndDate);
                    product.JobEnd = idjob;
                    await _unitOfWork.SaveChangesAsync();
                }
              


                if (request.IsCloneEvent)
                {
                    if (!string.IsNullOrEmpty(request.JsonProduct))
                    {
                        var arid = Common.ConverJsonToArrIntByNotNull(request.JsonProduct);
                        //check và xóa các sp k cần chạy km nữa
                        var getpro = _RepositoryProduct.Entities.Where(m => !arid.Contains(m.Id) && m.IdPromotionRun == request.Id).ToList();
                        if (getpro.Count > 0)
                        {
                            getpro.ForEach(x => { x.IdPromotionRun = 0; x.isRunPromotion = false; });
                            await  _RepositoryProduct.UpdateRangeAsync(getpro);
                            await _unitOfWork.SaveChangesAsync();

                        }
                        // update các sp cho event clone
                        var updatenew = _RepositoryProduct.Entities.Where(m=> arid.Contains(m.Id)).ToList();
                        if (updatenew.Count>0)
                        {
                            updatenew.ForEach(x => { x.IdPromotionRun = product.Id; x.isRunPromotion = true; });
                            await _unitOfWork.SaveChangesAsync(); 
                        }
                    }
                    else
                    {
                        var updatenew = _RepositoryProduct.Entities.Where(m => m.IdPromotionRun== request.Id).ToList();
                        if (updatenew.Count > 0)
                        {
                            updatenew.ForEach(x => { x.IdPromotionRun = product.Id; x.isRunPromotion = true; });
                            await _unitOfWork.SaveChangesAsync();
                        }
                    }
                }

                 await _unitOfWork.CommitAsync();
                await _distributedCache.RemoveAsync(PromotionRunCacheKeys.ListKey);
                return Result<int>.Success(product.Id);
            }
            catch (Exception e)
            {
               await  _unitOfWork.RollbackAsync();
                return await Result<int>.FailAsync(e.Message);
            }
           
        }
    }
}
