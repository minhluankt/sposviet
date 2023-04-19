using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
   public interface IEmailRepository<T> where T : class
    {
        Task<T> GetMailSettingCacheAsync();
        T GetMailSetting();
    }
}
