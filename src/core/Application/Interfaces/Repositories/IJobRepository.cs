
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IJobRepository
    {
        //Task<bool> StartJob(RemindWork model);
        //Task<bool> StartJobDaily(RemindWork model);
        bool DeleteJob(string jobId, bool daily = false);
    }
}
