using AutoMapper;
using TutorService.Application.DTOs.SavedContent;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class SavedContentMappingProfile : Profile
{
    public SavedContentMappingProfile()
    {
        CreateMap<SavedContent, SavedContentDto>()
            .ForMember(d => d.FolderId, opt => opt.MapFrom(s => s.FolderId))
            .ForMember(d => d.FolderName, opt => opt.MapFrom(s => s.Folder != null ? s.Folder.Name : null));

        CreateMap<SavedContentFolder, SavedContentFolderDto>()
            .ForMember(d => d.ItemCount, opt => opt.MapFrom(s => s.SavedContents.Count));
    }
}