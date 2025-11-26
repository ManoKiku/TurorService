using TutorService.Application.DTOs.Lesson;
using TutorService.Domain.Enums;

namespace TutorService.Application.Interfaces;

public interface ILessonService
{
    Task<LessonDto> CreateAsync(Guid tutorId, LessonCreateRequest request);
    Task<LessonsResponse> GetLessonsAsync(
        Guid currentUserId,
        string currentUserRole,
        Guid? userId = null,
        LessonStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? tutorId = null,
        Guid? studentId = null,
        int page = 1,
        int pageSize = 20);
    Task<LessonDto?> GetByIdAsync(Guid id, Guid currentUserId, string currentUserRole);
    Task<LessonDto> UpdateAsync(Guid id, LessonUpdateRequest request, Guid currentUserId, string currentUserRole);
    Task<bool> DeleteAsync(Guid id, Guid currentUserId, string currentUserRole);
    Task<IEnumerable<LessonDto>> GetUpcomingLessonsAsync(Guid userId, int daysAhead = 7);
    Task<IEnumerable<LessonDto>> GetCalendarLessonsAsync(Guid userId, int month, int year);
}