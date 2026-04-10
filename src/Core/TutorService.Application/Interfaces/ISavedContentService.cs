using TutorService.Application.DTOs.Assigment;
using TutorService.Application.DTOs.SavedContent;

namespace TutorService.Application.Interfaces;

public interface ISavedContentService
{
    Task<SavedContentDto> CreateAsync(Guid tutorId, SavedContentCreateRequest request);
    Task<IEnumerable<SavedContentDto>> GetByTutorAsync(Guid tutorId);
    Task<bool> DeleteAsync(Guid id, Guid tutorId);
    Task<FileDownloadResponse> DownloadFileAsync(Guid id, Guid tutorId);
}