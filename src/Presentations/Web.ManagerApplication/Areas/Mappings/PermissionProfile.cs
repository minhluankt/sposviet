using Application.Features.Permissions.Commands;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ManagerApplication.Areas.Mappings
{
    internal class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<UpdatePermissionCommand, Domain.Entities.Permission>().ReverseMap();
        }
    }
}
