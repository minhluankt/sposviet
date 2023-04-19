using Application.CacheKeys;
using Application.Enums;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class PromotionRunRepository : IPromotionRunRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<ProductPepository> _log;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IRepositoryAsync<PromotionRun> _repositoryPromotionRun;
        public PromotionRunRepository(IUnitOfWork unitOfWork, ILogger<ProductPepository> log,
            IServiceScopeFactory serviceScopeFactory,IDistributedCache distributedCach,
            IRepositoryAsync<PromotionRun> repositoryPromotionRun)
        {
            _distributedCache = distributedCach;
            _serviceScopeFactory = serviceScopeFactory;
            _unitOfWork = unitOfWork;
            _log = log;
            _repositoryPromotionRun = repositoryPromotionRun;
        }
        public void CheckUpdateStatus(int id,int status = (int)StatusPromotionRun.Done)
        {
            _log.LogInformation("CheckUpdateStatus StatusPromotionRun");
           // var task = Task.Run(() =>
            //{
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                    using (IDbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            PromotionRun getid = context.PromotionRun.Find(id);
                            if (getid != null)
                            {
                                if (getid.Status != status)
                                {
                                    if (getid.EndDate < DateTime.Now)
                                    {
                                        getid.TimeRemain = DateTime.Now.Subtract(getid.EndDate).TotalSeconds;
                                    }
                                    getid.Status = status;
                                    getid.LastModifiedOn = DateTime.Now;
                                    context.PromotionRun.Update(getid);
                                    context.SaveChanges();

                                    if(status>= (int)StatusPromotionRun.Done)
                                    {
                                        var UpdatePro = context.Product.Where(x => x.IdPromotionRun == getid.Id);
                                        if (UpdatePro.Count()>0)
                                        {
                                            UpdatePro.ToList().ForEach(c => { c.isRunPromotion = false; c.DiscountRun = 0; c.PriceDiscountRun = 0; });
                                            context.Product.UpdateRange(UpdatePro);
                                            context.SaveChanges();
                                        }
                                    }
                                }
                            }
                            _log.LogInformation("CheckUpdateStatus success");
                            transaction.Commit();
                             _distributedCache.Remove(PromotionRunCacheKeys.ListKey);
                            context.Dispose();
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            context.Dispose();
                            _log.LogError(e,e.Message);
                        }
                    }
                   
                }

           // });
            //PromotionRun getid = _repositoryPromotionRun.GetById(id);
            //if (getid != null)
            //{
            //    if (getid.EndDate<=DateTime.Now)
            //    {
            //        getid.Status = (int)StatusPromotionRun.Done;
            //        _unitOfWork.SaveChangesAsync();
            //    }
            //} 
        }
    }
}
