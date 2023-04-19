using Application.Features.CompanyInfo.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;

namespace Web.ManagerCompany.Mappings
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<CreateCompanyInfoCommand, CompanyAdminInfoViewModel>().ReverseMap();
            CreateMap<UpdateCompanyInfoCommand, CompanyAdminInfoViewModel>().ReverseMap();
            CreateMap<CompanyAdminInfoViewModel, CompanyAdminInfo>().ReverseMap();
        }
    }
}
