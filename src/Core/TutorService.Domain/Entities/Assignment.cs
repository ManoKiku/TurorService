namespace TutorService.Domain.Entities;

public class Assignment : BaseEntity
{
    public Guid LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}
