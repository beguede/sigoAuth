using AuthService.Domain.Identity;
using AutoMapper;

namespace AuthService.Application.Mapping
{
    public class IdentityMap : Profile
    {
        public IdentityMap()
        {
            CreateMap<UserSignUpResource, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(ur => ur.Email));
        }
    }
}
