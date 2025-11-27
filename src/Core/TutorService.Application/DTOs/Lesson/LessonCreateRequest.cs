namespace TutorService.Application.DTOs.Lesson;

public class LessonCreateRequest
{
    public Guid StudentId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Title { get; set; } = string.Empty;
}