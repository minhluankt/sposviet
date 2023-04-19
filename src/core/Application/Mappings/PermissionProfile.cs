using Application.Features.Permissions.Commands;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
    internal class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<CreatePermissionCommand, Permission>().ReverseMap();
        }
    }
}
