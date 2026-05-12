    using Microsoft.EntityFrameworkCore;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;
using TutorService.Infrastructure.Data;

namespace TutorService.Infrastructure.Repositories;

public class SavedContentFolderRepository : BaseRepository<SavedContentFolder>, ISavedContentFolderRepository
{
    public SavedContentFolderRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<SavedContentFolder>> GetByTutorIdAsync(Guid tutorId)
    {
        return await _dbSet
            .Include(f => f.SavedContents)
            .Where(f => f.TutorId == tutorId)
            .OrderBy(f => f.Name)
            .ToListAsync();
    }

    public async Task<SavedContentFolder?> GetByIdWithContentsAsync(Guid id)
    {
        return await _dbSet
            .Include(f => f.SavedContents)
            .FirstOrDefaultAsync(f => f.Id == id);
    }
}