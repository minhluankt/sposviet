using Infrastructure.Infrastructure.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public interface IDbFactory : IDisposable
    {
        ApplicationDbContext Init();
    }
}
