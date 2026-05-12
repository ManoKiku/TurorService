using AutoMapper;
using TutorService.Application.DTOs.Review;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.UserName, opt => opt.Ignore());
        CreateMap<ReviewCreateRequest, Review>();
        CreateMap<ReviewUpdateRequest, Review>();
    }
}