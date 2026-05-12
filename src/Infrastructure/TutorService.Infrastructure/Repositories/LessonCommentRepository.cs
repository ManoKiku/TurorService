// Infrastructure/Repositories/LessonCommentRepository.cs
using Microsoft.EntityFrameworkCore;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;
using TutorService.Infrastructure.Data;

namespace TutorService.Infrastructure.Repositories;

public class LessonCommentRepository : BaseRepository<LessonComment>, ILessonCommentRepository
{
    public LessonCommentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<LessonComment>> GetByLessonIdAsync(Guid lessonId)
    {
        return await _context.LessonComments
            .Include(lc => lc.Tutor)
            .ThenInclude(t => t!.User)
            .Where(lc => lc.LessonId == lessonId)
            .OrderBy(lc => lc.CreatedAt)
            .ToListAsync();
    }

    public async Task<LessonComment?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.LessonComments
            .Include(lc => lc.Tutor)
            .ThenInclude(t => t!.User)
            .FirstOrDefaultAsync(lc => lc.Id == id);
    }
}