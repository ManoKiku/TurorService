using TutorService.Domain.Enums;

namespace TutorService.Domain.Entities;

public class LessonTask : BaseEntity
{
    public Guid LessonId { get; set; }
    public Lesson? Lesson { get; set; }

    public Guid StudentId { get; set; }
    public User? Student { get; set; }

    public string? FileName { get; set; }
    public string? MongoFileId { get; set; }
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }

    public string? Link { get; set; }

    public SubmissionType Type { get; set; }
}