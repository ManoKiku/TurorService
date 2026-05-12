namespace TutorService.Domain.Entities;

public class LessonComment : BaseEntity
{
    public Guid LessonId { get; set; }
    public Lesson? Lesson { get; set; }

    public Guid TutorId { get; set; }
    public TutorProfile? Tutor { get; set; }

    public string Text { get; set; } = string.Empty;
}