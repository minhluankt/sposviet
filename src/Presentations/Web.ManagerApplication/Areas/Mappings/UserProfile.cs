using AutoMapper;
using Domain.Identity;
using Model;

namespace Web.ManagerApplication.Areas.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>().ReverseMap();
         
        }
    }
}
