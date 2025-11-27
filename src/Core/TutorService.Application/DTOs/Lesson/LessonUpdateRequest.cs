using TutorService.Domain.Enums;

namespace TutorService.Application.DTOs.Lesson;

public class LessonUpdateRequest
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Title { get; set; } = string.Empty;
    public LessonStatus Status { get; set; }
}