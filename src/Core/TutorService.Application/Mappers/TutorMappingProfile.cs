using AutoMapper;
using TutorService.Application.DTOs.Tutor;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class TutorMappingProfile : Profile
{
    public TutorMappingProfile()
    {
        CreateMap<TutorProfile, TutorProfileDto>()
            .ForMember(d => d.TotalReviews, opt => opt.MapFrom(t => t.Reviews.Count))
            .ForMember(d => d.AverageRating, opt => opt.MapFrom(t => t.Reviews.Any() ? t.Reviews.Average(r => r.Rating) : 0));
            
        CreateMap<TutorProfileDto, TutorProfile>();
        CreateMap<TutorPost, TutorPostDto>()
            .ForMember(d => d.SubjectName, opt => opt.MapFrom(s => s.Subject!.Name))
            .ForMember(d => d.HourlyRate, opt => opt.MapFrom(s => s.Tutor!.HourlyRate))
            .ForMember(d => d.TutorName,
                opt => opt.MapFrom(s => s.Tutor!.User!.FirstName + " " + s.Tutor.User.LastName))
            .ForMember(d => d.Tags, opt => opt.MapFrom(s => s.TutorPostTags.Select(t => t.Tag)))
            .ForMember(d => d.TotalReviews, opt => opt.MapFrom(s => s.Tutor!.Reviews.Count))
            .ForMember(d => d.AverageRating, opt => opt.MapFrom(s => s.Tutor!.Reviews.Any() ? s.Tutor.Reviews.Average(r => r.Rating) : 0));
        CreateMap<TutorPostDto, TutorPost>();
        CreateMap<Tag, TagDto>();
    }
}