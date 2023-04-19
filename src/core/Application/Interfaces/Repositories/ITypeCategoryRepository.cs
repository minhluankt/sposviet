using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface ITypeCategoryRepository<T>
    {
        T GetByCode(string code,int? ProductType=null);// 0 là sp 1 là phutung
    }
}
