namespace TutorService.Application.DTOs.Assigment;

public class AssignmentDto
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string DownloadUrl => $"/api/assignments/{Id}/download";
}