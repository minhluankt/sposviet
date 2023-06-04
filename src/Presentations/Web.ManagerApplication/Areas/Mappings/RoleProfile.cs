using AutoMapper;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ManagerApplication.Areas.Admin.Models;
using Web.ManagerApplication.Models;

namespace Web.ManagerApplication.Areas.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<ApplicationRole, RoleViewModel>().ReverseMap();
        }
    }
}
