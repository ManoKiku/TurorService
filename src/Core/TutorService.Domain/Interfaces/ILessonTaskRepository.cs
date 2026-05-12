using TutorService.Domain.Entities;

namespace TutorService.Domain.Interfaces;

public interface ILessonTaskRepository : IRepository<LessonTask>
{
    Task<IEnumerable<LessonTask>> GetByLessonIdAsync(Guid lessonId);
    Task<LessonTask?> GetByIdWithDetailsAsync(Guid id);
}