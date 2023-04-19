
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface  ITeleBotRepository<T>
    {
       Task SendMess(T model,string text="");

    }
}
