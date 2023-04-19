using Application.Interfaces.Shared;
using Infrastructure.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class DbFactory : Disposable, IDbFactory
    {
        private ApplicationDbContext dbContext;

        public DbFactory(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public ApplicationDbContext Init()
        {
            //return dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            return dbContext;
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
