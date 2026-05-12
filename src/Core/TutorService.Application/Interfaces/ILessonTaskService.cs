using TutorService.Application.DTOs.LessonTask;

namespace TutorService.Application.Interfaces;

public interface ILessonTaskService
{
    Task<LessonTaskDto> AddTaskAsync(Guid studentId, LessonTaskCreateRequest request);
    Task<IEnumerable<LessonTaskDto>> GetTasksForLessonAsync(Guid lessonId, Guid currentUserId, string currentUserRole);
    Task<bool> DeleteTaskAsync(Guid taskId, Guid currentUserId, string currentUserRole);
}