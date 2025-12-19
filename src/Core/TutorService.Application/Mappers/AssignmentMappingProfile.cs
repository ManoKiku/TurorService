using AutoMapper;
using TutorService.Application.DTOs.Assigment;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class AssignmentMappingProfile : Profile
{
    public AssignmentMappingProfile()
    {
        CreateMap<Assignment, AssignmentDto>();
    }
    
}