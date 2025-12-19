using TutorService.Domain.Entities;

namespace TutorService.Domain.Interfaces;

public interface IAssignmentRepository : IRepository<Assignment>
{
    Task<IEnumerable<Assignment>> GetByLessonIdAsync(Guid lessonId);
    Task<Assignment?> GetByIdWithDetailsAsync(Guid id);
    Task<bool> IsUserParticipantAsync(Guid assignmentId, Guid userId);
}