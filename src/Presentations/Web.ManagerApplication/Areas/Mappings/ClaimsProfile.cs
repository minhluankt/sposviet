using AutoMapper;
using Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.ManagerApplication.Areas.Admin.Models;
using Web.ManagerApplication.Models;

namespace Web.ManagerApplication.Areas.Mappings
{
    public class ClaimsProfile : Profile
    {
        public ClaimsProfile()
        {
            CreateMap<Claim, RoleClaimsViewModel>().ReverseMap();
        }
    }
}
