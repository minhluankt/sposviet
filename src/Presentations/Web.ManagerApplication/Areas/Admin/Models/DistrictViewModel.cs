using Domain.Entities;
using System.Collections.Generic;

namespace Web.ManagerApplication.Areas.Admin.Models
{
    public class DistrictViewModel : District
    {
        public List<City> Citys { get; set; }
    }
}
