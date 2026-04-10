using TutorService.Domain.Entities;

namespace TutorService.Domain.Interfaces;

public interface ISavedContentRepository : IRepository<SavedContent>
{
    Task<IEnumerable<SavedContent>> GetByTutorIdAsync(Guid tutorId);
    Task<SavedContent?> GetByIdWithTutorAsync(Guid id);
}