using TutorService.Application.DTOs.Assigment;

namespace TutorService.Application.Interfaces;

public interface IAssignmentService
{
    Task<AssignmentDto> CreateAsync(Guid tutorId, AssignmentCreateRequest request);
    Task<IEnumerable<AssignmentDto>> GetByLessonIdAsync(Guid lessonId, Guid currentUserId, string currentUserRole);
    Task<AssignmentDto?> GetByIdAsync(Guid id, Guid currentUserId, string currentUserRole);
    Task<bool> DeleteAsync(Guid id, Guid currentUserId, string currentUserRole);
    Task<FileDownloadResponse> DownloadFileAsync(Guid id, Guid currentUserId, string currentUserRole);
}