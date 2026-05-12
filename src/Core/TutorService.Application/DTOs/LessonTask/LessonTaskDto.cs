using TutorService.Domain.Enums;

namespace TutorService.Application.DTOs.LessonTask;

public class LessonTaskDto
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    public string? Link { get; set; }
    public SubmissionType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}