using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CategoryMenuModel : CategoryProduct
    {
        public string slugPattern { get; set; }
        public int IdTypeCategory { get; set; }
    }
}
