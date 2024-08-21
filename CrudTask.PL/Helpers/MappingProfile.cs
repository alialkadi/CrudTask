using AutoMapper;
using CrudTask.DAL.Data.Entities;
using CrudTask.PL.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace CrudTask.PL.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, UserViewModel>().ReverseMap();
            CreateMap<IdentityRole, RoleViewModel>().ReverseMap();
        }
    }
}
