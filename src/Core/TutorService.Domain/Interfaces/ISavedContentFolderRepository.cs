using TutorService.Domain.Entities;

namespace TutorService.Domain.Interfaces;

public interface ISavedContentFolderRepository : IRepository<SavedContentFolder>
{
    Task<IEnumerable<SavedContentFolder>> GetByTutorIdAsync(Guid tutorId);
    Task<SavedContentFolder?> GetByIdWithContentsAsync(Guid id);
}