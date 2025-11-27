using AutoMapper;
using TutorService.Application.DTOs.Lesson;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class LessonMappingProfile : Profile
{
    public LessonMappingProfile()
    {
        CreateMap<Lesson, LessonDto>()
            .ForMember(d => d.TutorName, opt => opt.MapFrom(s => 
                s.Tutor != null && s.Tutor.User != null 
                    ? $"{s.Tutor.User.FirstName} {s.Tutor.User.LastName}" 
                    : string.Empty))
            .ForMember(d => d.StudentName, opt => opt.MapFrom(s => 
                s.Student != null 
                    ? $"{s.Student.FirstName} {s.Student.LastName}" 
                    : string.Empty));
    }
}