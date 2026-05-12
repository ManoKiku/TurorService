using AutoMapper;
using TutorService.Application.DTOs.LessonComment;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class LessonCommentMappingProfile : Profile
{
    public LessonCommentMappingProfile()
    {
        CreateMap<LessonComment, LessonCommentDto>()
            .ForMember(d => d.TutorName, opt =>
                opt.MapFrom(s => s.Tutor != null && s.Tutor.User != null
                    ? $"{s.Tutor.User.FirstName} {s.Tutor.User.LastName}"
                    : string.Empty));
    }
}