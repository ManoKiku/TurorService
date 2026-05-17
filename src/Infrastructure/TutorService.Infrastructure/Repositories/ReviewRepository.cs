using Microsoft.EntityFrameworkCore;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;
using TutorService.Infrastructure.Data;

namespace TutorService.Infrastructure.Repositories;

public class ReviewRepository : BaseRepository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Review>> GetByTutorProfileIdAsync(Guid tutorProfileId, int page, int pageSize)
    {
        return await _dbSet
            .Where(r => r.TutorProfileId == tutorProfileId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(r => r.User)
            .ToListAsync();
    }
    
    public async Task<int> GetCountByTutorProfileIdAsync(Guid tutorProfileId)
    {
        return await _dbSet.CountAsync(r => r.TutorProfileId == tutorProfileId && !r.IsDeleted);
    }

    public async Task<Review?> GetByUserAndTutorAsync(Guid userId, Guid tutorProfileId)
    {
        return await _dbSet
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.TutorProfileId == tutorProfileId && !r.IsDeleted);
    }

    public async Task<double?> GetAverageRatingForTutorAsync(Guid tutorProfileId)
    {
        var ratings = await _dbSet
            .Where(r => r.TutorProfileId == tutorProfileId && !r.IsDeleted)
            .Select(r => (double?)r.Rating)
            .ToListAsync();
        
        return ratings.Any() ? Math.Round(ratings.Average()!.Value, 1) : null;
    }

    public async Task<Dictionary<Guid, double?>> GetAverageRatingsForTutorsAsync(IEnumerable<Guid> tutorProfileIds)
    {
        var ids = tutorProfileIds.ToList();
        var ratings = await _dbSet
            .Where(r => ids.Contains(r.TutorProfileId) && !r.IsDeleted)
            .GroupBy(r => r.TutorProfileId)
            .Select(g => new { TutorProfileId = g.Key, Avg = g.Average(r => (double?)r.Rating) })
            .ToDictionaryAsync(k => k.TutorProfileId, v => v.Avg.HasValue ? Math.Round(v.Avg.Value, 1) : (double?)null);
        
        return ratings;
    }

    public async Task<Review?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(r => r.User)
            .Include(r => r.TutorProfile)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }

    public async Task<bool> UserHasReviewForTutorAsync(Guid userId, Guid tutorProfileId)
    {
        return await _dbSet.AnyAsync(r => r.UserId == userId && r.TutorProfileId == tutorProfileId && !r.IsDeleted);
    }
}