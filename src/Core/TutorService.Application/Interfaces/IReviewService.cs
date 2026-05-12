using TutorService.Application.DTOs.Review;

namespace TutorService.Application.Interfaces;

public interface IReviewService
{
    Task<ReviewDto> CreateReviewAsync(Guid userId, ReviewCreateRequest request);
    Task<(IEnumerable<ReviewDto> Reviews, int TotalCount)> GetReviewsByTutorAsync(Guid tutorProfileId, int page, int pageSize);
    Task<ReviewDto> UpdateReviewAsync(Guid reviewId, Guid userId, string userRole, ReviewUpdateRequest request);
    Task<bool> DeleteReviewAsync(Guid reviewId, Guid userId, string userRole);
}