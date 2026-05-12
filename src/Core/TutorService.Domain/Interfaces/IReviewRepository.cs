using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TutorService.Domain.Entities;

namespace TutorService.Domain.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    Task<IEnumerable<Review>> GetByTutorProfileIdAsync(Guid tutorProfileId, int page, int pageSize);
    Task<int> GetCountByTutorProfileIdAsync(Guid tutorProfileId);
    Task<Review?> GetByUserAndTutorAsync(Guid userId, Guid tutorProfileId);
    Task<double?> GetAverageRatingForTutorAsync(Guid tutorProfileId);
    Task<Dictionary<Guid, double?>> GetAverageRatingsForTutorsAsync(IEnumerable<Guid> tutorProfileIds);
    Task<Review?> GetByIdWithDetailsAsync(Guid id);
    Task<bool> UserHasReviewForTutorAsync(Guid userId, Guid tutorProfileId);
}