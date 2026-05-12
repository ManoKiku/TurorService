using Microsoft.EntityFrameworkCore;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;
using TutorService.Infrastructure.Data;

namespace TutorService.Infrastructure.Repositories;

public class SavedContentRepository : BaseRepository<SavedContent>, ISavedContentRepository
{
    public SavedContentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<SavedContent>> GetByTutorIdAsync(Guid tutorId)
    {
        return await _dbSet
            .Include(sc => sc.Tutor)
            .Include(sc => sc.Folder)
            .Where(sc => sc.TutorId == tutorId)
            .OrderByDescending(sc => sc.CreatedAt)
            .ToListAsync();
    }

    public async Task<SavedContent?> GetByIdWithTutorAsync(Guid id)
    {
        return await _dbSet
            .Include(sc => sc.Tutor)
            .Include(sc => sc.Folder) 
            .FirstOrDefaultAsync(sc => sc.Id == id);
    }
}