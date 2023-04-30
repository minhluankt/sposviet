using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class DashboardModel
    {
        public decimal DOANHSO { get; set; }
        public decimal DOANHSOHOMQUA { get; set; }
        public int DONDANGPHUCVU { get; set; }
        public int DOANHTHUDONDANGPHUCVU { get; set; }
        public int DONHUUY { get; set; }
        public int DONDAXONG { get; set; }
        public int DONDAXONGHOMQUA { get; set; }
        public int Customer { get; set; }
        public int CustomerHomQua { get; set; }
    }
}
