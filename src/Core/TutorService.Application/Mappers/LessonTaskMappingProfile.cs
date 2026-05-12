using AutoMapper;
using TutorService.Application.DTOs.LessonTask;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class LessonTaskMappingProfile : Profile
{
    public LessonTaskMappingProfile()
    {
        CreateMap<LessonTask, LessonTaskDto>()
            .ForMember(d => d.StudentName, opt =>
                opt.MapFrom(s => s.Student != null
                    ? $"{s.Student.FirstName} {s.Student.LastName}"
                    : string.Empty));
    }
}