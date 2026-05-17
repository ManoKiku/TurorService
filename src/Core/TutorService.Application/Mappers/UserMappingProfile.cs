using TutorService.Application.DTOs.User;
using AutoMapper;
using TutorService.Application.DTOs;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UpdateUserRequest, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<UserUpdateRequest, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<User, UserDto>()
            .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => src.IsEmailVerified))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarMongoFileId != string.Empty ? $"/api/users/{src.Id}/avatar" : null ));

        CreateMap<UserDto, User>();
    }
}