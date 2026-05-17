using AutoMapper;
using TutorService.Application.DTOs.StudentTutorRelation;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class StudentTutorRelationMappingProfile : Profile
{
    public StudentTutorRelationMappingProfile()
    {
        CreateMap<StudentTutorRelation, StudentTutorRelationDto>()
            .ForMember(d => d.StudentName, opt => opt.MapFrom(s =>
                s.Student != null ? $"{s.Student.FirstName} {s.Student.LastName}" : string.Empty))
            .ForMember(d => d.TutorName, opt => opt.MapFrom(s =>
                s.Tutor != null && s.Tutor.User != null
                    ? $"{s.Tutor.User.FirstName} {s.Tutor.User.LastName}"
                    : string.Empty))
            .ForMember(d => d.StudentAvatarUrl,
                opt => opt.MapFrom(s =>
                    s.Student!.AvatarMongoFileId != string.Empty ? $"/api/users/{s.Student.Id}/avatar" : null))
            .ForMember(d => d.TutorAvatarUrl,
                opt => opt.MapFrom(s =>
                    s.Tutor!.User!.AvatarMongoFileId != string.Empty ? $"/api/users/{s.Tutor.User.Id}/avatar" : null));
    }
}