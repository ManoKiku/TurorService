using TutorService.Domain.Entities;

namespace TutorService.Domain.Interfaces;

public interface ILessonCommentRepository : IRepository<LessonComment>
{
    Task<IEnumerable<LessonComment>> GetByLessonIdAsync(Guid lessonId);
    Task<LessonComment?> GetByIdWithDetailsAsync(Guid id);
}