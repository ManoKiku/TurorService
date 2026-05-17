using AutoMapper;
using TutorService.Application.DTOs.Review;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(s => s.User!.FirstName + " " + s.User.LastName))
            .ForMember(dest => dest.AvatarUrl,
                opt => opt.MapFrom(s =>
                    s.User!.AvatarMongoFileId != string.Empty ? $"/api/users/{s.UserId}/avatar" : null));
        CreateMap<ReviewCreateRequest, Review>();
        CreateMap<ReviewUpdateRequest, Review>();
    }
}