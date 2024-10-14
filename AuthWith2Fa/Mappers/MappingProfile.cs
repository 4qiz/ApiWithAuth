using AuthWith2Fa.Dtos.Request;
using AuthWith2Fa.Entities;
using AutoMapper;

namespace AuthWith2Fa.Mappers
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegistrationDto, User>().ForMember(u => u.UserName, opt => opt.MapFrom(dto => dto.Email));
            //CreateMap<UserRegistrationDto, User>().ForMember(u => u.Email, opt => opt.MapFrom(dto => dto.Email));
        }
    }
}
