using AutoMapper;
using WebApiKnowledge.Models;

namespace WebApiKnowledge
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AppUserModel, AppUser>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));
        }
    }
}
