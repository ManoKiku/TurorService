using Microsoft.EntityFrameworkCore;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;
using TutorService.Infrastructure.Data;

namespace TutorService.Infrastructure.Repositories;

public class LessonTaskRepository : BaseRepository<LessonTask>, ILessonTaskRepository
{
    public LessonTaskRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<LessonTask>> GetByLessonIdAsync(Guid lessonId)
    {
        return await _context.LessonTasks
            .Include(lt => lt.Student)
            .Where(lt => lt.LessonId == lessonId)
            .OrderBy(lt => lt.CreatedAt)
            .ToListAsync();
    }

    public async Task<LessonTask?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.LessonTasks
            .Include(lt => lt.Student)
            .FirstOrDefaultAsync(lt => lt.Id == id);
    }
}