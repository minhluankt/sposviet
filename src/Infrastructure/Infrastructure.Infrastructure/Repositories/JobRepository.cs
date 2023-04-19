using ApiHttpClient.Wrappers;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;


using Hangfire;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<JobRepository> _logger;

        // private readonly ITelegramBotClient _tele;
        private readonly CancellationToken CancellationToken;
        private IUnitOfWork _unitOfWork { get; set; }
        public JobRepository(IUnitOfWork unitOfWork,
         
          
            IDistributedCache distributedCach,
            ILogger<JobRepository> logger
           )
        {
            _unitOfWork = unitOfWork;
         
            _logger = logger;
            _distributedCache = distributedCach;

        }
       
       
        public bool DeleteJob(string jobId, bool daily = false)
        {
            if (daily)
            {
                RecurringJob.RemoveIfExists(jobId);
                return true;
            }
            return BackgroundJob.Delete(jobId);
        }
    }
}
