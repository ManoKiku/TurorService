using AutoMapper;
using TutorService.Application.DTOs.SavedContent;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class SavedContentMappingProfile : Profile
{
    public SavedContentMappingProfile()
    {
        CreateMap<SavedContent, SavedContentDto>();
    }
}