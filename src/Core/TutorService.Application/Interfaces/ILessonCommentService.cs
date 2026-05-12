using TutorService.Application.DTOs.LessonComment;

namespace TutorService.Application.Interfaces;

public interface ILessonCommentService
{
    Task<LessonCommentDto> AddCommentAsync(Guid tutorUserId, Guid lessonId, LessonCommentCreateRequest request);
    Task<IEnumerable<LessonCommentDto>> GetCommentsForLessonAsync(Guid lessonId, Guid currentUserId, string currentUserRole);
    Task<bool> DeleteCommentAsync(Guid commentId, Guid currentUserId, string currentUserRole);
}