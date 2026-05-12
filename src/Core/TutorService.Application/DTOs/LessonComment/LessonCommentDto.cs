namespace TutorService.Application.DTOs.LessonComment;

public class LessonCommentDto
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public Guid TutorId { get; set; }
    public string TutorName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}