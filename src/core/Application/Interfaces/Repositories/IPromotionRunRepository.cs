using Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface  IPromotionRunRepository
    {
        void CheckUpdateStatus(int id, int status = (int)StatusPromotionRun.Done);
    }
}
