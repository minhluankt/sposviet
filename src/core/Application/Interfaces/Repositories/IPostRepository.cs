using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IPostRepository<T> where T : class
    {
        void UpdateReView(int id);
        bool CheckPostbyCategoryId(int CategoryId);
        bool CheckPostbyListCategoryId(int[] lstCategoryId);
    }
}
